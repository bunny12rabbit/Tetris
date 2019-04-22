using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Create game config")]
public class GameConfig : ScriptableObject
{
   //  Where to spawn next figure
    public Vector2 spawnPosition = new Vector2(5f, 20f);

    // How long to wait before the figure recognizes that a button is being held
    public float buttonDownDelay = 0.05f;

    //  Position of preview figure
    public Vector2 previewFigurePosition = new Vector2(5, -3.5f);

    //  Amount of lines to clear required for level up
    public int linesToClearForLevelUp = 10;

    //  Width and Height of gaming field
    [Header("Game field size")]
    public int gridWidth = 10;
    public int gridHeight = 20;

    ////  Score u get for clearing number of lines
    //[Header("Score for cleared lines")]
    //public int scoreOneLine = 50;
    //public int scoreTwoLines = 100;
    //public int scoreThreeLines = 300;
    //public int scoreFourLines = 1200;

    ////  Score multiplier for number of lines cleared
    //[Header("Score multiplier for cleared lines")]
    //public int oneLineMultiplier = 20;
    //public int twoLinesMultiplier = 25;
    //public int threeLinesMultiplier = 30;
    //public int fourLinesMultiplier = 40;

    [Header("Figures prefabs")]
    public List<GameObject> figures = new List<GameObject>(7);

    //  Audio clips
    [Header("Audio clips")]
    public AudioClip moveSound;
    public AudioClip landSound;
    public AudioClip clearedLineSound;

    [Header("Music")]
    public AudioClip[] music;
    public AudioClip mainTheme;

    [Header("The delay while moving when button is held")]
    //  The delay which the figure will have while moving when the down button is held
    public float continuousVerticalSpeedDelay = 0.01f;
    //  The delay which the figure will have while moving when the left or right buttons are held
    public float continuousHorizontalSpeedDelay = 0.05f;

    [Header("Score bonus for quick setup figure")]
    //  The score that player suppose to get if he immediately set the figure down
    public int individualScore = 100;
    //  Decrement individualScore bonus by each second
    public int decrementBonusScoreEachSecBy = 10;

    [Header("Score and multiplier for number of lines cleared")]
    public List<LinesClearedScores> linesScores = new List<LinesClearedScores>(4);
}
