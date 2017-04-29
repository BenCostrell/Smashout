using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTrack;
    private AudioSource audioSrc;
    private AudioLowPassFilter lpf;
    public float lpFadeInDuration;
    public float lpFadeOutDuration;
    private bool lowPass;
    private bool fadeOut;
    public float fadeOutRate;
    private float baseVolume;
    private float elapsed;

    // Use this for initialization
    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        lpf = GetComponent<AudioLowPassFilter>();
        fadeOut = false;
        lowPass = true;
        baseVolume = audioSrc.volume;
    }

    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		//if (fadeOut)
  //      {
  //          audioSrc.volume = Mathf.Max(0, audioSrc.volume - fadeOutRate);
  //          if (audioSrc.volume == 0)
  //          {
  //              audioSrc.Stop();
  //              fadeOut = false;
  //          }
  //      }
    }
    
    public void PlayMainTrack()
    {
        audioSrc.volume = baseVolume;
        audioSrc.clip = mainTrack;
        audioSrc.loop = true;
        audioSrc.Play();
        fadeOut = false;
    }
    
    public void UnpauseMainTrack()
    {
        audioSrc.UnPause();
    }
    public void PauseMainTrack()
    {
        audioSrc.Pause();
    }
    public void StopMainTrack()
    {
        audioSrc.Stop();
    }

    public void FadeOutTrack()
    {
        fadeOut = true;
    }
}
