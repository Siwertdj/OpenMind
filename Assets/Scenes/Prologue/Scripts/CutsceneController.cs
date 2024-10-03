using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class CutsceneController : MonoBehaviour
{
    public PlayableDirector playableDirector;
    public void PauseTimeline()
    {
        playableDirector.Pause();
    }

    public void ContinueTimeline()
    {
        playableDirector.Play();
    }
}
