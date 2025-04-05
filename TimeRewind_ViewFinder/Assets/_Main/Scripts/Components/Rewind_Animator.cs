using System.Collections.Generic;
using TMPro;
using UnityEngine;

public struct AnimatorValues
{
    public int stateNameHash;
    public int nextStateNameHash;
    public float normalizedTime;
    public float nextNormalizedTime;
    public bool enterTransition;

    public override string ToString()
    {
        return
            $"stateNameHash:\n{stateNameHash}\n" +
            $"nextStateNameHash:\n{nextStateNameHash}\n" +
            $"normalizedTime:\n{normalizedTime}\n" +
            $"nextNormalizedTime\n:{nextNormalizedTime}\n" +
            $"enterTransition\n:{enterTransition}\n";
    }
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
            AnimatorStateInfo nextAnimatorStateInfo = animator.GetNextAnimatorStateInfo(i);
            AnimatorClipInfo[] animatorClipInfos = animator.GetNextAnimatorClipInfo(i);

            AnimatorValues animatorValues = new AnimatorValues();
            animatorValues.stateNameHash = animatorStateInfo.shortNameHash;
            animatorValues.nextStateNameHash = nextAnimatorStateInfo.shortNameHash;
            animatorValues.normalizedTime = animatorStateInfo.normalizedTime;
            animatorValues.nextNormalizedTime = nextAnimatorStateInfo.normalizedTime;
            animatorValues.enterTransition = animatorClipInfos.Length > 0;
            animatorBufferList[i].WriteBuffer(animatorValues);

            logText.text = animatorValues.ToString();
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

            if (animatorValues.enterTransition)
            {
                animator.Play(animatorValues.nextStateNameHash, i, animatorValues.nextNormalizedTime);
            }
            else
            {
                animator.Play(animatorValues.stateNameHash, i, animatorValues.normalizedTime);
            }

            logText.text = animatorValues.ToString();
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
                break;
        }
    }
}
