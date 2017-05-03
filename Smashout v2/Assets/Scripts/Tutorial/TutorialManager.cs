using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public string[] tutorialTextArray;
    public GameObject[] tutorialPatterns;
    public GameObject tutorialTextObject;
    private TextMesh tutorialText;

	// Use this for initialization
	void Start () {
        tutorialText = tutorialTextObject.GetComponent<TextMesh>();
        StartTutorialSequence();
	}
	
	// Update is called once per frame
	void Update () {
        Services.GameManager.tutorialTaskManager.Update();
	}

    void StartTutorialSequence()
    {
        SetTutorialText setInitialText = new SetTutorialText(tutorialTextArray[0], tutorialText);
        WaitForDashes waitForDashes = new WaitForDashes();
        SetTutorialText setDestroyBlockText = new SetTutorialText(tutorialTextArray[1], tutorialText);
        SpawnTutorialPattern spawnNormalPlats = new SpawnTutorialPattern(tutorialPatterns[0], true);
        SetTutorialText setDestroyPowerBlockText = new SetTutorialText(tutorialTextArray[2], tutorialText);
        SpawnTutorialPattern spawnPowerBlocks = new SpawnTutorialPattern(tutorialPatterns[1], true);
        SetTutorialText setStartGameText = new SetTutorialText(tutorialTextArray[3], tutorialText);
        SpawnTutorialPattern spawnHelperPlatforms = new SpawnTutorialPattern(tutorialPatterns[2], false);

        setInitialText
            .Then(waitForDashes)
            .Then(setDestroyBlockText)
            .Then(spawnNormalPlats)
            .Then(setDestroyPowerBlockText)
            .Then(spawnPowerBlocks)
            .Then(setStartGameText)
            .Then(spawnHelperPlatforms);

        Services.GameManager.tutorialTaskManager.AddTask(setInitialText);
        
    }
}
