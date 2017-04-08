using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public Player[] players;
    public Color[] playerColors;
    public Gradient[] trailColors;
    public Color[] bumpColors;
    public Vector3[] spawnpoints;
    public bool customSpawns;
    public bool shufflePlayerSpawns;
    public int numPlayers;
    public LevelQueue levels;
    public bool playQueueInOrder;
    public bool gameStarted;

    // Use this for initialization
    void Awake() {
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

        if (!customSpawns) //if spawnpoints are to be generated, generate them equally spaced from each other
        {
            spawnpoints = new Vector3[numPlayers];
            for (int i = 0; i < numPlayers; ++i) spawnpoints[i] = new Vector3(i * 200.0f / (numPlayers - 1) - 100.0f, 100);
        }

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
        foreach(Player p in players) Destroy(p.gameObject);
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
        gameStarted = false;

        scaleInCongrats
            .Then(waitToRestart);

        Services.TaskManager.AddTask(scaleInCongrats);
    }

    void InitializePlayers()
    {
        players = new Player[numPlayers];
        if (shufflePlayerSpawns)
        {
            int i = 0;
            foreach (int j in new UniqueRandomSample(0, numPlayers))
            {
                players[i] = InitializePlayer(i + 1, j);
                ++i;
            }
        }
        else for (int i = 0; i < numPlayers; ++i) players[i] = InitializePlayer(i + 1);
    }

    Player InitializePlayer(int num, int spawnNum = -1)
    {
        if (spawnNum == -1) spawnNum = num - 1;
        Player newPlayer = Instantiate(Services.PrefabDB.Player, spawnpoints[spawnNum], Quaternion.identity).GetComponent<Player>();
        newPlayer.color = playerColors[num - 1];
        newPlayer.trailColor = trailColors[num - 1];
        newPlayer.playerNum = num;
        return newPlayer;
    }
}
