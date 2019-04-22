using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public enum GameStates
{
    WaitingToStart,
    Playing,
    GameOver
}

public class GameController : MonoBehaviour
{
    [HideInInspector] public bool startingAtLevelZero;
    [HideInInspector] public int startingLevel;
    
    // Drops 1 unit in amount of seconds (0.5f would be 1 unit in 0.5 seconds)
    [HideInInspector] public float fallSpeed = 1.0f;
    [HideInInspector] public int currentLevel = 0;

    //  The states of the game to control the flow
    [HideInInspector] public GameStates state = GameStates.WaitingToStart;

    //  Lines cleared at once
    [HideInInspector] public int numberOfRowsToClear = 0;

    public GameObject gameField;

    //  Game field
    private Transform[,] grid;
    
    //  Lines cleared at the current level
    private int _numLinesCleared = 0;

    private GameObject _nextFigure;
    private GameObject _previewFigure;

    //  First instantiation of the figure at the beginning of the game
    private bool _firstSpawn;

    private int _gridWidth;
    private int _gridHeight;
    
    [Inject] private TimeController _timeController;
    [Inject] private GameConfig _config;
    [Inject] private ScoreSystem _scoreSystem;
    [Inject] private UIController _ui;
    [Inject] private AudioController _audioController;
    [Inject] private UserInput _input;

    private void Start()
    {
        _gridWidth = _config.gridWidth;
        _gridHeight = _config.gridHeight;
        grid = new Transform[_gridWidth, _gridHeight];
        _ui.currentScoreText.SetText("0");
        _ui.currentLevelText.SetText(currentLevel.ToString());
        _firstSpawn = false;

        if (gameField == null)
        {
            gameField = GameObject.FindGameObjectWithTag("GameField");
        }
    }

    private void Update()
    {
        if (state == GameStates.Playing)
        {
            UpdateLevel();
            UpdateSpeed();
            CheckPauseBtnPressed();
        }
    }

    /// <summary>
    /// Updates the level every linesToClear lines cleared (Default is 10)
    /// </summary>
    private void UpdateLevel()
    {
        if ((startingAtLevelZero) || (!startingAtLevelZero && _numLinesCleared / _config.linesToClearForLevelUp > startingLevel))
        {
            currentLevel = _numLinesCleared / _config.linesToClearForLevelUp;
        }
    }

    //  Update falling Speed of the figure, depending on level
    private void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    public void NumLinesCleared(int numLinesClearedThisTurn, int numLinesScore, int numLinesMultiplier)
    {
        _scoreSystem.currentScore += numLinesScore * (currentLevel * numLinesMultiplier);
        _numLinesCleared += numLinesClearedThisTurn;
        _audioController.PlayClearedLineAudio();
    }

