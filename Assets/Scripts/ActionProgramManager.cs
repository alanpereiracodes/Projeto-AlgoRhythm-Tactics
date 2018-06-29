using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionProgramManager : MonoBehaviour {


    //Referencia aos Componentes da Janela do Programa de Ação
    //Action Program
    public Transform programPanel;
    public Text programAPText;

    //Command Set
    public Transform commandSetPanel;

    //Command Description Window
    public GameObject descPanel;
    public Text descNameText, descDescriptionText;
    public Image descTargetImage;

    //
    public int currentAP;
    public List<Command> program;
    public List<GameObject> allCommands;

    //private
    private List<GameObject> cmdInCmdSet;

    void Start()
    {
        if (LevelManager._instance.actionProgram == null)
            LevelManager._instance.actionProgram = this;
        else
            Destroy(this);

        program = new List<Command>();
        cmdInCmdSet = new List<GameObject>();

        foreach(GameObject g in allCommands)
        {
            GameObject newGO = Instantiate(g);
            newGO.transform.SetParent(commandSetPanel);
            newGO.SetActive(false);
            cmdInCmdSet.Add(newGO);
        }
    }

    public void AddCommandToProgram(Command cmd)
    {
        //Cant add a Command with a AP higher than the current AP.
        if (currentAP < cmd.cmdCost)
            //Error sound or desabilita os botoes que tem o AP maior no Command Set mesmo
            return;

        //Cant add a Defensive/Reactive Command if already exists one of this type in the program.
        if(cmd.cmdType == Command.CommandType.Defensive || cmd.cmdType == Command.CommandType.Reactive)
        {
            foreach(Command c in program)
            {
                if (c.cmdType == cmd.cmdType)
                    return;
            }
        }

        //AP Cost Calcs
        currentAP -= cmd.cmdCost;
        UpdateAPText();

        //Instantiate the object
        GameObject cmdObj = Instantiate(cmd.gameObject);
        cmdObj.transform.SetParent(programPanel);
        Command newCmd = cmdObj.GetComponent<Command>();
        newCmd.ID = program.Count + 1;
        newCmd.isOnProgram = true;
        program.Add(newCmd);
    }

    public void RemoveCommandFromProgram(Command cmd)
    {
        currentAP += cmd.cmdCost;
        UpdateAPText();

        foreach(Command c in program)
        {
            if (c.ID > cmd.ID)
                c.ID--;
        }

        GameObject toDestroy = cmd.gameObject;
        program.Remove(cmd);
        Destroy(toDestroy);
    }

    public void ClearProgram()
    {
        if(program.Count > 0)
        {
            foreach (Command c in program)
            {
                GameObject toDestroy = c.gameObject;
                Destroy(toDestroy);
            }
            program.Clear();
        }
    }

    public void UpdateCommandSet()
    {
        ClearCommandSet();
        if (LevelManager._instance.turnPlayer.unitType == Unit.UnitType.Player)
        {
            Player p = LevelManager._instance.turnPlayer as Player;
            List<GameObject> commandPrefabs = new List<GameObject>();

            foreach(int i in p.commandSet)
            {
                foreach(GameObject g in cmdInCmdSet)
                {
                    if (g.GetComponent<Command>().ID == i)
                        g.SetActive(true);
                }
            }
        }
    }

    public void ClearCommandSet()
    {
        foreach(GameObject g in cmdInCmdSet)
        {
            if (g.activeInHierarchy)
                g.SetActive(false);
        }
    }

    public void UpdateDescription(Command cmd)
    {
        if(!descPanel.activeInHierarchy)
        {
            descPanel.SetActive(true);
        }

        descNameText.text = cmd.cmdName.ToUpper();
        descDescriptionText.text = cmd.cmdDesc;
        descTargetImage.sprite = cmd.cmdTargetImage;
    }

    public void HideDescPanel()
    {
        if (descPanel.activeInHierarchy)
        {
            descPanel.SetActive(false);
        }
    }

    public void UpdateActionPoints()
    {
        currentAP = LevelManager._instance.turnPlayer.actionPoints;
        UpdateAPText();
    }

    void UpdateAPText()
    {
        int maxAP = LevelManager._instance.turnPlayer.actionPoints;
        Mathf.Clamp(currentAP, 0, maxAP);
        programAPText.text = currentAP.ToString() + " / " + maxAP.ToString();
    }
}
