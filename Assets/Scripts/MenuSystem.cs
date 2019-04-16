using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    //  Instance of an object
    public static MenuSystem instance = null;

    public GameObject menuPanel;
    public GameObject levelSelectPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI levelSelectedText;
    public TextMeshProUGUI highScoreMenuText;
    public TextMeshProUGUI highScoreGameOverText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentLevelText;

#region Instance of an Object
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else if (instance == this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private void Start()
    {
        UpdateHighScoreMenuView();
        levelSelectedText.SetText("LEVEL 0");
        
    }

    /// <summary>
    /// Updates High Score on Menu screen
    /// </summary>
    public void UpdateHighScoreMenuView()
    {
        highScoreMenuText.SetText(PlayerPrefs.GetInt("highscore").ToString());
    }

    public void UpdateHighScoreGameOverView()
    {
        highScoreGameOverText.SetText(PlayerPrefs.GetInt("highscore").ToString());
    }

    /// <summary>
    /// Resets High Score
    /// </summary>
    public void ResetHighScore()
    {
        PlayerPrefs.DeleteKey("highscore");
        UpdateHighScoreMenuView();
    }

    /// <summary>
    /// Passing slider value - selected level to start from into the Game script
    /// </summary>
    /// <param name="value">Level to start from</param>
    public void LevelSelected(float value)
    {
        Game.startingLevel = (int)value;
        levelSelectedText.SetText("LEVEL " + value.ToString());
    }

    /// <summary>
    /// Play and Play again button
    /// </summary>
    public void PlayAgain()
    {
        Game.startingAtLevelZero = Game.startingLevel == 0 ? true : false;
        highScoreMenuText.SetText(PlayerPrefs.GetInt("highscore").ToString());
        menuPanel.SetActive(false);
        levelSelectPanel.SetActive(false);
        gameOverPanel.SetActive(true);
        SceneManager.LoadScene("Level");
    }

    /// <summary>
    /// Button to Exit the application
    /// </summary>
    public void OnExitPressed()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Updates High Score if it's greater than stored one and showing Menu
    /// </summary>
    public void GameOver(int currentScore, int currentLevel)
    {
        Game.instance.UpdateHighScore();
        Game.instance.scoreText.SetActive(false);
        Game.instance.levelText.SetActive(false);
        UpdateHighScoreMenuView();
        currentLevelText.SetText(currentLevel.ToString());
        currentScoreText.SetText(currentScore.ToString());
        UpdateHighScoreGameOverView();
        menuPanel.SetActive(true);
        levelSelectPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
}
