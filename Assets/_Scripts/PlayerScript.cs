using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PlayerScript : MonoBehaviour
{
    public int hp = 100;
    public GameObject gameOverPanel;
    public GameObject gameplayPanel;
    public GameObject startPanel;
    public Button restartButton;
    public Button startButton;
    public Button quitButton;
    public TMP_Text highScoreText;
    public TMP_Text hpText;

    private int highScore = 0;

    void Start()
    {
        hpText.SetText("HP: " + hp);
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (startPanel != null)
            startPanel.SetActive(true);

        Time.timeScale = 0f;

        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScoreUI();
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        hpText.SetText("HP: " + hp);
        if (hp <= 0)
        {
            Debug.Log("Player has died.");
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
            Time.timeScale = 0f;

            int currentScore = 0;
            var scoring = FindObjectOfType<ScoringScript>();
            if (scoring != null)
                currentScore = scoring.score;
            if (currentScore > highScore)
            {
                highScore = currentScore;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
            UpdateHighScoreUI();
        }
        else
        {
            Debug.Log("Player took damage. Remaining HP: " + hp);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void StartGame()
    {
        if (startPanel != null)
            startPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(true);


        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
            Debug.Log("Quit Game pressed.");
            Application.Quit();
    }

    private void UpdateHighScoreUI()
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }
}
