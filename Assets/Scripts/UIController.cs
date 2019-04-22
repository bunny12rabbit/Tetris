using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class UIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject selectLevelPanel;
    [SerializeField] private GameObject controlsPanel;

    [Header("Text fields")]
    public TextMeshProUGUI levelSelectedText;
    public TextMeshProUGUI highScoreMenuText;
    public TextMeshProUGUI gameOverHighScoreText;
    public TextMeshProUGUI gameOverScoreText;
    public TextMeshProUGUI gameOverLevelText;
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI currentLevelText;

    [Header("Buttons")]
    public GameObject menuBtns;
    public GameObject gameOverBtns;
    public GameObject pauseBtns;

    [Inject] private GameController _gameController;
    [Inject] private ScoreSystem _scoreSystem;
    [Inject] private GameConfig _config;

    public void ShowMenuPanel() => menuPanel.SetActive(true);
    public void HideMenuPanel() => menuPanel.SetActive(false);

    public void ShowUIPanel() => uiPanel.SetActive(true);
    public void HideUIPanel() => uiPanel.SetActive(false);

    public void ShowGameOverPanel() => gameOverPanel.SetActive(true);
    public void HideGameOverPanel() => gameOverPanel.SetActive(false);

    public void ShowSelectLevelPanel() => selectLevelPanel.SetActive(true);
    public void HideSelectLevelPanel() => selectLevelPanel.SetActive(false);

    public void ShowControlsPanel() => controlsPanel.SetActive(true);
    public void HideControlsPanel() => controlsPanel.SetActive(false);

    public void ShowMenuBtns() => menuBtns.SetActive(true);
    public void HideMenuBtns() => menuBtns.SetActive(false);

    public void ShowGameOverBtns() => gameOverBtns.SetActive(true);
    public void HideGameOverBtns() => gameOverBtns.SetActive(false);

    public void ShowPauseBtns() => pauseBtns.SetActive(true);
    public void HidePauseBtns() => pauseBtns.SetActive(false);

    private void Start()
    {
        UpdateHighScoreMenuView();
        levelSelectedText.SetText("LEVEL 0");
    }

    private void Update()
    {
        if (_gameController.state == GameStates.Playing)
        {
            UpdateUI();
        }
    }

    /// <summary>
    /// Updates UI Score and Level
    /// </summary>
    public void UpdateUI()
    {
        currentScoreText.SetText(_scoreSystem.currentScore.ToString());
        currentLevelText.SetText(_gameController.currentLevel.ToString());
    }

    /// <summary>
    /// Updates High Score on Menu screen
    /// </summary>
    public void UpdateHighScoreMenuView() => highScoreMenuText.SetText(PlayerPrefs.GetInt("highscore").ToString());

    /// <summary>
    /// Updates Gameover screen
    /// </summary>
    public void UpdateGameOverView()
    {
        gameOverScoreText.SetText(_scoreSystem.currentScore.ToString());
        gameOverLevelText.SetText(_gameController.currentLevel.ToString());
        gameOverHighScoreText.SetText(PlayerPrefs.GetInt("highscore").ToString());
    }

    /// <summary>
    /// Passing slider value - selected level to start from into the Game script
    /// </summary>
    /// <param name="value">Level to start from</param>
    public void LevelSelected(float value)
    {
        _gameController.startingLevel = (int)value;
        levelSelectedText.SetText("LEVEL " + value);
    }

    public void OnPlayBtnClicked()
    {
        HideMenuPanel();
        HideSelectLevelPanel();
        ShowGameOverPanel();
        ShowUIPanel();
        _gameController.Play();   
    }

    public void OnResetHighScoreBtnClicked()
    {
        PlayerPrefs.DeleteKey("highscore");
        UpdateHighScoreMenuView();
        gameOverHighScoreText.SetText("0");
    }

    public void OnExitBtnClicked() => _gameController.Exit();

    public void OnMainMenuBtnClicked() => SceneManager.LoadScene(0);

    public void OnBackBtnClicked()
    {
        HideControlsPanel();
        ShowMenuPanel();
    }

    public void ShowControlsScreen()
    {
        ShowControlsPanel();
        HideMenuPanel();
    }

    public void HidePauseScreen()
    {
        HideMenuPanel();
        ShowUIPanel();
        _gameController.ContinuePlaying();
    }

    public void ShowPauseScreen()
    {
        HideUIPanel();
        UpdateGameOverView();
        UpdateHighScoreMenuView();
        ShowMenuPanel();
        HideUIPanel();
        HideMenuBtns();
        ShowPauseBtns();
    }

    public void ShowGameOverScreen()
    {
        HideUIPanel();
        UpdateGameOverView();
        UpdateHighScoreMenuView();
        ShowMenuPanel();
        HideUIPanel();
        HideMenuBtns();
        ShowGameOverBtns();
    }
}
