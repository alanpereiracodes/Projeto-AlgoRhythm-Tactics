using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit {

    public List<Command.CommandName> commandSet;

    //Equipments
    //public Weapon playerWeapon;
    //

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
                lm.currentStatus = LevelManager.Status.Selecting;
                lm.hud.ActivateMenuPanel();
                lm.HideHighlightArrow();
            }
        }
    }

    private void OnMouseEnter()
    {
        if (action == ActionType.Ready && lm.currentStatus == LevelManager.Status.Waiting)
            lm.board.FindTile(coord).CellOnPlayerSelect();
    }

    private void OnMouseExit()
    {
        if(action == ActionType.Ready && lm.currentStatus == LevelManager.Status.Waiting)
            lm.board.FindTile(coord).CellDispose();
    }



}
