using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScoreSystem : MonoBehaviour
{
    [HideInInspector] public int currentScore;

    //  Stored High Score
    private int _startingHighScore;

    private List<LinesClearedScores> _linesScores;

    [Inject] private GameConfig _config;
    [Inject] private GameController _gameController;
    [Inject] private AudioController _audioController;

    private void Start()
    {
        currentScore = 0;
        _linesScores = _config.linesScores;
        GetStartedHighScore();
    }

    private void Update()
    {
        if (_gameController.state == GameStates.Playing)
        {
            UpdateScore();
        }
    }

    /// <summary>
    /// Updates score depending of number of rows cleared
    /// </summary>
    public void UpdateScore()
    {
        if (_gameController.numberOfRowsToClear > 0)
        {
            int rowsToClear = _gameController.numberOfRowsToClear;

            _gameController.NumLinesCleared(
                rowsToClear,
                GetLinesClearedScore(rowsToClear),
                GetLinesClearedMultiplier(rowsToClear)
                );
            _gameController.numberOfRowsToClear = 0;
        }
    }

    /// <summary>
    /// Updates High Score in PlayerPrefs if it's greater than stored one
    /// </summary>
    public void UpdateHighScore()
    {
        if (currentScore > _startingHighScore)
        {
            PlayerPrefs.SetInt("highscore", currentScore);
        }
    }

    /// <summary>
    /// Find score value from List class, contains scores and multipliers for number of lines cleared
    /// </summary>
    /// <param name="numLines"></param>
    /// <returns>int Score value</returns>
    private int GetLinesClearedScore(int numLines) => _linesScores.Find(num => num.numberOfLines == numLines).score;

    /// <summary>
    /// Find Multiplier value from List class, contains scores and multipliers for number of lines cleared
    /// </summary>
    /// <param name="numLines"></param>
    /// <returns>int Multiplier value</returns>
    private int GetLinesClearedMultiplier(int numLines) => _linesScores.Find(num => num.numberOfLines == numLines).multiplier;

    public void GetStartedHighScore()
    {
        _startingHighScore = PlayerPrefs.GetInt("highscore");
    }

}
