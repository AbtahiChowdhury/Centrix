using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private PlayerInput playerInput;
    private bool isPaused = false;
    public float xSensitivity { get; private set; }
    public float ySensitivity { get; private set; }
    private float musicDuration;

    public static GameManager Instance { get { return instance; } }

    public AudioClip[] audioClip;
    AudioSource audioSource;
    AudioListener audioListener;

    public GameObject spawnerPrefab;
    public GameObject bulletPrefab;
    public GameObject bombPrefab;

    public GameObject hud;
    public GameObject pauseMenu;
    public GameObject endOfLevel;

    public bool disableRandomBulletSpawning { get; set; }
    public float speed { get; set; }
    public int bulletsHit { get; set; }
    public int bulletsFired { get; set; }
    private int bombs;
    private float accuracy;
    private int finalScore;
    private bool disablePausing = false;

    public TextMeshProUGUI accuracyText;
    public TextMeshProUGUI finalScoreText;

    float BPM;
    float secPerBeat;
    float dsptimesong;
    float songPosition;

    public float spawnerRotationSpeed { get; set; }
    int numberOfSpawners;
    static GameObject[] spawnerArray;

    private void Popup()
    {
        if (songPosition > musicDuration)
        {
            disablePausing = true;
            finalScore = (int)Mathf.Lerp(0, 1000000f, accuracy);
            hud.gameObject.SetActive(false);
            finalScoreText.text = "Score: " + finalScore;
            endOfLevel.gameObject.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        } 
    }

    private void EnablePausing()
    {
        if (playerInput.pausing && !disablePausing)
        {
            AudioListener.pause = !AudioListener.pause;
            Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
            Cursor.visible = !Cursor.visible;
            if (Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
            }
            Player.instance.GetComponent<PlayerMovement>().paused = !Player.instance.GetComponent<PlayerMovement>().paused;
            isPaused = !isPaused;
            pauseMenu.gameObject.SetActive(isPaused);
            hud.gameObject.SetActive(!isPaused);
        }
    }

    public static GameObject[] getSpawners ()
    {
        return spawnerArray;
    }

    struct ToggleRandomBulletSpawningEventParameters
    {
        public float beat { get; set; }

        public ToggleRandomBulletSpawningEventParameters(float beat)
        {
            this.beat = beat;
        }
    }
    ArrayList toggleRandomBulletSpawningEvents;
    int toggleRandomBulletSpawningEventsIndex;

    struct SpawnerRotationSpeedEventParameters
    {
        public float beat { get; set; }
        public float speed { get; set; }

        public SpawnerRotationSpeedEventParameters(float beat, float speed)
        {
            this.beat = beat;
            this.speed = speed;
        }
    }
    ArrayList spawnerRotationSpeedEvents;
    int spawnerRotationSpeedEventsIndex;

    public float bulletSpeed { get; set; }
    struct BulletSpeedEventParameters
    {
        public float beat { get; set; }
        public float speed { get; set; }

        public BulletSpeedEventParameters(float beat, float speed)
        {
            this.beat = beat;
            this.speed = speed;
        }
    }
    ArrayList bulletSpeedEvents;
    int bulletSpeedEventsIndex;

    struct BulletSpawnEventParameters
    {
        public float beat { get; set; }
        public int spawner { get; set; }
        public float theta { get; set; }

        public BulletSpawnEventParameters(float beat, int spawner, float theta)
        {
            this.beat = beat;
            this.spawner = spawner;
            this.theta = theta;
        }
    }
    ArrayList bulletSpawnEvents;
    int bulletSpawnEventsIndex;

    struct AudioSyncerBiasChangeEventParameters
    {
        public float beat { get; set; }
        public float bias { get; set; }

        public AudioSyncerBiasChangeEventParameters(float beat, float bias)
        {
            this.beat = beat;
            this.bias = bias;
        }
    }
    ArrayList audioSyncerBiasChangeEvents;
    int audioSyncerBiasChangeEventsIndex;

    struct AudioSyncerTimeStepChangeEventParameters
    {
        public float beat { get; set; }
        public float timestep { get; set; }

        public AudioSyncerTimeStepChangeEventParameters(float beat, float timestep)
        {
            this.beat = beat;
            this.timestep = timestep;
        }
    }
    ArrayList audioSyncerTimeStepChangeEvents;
    int audioSyncerTimeStepChangeEventsIndex;

    public int difficulty;
    public int levelIndex;
    public float songPosInBeats;

    public int numberOfSamples;
    public float[] spectrum { get; set; }

    public bool debugMode;
    public float startTimeInSeconds;


    private void Awake()
    {
        playerInput = Player.instance.GetComponent<PlayerInput>();
        xSensitivity = ySensitivity = 100.0f;
        if (instance != null && instance !=this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Game")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        audioListener = GetComponent<AudioListener>();
        
        spectrum = new float[numberOfSamples];

        audioSource = GetComponent<AudioSource>();
        disableRandomBulletSpawning = false;
        speed = 1f;
        bulletsFired = 1;
        bulletsHit = 0;
        bombs = 3;
        UpdateHUD();

        SetUpLevel();

        if (debugMode)
        {
            secPerBeat = 60f / BPM;
            dsptimesong = (float)AudioSettings.dspTime;
            audioSource.time = startTimeInSeconds;
            songPosInBeats = startTimeInSeconds / secPerBeat;
            while (toggleRandomBulletSpawningEventsIndex < toggleRandomBulletSpawningEvents.Count && songPosInBeats > ((ToggleRandomBulletSpawningEventParameters)toggleRandomBulletSpawningEvents[toggleRandomBulletSpawningEventsIndex]).beat)
            {
                disableRandomBulletSpawning = !disableRandomBulletSpawning;
                toggleRandomBulletSpawningEventsIndex++;
            }
            while (bulletSpeedEventsIndex < bulletSpeedEvents.Count && songPosInBeats > ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).beat)
            {
                bulletSpeed = ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).speed;
                bulletSpeedEventsIndex++;
            }
            while (spawnerRotationSpeedEventsIndex < spawnerRotationSpeedEvents.Count && songPosInBeats > ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).beat)
            {
                spawnerRotationSpeed = ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).speed;
                spawnerRotationSpeedEventsIndex++;
            }
            while (bulletSpawnEventsIndex < bulletSpawnEvents.Count && songPosInBeats > ((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).beat)
            {
                bulletSpawnEventsIndex++;
            }
            while (audioSyncerBiasChangeEventsIndex < audioSyncerBiasChangeEvents.Count && songPosInBeats > ((AudioSyncerBiasChangeEventParameters)audioSyncerBiasChangeEvents[audioSyncerBiasChangeEventsIndex]).beat)
            {
                GetComponent<AudioSyncer>().bias = ((AudioSyncerBiasChangeEventParameters)audioSyncerBiasChangeEvents[audioSyncerBiasChangeEventsIndex]).bias;
                audioSyncerBiasChangeEventsIndex++;
            }
            while (audioSyncerTimeStepChangeEventsIndex < audioSyncerTimeStepChangeEvents.Count && songPosInBeats > ((AudioSyncerTimeStepChangeEventParameters)audioSyncerTimeStepChangeEvents[audioSyncerTimeStepChangeEventsIndex]).beat)
            {
                GetComponent<AudioSyncer>().timeStep = ((AudioSyncerTimeStepChangeEventParameters)audioSyncerTimeStepChangeEvents[audioSyncerTimeStepChangeEventsIndex]).timestep;
                audioSyncerBiasChangeEventsIndex++;
            }
            audioSource.Play();
        }
        else
        {
            secPerBeat = 60f / BPM;
            dsptimesong = (float)AudioSettings.dspTime;
            GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        EnablePausing();
        Popup();
        if (debugMode)
        {
            songPosition = (float)((AudioSettings.dspTime - dsptimesong) + startTimeInSeconds);
            songPosInBeats = songPosition / secPerBeat;
        }
        else
        {
            songPosition = (float)(AudioSettings.dspTime - dsptimesong);
            songPosInBeats = songPosition / secPerBeat;
        }

        //Debug.Log("" + audioSource.time + " -> " + songPosInBeats);

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float avg = 0;
        for(int i = 0; i < numberOfSamples/4; i++)
        {
            avg += spectrum[i];
        }
        avg /= (numberOfSamples / 4);
        speed = Mathf.Lerp(1f, 3f, avg * 50f);

        UpdateHUD();
        CheckForEvents();

        if (playerInput.bombClear)
        {
            SpawnBomb();
        }
    }

    //For Menu Start/Restart Game
    public void StartGame()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Game");
    }
    public void MainMenu()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("Main Menu");
    }

    public void ExitGame()
    {
        //Close the application        
        Application.Quit();
    }

    void CreateSpawners()
    {
        for(int i = 0; i < numberOfSpawners; i++)
        {
            spawnerArray[i] = Instantiate(spawnerPrefab);
            spawnerArray[i].transform.Rotate(0, 0, i * (360 / numberOfSpawners));
            spawnerArray[i].GetComponent<Spawner>().theta = i * (360 / numberOfSpawners);
        }
    }

    void CheckForEvents()
    {
        //toggle random bullet spawning events
        if (toggleRandomBulletSpawningEventsIndex < toggleRandomBulletSpawningEvents.Count && songPosInBeats > ((ToggleRandomBulletSpawningEventParameters)toggleRandomBulletSpawningEvents[toggleRandomBulletSpawningEventsIndex]).beat)
        {
            disableRandomBulletSpawning = !disableRandomBulletSpawning;
            toggleRandomBulletSpawningEventsIndex++;
        }

        //bullet speed events
        if (bulletSpeedEventsIndex < bulletSpeedEvents.Count && songPosInBeats > ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).beat)
        {
            bulletSpeed = ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).speed;
            bulletSpeedEventsIndex++;
        }

        //spawner rotation speed events
        if (spawnerRotationSpeedEventsIndex < spawnerRotationSpeedEvents.Count && songPosInBeats > ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).beat)
        {
            spawnerRotationSpeed = ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).speed;
            spawnerRotationSpeedEventsIndex++;
        }

        //bullet spawn events
        while (bulletSpawnEventsIndex < bulletSpawnEvents.Count && songPosInBeats > ((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).beat)
        {
            spawnerArray[((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).spawner].GetComponent<Spawner>().SpawnBullet(((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).theta);
            bulletSpawnEventsIndex++;
        }

        //audio syncer bias events
        if (audioSyncerBiasChangeEventsIndex < audioSyncerBiasChangeEvents.Count && songPosInBeats > ((AudioSyncerBiasChangeEventParameters)audioSyncerBiasChangeEvents[audioSyncerBiasChangeEventsIndex]).beat)
        {
            GetComponent<AudioSyncer>().bias = ((AudioSyncerBiasChangeEventParameters)audioSyncerBiasChangeEvents[audioSyncerBiasChangeEventsIndex]).bias;
            audioSyncerBiasChangeEventsIndex++;
        }

        //audio syncer timestep events
        if (audioSyncerTimeStepChangeEventsIndex < audioSyncerTimeStepChangeEvents.Count && songPosInBeats > ((AudioSyncerTimeStepChangeEventParameters)audioSyncerTimeStepChangeEvents[audioSyncerTimeStepChangeEventsIndex]).beat)
        {
            GetComponent<AudioSyncer>().timeStep = ((AudioSyncerTimeStepChangeEventParameters)audioSyncerTimeStepChangeEvents[audioSyncerTimeStepChangeEventsIndex]).timestep;
            audioSyncerBiasChangeEventsIndex++;
        }
    }

    void SetUpLevel()
    {

        toggleRandomBulletSpawningEventsIndex = 0;
        toggleRandomBulletSpawningEvents = new ArrayList();

        spawnerRotationSpeedEventsIndex = 0;
        spawnerRotationSpeedEvents = new ArrayList();

        bulletSpawnEventsIndex = 0;
        bulletSpawnEvents = new ArrayList();

        bulletSpeedEventsIndex = 0;
        bulletSpeedEvents = new ArrayList();

        audioSyncerBiasChangeEventsIndex = 0;
        audioSyncerBiasChangeEvents = new ArrayList();

        audioSyncerTimeStepChangeEventsIndex = 0;
        audioSyncerTimeStepChangeEvents = new ArrayList();

        audioSource.clip = audioClip[levelIndex];
        musicDuration = audioSource.clip.length + 5f;

        switch (levelIndex)
        {
            case 0:
                Level1();
                break;
            case 1:
                Level2();
                break;
            case 2:
                Level3();
                break;
            default:
                break;
        }
    }

    void Level1()
    {
        //Initial level setup
        BPM = 170f;
        bulletSpeed = 1f;
        spawnerRotationSpeed = 10f;
        numberOfSpawners = 6;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();
        GetComponent<AudioSyncer>().bias = 30f;
        GetComponent<AudioSyncer>().timeStep = 0.15f;

        //Spawner movement
        EnqueueSpawnerRotationSpeedEventOverTime(30f, 49f, 10f, 60f);
        EnqueueSpawnerRotationSpeedEvent(49f, -60f);
        EnqueueSpawnerRotationSpeedEvent(113f, 60f);
        EnqueueSpawnerRotationSpeedEventOverTime(176f, 240f, 60f, 5f);
        EnqueueSpawnerRotationSpeedEvent(240.5f, -30f);
        EnqueueSpawnerRotationSpeedEvent(257f, 30f);
        EnqueueSpawnerRotationSpeedEventOverTime(273f, 287f, 30f, 40f);
        EnqueueSpawnerRotationSpeedEventOverTime(287f, 298f, 40f, 70f);
        EnqueueSpawnerRotationSpeedEventOverTime(298f, 304f, -10f, -20f);
        EnqueueSpawnerRotationSpeedEvent(307f, -70f);
        EnqueueSpawnerRotationSpeedEvent(369f, -10f);
        EnqueueSpawnerRotationSpeedEvent(371f, 70f);
        EnqueueSpawnerRotationSpeedEventOverTime(425f, 432f, 20f, 100f);
        EnqueueSpawnerRotationSpeedEventOverTime(434f, 490f, 50f, 0f);

        //Bullet spawning
        EnqueueSurroundPlayer(2f, 46f, 0.5f, 4f, 1);
        EnqueueSurroundPlayer(49f, 112f, 0.5f, 2.25f, -1);
        EnqueueSurroundPlayer(113f, 180f, 0.5f, 2.25f, 1);
        EnqueueSurroundPlayer(176f, 240f, 0.25f, 40f, 7.5f);
        EnqueueSurroundPlayer(240f, 257f, 0.25f, 1.5f, 1);
        EnqueueSurroundPlayer(257f, 270f, 0.25f, 1.5f, -1);
        EnqueueSurroundPlayer(270f, 298f, 0.25f, 35f, 45f);
        EnqueueSurroundPlayer(304f, 369f, 0.15f, 4f, -1);
        EnqueueSurroundPlayer(371f, 425f, 0.15f, 4f, 1);

        //Bullet speed
        EnqueueBulletSpeedEvent(49f, 4f);
        EnqueueBulletSpeedEventOverTime(176f, 240f, 4f, 1.5f);
        EnqueueBulletSpeedEvent(240.5f, 2.5f);
        EnqueueBulletSpeedEvent(257f, 3f);
        EnqueueBulletSpeedEventOverTime(273f, 287f, 3f, 4f);
        EnqueueBulletSpeedEventOverTime(287f, 298f, 4f, 6f);
        EnqueueBulletSpeedEventOverTime(298f, 304f, 1.5f, 2f);
        EnqueueBulletSpeedEvent(307f, 4.5f);
        EnqueueBulletSpeedEvent(369f, 1f);
        EnqueueBulletSpeedEvent(371f, 4.5f);
        EnqueueBulletSpeedEventOverTime(425f, 432f, 3f, 7f);
        EnqueueBulletSpeedEvent(371f, 2f);

        //Toggle Random Bullet Spawning

        //Audio Syncer Bias (init 30f)
        EnqueueChangeAudioSyncerBias(193f, 15f);
        EnqueueChangeAudioSyncerBias(240f, 30f);
        EnqueueChangeAudioSyncerBias(245f, 10f);
        EnqueueChangeAudioSyncerBias(432f, 30f);

        //Audio Syncer Timestep (init 0.15f)
        EnqueueChangeAudioSyncerTimeStep(288f, 0.1f);
        EnqueueChangeAudioSyncerTimeStep(432f, 0.15f);
    }

    void Level2()
    {
        //Initial level setup
        BPM = 140f;
        bulletSpeed = 1f;
        spawnerRotationSpeed = 15f;
        numberOfSpawners = 6;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();
        GetComponent<AudioSyncer>().bias = 100f;
        GetComponent<AudioSyncer>().timeStep = 0.1f;

        //Spawner movement
        EnqueueSpawnerRotationSpeedEvent(36f, -30f);
        EnqueueSpawnerRotationSpeedEvent(68f, 30f);
        EnqueueSpawnerRotationSpeedEvent(100f, 10f);
        EnqueueSpawnerRotationSpeedEvent(100f, 15f);
        EnqueueSpawnerRotationSpeedEvent(164f, -30f);
        EnqueueSpawnerRotationSpeedEvent(195f, -40f);
        EnqueueSpawnerRotationSpeedEventOverTime(195f, 212f, -40f, -70f);
        EnqueueSpawnerRotationSpeedEventOverTime(212f, 220f, -70f, -120f);
        EnqueueSpawnerRotationSpeedEventOverTime(220f, 225f, -120f, -220f);
        EnqueueSpawnerRotationSpeedEventOverTime(225f, 228f, -220f, -500f);
        EnqueueSpawnerRotationSpeedEvent(228.5f, 150f);
        EnqueueSpawnerRotationSpeedEventOverTime(230f, 232f, 150f, 50f);
        EnqueueSpawnerRotationSpeedEvent(232.1f, 150f);
        EnqueueSpawnerRotationSpeedEvent(234f, 250f);
        EnqueueSpawnerRotationSpeedEvent(237f, 150f);
        EnqueueSpawnerRotationSpeedEventOverTime(238f, 240f, 150f, 50f);
        EnqueueSpawnerRotationSpeedEvent(240.1f, 150f);
        EnqueueSpawnerRotationSpeedEvent(242f, 250f);
        EnqueueSpawnerRotationSpeedEvent(244f, 150f);
        EnqueueSpawnerRotationSpeedEventOverTime(246f, 248f, 150f, 50f);
        EnqueueSpawnerRotationSpeedEvent(248.1f, 150f);
        EnqueueSpawnerRotationSpeedEventOverTime(255f, 258f, 150f, 50f);
        EnqueueSpawnerRotationSpeedEvent(258.1f, 250f);
        EnqueueSpawnerRotationSpeedEvent(260f, 300f);
        EnqueueSpawnerRotationSpeedEventOverTime(284f, 291f, 300f, 30f);
        EnqueueSpawnerRotationSpeedEvent(292f, -60f);
        EnqueueSpawnerRotationSpeedEventOverTime(356f, 386f, -60f, 0f);

        //Bullet spawning
        EnqueueSurroundPlayer(1f, 36f, 0.25f, 35f, 10f);
        EnqueueOscillateSurroundPlayer(36f, 68f, 0.1f, -45f, -30f, 2f);
        EnqueueOscillateSurroundPlayer(68f, 100f, 0.1f, 45f, 20f, 1f);
        EnqueueOscillateSurroundPlayer(100f, 132f, 0.2f, 45f, 10f, 0.125f);
        EnqueueOscillateSurroundPlayer(132f, 164f, 0.2f, 45f, 20f, 0.15f);
        EnqueueOscillateSurroundPlayer(164f, 195f, 0.2f, -45f, -30f, 0.2f);
        EnqueueOscillateSurroundPlayer(195f, 220f, 0.1f, -45f, -30f, 0.2f);
        EnqueueOscillateSurroundPlayer(220f, 227f, 0.05f, -45f, -25f, 0.01f);
        EnqueueSurroundPlayer(228f, 256f, 0.1f, 3, 0f);
        EnqueueSurroundPlayer(260f, 284f, 0.15f, 2.5f, 1);
        EnqueueOscillateSurroundPlayer(292f, 356f, 0.1f, -45f, -25f, 0.1f);

        //Toggle Random Bullet Spawning
        EnqueueToggleRandomBulletSpawningEvent(228f);
        EnqueueToggleRandomBulletSpawningEvent(258f);
        EnqueueToggleRandomBulletSpawningEvent(370f);

        //Bullet speed
        EnqueueBulletSpeedEvent(36f, 3f);
        EnqueueBulletSpeedEvent(100f, 1.5f);
        EnqueueBulletSpeedEvent(132f, 2.5f);
        EnqueueBulletSpeedEventOverTime(195f, 220f, 2.5f, 4.5f);
        EnqueueBulletSpeedEventOverTime(220f, 228f, 4.5f, 7f);
        EnqueueBulletSpeedEvent(228.5f, 3f);
        EnqueueBulletSpeedEventOverTime(230f, 232f, 3f, 1f);
        EnqueueBulletSpeedEvent(232.1f, 3f);
        EnqueueBulletSpeedEvent(234f, 4.5f);
        EnqueueBulletSpeedEvent(237f, 3f);
        EnqueueBulletSpeedEventOverTime(238f, 240f, 3f, 1f);
        EnqueueBulletSpeedEvent(240.1f, 3f);
        EnqueueBulletSpeedEvent(242f, 4.5f);
        EnqueueBulletSpeedEvent(244f, 3f);
        EnqueueBulletSpeedEventOverTime(246f, 248f, 3f, 1f);
        EnqueueBulletSpeedEvent(248.1f, 3f);
        EnqueueBulletSpeedEventOverTime(255f, 258f, 3f, 1f);
        EnqueueBulletSpeedEvent(258.1f, 5f);
        EnqueueBulletSpeedEventOverTime(284f, 291f, 5f, 2f);
        EnqueueBulletSpeedEvent(292f, 3f);
        EnqueueBulletSpeedEventOverTime(356f, 386f, 3f, 0.5f);

        //Audio Syncer Bias (init 15f)
        EnqueueChangeAudioSyncerBias(36f, 15f);
        EnqueueChangeAudioSyncerBias(100f, 5f);
        EnqueueChangeAudioSyncerBias(160f, 15f);
        EnqueueChangeAudioSyncerBias(260f, 30f);
        EnqueueChangeAudioSyncerBias(284f, 15f);

        //Audio Syncer Timestep (init 0.15f)
        EnqueueChangeAudioSyncerBias(100f, 0.1f);
        EnqueueChangeAudioSyncerBias(132f, 1f);
        EnqueueChangeAudioSyncerBias(260f, 20f);
        EnqueueChangeAudioSyncerBias(284f, 10f);
        EnqueueChangeAudioSyncerBias(356f, 1f);
    }

    void Level3()
    {
        //Initial level setup
        BPM = 180f;
        bulletSpeed = 0.5f;
        spawnerRotationSpeed = 15f;

        numberOfSpawners = 12;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();
        GetComponent<AudioSyncer>().bias = 15f;
        GetComponent<AudioSyncer>().timeStep = 0.08f;

        //Spawner movement
        EnqueueSpawnerRotationSpeedEvent(50f, 30f);
        EnqueueSpawnerRotationSpeedEvent(148f, 25f);
        EnqueueSpawnerRotationSpeedEvent(197f, 15f);
        EnqueueSpawnerRotationSpeedEvent(247f, 30f);
        EnqueueSpawnerRotationSpeedEventOverTime(295f, 345f, 30f, 45f);
        EnqueueSpawnerRotationSpeedEvent(346f, 20f);

        //Bullet spawning
        EnqueueSurroundPlayer(310f, 375f, 1f, -45f, -5f);

        //Bullet speed
        EnqueueBulletSpeedEvent(50f, 1f);
        EnqueueBulletSpeedEvent(148f, 0.75f);
        EnqueueBulletSpeedEvent(197f, 0.5f);
        EnqueueBulletSpeedEvent(247f, 1f);
        EnqueueBulletSpeedEventOverTime(295f, 345f, 1f, 2f);
        EnqueueBulletSpeedEvent(346f, 0.75f);

        //Toggle Random Bullet Spawning
        EnqueueToggleRandomBulletSpawningEvent(360f);

        //Audio Syncer Bias (init 15f)

        //Audio Syncer Timestep (init 0.15f)
        EnqueueChangeAudioSyncerTimeStep(325f, 0.05f);
        EnqueueChangeAudioSyncerTimeStep(345f, 2f);
    }












    void EnqueueToggleRandomBulletSpawningEvent(float beat)
    {
        toggleRandomBulletSpawningEvents.Add(new ToggleRandomBulletSpawningEventParameters(beat));
    }

    void EnqueueBulletSpeedEvent(float beat, float speed)
    {
        bulletSpeedEvents.Add(new BulletSpeedEventParameters(beat, speed));
    }

    void EnqueueBulletSpeedEventOverTime(float startBeat, float endBeat, float startSpeed, float endSpeed)
    {
        int numberOfSteps = ((int)(endBeat - startBeat)) * 5;
        float beatStepSize = (endBeat - startBeat) / numberOfSteps;
        float speedStepSize = (endSpeed - startSpeed) / numberOfSteps;
        float currentSpeed = startSpeed;

        for (float i = startBeat; i < endBeat; i += beatStepSize)
        {
            EnqueueBulletSpeedEvent(i, currentSpeed);
            currentSpeed += speedStepSize;
        }
    }

    void EnqueueBulletSpawnEvent(float beat, int spawner, float offset)
    {
        bulletSpawnEvents.Add(new BulletSpawnEventParameters(beat, spawner, offset));
    }

    void EnqueueSpawnerRotationSpeedEvent(float beat, float speed)
    {
        spawnerRotationSpeedEvents.Add(new SpawnerRotationSpeedEventParameters(beat, speed));
    }

    void EnqueueSpawnerRotationSpeedEventOverTime(float startBeat, float endBeat, float startSpeed, float endSpeed)
    {
        int numberOfSteps = ((int)(endBeat - startBeat)) * 5;
        float beatStepSize = (endBeat - startBeat) / numberOfSteps;
        float speedStepSize = (endSpeed - startSpeed) / numberOfSteps;
        float currentSpeed = startSpeed;

        for (float i = startBeat; i < endBeat; i += beatStepSize)
        {
            EnqueueSpawnerRotationSpeedEvent(i, currentSpeed);
            currentSpeed += speedStepSize;
        }
    }

    void EnqueueOscillateSurroundPlayer(float startBeat, float endBeat, float rateOfFire, float startAngleOffCenter, float endAngleOffCenter, float reps)
    {
        float a = Mathf.Abs(endAngleOffCenter - startAngleOffCenter) / 2;
        float b = Mathf.PI * reps;
        float c = -1 * startBeat;
        float d = Mathf.Lerp(startAngleOffCenter, endAngleOffCenter, 0.5f);

        for (float i = startBeat; i < endBeat; i += rateOfFire)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, a * Mathf.Sin((b * i) - c) + d);
            }
        }
    }

    void EnqueueSurroundPlayer(float startBeat, float endBeat, float rateOfFire, float distanceFromCenter, int sign)
    {
        float thetaDistanceFromCenter = Mathf.Rad2Deg * sign * (Mathf.Atan(distanceFromCenter / 4.85f));

        for (float i = startBeat; i < endBeat; i += rateOfFire)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, thetaDistanceFromCenter);
            }
        }
    }

    void EnqueueSurroundPlayer(float startBeat, float endBeat, float rateOfFire, float angleOffCenter)
    {
        for (float i = startBeat; i < endBeat; i += rateOfFire)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, angleOffCenter);
            }
        }
    }

    void EnqueueSurroundPlayer(float startBeat, float endBeat, float rateOfFire, int spawnerSteps, float angleOffCenter)
    {
        for (float i = startBeat; i < endBeat; i += rateOfFire)
        {
            for (int j = 0; j < numberOfSpawners; j+=spawnerSteps)
            {
                EnqueueBulletSpawnEvent(i, j, angleOffCenter);
            }
        }
    }

    void EnqueueSurroundPlayer(float startBeat, float endBeat, float rateOfFire, float startAngleOffCenter, float endAngleOffCenter)
    {
        float angleStepSize = (endAngleOffCenter - startAngleOffCenter) / ((endBeat - startBeat) / rateOfFire);
        float currentAngle = startAngleOffCenter;

        for (float i = startBeat; i < endBeat; i += rateOfFire)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, currentAngle);
            }
            currentAngle += angleStepSize;
        }
    }

    void EnqueueChangeAudioSyncerBias(float beat, float bias)
    {
        audioSyncerBiasChangeEvents.Add(new AudioSyncerBiasChangeEventParameters(beat, bias));
    }

    void EnqueueChangeAudioSyncerTimeStep(float beat, float timeStep)
    {
        audioSyncerTimeStepChangeEvents.Add(new AudioSyncerTimeStepChangeEventParameters(beat, timeStep));
    }

    public void UpdateHUD()
    {
        accuracy = Mathf.Clamp((bulletsFired - bulletsHit) / (float)bulletsFired, 0f, 100f);
        accuracyText.text = "Accuracy: " + (accuracy * 100).ToString("F2") + "%\nBombs: " + bombs;
    }

    public void SpawnBomb()
    {
        if (bombs > 0)
        {
            Instantiate(bombPrefab, Player.instance.transform.position, Quaternion.identity);
            bombs--;
        }
    }
}
