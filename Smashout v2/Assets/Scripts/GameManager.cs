using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public int matchSet = 3;
    [Space(10)]

    public LevelQueue levelQueue;
    public bool playQueueInOrder { get { return _orderedQueue; } set { _orderedQueue = value; levels = (value ? levelQueue.levels : levelQueue.levels.shuffle()); currentLevel = levels.GetEnumerator(); } }
    [SerializeField]
    private bool _orderedQueue;
    [Space(10)]

    public int numPlayers;
    public Color[] playerColors;
    public Color[] stunColors;
	public Gradient[] fireColors;
    public Color[] bumpColors;
    public RenderTexture[] reticleRenderTextures;
    private GameObject canvas;
    [Space(10)]

    public Vector3[] spawnpoints;
    public bool customSpawns;
    public bool shufflePlayerSpawns;

    private AudioSource audioSrc;
    public AudioClip deathClip;
   

    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public ReticleController[] reticles;
    [HideInInspector]
    public bool gameStarted;
    [HideInInspector]
    public int blueTrack = 0;
    [HideInInspector]
    public int greenTrack = 0;
    [HideInInspector]
    public bool won = false;
    [HideInInspector]
    public int round = 0;
    [Space(10)]

    public GameObject musicManagerPrefab;
	public bool preMatch;
	public string preMatchName;
	public float preMatchTransitionDur;
    public float preMatchDissapearPercent;
	private bool runPreMatch;
    private bool oneTimePreMatch;
    private bool[] readyList;
    [Space(10)]

    private ScaleInMatch script;

    private LevelQueue.Levels levels;
    private LevelQueue.Levels.Enumerator currentLevel;

    public float roundThreshold;
    public float durationRoundTask;

    private static bool musicManagerLoaded = false;

    private MusicManager muse;

    // Use this for initialization
    void Awake() {
        SceneManager.sceneLoaded += OnSceneLoad;
        InitializeServices();
        audioSrc = GetComponent<AudioSource>();
        muse = Services.MusicManager;
    }

    void Start()
    {
        script = Services.UIManager.matchCount.GetComponent<ScaleInMatch>();
        script.blueRound = 0;
        script.greenRound = 0;
        //should initialize levels and currentLevel
        playQueueInOrder = playQueueInOrder;
        currentLevel.MoveNext();

		if (preMatch)
		{
			runPreMatch = true;
            readyList = new bool[numPlayers];
            setAllReadyFalse ();
		}

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
        ///temporary, just play music all the time
        if (!musicManagerLoaded)
        {
            muse.PlayMainTrack();
            musicManagerLoaded = true;
        }
    }

	// Update is called once per frame
	void Update () {
        Services.InputManager.GetInput();
        Services.TaskManager.Update();
		checkIsReady ();
	}

	//This is the method that is used for checking if
	//Both players hit the start block
	void checkIsReady()
	{
		if (preMatch)
		{
            foreach(bool ready in readyList)
            {
                if (ready == false) return;
            }
			setAllReadyFalse ();
			Services.BlockManager.DestroyAllBlocks(true);
            foreach (Player p in players)
            {
                p.LockAllInput();
                p.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;

            }
            GameObject[] startText;
            startText = GameObject.FindGameObjectsWithTag("PreMatchText");
            foreach (GameObject text in startText)
            {
                Destroy(text);
            }
            //foreach (ReticleController reticle in reticles) Destroy(reticle.gameObject);
            gameStarted = false;
            //SceneManager.UnloadSceneAsync(currentLevel.Current);
            preMatchTransition transition = new preMatchTransition(preMatchTransitionDur, preMatchDissapearPercent);
            LPFadeTask lpfade = new LPFadeTask(muse.GetComponent<AudioLowPassFilter>(), 1000, 22000, 1, 1, muse.lpFadeOutDuration, Easing.QuadEaseIn);
            Services.TaskManager.AddTask(transition);
            Services.TaskManager.AddTask(lpfade);
		}
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
        Services.BlockManager = gameObject.transform.GetComponentInChildren<BlockManager>();
        Services.UIManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        Services.InputManager = new InputManager();
        if (!musicManagerLoaded)
        {
            MusicManager mm = Instantiate(musicManagerPrefab).GetComponent<MusicManager>();
            Services.MusicManager = mm;
            DontDestroyOnLoad(mm.gameObject);
        }
    }

    public void StartGame()
    {
        round++;
		if (won == true) {
			round = 0;
			greenTrack = 0;
			blueTrack = 0;
			won = false;

			Start ();
		} else if (runPreMatch)
		{
			SceneManager.LoadScene (preMatchName, LoadSceneMode.Additive);
			round = 0;
			runPreMatch = false;
		}
        else
        {
			Debug.Log ("StartGame load level scene");
            SceneManager.LoadScene(currentLevel.Current, LoadSceneMode.Additive);
        }
        
    }

    void OnSceneLoad(Scene s, LoadSceneMode m)
    {
        Debug.Log("Loaded " + s.name);

        transform.Find("DefaultBlockManager").gameObject.SetActive(false);
        if (s == SceneManager.GetSceneByName("main")) return;
        Camera.main.GetComponent<CameraController>().SetLight(false);
        Services.BlockManager = FindObjectOfType<BlockManager>();
        if (Services.BlockManager == null)
        {
            //Debug.Log("No Block Manager detected. Using Default.");
            transform.Find("DefaultBlockManager").gameObject.SetActive(true);
            Services.BlockManager = GetComponentInChildren<BlockManager>();
        }
        Services.BlockManager.GenerateLevel();
        canvas = GameObject.FindGameObjectWithTag("Canvas");
        InitializePlayers();
        Services.UIManager.matchCount.SetActive(true);
        //Services.MusicManager.UnpauseMainTrack();
        gameStarted = true;
    }

    void Reset(Reset e)
    {
        SceneManager.LoadScene("main",LoadSceneMode.Single);
        gameStarted = false;
    }

    public void LevelReset()
    {
        foreach (Player p in players)
        {
            p.LockAllInput();
            Destroy(p.gameObject);
        }
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
        //foreach (string l in levels) Debug.Log(l);
        Services.UIManager.SetUpUI();
        Services.EventManager.Register<GameOver>(GameOver);

        StartGame();
    }

    void GameOver(GameOver e)
    {
        GetComponent<AudioSource>().Play();
        Services.EventManager.Unregister<GameOver>(GameOver);
        Services.BlockManager.DestroyAllBlocks(true);
        RoundTask roundTask = new RoundTask(3 - e.losingPlayer, durationRoundTask, roundThreshold);
        ScaleInCongrats scaleInCongrats = new ScaleInCongrats(3 - e.losingPlayer);
        WaitForTime waitForBlocksToDie = new WaitForTime(Services.BlockManager.blockTypes[0].GetComponent<Block>().deathTime);
        LPFadeTask lowPassFadeIn = new LPFadeTask(muse.GetComponent<AudioLowPassFilter>(), 22000, 1000, 1, 1, muse.lpFadeInDuration, Easing.QuadEaseOut);
        WaitToRestart waitToRestart = new WaitToRestart();
        gameStarted = false;
        if((blueTrack == matchSet/2 && (3 - e.losingPlayer) == 1) || (greenTrack == matchSet/2 && (3 - e.losingPlayer) == 2))
        {
            roundTask
                .Then(waitForBlocksToDie);

            lowPassFadeIn
                .Then(waitToRestart);

            Services.TaskManager.AddTask(lowPassFadeIn);
        }
        else
        {
            roundTask
                .Then(waitForBlocksToDie)
                .Then(waitToRestart);
        }

        Camera.main.GetComponent<CameraController>().SetLight(true);

        Services.TaskManager.AddTask(roundTask);
        Services.TaskManager.AddTask(scaleInCongrats);
        //Services.MusicManager.PauseMainTrack();


        //Services.MusicManager.FadeOutTrack();
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
		newPlayer.fireColor = fireColors [num - 1];
        newPlayer.playerNum = num;
        newPlayer.transform.GetComponentInChildren<Camera>().targetTexture = reticleRenderTextures[num - 1];

        ReticleController reticle = Instantiate(Services.PrefabDB.Reticle, canvas.transform).GetComponent<ReticleController>();
        reticle.GetComponent<RectTransform>().localScale = 1.5f * Vector3.one;
        reticle.InitializeReticle(newPlayer, reticleRenderTextures[num - 1]);
        reticles[num - 1] = reticle;

        return newPlayer;
    }

	void setAllReadyFalse()
	{
        for(int i = 0; i < readyList.Length; i++)
        {
            readyList[i] = false;
        }
	}

    public void setReady(int blockNum)
    {
        readyList[blockNum - 1] = true;
    }
}
