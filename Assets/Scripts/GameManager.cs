using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public GameObject playerPrefab;
	private Player player1;
	private Player player2;
	public Vector3 spawnPoint_P1;
	public Vector3 spawnPoint_P2;

	// Use this for initialization
	void Start () {
		InitializePlayers ();
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
}
