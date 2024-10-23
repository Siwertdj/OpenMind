using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TransitionAnimator : MonoBehaviour
{
    public static TransitionAnimator i;

    [SerializeField] private Animator animator;

    public enum AnimationType
    {
        Fade = 0,
        Wipe = 1
    }

    void Start()
    {
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

        animator.SetTrigger("SceneLoading");
        animator.SetInteger("AnimationType", (int)type); // Set animation type
        animator.speed = timeScale;

        StartCoroutine(AnimationCoroutine(tcs));

        return tcs.Task;
    }

    public Task PlayEndAnimation(AnimationType type = AnimationType.Fade, float timeScale = 1)
    {
        var tcs = new TaskCompletionSource<bool>();

        animator.SetTrigger("SceneLoaded"); // Set trigger
        animator.SetInteger("AnimationType", (int)type); // Set animation type
        animator.speed = timeScale;

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
