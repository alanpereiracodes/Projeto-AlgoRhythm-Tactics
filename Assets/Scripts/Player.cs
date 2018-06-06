using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    void Start()
    {
        lm = LevelManager._instance;
        board = lm.board;
    }

    private void OnMouseOver()
    {
        if (onTurn && !lm.isRunning)
        {
            if (Input.GetMouseButtonDown(0))                                    //Captura o botao de click esquerod do Mouse
            {
                lm.turnPlayer = this;
                MovementSetup();
            }
        }
    }

    private void OnMouseEnter()
    {
        if (action == ActionType.Ready)
            board.FindTile(coord).CellOnPlayerSelect();
    }

    private void OnMouseExit()
    {
        if(action == ActionType.Ready)
            board.FindTile(coord).CellDispose();
    }




}
