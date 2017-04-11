using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public ReticleController[] reticles;
    public int numPlayers;
    public Color[] playerColors;
    public Gradient[] trailColors;
	//public Color[] fireStartColors;
	public Gradient[] fireColors;
    public Color[] bumpColors;
    public RenderTexture[] reticleRenderTextures;
    private GameObject canvas;
    [Space(10)]
    public Vector3[] spawnpoints;
    public bool customSpawns;
    public bool shufflePlayerSpawns;
    [Space(10)]
    public LevelQueue levelQueue;
    public bool playQueueInOrder { get { return _orderedQueue; } set { _orderedQueue = value; levels = (value ? levelQueue.levels : levelQueue.levels.shuffle()); currentLevel = levels.GetEnumerator(); } }
    [SerializeField]
    private bool _orderedQueue;
    [Space(10)]
    public bool gameStarted;

    private LevelQueue.Levels levels;
    private LevelQueue.Levels.Enumerator currentLevel;

    // Use this for initialization
    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoad;
        InitializeServices();
    }

    void Start()
    {
        //should initialize levels and currentLevel
        playQueueInOrder = playQueueInOrder;
        currentLevel.MoveNext();

        Cursor.visible = false;

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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoad;
    }

    void InitializeServices()
    {
        Services.GameManager = this;
        Services.EventManager = new EventManager();
        Services.TaskManager = new TaskManager();
        Services.PrefabDB = Resources.Load<PrefabDB>("Prefabs/PrefabDB");
        //Services.BlockManager = GameObject.FindGameObjectWithTag("BlockManager").GetComponent<BlockManager>();
        Services.UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        Services.InputManager = new InputManager();
    }

    void StartGame()
    {
        SceneManager.LoadScene(currentLevel.Current, LoadSceneMode.Additive);
    }

    void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        if (s == SceneManager.GetSceneByName("main")) return;
        Services.BlockManager = FindObjectOfType<BlockManager>();
        Services.BlockManager.GenerateLevel();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        InitializePlayers();
        gameStarted = true;
    }

    void Reset(Reset e)
    {
        SceneManager.LoadScene("main",LoadSceneMode.Single);
        gameStarted = false;
    }

    public void LevelReset()
    {
        foreach (Player p in players) Destroy(p.gameObject);
        foreach (ReticleController reticle in reticles) Destroy(reticle.gameObject);
        Services.BlockManager.DestroyAllBlocks(false);
        Services.UIManager.SetUpUI();
        Services.EventManager.Register<GameOver>(GameOver);
        StartGame();
    }

    public void NextLevel()
    {
        foreach (Player p in players) Destroy(p.gameObject);
        foreach (ReticleController reticle in reticles) Destroy(reticle.gameObject);
        SceneManager.UnloadSceneAsync(currentLevel.Current);
        if (!currentLevel.MoveNext())
        {
            //reset currentLevel and reshuffle if need be
            currentLevel.Reset();
            currentLevel.MoveNext();
        }
        foreach (string l in levels) Debug.Log(l);
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
        reticles = new ReticleController[numPlayers];
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
		newPlayer.fireColor = fireColors [num - 1];
        newPlayer.playerNum = num;
        newPlayer.transform.GetComponentInChildren<Camera>().targetTexture = reticleRenderTextures[num - 1];

        ReticleController reticle = Instantiate(Services.PrefabDB.Reticle, canvas.transform).GetComponent<ReticleController>();
        reticle.GetComponent<RectTransform>().localScale = 1.5f * Vector3.one;
        reticle.InitializeReticle(newPlayer, reticleRenderTextures[num - 1]);
        reticles[num - 1] = reticle;

        return newPlayer;
    }
}
