using System;
using System.Collections;
using _Scripts;
using DG.Tweening;
using Game;
using UnityEngine;
using Random = UnityEngine.Random;
public class Player : MonoBehaviour
{
    private static readonly int DoanDung = Animator.StringToHash("DoanDung");
    [SerializeField] private Animator animator;
    [SerializeField] private AnimFace animFace;

    [SerializeField] private AnimationClip[] animClipsRandom;
    public bool isRandomAnimation;
    private void OnEnable()
    {
        if (isRandomAnimation)
        {
            RandomAnimation();
        }
    }
    public void RandomAnimation()
    {
        animator.Play(animClipsRandom[Random.Range(0, animClipsRandom.Length)].name);
    }
    public void PlayAnimation(int id)
    {
        animator.SetTrigger(id);
    }
    public void PlayAnimWithFace(StateFace animName, bool IsSmile = false)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()))
        {
            if (animName.ToString().Equals("Smile"))
            {
                animator.SetTrigger(DoanDung);
            }
            else
            {
                animator.SetTrigger(animName.ToString());

            }
            StartCoroutine(WaitForAnimationStart(animator, animName, IsSmile));
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    private IEnumerator WaitForAnimationStart(Animator anim, StateFace animName, bool isSmile = false)
    {
        if(!isSmile) yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName(animName.ToString()));
        else yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("DoanDung"));
        float animLength = anim.GetCurrentAnimatorStateInfo(0).length;
        if (animName == StateFace.DoanSai)
        {
            AudioManager.instance.PlaySoundAngry();
        }
        if (animFace == null) yield break;
        animFace.SetState(animName, animLength);
    }
}