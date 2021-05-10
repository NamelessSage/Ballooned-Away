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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayMessage(bool death, float timeALive, int score)
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
        scoreText.text = score.ToString();
        timeAliveText.text = timeALive.ToString();
    }

    public void PauseGame()
    {
        GameOverMenuUI.SetActive(true);
        Time.timeScale = 0f;
    }

}
