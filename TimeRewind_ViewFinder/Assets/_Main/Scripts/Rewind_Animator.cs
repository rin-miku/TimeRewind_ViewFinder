using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Rewind_Animator : RewindBase
{
    private Animator animator;
    private AnimatorControllerParameter[] animatorControllerParameters;

    private void Start()
    {
        animatorControllerParameters = animator.parameters;
    }

    public override void Record()
    {
        animator.speed = 1f;
    }

    public override void Rewind()
    {
        animator.speed = 0f;
    }
}
