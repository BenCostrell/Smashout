using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public GameObject title;
    public GameObject startPrompt;
    public GameObject congrats;
    public GameObject restartPrompt;
    public GameObject matchCount;

    public float titleScaleInTime;
    public float congratsScaleInTime;
    
    // Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetUpUI()
    {
        title.SetActive(false);
        startPrompt.SetActive(false);
        congrats.SetActive(false);
        restartPrompt.SetActive(false);
        matchCount.SetActive(false);
    }
}
