using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SpriteBlinkController : MonoBehaviour
{
    public Animator animator;
    public void OnSetAnimatorFalse()
    {
        animator.enabled = false;
    }

    public void OnSetAnimatorTrue()
    {
        animator.enabled = true;
    }

}