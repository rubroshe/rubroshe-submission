using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    int score = 0; // Used for score AND high score
    int kills;
    public bool isGameOver = false;
    public bool isFightingBoss = false;
    [SerializeField] float winTime = 300f; // 5 minutes 
    private float gameTimer = 0f;
    public static float elapsedTime;


    public static GameManager instance;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI highScoreText;
    [SerializeField] GameObject gameOverText;
    [SerializeField] GameObject gameWinText;
    [SerializeField] TextMeshProUGUI gameTimerText;
    [SerializeField] TextMeshProUGUI currentLevelText;


    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
       UpdateHighScoreText();
       elapsedTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Submit") && isGameOver)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1; // resumes time in game
        }

        if (!isGameOver && !isFightingBoss)
        {
            gameTimer += Time.deltaTime;
            
            // Convert the gameTimer into mins and seconds
            int minutes = Mathf.FloorToInt(gameTimer / 60f);
            int seconds = Mathf.FloorToInt(gameTimer % 60f);
            string textTime = string.Format("{0:00}:{1:00}", minutes, seconds);

            // update the timer
            gameTimerText.text = textTime;  

            // Check if win condition (time passed) is met
            if (gameTimer >= winTime)
            {
                InitiateGameWin();
            }
        }

        if (isFightingBoss)
        {
            if (GameObject.FindGameObjectWithTag("Player") == null)
            {
                
                InitiateGameOver();
               
            }
            else if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                
                InitiateGameWin();
            }
        }

        
    }

    public void IncreaseScore(int amount)
    {
        score += amount;
        scoreText.text = "Score: " + score;
        CheckHighScore();
       // PlayerPrefs.SetInt("HighScore", score);
       // PlayerPrefs.GetInt("HighScore");
        UpdateHighScoreText();
    }

    public void IncreaseLevel(float level)
    {
        currentLevelText.text = "Level: " + level;
    }

    void CheckHighScore()
    {
        if (score > PlayerPrefs.GetInt("HighScore", 0))
        {
            PlayerPrefs.SetInt("HighScore", score);

            
        }
    }

    void UpdateHighScoreText()
    {
        highScoreText.text = $"HighScore: {PlayerPrefs.GetInt("HighScore", 0)}";
    }

   
    public void InitiateBossFight()
    {
        isFightingBoss = true;
        string textTime2 = string.Format("{0:00}:{1:00}", 5, 0);
        
        gameTimerText.text = textTime2;
        
    }
    public void InitiateGameOver()
    {
        isFightingBoss = false;
        isGameOver = true;
        Time.timeScale = 0; // stops time in the game
        gameOverText.SetActive(true); // show game over text

        
    }

    public void InitiateGameWin()
    {
        isFightingBoss = false;
        isGameOver = true;
        Time.timeScale = 0; 
        gameWinText.SetActive(true);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Kill all enemies and their effects  (add dying animation [in enemy die()] if possible)
       foreach (GameObject enemy in enemies)
        {
          Destroy(enemy);   
       }
    }

    public void TimePassed()
    {
        
    }
}
