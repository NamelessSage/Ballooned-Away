using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    public Text message;
    public Text scoreText;
    public Text timeAliveText;
    public GameObject GameOverMenuUI;

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayMessage(bool death, int score, float timeALive)
    {
        PauseGame();
        if (death == true)
        {
            message.text = "Game over";
        }
        else
        {
            message.text = "You won!";
        }
        scoreText.text = "Score: " + score.ToString();
        timeAliveText.text = "";//"TimeAlive: " + timeALive.ToString();
    }

    public void PauseGame()
    {
        GameOverMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

}
