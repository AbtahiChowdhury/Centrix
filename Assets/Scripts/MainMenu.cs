using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    //public GameManager GameManager;
    public GameObject sensitivitySlider;

    void Start()
    {
        if (gameObject.name == "MainMenu")
        {
            sensitivitySlider.GetComponent<Slider>().value = PlayerMovement.sensitivity;
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OpenMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }

    public void SetSensitivity(float Sensitivity)
    {
        PlayerMovement.sensitivity = Sensitivity;
    }

    public void StartLevel1()
    {
        GameManager.levelIndex = 0;
        SceneManager.LoadScene("Game");
    }

    public void StartLevel2()
    {
        GameManager.levelIndex = 1;
        SceneManager.LoadScene("Game");
    }

    public void StartLevel3()
    {
        GameManager.levelIndex = 2;
        SceneManager.LoadScene("Game");
    }
}
