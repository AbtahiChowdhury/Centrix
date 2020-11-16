using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public float sensitivity;
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
        sensitivity = Sensitivity;
        Debug.Log(sensitivity);
    }

    public void StartLevel1()
    {
        //load 0 into some static class
        SceneManager.LoadScene("Game");
    }

    public void StartLevel2()
    {
        //load 1 into some static class
        SceneManager.LoadScene("Game");
    }

    public void StartLevel3()
    {
        //load 2 into some static class
        SceneManager.LoadScene("Game");
    }
}
