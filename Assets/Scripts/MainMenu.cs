using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameManager GameManager;
    //public float sensitivity = 1.5f;
    //public static float changeSensitivity = 1.5f; //To pass into PlayerMovement

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
        //load 0 into some static class
        GameManager.levelIndex = 0;
        SceneManager.LoadScene("Game");
    }

    public void StartLevel2()
    {
        //load 1 into some static class
        GameManager.levelIndex = 1;
        SceneManager.LoadScene("Game");
    }

    public void StartLevel3()
    {
        //load 2 into some static class
        GameManager.levelIndex = 2;
        SceneManager.LoadScene("Game");
    }
}
