using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    [Multiline]
    public string[] tutorialTextArray;
    public GameObject[] tutorialPatterns;
    public GameObject tutorialTextObject;
    public float textScaleInDuration;
    public float waitBetweenStuff;
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
        SetTutorialText setDontFallText = new SetTutorialText(tutorialTextArray[5], tutorialText, textScaleInDuration);
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
        SetTutorialText setInitialText = new SetTutorialText(tutorialTextArray[0], tutorialText, textScaleInDuration);
        WaitForDashes waitForDashes = new WaitForDashes(1);
        WaitForTime wait1 = new WaitForTime(waitBetweenStuff);
        SetTutorialText setDashRefreshText = new SetTutorialText(tutorialTextArray[1], tutorialText, textScaleInDuration);
        WaitForDashes waitForMultipleDashes = new WaitForDashes(5);
        WaitForTime wait2 = new WaitForTime(waitBetweenStuff);
        SetTutorialText setDestroyBlockText = new SetTutorialText(tutorialTextArray[2], tutorialText, textScaleInDuration);
        WaitForTime wait3 = new WaitForTime(waitBetweenStuff);
        SpawnTutorialPattern spawnNormalPlats = new SpawnTutorialPattern(tutorialPatterns[0], true);
        SetTutorialText setDestroyPowerBlockText = new SetTutorialText(tutorialTextArray[3], tutorialText, textScaleInDuration);
        WaitForTime wait4 = new WaitForTime(waitBetweenStuff);
        SpawnTutorialPattern spawnPowerBlocks = new SpawnTutorialPattern(tutorialPatterns[1], true);
        SetTutorialText setStartGameText = new SetTutorialText(tutorialTextArray[4], tutorialText, textScaleInDuration);
        WaitForTime wait5 = new WaitForTime(waitBetweenStuff);
        SpawnTutorialPattern spawnStartPlats = new SpawnTutorialPattern(tutorialPatterns[2], true);
        SetTutorialText setDontFallText = new SetTutorialText(tutorialTextArray[5], tutorialText, textScaleInDuration);
        WaitForTime wait6 = new WaitForTime(waitBetweenStuff*2);
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
