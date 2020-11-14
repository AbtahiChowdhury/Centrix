using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI levelInfo;

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

    public void SetLevel1Info()
    {
        levelInfo.text = "Level: Samsara\n\n" +
                         "Song: Samsara\n" +
                         "Artist: Xtrullor";
    }

    public void SetLevel2Info()
    {
        levelInfo.text = "Level: Glacier Galaxy\n\n" +
                         "Song: Glacier Galaxy\n" +
                         "Artist: EEK! & Lockyn";
    }

    public void SetLevel3Info()
    {
        levelInfo.text = "Level: Abyss of 7th\n\n" +
                         "Song: Abyss of 7th\n" +
                         "Artist: Nexhend";
    }
}