    /// <summary>
    /// Round the specified position
    /// </summary>
    /// <param name="pos">position</param>
    /// <returns></returns>
    public Vector2 Round(Vector2 pos) => new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));


    /// <summary>
    /// Check is above grid
    /// </summary>
    /// <param name="figure"></param>
    /// <returns></returns>
    public bool CheckIsAboveGrid(FigurePositionController figure)
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            foreach (Transform block in figure.transform)
            {
                Vector2 pos = Round(block.position);
                if (pos.y > _gridHeight - 1)
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Check is inside grid
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public bool CheckIsInsideGrid(Vector2 pos) => (int)pos.x >= 0 && (int)pos.x < _gridWidth && (int)pos.y >= 0;

    /// <summary>
    /// Determinate if there's a full row at the specified y
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsFullRowAt(int y)
    {
        // The y parameter, it's the row we will iterate over in the grid array in order to check each x position for a transform
        for (int x = 0; x < _gridWidth; x++)
        {
            // If we find the position that returns NULL instead of a transform, then we know that this row isn't full
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        //  Since we found a full row, we increment the full row variable
        numberOfRowsToClear++;

        //  If we iterated over the entire loop and didn't encounter any NULL position, then we return true
        return true;
    }

    /// <summary>
    /// Deletes block at y
    /// </summary>
    /// <param name="y"></param>
    public void DeleteBlockAt(int y)
    {
        for (int x = 0; x < _gridWidth; x++)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    /// <summary>
    /// Move row down
    /// </summary>
    /// <param name="y"></param>
    public void MoveRowDown(int y)
    {
        // Iterate over each block in the row of the y coordinate
        for (int x = 0; x < _gridWidth; x++)
        {
            //  Check if the current x and y in the grid array doesn't equal null
            if (grid[x, y] == null) continue;
            //  If it doesn't then have to set the current transform one position below in the grid
            grid[x, y - 1] = grid[x, y];

            //  Then set the current transform to null
            grid[x, y] = null;

            //  and then adjust the position of the sprite to move down by 1
            grid[x, y - 1].position += new Vector3(0, -1, 0);
        }
    }

    /// <summary>
    /// Moves down all rows
    /// </summary>
    /// <param name="y"></param>
    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < _gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    /// <summary>
    /// Deletes the row if it's full
    /// </summary>
    public void DeleteRow()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteBlockAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    /// <summary>
    /// Updates the grid
    /// </summary>
    /// <param name="figure"></param>
    public void UpdateGrid(FigurePositionController figure)
    {
        //  Update grid array. Doing this by starting a for loop that iterates over all the grid rows starting at 0
        for (int y = 0; y < _gridHeight; y++)
        {
            //  For each row, iterate over each individual x coordinate that represents a spot on the grid where a block could be
            for (int x = 0; x < _gridWidth; x++)
            {
                //  For each iteration, checking the grid array for a null value
                if (grid[x, y] != null)
                {
                    //  If there's a transform stored at the current index of the array then checking if the transform parent is the transform of the figure
                    if (grid[x, y].parent == figure.transform)
                    {
                        //  if it is then setting that position in the array to null
                        grid[x, y] = null;
                    }
                }
            }
        }

        //  Stepping through all of the blocks (children) of the calling figure
        foreach (Transform block in figure.transform)
        {
            //  Then creating a vector2 with the rounded position of current block
            Vector2 pos = Round(block.position);

            //  Then checking the position of the block to make sure it's below the top line of the grid
            if (pos.y < _gridHeight)
            {
                //  If it is, then setting the block (transform) at the position of the block
                grid[(int)pos.x, (int)pos.y] = block;
            }
        }
    }

    /// <summary>
    /// Gets the transform at grid position. Because instantiation of the figure above the height of the grid which is not part of the array,
    /// have to return null instead of attempting to return a transform that doesn't exist. If the figure is below the height of the grid,
    /// return the transform at the position
    /// </summary>
    /// <param name="pos">Position</param>
    /// <returns></returns>
    public Transform GetTransformAtGridPosition(Vector2 pos) => (pos.y > _gridHeight - 1) ? null : grid[(int)pos.x, (int)pos.y];


    /// <summary>
    /// Spawns the next figure and preview figure from the resources asset folder using the GetRandomFigure method to select a random prefab
    /// </summary>
    public void SpawnNextFigure()
    {
        //  First spawn at the start of the game spawn next figure and preview figure. Set the position of preview, and disabled it's script, so it doesn't move
        if (!_firstSpawn)
        {
            _firstSpawn = true;
            _nextFigure = Instantiate(GetRandomFigure(), _config.spawnPosition, Quaternion.identity);
            _previewFigure = Instantiate(GetRandomFigure(), _config.previewFigurePosition, Quaternion.identity);
            _input.SetCurrentFigure(_nextFigure);
            _previewFigure.GetComponent<FigurePositionController>().enabled = false;
        }
        //  Normal spawning of next figure and preview figure
        else
        {
            _previewFigure.transform.localPosition = _config.spawnPosition;
            _nextFigure = _previewFigure;
            _input.SetCurrentFigure(_nextFigure);
            _nextFigure.GetComponent<FigurePositionController>().enabled = true;

            _previewFigure = Instantiate(GetRandomFigure(), _config.previewFigurePosition, Quaternion.identity);
            _previewFigure.GetComponent<FigurePositionController>().enabled = false;

        }
    }

    private GameObject GetRandomFigure()
    {
        int randomIndex = Random.Range(1, _config.figures.Count-1);
        return _config.figures[randomIndex];
    }

    /// <summary>
    /// Updates High Score if it's greater than stored one and showing Menu
    /// </summary>
    public void GameOver()
    {
        _scoreSystem.UpdateHighScore();
        _ui.ShowGameOverScreen();
    }

    public void Play()
    {
        currentLevel = startingLevel;
        SpawnNextFigure();
        state = GameStates.Playing;
        gameField.SetActive(true);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void CheckPauseBtnPressed()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                _timeController.SetPauseOn();
                _ui.ShowPauseScreen();
                _input.enabled = false;
            }
            else
            {
                _timeController.SetPauseOff();
                _ui.HidePauseScreen();
                _input.enabled = true;
            }
        }
    }

    public void ContinuePlaying()
    {
        _timeController.SetPauseOff();
        _input.enabled = true;
    }
}
