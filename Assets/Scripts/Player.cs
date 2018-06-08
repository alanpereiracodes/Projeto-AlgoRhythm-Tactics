using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    void Start()
    {
        lm = LevelManager._instance;
    }

    private void OnMouseOver()
    {
        if (onTurn && !lm.isRunning)
        {
            if (Input.GetMouseButtonDown(0))                                    //Captura o botao de click esquerod do Mouse
            {
                //Abre o balão de Menu
                lm.hud.ActivateMenuPanel();
                lm.HideHighlightArrow();
            }
        }
    }

    private void OnMouseEnter()
    {
        if (action == ActionType.Ready)
            lm.board.FindTile(coord).CellOnPlayerSelect();
    }

    private void OnMouseExit()
    {
        if(action == ActionType.Ready)
            lm.board.FindTile(coord).CellDispose();
    }




}
