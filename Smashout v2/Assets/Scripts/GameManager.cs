using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player player1;
    public Player player2;

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
        Services.BlockManager = new BlockManager();
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
        player1 = InitializePlayer(1);
        player2 = InitializePlayer(2);
    }

    Player InitializePlayer(int num)
    {
        Player newPlayer = Instantiate(Services.PrefabDB.Player, spawnpoints[num-1], Quaternion.identity).GetComponent<Player>();
        newPlayer.playerNum = num;
        return newPlayer;
    }
}
