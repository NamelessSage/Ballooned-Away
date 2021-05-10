using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
    private bool death = false;
    public Text message;
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    public void DisplayMessage()
    {
        if (death == true)
        {
            message.text = "Game over";
        }
        else
        {
            message.text = "You won!";
        }
    }
}
