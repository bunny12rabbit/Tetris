using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class FigurePositionController : MonoBehaviour
{
    //  Player want's to move figure down
    [HideInInspector] public bool moveDown = false;

    //  Used to specify whether of not the figure is allowed to rotate
    [SerializeField] private bool _allowRotation = true;
    //  Used to limit the rotation of the figure to a 90 / -90 rotation (To / From)
    [SerializeField] private bool _limitRotation = false;

    //  Movement delay when button held
    private float _verticalTimer = 0;
    private float _horizontalTimer = 0;

    // How long to wait before the figure recognizes that a button is being held
    private float _buttonDownDelay;
    private float _buttonDownWaitTimerHorizontal = 0;
    private float _buttonDownWaitTimerVertical = 0;
    private float _fallSpeed;
    //  Countdown timer for fall speed
    private float _fallDelay = 0;

    //  The score that player suppose to get if he immediately set the figure down
    [HideInInspector] private int _individualScore;
    private int _decrementBonusScoreEachSecBy;
    private float _individualScoreTimer;

    private bool _movedImmediateHorizontal = false;
    private bool _movedImmediateVertical = false;

    [Inject] private GameConfig _config;
    [Inject] private GameController _gameController;
    [Inject] private AudioController _audioController;
    [Inject] private ScoreSystem _scoreSystem;

    private void Start()
    {
        _fallSpeed = _gameController.fallSpeed;
        _buttonDownDelay = _config.buttonDownDelay;
        _individualScore = _config.individualScore;
        _decrementBonusScoreEachSecBy = _config.decrementBonusScoreEachSecBy;
    }

    private void Update()
    {
        if (_gameController.state == GameStates.Playing)
        {
            FigureFall();
            UpdateFallSpeed();
            UpdateIndividualScore();
        }
    }

    //  Decrement individualScore bonus by _decrementBonusScoreEachSecBy each second
    private void UpdateIndividualScore()
    {
        if (_individualScoreTimer < 1)
        {
            _individualScoreTimer += Time.deltaTime;
        }
        else
        {
            _individualScoreTimer = 0;
            _individualScore = Mathf.Max(_individualScore - _decrementBonusScoreEachSecBy, 0);
        }
    }

    /// <summary>
    /// Updates falling speed of figures (levels)
    /// </summary>
    private void UpdateFallSpeed()
    {
        _fallSpeed = _gameController.fallSpeed;
    }

    /// <summary>
    /// Figure fall movement
    /// </summary>
    private void FigureFall()
    {
        if (Time.time - _fallDelay >= _fallSpeed)
        {
            MoveDown();
        }
    }

    //  Moves the figure Right
    public void MoveRight()
    {
        //  Move when button pressed just once
        if (_movedImmediateHorizontal)
        {
            if (_buttonDownWaitTimerHorizontal < _buttonDownDelay)
            {
                _buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            //  Delay between the moves when button held
            if (_horizontalTimer < _config.continuousHorizontalSpeedDelay)
            {
                _horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!_movedImmediateHorizontal)
        {
            _movedImmediateHorizontal = true;
        }

        //  Reset the timer
        _horizontalTimer = 0;

        //  First attempt to move the figure to the right
        transform.position += new Vector3(1, 0, 0);

        //  Then check if the figure is at a valid position
        if (CheckIsValidPosition())
        {
            //  If it is, then call the UpdateGrid method which records this figure's new position
            _gameController.UpdateGrid(this);
            _audioController.PlayMoveAudio();
        }
        else
        {
            //  If it isn't then move the figure back to the left
            transform.position += new Vector3(-1, 0, 0);
        }
    }

    //  Moves the figure Left
    public void MoveLeft()
    {
        //  Move when button pressed just once
        if (_movedImmediateHorizontal)
        {
            if (_buttonDownWaitTimerHorizontal < _buttonDownDelay)
            {
                _buttonDownWaitTimerHorizontal += Time.deltaTime;
                return;
            }

            //  Delay for the move when button held
            if (_horizontalTimer < _config.continuousHorizontalSpeedDelay)
            {
                _horizontalTimer += Time.deltaTime;
                return;
            }
        }

        if (!_movedImmediateHorizontal)
        {
            _movedImmediateHorizontal = true;
        }

        //  Reset the timer
        _horizontalTimer = 0;

        transform.position += new Vector3(-1, 0, 0);

        if (CheckIsValidPosition())
        {
            _gameController.UpdateGrid(this);
            _audioController.PlayMoveAudio();
        }
        else
        {
            transform.position += new Vector3(1, 0, 0);
        }
    }

    //  Rotates the figure
    public void Rotate()
    {
        //  The up key was pressed, let's fires check if the figure is allowed to rotate
        if (_allowRotation)
        {
            //  If it is, then need to check if the rotation is limited to just back and forth
            if (_limitRotation)
            {
                //  If it is, then need to check what the current rotation is
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    //  If it's at 90 then we know it was already rotated, so rotate it back by -90
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    //  If it isn't, then we rotate it to 90
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                //  If it isn't, then rotate it to 90
                transform.Rotate(0, 0, 90);
            }

            //  Now we check if the figure is at a valid position after attempting a rotation
            if (CheckIsValidPosition())
            {
                //  If the position is valid, we update the grid
                _gameController.UpdateGrid(this);
                _audioController.PlayRotateAudio();
            }
            else
            {
                //  if it isn't, than rotate it back -90
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }

            }
        }
    }

    //  Moves the figure down, and if it's above the grid, calling GameOver method
    public void MoveDown()
    {
        //  Move when button pressed just once
        if (_movedImmediateVertical)
        {
            if (_buttonDownWaitTimerVertical < _buttonDownDelay)
            {
                _buttonDownWaitTimerVertical += Time.deltaTime;
                return;
            }

            //  Delay for the move when button held
            if (_verticalTimer < _config.continuousVerticalSpeedDelay)
            {
                _verticalTimer += Time.deltaTime;
                return;
            }
        }

        if (!_movedImmediateVertical)
        {
            _movedImmediateVertical = true;
        }

        //  Reset the timer
        _verticalTimer = 0;

        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            _gameController.UpdateGrid(this);

            //  If player want to move down the figure, only then play sound
            if (moveDown)
            {
                _audioController.PlayMoveAudio();
            }
        }
        else
        {
            transform.position += new Vector3(0, 1, 0);
            _gameController.DeleteRow();
            if (_gameController.CheckIsAboveGrid(this))
            {
                _gameController.GameOver();
                _gameController.state = GameStates.GameOver;
            }
            _audioController.PlayLandAudio();
            _gameController.SpawnNextFigure();
            _scoreSystem.currentScore += _individualScore;

            enabled = false;
        }

        _fallDelay = Time.time;
    }
    
    private bool CheckIsValidPosition()
    {
        foreach (Transform block in transform)
        {
            Vector2 pos = _gameController.Round(block.position);

            if (_gameController.CheckIsInsideGrid(pos) == false)
            {
                return false;
            }

            if (_gameController.GetTransformAtGridPosition(pos) != null &&
                _gameController.GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }

    public void ResetBtnPressedTimersHorizontal()
    {
        _movedImmediateHorizontal = false;
        _horizontalTimer = 0;
        _buttonDownWaitTimerHorizontal = 0;
    }

    public void ResetBtnPressedTimersVertical()
    {
        _movedImmediateVertical = false;
        _verticalTimer = 0;
        _buttonDownWaitTimerVertical = 0;
    }
}
