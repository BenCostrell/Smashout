using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Color[] playerColors;
    public Vector3[] spawnpoints;
    
    // Use this for initialization
	void Awake () {
        InitializeServices();
	}
	
    void Start()
    {
        Services.EventManager.Register<Reset>(Reset);
        Services.EventManager.Register<GameOver>(GameOver);
        Services.BlockManager.GenerateLevel();
        InitializePlayers();
        Services.UIManager.SetUpUI();
    }

	// Update is called once per frame
	void Update () {
        Services.InputManager.GetInput();
        Services.TaskManager.Update();
	}

    void InitializeServices()
    {
        Services.GameManager = this;
        Services.EventManager = new EventManager();
        Services.TaskManager = new TaskManager();
        Services.PrefabDB = Resources.Load<PrefabDB>("Prefabs/PrefabDB");
        Services.BlockManager = GameObject.FindGameObjectWithTag("BlockManager").GetComponent<BlockManager>();
        Services.UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        Services.InputManager = new InputManager();
    }

    void Reset(Reset e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void GameOver(GameOver e)
    {

    }

    void InitializePlayers()
    {
        players = new Player[2];
        players[0] = InitializePlayer(1);
        players[1] = InitializePlayer(2);
    }

    Player InitializePlayer(int num)
    {
        Player newPlayer = Instantiate(Services.PrefabDB.Player, spawnpoints[num-1], Quaternion.identity).GetComponent<Player>();
        newPlayer.color = playerColors[num - 1];
        newPlayer.playerNum = num;
        return newPlayer;
    }
}
