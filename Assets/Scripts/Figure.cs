using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Figure : MonoBehaviour
{
    #region Private variables shown in the inspector
    [SerializeField] private float continuousVerticalSpeed = 0.01f; //  The delay which the figure will have while moving when the down button is held
    [SerializeField] private float continuousHorizontalSpeed = 0.05f;    //  The delay which the figure will have while moving when the left or right buttons are held
    [SerializeField] private bool allowRotation = true;   //  Used this to scecify whether of not the figure is allowed to rotate
    [SerializeField] private bool limitRotation = false;  //  Used to limit the rotation of the figure to a 90 / -90 rotation (To / From)
    [SerializeField] private int individualScore = 100;   //  The score that player suppose to get if he immediately set the figure down
    #endregion

    #region Private variables
    private float _verticalTimer = 0;
    private float _horizontalTimer = 0;
    private float _buttonDownDelay = 0.05f;   // How long to wait before the figure recognizes that a button is being held
    private float _buttonDownWaitTimerHorizontal = 0;
    private float _buttonDownWaitTimerVertical = 0;
    private float _fallSpeed;
    private float _fall = 0; //  Counddown timer for fall speed

    private bool _movedImmediateHorizontal = false;
    private bool _movedImmediateVertical = false;
    private bool _moveDown = false;

    private float _individualScoreTime; 
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _fallSpeed = Game.fallSpeed;
    }

    /// <summary>
    /// Update this instance
    /// </summary>
    void Update()
    {
        UpdateFallSpeed();
        CheckUserInput();
        UpdateIndividualScore();
    }

    /// <summary>
    /// Updates falling speed of figures (levels)
    /// </summary>
    void UpdateFallSpeed()
    {
        _fallSpeed = Game.fallSpeed;
    }

    void UpdateIndividualScore()
    {
        if (_individualScoreTime < 1)
        {
            _individualScoreTime += Time.deltaTime;
        }
        else
        {
            _individualScoreTime = 0;
            individualScore = Mathf.Max(individualScore - 10, 0);
        }
    }
    /// <summary>
    /// Checks the user input. This method checks the keys that the player can press to manipulate the position of the figure
    ///  The options here will be left, right, up and down
    ///  Left and right will move the figure one unit to the left or right
    ///  Down will move the figure 1 unit down
    ///  Up will rotate the figure
    /// </summary>
    void CheckUserInput()
    {


        if (!Game.gameOver)
        {
#if UNITY_ANDROID

            //  Touch Control

            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    //  Check if touched once to Move, not holding
                    if (CheckLeftOrRightMoveOneceTouch(touch))
                    {
                        ResetBtnPressedHorizontal();
                    }
                    if (CheckDownMoveOneceTouch(touch))
                    {
                        ResetBtnPressedTimersVertical();
                        _moveDown = false;
                    }

                    //  Then attempt to move accordingly
                    if (WannaMoveRightTouch(touch))
                    {
                        //  Move Right
                        MoveRight();
                    }

                    if (WannaMoveLeftTouch(touch))
                    {
                        //  Move Left
                        MoveLeft();
                    }

                    if (WannaMoveDownTouch(touch))
                    {
                        //  Move Down
                        _moveDown = true;
                        MoveDown();
                    }

                    if (WannaRotateTouch(touch))
                    {
                        //  Rotate
                        Rotate();
                    }
                }
            }

            if (Time.time - _fall >= _fallSpeed)
            {
                MoveDown();
            }
#else
            
            //  Reset timers if player pressed the button just once
            if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                _movedImmediateHorizontal = false;
                _horizontalTimer = 0;
                _buttonDownWaitTimerHorizontal = 0;
            }

            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                _movedImmediateVertical = false;
                _verticalTimer = 0;
                _buttonDownWaitTimerVertical = 0;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                MoveRight();
            }

            if (Input.GetKey(KeyCode.LeftArrow)) // to Left
            {
                MoveLeft();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))   // Rotation
            {
                Rotate();
            }

            if (Input.GetKey(KeyCode.DownArrow) || Time.time - _fall >= _fallSpeed)    //Move Down
            {
                MoveDown();
            }
        }
