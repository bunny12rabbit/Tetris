using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UserInput : MonoBehaviour
{
    private FigurePositionController _figureController;
    [Inject] private GameController _gameController;

    private void Update()
    {
        if (_gameController.state == GameStates.Playing && _figureController != null)
        {
            CheckUserInput();
        }
    }

    public void SetCurrentFigure(GameObject figure)
    {
        _figureController = figure.GetComponent<FigurePositionController>();
    }

    private void CheckUserInput()
    {
        if (_gameController.state == GameStates.Playing)
        {
            if (Input.touchCount > 0)
            {
                foreach (Touch touch in Input.touches)
                {
                    //  Check if touched once to Move, not holding
                    if (CheckLeftOrRightMoveOnceTouch(touch))
                    {
                        _figureController.ResetBtnPressedTimersHorizontal();
                    }

                    if (CheckDownMoveOnceTouch(touch))
                    {
                        _figureController.ResetBtnPressedTimersVertical();
                        _figureController.moveDown = false;
                    }

                    //  Then attempt to move accordingly
                    if (MoveRightTouch(touch))
                    {
                        //  Move Right
                        _figureController.MoveRight();
                    }

                    if (MoveLeftTouch(touch))
                    {
                        //  Move Left
                        _figureController.MoveLeft();
                    }

                    if (MoveDownTouch(touch))
                    {
                        //  Move Down
                        _figureController.moveDown = true;
                        _figureController.MoveDown();
                    }

                    if (RotateTouch(touch))
                    {
                        //  Rotate
                        _figureController.Rotate();
                    }
                }
            }
            
        }
    }


    private bool CheckLeftOrRightMoveOnceTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
                                                                (touch.position.x > Screen.width / 2 || touch.position.x < Screen.width / 2);

    private bool CheckDownMoveOnceTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
                                                         (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 3);

    private bool MoveRightTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
                                                (touch.position.x > Screen.width / 2 && touch.position.y > Screen.height / 4);

    private bool MoveLeftTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
                                               (touch.position.x < Screen.width / 2 && touch.position.y > Screen.height / 4);

    private bool MoveDownTouch(Touch touch) => touch.phase == TouchPhase.Stationary &&
                                               (touch.position.x < Screen.width / 2 && touch.position.y < Screen.height / 4);

    private bool RotateTouch(Touch touch) => touch.phase == TouchPhase.Ended &&
                                                  (touch.position.x > Screen.width / 2 && touch.position.y < Screen.height / 4);

}