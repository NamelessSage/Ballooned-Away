using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject PauseMenuUI; 
    public GameObject OptionsMenuUI; 
    public GameObject AboutMenuUI; 

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused == true)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        if (PauseMenuUI.activeSelf)
            PauseMenuUI.SetActive(false);
        else if (OptionsMenuUI.activeSelf)
            OptionsMenuUI.SetActive(false);
        else if (AboutMenuUI.activeSelf)
            AboutMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}


