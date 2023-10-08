using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverUI;
    public GameObject gameWonUI;
    public GameObject gameCompleteUI;

    public const int SCENCE_AMOUNT = 3;

    private void Start()
    {
        FindObjectOfType<Player>().OnDeath += OnGameOver;
        FindObjectOfType<Spawner>().OnFireAllEnemy += OnGameWon;
    }

    void OnGameOver()
    {
        this.gameOverUI.SetActive(true);
    }

    void OnGameWon()
    {
        if ((SceneManager.GetActiveScene().buildIndex + 1) > SCENCE_AMOUNT)
        {
            this.OnGameComplete();
            return;
        }
        this.gameWonUI.SetActive(true);
    }

    void OnGameComplete()
    {
        this.gameCompleteUI.SetActive(true);
    }

    // UI Input
    public void OnPlayAgainBnt()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnRestartGame()
    {
        SceneManager.LoadScene(1);
    }

    // UI Level
    public void ToNextLevel()
    {
        if ((SceneManager.GetActiveScene().buildIndex + 1) > SCENCE_AMOUNT) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ToPrevLevel()
    {
        if ((SceneManager.GetActiveScene().buildIndex - 1) < 1) return;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

}