using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct AnimatorValues
{
    public int stateNameHash;
    public float stateTime;
    public float clipLength;
    public float normalizedTime;
    public AnimatorTransitionValues animatorTransitionValues;
}

public struct AnimatorTransitionValues
{
    public bool inTransition;
    public int stateHashName;
    public int currentStateHashName;
    public DurationUnit durationUnit;
    public float duration;
    public float normalizedTime;
    public float t;
}

public struct AnimatorParameterValues
{
    public AnimatorControllerParameterType type;
    public int nameHash;
    public float value;
}

[RequireComponent(typeof(Animator))]
public class Rewind_Animator : RewindBase
{
    public TMP_Text logText;
    private Animator animator;
    private List<RewindBuffer<AnimatorValues>> animatorBufferList = new List<RewindBuffer<AnimatorValues>>();
    private List<RewindBuffer<AnimatorParameterValues>> animatorParameterBufferList = new List<RewindBuffer<AnimatorParameterValues>>();
    private AnimatorControllerParameter[] animatorControllerParameters;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animatorControllerParameters = animator.parameters;
        for(int i = 0; i < animator.layerCount; i++)
        {
            animatorBufferList.Add(new RewindBuffer<AnimatorValues>());
        }
        for(int i = 0; i < animatorControllerParameters.Length; i++)
        {
            animatorParameterBufferList.Add(new RewindBuffer<AnimatorParameterValues>());
        }
    }

    public override void Record()
    {
        animator.speed = 1f;

        for(int i = 0; i < animator.layerCount; i++)
        {
            AnimatorStateInfo animatorStateInfo = animator.GetCurrentAnimatorStateInfo(i);
            AnimatorStateInfo animatorNextStateInfo = animator.GetNextAnimatorStateInfo(i);
            AnimatorClipInfo[] animatorClipInfos = animator.GetNextAnimatorClipInfo(i);
            AnimatorValues lastAnimatorValues = animatorBufferList[i].ReadLastBuffer();

            AnimatorTransitionValues animatorTransitionValues;
            if (animatorClipInfos.Length > 0)
            {
                AnimatorTransitionInfo animatorTransitionInfo = animator.GetAnimatorTransitionInfo(i);
                animatorTransitionValues.inTransition = true;
                animatorTransitionValues.stateHashName = Animator.StringToHash(animatorClipInfos[0].clip.name);
                animatorTransitionValues.currentStateHashName = animatorStateInfo.shortNameHash;
                animatorTransitionValues.durationUnit = animatorTransitionInfo.durationUnit;
                animatorTransitionValues.duration = animatorTransitionInfo.duration;
                animatorTransitionValues.normalizedTime = animatorTransitionInfo.normalizedTime;
                animatorTransitionValues.t = lastAnimatorValues.animatorTransitionValues.t + Time.fixedDeltaTime;
            }
            else
            {
                animatorTransitionValues.inTransition = false;
                animatorTransitionValues.stateHashName = 0;
                animatorTransitionValues.currentStateHashName = 0;
                animatorTransitionValues.durationUnit = DurationUnit.Fixed;
                animatorTransitionValues.duration = 0;
                animatorTransitionValues.normalizedTime = 0;
                animatorTransitionValues.t = 0;
            }

            //AnimatorValues lastAnimatorValues = animatorBufferList[i].ReadLastBuffer();
            AnimatorValues animatorValues;
            animatorValues.stateNameHash = animatorStateInfo.shortNameHash;
            if (lastAnimatorValues.stateNameHash.Equals(animatorValues.stateNameHash))
            {
                animatorValues.stateTime = lastAnimatorValues.stateTime + Time.fixedDeltaTime;
                animatorValues.clipLength = lastAnimatorValues.clipLength;
            }
            else
            {
                animatorValues.stateTime = 0f;
                animatorValues.clipLength = animatorStateInfo.length;
            }
            animatorValues.normalizedTime = animatorStateInfo.normalizedTime;
            animatorValues.animatorTransitionValues = animatorTransitionValues;
            animatorBufferList[i].WriteBuffer(animatorValues);

            logText.text = 
                $"id:\t{animatorStateInfo.shortNameHash}\n" +
                $"d:\t{animatorNextStateInfo.normalizedTime}\n" +
                $"tC:\t{animatorValues.stateTime / animatorValues.clipLength}\n" +
                $"tN:\t{animatorStateInfo.normalizedTime}\n" +
                $"hasTs:{animatorTransitionValues.inTransition}\n" +
                $"tsId:\t{animatorTransitionValues.stateHashName}\n" +
                $"tsD:\t{animatorTransitionValues.duration}\n" +
                $"tsN:\t{animatorTransitionValues.normalizedTime}\n" +
                $"t:\t{animatorTransitionValues.t}";
        }

        for(int i = 0; i < animatorControllerParameters.Length; i++)
        {
            AnimatorControllerParameter animatorControllerParameter = animatorControllerParameters[i];
            AnimatorParameterValues parameterValues;
            parameterValues.type = animatorControllerParameter.type;
            parameterValues.nameHash = animatorControllerParameter.nameHash;
            parameterValues.value = GetAnimatorParameterValue(animatorControllerParameter);
            animatorParameterBufferList[i].WriteBuffer(parameterValues);
        }
    }

    public override void Rewind()
    {
        animator.speed = 0f;

        for (int i = 0; i < animator.layerCount; i++)
        {
            AnimatorValues animatorValues = animatorBufferList[i].ReadBuffer();

            //animator.Play(animatorValues.stateNameHash, i, animatorValues.stateTime / animatorValues.clipLength);
            //animator.Play(animatorValues.stateNameHash, i, animatorValues.normalizedTime);
            AnimatorTransitionValues animatorTransitionValues = animatorValues.animatorTransitionValues;
            if (animatorValues.animatorTransitionValues.inTransition)
            {
                animator.Play(animatorValues.stateNameHash, i, animatorValues.normalizedTime);
                animator.Update(0);
                //AnimatorTransitionValues animatorTransitionValues = animatorValues.animatorTransitionValues;
                if (animatorValues.animatorTransitionValues.durationUnit.Equals(DurationUnit.Fixed))
                {
                    animator.CrossFadeInFixedTime(animatorTransitionValues.stateHashName, animatorTransitionValues.duration, i, 0, animatorTransitionValues.normalizedTime);
                }
                else
                {
                    animator.CrossFade(animatorTransitionValues.stateHashName, animatorTransitionValues.duration, i, 0, animatorTransitionValues.normalizedTime);
                }
            }
            else
            {
                animator.Play(animatorValues.stateNameHash, i, animatorValues.normalizedTime);
            }

            logText.text =
                $"id:\t{animatorValues.stateNameHash}\n" +
                $"tC:\t{animatorValues.stateTime / animatorValues.clipLength}\n" +
                $"tN:\t{animatorTransitionValues.normalizedTime}\n" +
                $"hasTs:{animatorTransitionValues.inTransition}\n" +
                $"tsId:\t{animatorTransitionValues.stateHashName}\n" +
                $"tsD:\t{animatorTransitionValues.duration}\n" +
                $"tsN:\t{animatorTransitionValues.normalizedTime}";
        }

        for(int i = 0; i < animatorControllerParameters.Length; i++)
        {
            AnimatorParameterValues parameterValues = animatorParameterBufferList[i].ReadBuffer();
            SetAnimatorParameterValue(parameterValues);
        }
    }

    private float GetAnimatorParameterValue(AnimatorControllerParameter parameter)
    {
        float value = 0;
        switch (parameter.type)
        {
            case AnimatorControllerParameterType.Float:
                value = animator.GetFloat(parameter.nameHash);
                break;
            case AnimatorControllerParameterType.Int:
                value = animator.GetInteger(parameter.nameHash);
                break;
            case AnimatorControllerParameterType.Bool:
                value = animator.GetBool(parameter.nameHash) ? 1 : 0;
                break;
            case AnimatorControllerParameterType.Trigger:
                break;
        }
        return value;
    }

    private void SetAnimatorParameterValue(AnimatorParameterValues parameter)
    {
        switch (parameter.type)
        {
            case AnimatorControllerParameterType.Float:
                animator.SetFloat(parameter.nameHash, parameter.value);
                break;
            case AnimatorControllerParameterType.Int:
                animator.SetInteger(parameter.nameHash, (int)parameter.value);
                break;
            case AnimatorControllerParameterType.Bool:
                animator.SetBool(parameter.nameHash, parameter.value.Equals(1));
                break;
            case AnimatorControllerParameterType.Trigger:
                if (parameter.value.Equals(0))
                {
                    animator.ResetTrigger(parameter.nameHash);
                }
                else
                {
                    animator.SetTrigger(parameter.nameHash);
                }
                break;
        }
    }
}