#endif
        }
        
        bool CheckLeftOrRightMoveOneceTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
            (touch.position.x > Screen.width / 2 || touch.position.x < Screen.width / 2);

        bool CheckDownMoveOneceTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
            (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 3);

        void ResetBtnPressedTimersVertical()
        {
            _movedImmediateHorizontal = false;
            _horizontalTimer = 0;
            _buttonDownWaitTimerHorizontal = 0;
        }

        void ResetBtnPressedHorizontal()
        {
            _movedImmediateVertical = false;
            _verticalTimer = 0;
            _buttonDownWaitTimerVertical = 0;
        }

        bool WannaMoveRightTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
            (touch.position.x > Screen.width / 2 && touch.position.y > Screen.height / 4);

        bool WannaMoveLeftTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
            (touch.position.x < Screen.width / 2 && touch.position.y > Screen.height / 4);

        bool WannaMoveDownTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
            (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 4);

        bool WannaRotateTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
            (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height / 4);

        //  Moves the figure Right
        void MoveRight()
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
                if (_horizontalTimer < continuousHorizontalSpeed)
                {
                    _horizontalTimer += Time.deltaTime;
                    return;
                }
            }

            if (!_movedImmediateHorizontal)
            {
                _movedImmediateHorizontal = true;
            }

            _horizontalTimer = 0;    //  Reset the timer

            //  First attemt to move the figure to the right
            transform.position += new Vector3(1, 0, 0);

            //  Then check if the figure is at a valid position
            if (CheckIsValidPosition())
            {
                //  If it is, then call the UpdateGrid method which records this figure's new position
                Game.instance.UpdateGrid(this);
                Game.instance.PlayMoveAudio();
            }
            else
            {
                //  If it isn't then move the figure back to the left
                transform.position += new Vector3(-1, 0, 0);
            }
        }

        //  Moves the figure Left
        void MoveLeft()
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
                if (_horizontalTimer < continuousHorizontalSpeed)
                {
                    _horizontalTimer += Time.deltaTime;
                    return;
                }
            }

            if (!_movedImmediateHorizontal)
            {
                _movedImmediateHorizontal = true;
            }

            _horizontalTimer = 0;    //  Reset the timer

            transform.position += new Vector3(-1, 0, 0);

            if (CheckIsValidPosition())
            {
                Game.instance.UpdateGrid(this);
                Game.instance.PlayMoveAudio();
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }

        //  Rotates the figure
        void Rotate()
        {
            //  The up key was pressed, let's fires check if the figure is allowed to rotate
            if (allowRotation)
            {
                //  If it is, then need to check if the rotation is limited to just back and forth
                if (limitRotation)
                {
                    //  If it is, then need to check what the current rotation is
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        //  If it's at 90 then we know it was already rotated, so we rotate it back by -90
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
                    Game.instance.UpdateGrid(this);
                    Game.instance.PlayRotateAudio();
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
        void MoveDown()
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
                if (_verticalTimer < continuousVerticalSpeed)
                {
                    _verticalTimer += Time.deltaTime;
                    return;
                }
            }

            if (!_movedImmediateVertical)
            {
                _movedImmediateVertical = true;
            }

            _verticalTimer = 0;  //  Reset the timer

            transform.position += new Vector3(0, -1, 0);

            if (CheckIsValidPosition())
            {
                Game.instance.UpdateGrid(this);

                if (Input.GetKey(KeyCode.DownArrow) || _moveDown)
                {
                    Game.instance.PlayMoveAudio();
                }
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                Game.instance.DeleteRow();
                if (Game.instance.CheckIsAboveGrid(this))
                {
                    MenuSystem.instance.GameOver(Game.instance.currentLevel, Game.currentScore);
                    Game.gameOver = true;
                }
                Game.instance.SpawnNextFigure();
                Game.currentScore += individualScore;

                enabled = false;
            }

            _fall = Time.time;
        }

        /// <summary>
        /// Checks is valid position
        /// </summary>
        /// <returns><c>true</c>, if is valid position was checked, <c>false</c> otherwise</returns>
        bool CheckIsValidPosition()
        {
            foreach (Transform block in transform)
            {
                Vector2 pos = Game.instance.Round(block.position);

                if (Game.instance.CheckIsInsideBorder(pos) == false)
                {
                    return false;
                }

                if (Game.instance.GetTransformAtGridPosition(pos) != null &&
                    Game.instance.GetTransformAtGridPosition(pos).parent != transform)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
