using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public bool isGameOver;
    public float xSensitivity { get; private set; }
    public float ySensitivity { get; private set; }
   

    public static GameManager Instance { get { return instance; } }

    public AudioClip[] audioClip;
    AudioSource audioSource;
    AudioListener audioListener;

    public GameObject spawnerPrefab;
    public GameObject bulletPrefab;

    public float speed { get; set; }
    public int difficulty { get; set; }

    float BPM;
    float secPerBeat;
    float dsptimesong;
    float songPosition;

    public float spawnerRotationSpeed { get; set; }
    int numberOfSpawners;
    static GameObject[] spawnerArray;

    public static GameObject[] getSpawners ()
    {
        return spawnerArray;
    }

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

    public int levelIndex;
    public float songPosInBeats;

    public int numberOfSamples;
    public float[] spectrum { get; set; }

    public bool debugMode;
    public float startTimeInSeconds;

    private void Awake()
    {
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
        isGameOver = false;

        audioListener = GetComponent<AudioListener>();
        spectrum = new float[numberOfSamples];

        audioSource = GetComponent<AudioSource>();

        speed = 1f;

        SetUpLevel();

        if (debugMode)
        {
            secPerBeat = 60f / BPM;
            dsptimesong = (float)AudioSettings.dspTime;
            audioSource.time = startTimeInSeconds;
            songPosInBeats = startTimeInSeconds / secPerBeat;
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
        if(debugMode)
        {
            songPosition = (float)((AudioSettings.dspTime - dsptimesong) + startTimeInSeconds);
            songPosInBeats = songPosition / secPerBeat;
        }
        else
        {
            songPosition = (float)(AudioSettings.dspTime - dsptimesong);
            songPosInBeats = songPosition / secPerBeat;
        }
        Debug.Log(songPosInBeats);

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float avg = 0;
        for(int i = 0; i < numberOfSamples/4; i++)
        {
            avg += spectrum[i];
        }
        avg /= (numberOfSamples / 4);
        speed = Mathf.Lerp(1f, 3f, avg * 50f);


        CheckForSpawnerSpeedChangeEvent();
        CheckForBulletSpawnEvent();
    }

    //Game Over Menu
    public void EndGame()
    {
        isGameOver = true;
        if (isGameOver)
        {
            isGameOver = false;
            Debug.Log("GAME OVER");
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene("GameOver"); //TODO: Make this scene
        }

    }

    //For Menu Start/Restart Game
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }
    
    public void ExitGame()
    {//Close the application        
        Application.Quit();
    }

    void SetUpLevel()
    {
        spawnerRotationSpeedEventsIndex = 0;
        spawnerRotationSpeedEvents = new ArrayList();

        bulletSpawnEventsIndex = 0;
        bulletSpawnEvents = new ArrayList();

        audioSource.clip = audioClip[levelIndex];
        switch (levelIndex)
        {
            case 0:
                //level 1 - samsara
                Level1();
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                Level4();
                break;
            default:
                break;
        }
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

    void CheckForSpawnerSpeedChangeEvent()
    {
        if (spawnerRotationSpeedEventsIndex < spawnerRotationSpeedEvents.Count && songPosInBeats > ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).beat)
        {
            spawnerRotationSpeed = ((SpawnerRotationSpeedEventParameters)spawnerRotationSpeedEvents[spawnerRotationSpeedEventsIndex]).speed;
            spawnerRotationSpeedEventsIndex++;
        }
    }

    void CheckForBulletSpawnEvent()
    {
        while (bulletSpawnEventsIndex < bulletSpawnEvents.Count && songPosInBeats > ((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).beat)
        {
            spawnerArray[((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).spawner].GetComponent<Spawner>().SpawnBullet(((BulletSpawnEventParameters)bulletSpawnEvents[bulletSpawnEventsIndex]).theta);
            bulletSpawnEventsIndex++;
        }
    }

    void Level1()
    {
        //Initial level setup
        audioSource.clip = audioClip[levelIndex];
        BPM = 170f;
        difficulty = 1;
        bulletSpeed = 3f;
        spawnerRotationSpeed = 10f;

        numberOfSpawners = 6;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();

        //Spawner movement
        EnqueueSpawnerRotationSpeedEventOverTime(30f, 49f, 100, 10f, 30f);

        EnqueueSpawnerRotationSpeedEvent(49f, -60f);
        EnqueueSpawnerRotationSpeedEvent(58f, 60f);
        EnqueueSpawnerRotationSpeedEvent(60f, -60f);
        EnqueueSpawnerRotationSpeedEvent(62f, 60f);
        EnqueueSpawnerRotationSpeedEvent(64f, -60f);
        EnqueueSpawnerRotationSpeedEvent(67f, 60f);

        //Bullet spawning
        for (float i = 49f; i < 180f; i += 0.5f)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, 35f);
            }
        }
    }

    void Level4()
    {
        //Initial level setup
        audioSource.clip = audioClip[levelIndex];
        BPM = 135f;
        difficulty = 1;
        bulletSpeed = 3f;
        spawnerRotationSpeed = 100f;

        numberOfSpawners = 8;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();

        EnqueueSpawnerRotationSpeedEventOverTime(260f, 275f, 130, 50f, 200f);
        EnqueueSpawnerRotationSpeedEventOverTime(276f, 277f, 10, 200f, -100f);
        EnqueueSpawnerRotationSpeedEvent(277.1f, -80f);
        EnqueueSpawnerRotationSpeedEvent(335f, 0f);
        EnqueueSpawnerRotationSpeedEvent(340f, -100f);
        //Spawner movement
        /*
        EnqueueSpawnerRotationSpeedEventOverTime(30f, 49f, 100, 10f, 30f);

        EnqueueSpawnerRotationSpeedEvent(49f, -60f);
        EnqueueSpawnerRotationSpeedEvent(58f, 60f);
        EnqueueSpawnerRotationSpeedEvent(60f, -60f);
        EnqueueSpawnerRotationSpeedEvent(62f, 60f);
        EnqueueSpawnerRotationSpeedEvent(64f, -60f);
        EnqueueSpawnerRotationSpeedEvent(67f, 60f);

        //Bullet spawning
        bulletSpeed = 3f;
        for (float i = 49f; i < 180f; i += 0.5f)
        {
            for (int j = 0; j < numberOfSpawners; j++)
            {
                EnqueueBulletSpawnEvent(i, j, 35f);
            }
        }
        */
    }



    void EnqueueBulletSpawnEvent(float beat, int spawner, float offset)
    {
        bulletSpawnEvents.Add(new BulletSpawnEventParameters(beat, spawner, offset));
    }

    void EnqueueSpawnerRotationSpeedEvent(float beat, float speed)
    {
        spawnerRotationSpeedEvents.Add(new SpawnerRotationSpeedEventParameters(beat, speed));
    }

    void EnqueueSpawnerRotationSpeedEventOverTime(float startBeat, float endBeat, int numberOfSteps, float startSpeed, float endSpeed)
    {
        int counter = 0;
        for (float i = startBeat; i < endBeat; i += (endBeat - startBeat) / numberOfSteps)
        {
            EnqueueSpawnerRotationSpeedEvent(i, startSpeed + (counter * (endBeat / numberOfSteps)));
            counter++;
        }
        counter = 0;
    }
}
