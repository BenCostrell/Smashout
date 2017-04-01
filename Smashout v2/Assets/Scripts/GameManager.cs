using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Color[] playerColors;
    public Color[] bumpColors;
    public Vector3[] spawnpoints;
    public bool gameStarted;
    
    // Use this for initialization
	void Awake () {
        InitializeServices();
	}
	
    void Start()
    {
        Services.EventManager.Register<Reset>(Reset);
        Services.EventManager.Register<GameOver>(GameOver);
        Services.UIManager.SetUpUI();
        gameStarted = false;

        ScaleInTitle scaleInTitle = new ScaleInTitle();
        WaitToStart waitToStart = new WaitToStart();
        ActionTask startGame = new ActionTask(StartGame);

        scaleInTitle
            .Then(waitToStart)
            .Then(startGame);

        Services.TaskManager.AddTask(scaleInTitle);
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

    void StartGame()
    {
        Services.BlockManager.GenerateLevel();
        InitializePlayers();
        gameStarted = true;
    }

    void Reset(Reset e)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SoftReset()
    {
        Destroy(players[0].gameObject);
        Destroy(players[1].gameObject);
        Services.BlockManager.DestroyAllBlocks(false);
        Services.UIManager.SetUpUI();
        Services.EventManager.Register<GameOver>(GameOver);
        StartGame();
    }

    void GameOver(GameOver e)
    {
        Services.EventManager.Unregister<GameOver>(GameOver);
        Services.BlockManager.DestroyAllBlocks(true);
        ScaleInCongrats scaleInCongrats = new ScaleInCongrats(3 - e.losingPlayer);
        WaitToRestart waitToRestart = new WaitToRestart();

        scaleInCongrats
            .Then(waitToRestart);

        Services.TaskManager.AddTask(scaleInCongrats);
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
