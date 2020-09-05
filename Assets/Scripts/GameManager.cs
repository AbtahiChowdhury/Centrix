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

    public AudioClip audioClip;
    AudioSource audioSource;
    AudioListener audioListener;

    public float speedMultiplier;
    public float speed { get; set; }

    public float BPM;
    public float songPosInBeats;
    float secPerBeat;
    float dsptimesong;
    float songPosition;

    public int numberOfSamples;
    public float[] spectrum { get; set; }

    public bool debugMode;
    public float startTimeInSeconds;

    private void Awake()
    {
        xSensitivity = ySensitivity = 1.0f;
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
            Cursor.visible = false;
        }
        else
        {
            Cursor.visible = true;
        }
        isGameOver = false;

        audioListener = GetComponent<AudioListener>();
        spectrum = new float[numberOfSamples];

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = audioClip;

        speed = 1f;

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

        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
        float avg = 0;
        for(int i = 0; i < numberOfSamples/4; i++)
        {
            avg += spectrum[i];
        }
        avg /= (numberOfSamples / 4);
        speed = speedMultiplier * Mathf.Lerp(1f, 3f, avg * 50f);
        //Debug.Log(avg);

       // Debug.Log(songPosInBeats);
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


}
