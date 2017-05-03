using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour {

    public string[] tutorialTextArray;
    public GameObject[] tutorialPatterns;
    public GameObject tutorialTextObject;
    private TextMesh tutorialText;
    public float initialDelay;
    public float textAnimDuration;
    public float blockAppearDelay;
    public Easing.Function ease = Easing.QuadEaseIn;

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
        SetTutorialText wait = new SetTutorialText("", tutorialText, initialDelay, 0, ease);
        SetTutorialText setInitialText = new SetTutorialText(tutorialTextArray[0], tutorialText, textAnimDuration, blockAppearDelay, ease);
        WaitForDashes waitForDashes = new WaitForDashes();
        SetTutorialText setDestroyBlockText = new SetTutorialText(tutorialTextArray[1], tutorialText, textAnimDuration, blockAppearDelay, ease);
        SpawnTutorialPattern spawnNormalPlats = new SpawnTutorialPattern(tutorialPatterns[0], true);
        SetTutorialText setDestroyPowerBlockText = new SetTutorialText(tutorialTextArray[2], tutorialText, textAnimDuration, blockAppearDelay, ease);
        SpawnTutorialPattern spawnPowerBlocks = new SpawnTutorialPattern(tutorialPatterns[1], true);
        SetTutorialText setStartGameText = new SetTutorialText(tutorialTextArray[3], tutorialText, textAnimDuration, blockAppearDelay, ease);
        SpawnTutorialPattern spawnHelperPlatforms = new SpawnTutorialPattern(tutorialPatterns[2], false);

        wait
            .Then(setInitialText)
            .Then(waitForDashes)
            .Then(setDestroyBlockText)
            .Then(spawnNormalPlats)
            .Then(setDestroyPowerBlockText)
            .Then(spawnPowerBlocks)
            .Then(setStartGameText)
            .Then(spawnHelperPlatforms);

        Services.GameManager.tutorialTaskManager.AddTask(wait);
        
    }
}
