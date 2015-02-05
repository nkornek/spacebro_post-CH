using UnityEngine;
using System.Collections;

public class AudioControl : MonoBehaviour {

    public AudioSource srcSeqSound, winLoss;
    public AudioClip[] successAudioClips, winLossClips;


    public void playASound(int whichSound)
    {
        srcSeqSound.clip = successAudioClips[whichSound];
        srcSeqSound.Play();
    }

    public void WinorLose(int won)
    {
        winLoss.clip = winLossClips[won];
        winLoss.Play();
    }
}
