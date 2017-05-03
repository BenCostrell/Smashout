using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [Multiline]
    public string[] tutorialTextArray;
    public GameObject[] tutorialPatterns;
    public GameObject tutorialTextObject;
    private TextMesh tutorialText;

	// Use this for initialization
	void Start () {
        tutorialText = tutorialTextObject.GetComponent<TextMesh>();
        if (Services.GameManager.tutorialOn)
        {
            StartTutorialSequence();
            Services.GameManager.tutorialOn = false;
        }
        else
        {
            StartNonTutorialSequence();
        }
	}
	
	// Update is called once per frame
	void Update () {
        Services.GameManager.tutorialTaskManager.Update();
	}

    void StartNonTutorialSequence()
    {
        SpawnTutorialPattern spawnStartPlats = new SpawnTutorialPattern(tutorialPatterns[2], true);
        SetTutorialText setDontFallText = new SetTutorialText(tutorialTextArray[5], tutorialText);
        WaitForTime wait6 = new WaitForTime(0.8f);
        ActionTask beginGame = new ActionTask(Services.GameManager.BeginMatch);

        spawnStartPlats
                    .Then(setDontFallText)
                    .Then(wait6)
                    .Then(beginGame);
        Services.GameManager.tutorialTaskManager.AddTask(spawnStartPlats);
    }

    void StartTutorialSequence()
    {
        SetTutorialText setInitialText = new SetTutorialText(tutorialTextArray[0], tutorialText);
        WaitForDashes waitForDashes = new WaitForDashes(1);
        WaitForTime wait1 = new WaitForTime(0.5f);
        SetTutorialText setDashRefreshText = new SetTutorialText(tutorialTextArray[1], tutorialText);
        WaitForDashes waitForMultipleDashes = new WaitForDashes(5);
        WaitForTime wait2 = new WaitForTime(0.5f);
        SetTutorialText setDestroyBlockText = new SetTutorialText(tutorialTextArray[2], tutorialText);
        WaitForTime wait3 = new WaitForTime(0.5f);
        SpawnTutorialPattern spawnNormalPlats = new SpawnTutorialPattern(tutorialPatterns[0], true);
        SetTutorialText setDestroyPowerBlockText = new SetTutorialText(tutorialTextArray[3], tutorialText);
        WaitForTime wait4 = new WaitForTime(0.5f);
        SpawnTutorialPattern spawnPowerBlocks = new SpawnTutorialPattern(tutorialPatterns[1], true);
        SetTutorialText setStartGameText = new SetTutorialText(tutorialTextArray[4], tutorialText);
        WaitForTime wait5 = new WaitForTime(0.5f);
        SpawnTutorialPattern spawnStartPlats = new SpawnTutorialPattern(tutorialPatterns[2], true);
        SetTutorialText setDontFallText = new SetTutorialText(tutorialTextArray[5], tutorialText);
        WaitForTime wait6 = new WaitForTime(0.65f);
        ActionTask beginGame = new ActionTask(Services.GameManager.BeginMatch);

        setInitialText
            .Then(waitForDashes)
            .Then(wait1)
            .Then(setDashRefreshText)
            .Then(waitForMultipleDashes)
            .Then(wait2)
            .Then(setDestroyBlockText)
            .Then(wait3)
            .Then(spawnNormalPlats)
            .Then(setDestroyPowerBlockText)
            .Then(wait4)
            .Then(spawnPowerBlocks)
            .Then(setStartGameText)
            .Then(wait5)
            .Then(spawnStartPlats)
            .Then(setDontFallText)
            .Then(wait6)
            .Then(beginGame);

        Services.GameManager.tutorialTaskManager.AddTask(setInitialText);
        
    }
}
