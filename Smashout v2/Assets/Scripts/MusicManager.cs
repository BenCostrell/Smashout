using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public AudioClip mainTrack;
    private AudioSource audioSrc;
    private bool fadeOut;
    public float fadeOutRate;
    private float baseVolume;

    // Use this for initialization
    void Awake()
    {
        audioSrc = GetComponent<AudioSource>();
        fadeOut = false;
        baseVolume = audioSrc.volume;
    }

    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (fadeOut)
        {
            audioSrc.volume = Mathf.Max(0, audioSrc.volume - fadeOutRate);
            if (audioSrc.volume == 0)
            {
                audioSrc.Stop();
                fadeOut = false;
            }
        }
	}

    public void PlayMainTrack()
    {
        Debug.Log("playing");
        audioSrc.volume = baseVolume;
        audioSrc.clip = mainTrack;
        audioSrc.loop = true;
        audioSrc.Play();
        fadeOut = false;
    }

    public void FadeOutTrack()
    {
        fadeOut = true;
    }
}
