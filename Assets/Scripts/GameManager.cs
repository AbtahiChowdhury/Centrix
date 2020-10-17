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
    public int bulletsHit { get; set; }

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

    public int difficulty;
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
        Debug.Log("" + audioSource.time + " -> " + songPosInBeats);

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
        CheckForBulletSpeedChangeEvent();
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

        bulletSpeedEventsIndex = 0;
        bulletSpeedEvents = new ArrayList();

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

    void CheckForBulletSpeedChangeEvent()
    {
        if (bulletSpeedEventsIndex < bulletSpeedEvents.Count && songPosInBeats > ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).beat)
        {
            bulletSpeed = ((BulletSpeedEventParameters)bulletSpeedEvents[bulletSpeedEventsIndex]).speed;
            bulletSpeedEventsIndex++;
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
        BPM = 170f;
        bulletSpeed = 1f;
        spawnerRotationSpeed = 10f;
        numberOfSpawners = 6;
        spawnerArray = new GameObject[numberOfSpawners];
        CreateSpawners();

        //Spawner movement
        EnqueueSpawnerRotationSpeedEventOverTime(30f, 49f, 100, 10f, 60f);
        EnqueueSpawnerRotationSpeedEvent(49f, -60f);
        EnqueueSpawnerRotationSpeedEvent(113f, 60f);
        EnqueueSpawnerRotationSpeedEventOverTime(176f, 240f, 320, 60f, 5f);
        EnqueueSpawnerRotationSpeedEvent(240.5f, -30f);
        EnqueueSpawnerRotationSpeedEvent(257f, 30f);

        //Bullet spawning
        EnqueueSurroundPlayer(2f, 46f, 0.5f, 4f, 1);
        EnqueueSurroundPlayer(49f, 112f, 0.5f, 2.25f, -1);
        EnqueueSurroundPlayer(113f, 180f, 0.5f, 2.25f, 1);
        EnqueueSurroundPlayer(176f, 240f, 0.25f, 40f, 7.5f);
        EnqueueSurroundPlayer(240f, 257f, 0.25f, 1.5f, 1);
        EnqueueSurroundPlayer(257f, 270f, 0.25f, 1.5f, -1);

        //Bullet speed
        EnqueueBulletSpeedEvent(49f, 4f);
        EnqueueBulletSpeedEventOverTime(176f, 240f, 320, 4f, 1.5f);
        EnqueueBulletSpeedEvent(240.5f, 2.5f);
        EnqueueBulletSpeedEvent(257f, 3f);
        EnqueueBulletSpeedEventOverTime(270f, 287f, 85, 3f, 5f);
    }

    void Level4()
    {
        //Initial level setup
        BPM = 135f;
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
    }





    void EnqueueBulletSpeedEvent(float beat, float speed)
    {
        bulletSpeedEvents.Add(new BulletSpeedEventParameters(beat, speed));
    }

    void EnqueueBulletSpeedEventOverTime(float startBeat, float endBeat, int numberOfSteps, float startSpeed, float endSpeed)
    {
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

    void EnqueueSpawnerRotationSpeedEventOverTime(float startBeat, float endBeat, int numberOfSteps, float startSpeed, float endSpeed)
    {
        float beatStepSize = (endBeat - startBeat) / numberOfSteps;
        float speedStepSize = (endSpeed - startSpeed) / numberOfSteps;
        float currentSpeed = startSpeed;

        for (float i = startBeat; i < endBeat; i += beatStepSize)
        {
            EnqueueSpawnerRotationSpeedEvent(i, currentSpeed);
            currentSpeed += speedStepSize;
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
}
