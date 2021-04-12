using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool gameHasEnded = false;
    public GameObject endGamePanel;
    public GameObject deathPanel;   

    public void EndGame()
    {
        if(!gameHasEnded)
        {
            gameHasEnded = true;
            endGamePanel.SetActive(true);
            Time.timeScale = 0f;
            Debug.Log("Game Over");
        }        
    }
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        endGamePanel.SetActive(false);
    }

    public void PlayerDead()
    {
        gameHasEnded = true;
        deathPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Player Died");

    }
}
