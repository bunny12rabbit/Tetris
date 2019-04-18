using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Game : MonoBehaviour
{
    #region Variables
    #region Static Variables
    public static bool startingAtLevelZero;
    public static int startingLevel;

    // Instance of an Object
    public static Game instance = null;
    #endregion

    #region Public variables to access from outside classes through instance of an object, Hidden in the inspector
    [HideInInspector] public bool gameOver = false;

    // Drops 1 unit in amount of seconds (0.5f would be 1 unit in 0.5 seconds)
    [HideInInspector] public float fallSpeed = 1.0f;   

    [HideInInspector] public int currentScore = 0;

    [HideInInspector] public int currentLevel = 0;
    #endregion

    #region Private variables shown in the Inspector
    //  Position of preview figure
    [SerializeField] private Vector2 _previewFigrePosition = new Vector2(4.5f, -3.5f);

    //  Folder name in Resources folder, where the figure's prefabs are stored
    [SerializeField] private string _FiguresFolderInResources = "Prefabs"; 
    
    //  Amount of lines to clear required for level up
    [SerializeField] private int _linesToClearForLevelUp = 10;

    //  Width and Height of gaming field
    [Header("Game field size")]
    [SerializeField] private int _gridWidth = 10;
    [SerializeField] private int _gridHeight = 20;

    //  Score u get for clearing number of lines
    [Header("Score for cleared lines")]
    [SerializeField] private int _scoreOneLine = 50;
    [SerializeField] private int _scoreTwoLines = 100;
    [SerializeField] private int _scoreThreeLines = 300;
    [SerializeField] private int _scoreFourLines = 1200;

    //  Score multiplyer for number of lines cleared
    [Header("Score multiplyer for cleard lines")]
    [SerializeField] private int _oneLineMultiplyer = 20;
    [SerializeField] private int _twoLinesMultiplyer = 25;
    [SerializeField] private int _threeLinesMultiplyer = 30;
    [SerializeField] private int _fourLinesMultiplyer = 40;

    //  Audio clips
    [Header("Audio")]
    [SerializeField] private AudioClip _moveSound;
    [SerializeField] private AudioClip _landSound;
    [SerializeField] private AudioClip _clearedLineSound;

    //  UI Score and Level texts
    [Header("Score and level text reference")]
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _levelText;
    #endregion

    #region Public variables
    //  UI Reference
    public GameObject scoreText;
    public GameObject levelText;

    [Header("The delay while moving when button is held")]
    //  The delay which the figure will have while moving when the down button is held
    public float _continuousVerticalSpeed = 0.01f;
    //  The delay which the figure will have while moving when the left or right buttons are held
    public float _continuousHorizontalSpeed = 0.05f;

    [Header("Score bonus for quck setup figure")]
    //  The score that player suppose to get if he immediately set the figure down
    public int individualScore = 100;
    #endregion

    #region Private variables
    //  Game field
    private Transform[,] grid;
    //  Lines cleared at once
    private int _numberOfRowsToClear = 0;
    //  Lines cleared at the current level
    private int _numLinesCleared = 0;
    //  Stored High Score
    private int _startingHighScore; 

    private GameObject _nextFigure;
    private GameObject _previewFigure;

    private AudioSource _audioSource;

    //  First instantiation of the figure at the beginning of the game
    private bool _gameStarted = false;  

    #endregion

    #endregion

    #region Singleton
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    #endregion

    // Start is called before the first frame update
    private void Start()
    {
        grid = new Transform[_gridWidth, _gridHeight];
        currentLevel = startingLevel;
        currentScore = 0;
        _scoreText.SetText("0");
        _levelText.SetText(currentLevel.ToString());
        _audioSource = GetComponent<AudioSource>();
        gameOver = false;
        _startingHighScore = PlayerPrefs.GetInt("highscore");
        SpawnNextFigure();
    }

    private void Update()
    {
        UpdateScore();
        UpdateUI();
        UpdateLevel();
        UpdateSpeed();
    }

    /// <summary>
    /// Updates the level every linesToClear lines cleared (Default is 10)
    /// </summary>
    private void UpdateLevel()
    {
        if ((startingAtLevelZero) || (!startingAtLevelZero && _numLinesCleared / _linesToClearForLevelUp > startingLevel))
        {
            currentLevel = _numLinesCleared / _linesToClearForLevelUp;
        }
    }

    //  Update falling Speed of the figure, depending on level
    private void UpdateSpeed()
    {
        fallSpeed = 1.0f - ((float)currentLevel * 0.1f);
    }

    /// <summary>
    /// Udates UI Score and Level
    /// </summary>
    public void UpdateUI()
    {
        _scoreText.SetText(currentScore.ToString());
        _levelText.SetText(currentLevel.ToString());
    }

    /// <summary>
    /// Updates score depending of number of rows cleared
    /// </summary>
    public void UpdateScore()
    {
        if (_numberOfRowsToClear > 0)
        {
            switch (_numberOfRowsToClear)
            {
                case 1:
                    ClearedOneLine();
                    break;
                case 2:
                    ClearedTwoLines();
                    break;
                case 3:
                    ClearedThreeLines();
                    break;
                case 4:
                    ClearedFourLines();
                    break;
            }
            PlayLandAudio();
            _numberOfRowsToClear = 0;
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
    /// Plays audio clip when figure is moved left, right or down
    /// </summary>
    public void PlayMoveAudio()
    {
        _audioSource.PlayOneShot(_moveSound);
    }

    /// <summary>
    /// Plays audio clip when figure is rotated
    /// </summary>
    public void PlayRotateAudio()
    {
        _audioSource.PlayOneShot(_moveSound);
    }

    /// <summary>
    /// Playes audio clip when figure lands
    /// </summary>
    public void PlayLandAudio()
    {
        _audioSource.PlayOneShot(_landSound);
    }

    public void ClearedOneLine()
    {
        currentScore += _scoreOneLine + (currentLevel * _oneLineMultiplyer);
        _audioSource.PlayOneShot(_clearedLineSound);
        _numLinesCleared++;
    }

    public void ClearedTwoLines()
    {
        currentScore += _scoreTwoLines * (currentLevel * _twoLinesMultiplyer);
        _audioSource.PlayOneShot(_clearedLineSound);
        _numLinesCleared += 2;
    }

    public void ClearedThreeLines()
    {
        currentScore += _scoreThreeLines + (currentLevel * _threeLinesMultiplyer);
        _audioSource.PlayOneShot(_clearedLineSound);
        _numLinesCleared += 3;
    }

    public void ClearedFourLines()
    {
        currentScore += _scoreFourLines + (currentLevel * _fourLinesMultiplyer);
        _audioSource.PlayOneShot(_clearedLineSound);
        _numLinesCleared += 4;
    }

    /// <summary>
    /// Check is above grid
    /// </summary>
    /// <param name="figure"></param>
    /// <returns></returns>
    public bool CheckIsAboveGrid(Figure figure)
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
    public bool CheckIsInsideBorder(Vector2 pos) => (int)pos.x >= 0 && (int)pos.x < _gridWidth && (int)pos.y >= 0;

    /// <summary>
    /// Determinate if there's a full row at the specified y
    /// </summary>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsFullRowAt(int y)
    {
        // The y parametr, it's the row we will iterate over in the grid array in order to check each x possition for a transform
        for (int x = 0; x < _gridWidth; x++)
        {
            // If we find the position that returns NULL instead of a transform, then we know that this row isn't full
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        //  Since we found a full row, we increment the full row variable
        _numberOfRowsToClear++;

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
        /// Iterate over each block in the row of the y coordinate
        for (int x = 0; x < _gridWidth; x++)
        {
            //  Check if the current x and y in the grid array doesn't equal null
            if (grid[x, y] != null)
            {
                //  If it doesn't then have to set the current transform one possition below in the grid
                grid[x, y - 1] = grid[x, y];

                //  Then set the current transform to null
                grid[x, y] = null;

                //  and then adjust the position of the sprite to move down by 1
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    /// <summary>
    /// Moves down all rows
    /// </summary>
    /// <param name="y"></param>
    public void MoveAllRawsDown(int y)
    {
        for (int i = y; i < _gridHeight; i++)
        {
            MoveRowDown(i);
        }
    }

    /// <summary>
    /// Deletes the row
    /// </summary>
    public void DeleteRow()
    {
        for (int y = 0; y < _gridHeight; y++)
        {
            if (IsFullRowAt(y))
            {
                DeleteBlockAt(y);
                MoveAllRawsDown(y + 1);
                --y;
            }
        }
    }

    /// <summary>
    /// Updates the grid
    /// </summary>
    /// <param name="figure"></param>
    public void UpdateGrid(Figure figure)
    {
        //  Update our grid array. Doing this by starting a for loop that iterates over all the grid rows starting at 0
        for (int y = 0; y < _gridHeight; y++)
        {
            //  For each row, iterate over each individual x coordinate that represents a spot on the grid where a block could be
            for (int x = 0; x < _gridWidth; x++)
            {
                //  For each iteration, checking the grid array for a null value
                if (grid[x, y] != null)
                {
                    //  If there's a transform stored at the current index of the array then cheking if the transform parent os the transform of the
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
        if (!_gameStarted)
        {
            _gameStarted = true;
            _nextFigure = (GameObject)Instantiate(Resources.Load(GetRandomFigure(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
            _previewFigure = (GameObject)Instantiate(Resources.Load(GetRandomFigure(), typeof(GameObject)), _previewFigrePosition, Quaternion.identity);
            _previewFigure.GetComponent<Figure>().enabled = false;
        }
        //  Normal spawning of next figure and preview figure
        else
        {
            _previewFigure.transform.localPosition = new Vector2(5.0f, 20.0f);
            _nextFigure = _previewFigure;
            _nextFigure.GetComponent<Figure>().enabled = true;

            _previewFigure = (GameObject)Instantiate(Resources.Load(GetRandomFigure(), typeof(GameObject)), _previewFigrePosition, Quaternion.identity);
            _previewFigure.GetComponent<Figure>().enabled = false;

        }
    }

    /// <summary>
    /// Round the specified postition
    /// </summary>
    /// <param name="pos">position</param>
    /// <returns></returns>
    public Vector2 Round(Vector2 pos) => new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));

    /// <summary>
    /// Get's a random figure. Using the switch condition to test for each possible random number case and then assign
    /// a different prefab name to the randomFigureName variable
    /// </summary>
    /// <returns></returns>
    private string GetRandomFigure()
    {
        int randomFigure = Random.Range(1, 8);
        string randomFigureName = "";

        switch (randomFigure)
        {
            case 1:
                randomFigureName = "Figure_T";
                break;
            case 2:
                randomFigureName = "Figure_Long";
                break;
            case 3:
                randomFigureName = "Figure_Square";
                break;
            case 4:
                randomFigureName = "Figure_J";
                break;
            case 5:
                randomFigureName = "Figure_L";
                break;
            case 6:
                randomFigureName = "Figure_S";
                break;
            case 7:
                randomFigureName = "Figure_Z";
                break;
        }

        //  Adding path to the folder in Resources folder that contains prefabs
        return _FiguresFolderInResources + "/" + randomFigureName;
    }
}
