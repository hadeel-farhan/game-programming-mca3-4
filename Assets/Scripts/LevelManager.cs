using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{

    public Text gameText;
    public Text scoreText;
    public int scoreToBeat = 1;

    public static bool isGameOver = false;
    public static int score;

    public string nextLevel;


    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
        score = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isGameOver) {
            if (PlayerHealth.currentHealth <= 0) {
                LevelLost();
            }
            SetScoreText();
        }

        if (score >= scoreToBeat) {
            LevelBeat();
        }
    }

    void SetScoreText() {
        scoreText.text = "Score: " + score;
    }

    public void LevelLost() {
        isGameOver = true;
        gameText.gameObject.SetActive(true);
        gameText.text = "GAME OVER!";

        Invoke("LoadCurrentLevel", 2);

    }
    public void LevelBeat() {
        isGameOver = true;
        gameText.gameObject.SetActive(true);
        SetScoreText();
        gameText.text = "YOU WIN!";


        // If there is a next level, invoke it. Otherwise stay on same screen
        if(!string.IsNullOrEmpty(nextLevel)) {
            Invoke("LoadNextLevel", 2);
        }
    }

    void LoadNextLevel() {
        SceneManager.LoadScene(nextLevel);
    }

    void LoadCurrentLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
