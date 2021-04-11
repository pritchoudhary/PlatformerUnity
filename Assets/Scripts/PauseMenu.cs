using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool isGamePause = false;
    public GameObject pauseMenuGameObject;
    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePause)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void ResumeGame()
    {
        pauseMenuGameObject.SetActive(false);
        Time.timeScale = 1f;
        isGamePause = false;
    }
    void PauseGame()
    {
        pauseMenuGameObject.SetActive(true);
        Time.timeScale = 0f;
        isGamePause = true;
    }

    public void GetMenu()
    {
        Debug.Log("Loading Menu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");

    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
