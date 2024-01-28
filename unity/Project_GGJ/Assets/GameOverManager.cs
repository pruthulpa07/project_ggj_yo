using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas; // Reference to your game over canvas
    bool gameOverFlag = false;

    private void Start()
    {
        // Hide the game over canvas initially
        gameOverCanvas.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        if (!gameOverFlag)
        {
            // Show the game over canvas
            gameOverCanvas.SetActive(true);
            // Optionally pause the game
            //Time.timeScale = 0;
            Debug.Log("Game Over!");
            gameOverFlag = true;
        }
        
    }

    public void RestartGame()
    {
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        gameOverFlag = false;
    }
}
