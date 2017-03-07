using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public GameObject playerPrefab;
	private Player player1;
	private Player player2;
	public Vector3 spawnPoint_P1;
	public Vector3 spawnPoint_P2;
	private BlockManager blockManager;
	public bool gameOver;
	public GameObject gameOverText;
	public float gameOverTextTweenDuration;

	// Use this for initialization
	void Start () {
		InitializePlayers ();
		blockManager = GameObject.FindGameObjectWithTag ("BlockManager").GetComponent<BlockManager> ();
		gameOver = false;
		gameOverText.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetButtonDown ("Reset")) {
			SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
		}
	}

	void InitializePlayers(){
		player1 = InitializePlayer (1);
		player2 = InitializePlayer (2);
	}

	Player InitializePlayer(int playerNum){
		GameObject playerObj;
		Player player;
		Color defColor = Color.white;
		playerObj = Instantiate (playerPrefab, Vector3.zero, Quaternion.identity) as GameObject;
		player = playerObj.GetComponent<Player> ();
		player.playerNum = playerNum;
		if (playerNum == 1) {
			player.transform.position = spawnPoint_P1;
			playerObj.GetComponentsInChildren<SpriteRenderer> () [1].color = Color.green;
			defColor = new Color(0.5f, 1, 0.5f);
		}
		else if (playerNum == 2) {
			player.transform.position = spawnPoint_P2;
			playerObj.GetComponentsInChildren<SpriteRenderer> () [1].color = Color.blue;
			defColor = new Color(0.5f, 0.5f, 1);
		}
		playerObj.GetComponentsInChildren<SpriteRenderer> () [2].color = defColor;
		player.defaultColor = defColor;

		return player;
	}

	public void GameOver (int playerNum){
		Debug.Log ("game over");
		blockManager.DestroyAllBlocks ();
		StartCoroutine (WaitToShowText (playerNum));
	}

	IEnumerator WaitToShowText(int playerNum){
		yield return new WaitForSeconds(blockManager.blockDeathTime);
		ShowGameOverText(playerNum);
	}

	void ShowGameOverText(int playerNum){
		Color textColor = Color.white;
		string winText = "whoops";
		if (playerNum == 1) {
			textColor = Color.blue;
			winText = "BLUE";
		} else if (playerNum == 2) {
			textColor = Color.green;
			winText = "GREEN";
		}
		Text[] textObjs= gameOverText.GetComponentsInChildren<Text>();
		textObjs [0].color = textColor;
		textObjs [1].color = textColor;
		textObjs [0].text = winText + " WINS";
		gameOverText.SetActive (true);
		iTween.ScaleFrom(gameOverText, iTween.Hash("scale", 0f * Vector3.one, "duration", gameOverTextTweenDuration,
			"easetype", iTween.EaseType.easeOutExpo));
	}
}
