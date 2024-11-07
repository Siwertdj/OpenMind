using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TransitionAnimator : MonoBehaviour
{
    /// <summary>
    /// The static instance of the transition animator
    /// </summary>
    public static TransitionAnimator i;

    [SerializeField] private Animator animator;

    /// <summary>
    /// Different kinds of animations
    /// </summary>
    public enum AnimationType
    {
        Fade = 0,
        Wipe = 1
    }

    private void Start()
    {
        //Debug.Log("hi2");
        i = this;
    }
    
    /// <summary>
    /// The function that should be called to start the fade animation.
    /// Only fades to black.
    /// Can be awaited.
    /// </summary>
    public Task PlayStartAnimation(AnimationType type = AnimationType.Fade, float timeScale = 1)
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayAnimation("SceneLoading", type, timeScale, tcs);

        return tcs.Task;
    }

    public Task PlayEndAnimation(AnimationType type = AnimationType.Fade, float timeScale = 1)
    {
        var tcs = new TaskCompletionSource<bool>();

        PlayAnimation("SceneLoaded", type, timeScale, tcs);

        return tcs.Task;
    }

    private Task PlayAnimation(string trigger, AnimationType type, float timeScale, TaskCompletionSource<bool> tcs)
    {
        // Set animator vars
        animator.SetTrigger(trigger); // Set trigger
        animator.SetInteger("AnimationType", (int)type); // Set animation type
        animator.speed = timeScale; // Set speed

        // Wait for animation to finish & return completion
        StartCoroutine(AnimationCoroutine(tcs));
        return tcs.Task;
    }

    /// <summary>
    /// Helper coroutine for the animation.
    /// </summary>
    /// <param name="tcs"></param>
    private IEnumerator AnimationCoroutine(TaskCompletionSource<bool> tcs)
    {
        yield return null; // Wait for the animator to update clip

        // Await the length of the animation
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        tcs.SetResult(true);
    }
}
