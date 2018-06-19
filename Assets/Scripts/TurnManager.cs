using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour {

    private LevelManager lm;

    public List<Unit> units;

    public GameObject turnOrderPanel, turnUnitInfoPrefab, nextTurnUnitInfo;
    public Image nextTurnUnitImage;

    Queue<Unit> turnOrder;
    List<GameObject> unitsAtTurnPanel;

    void Awake()
    {
        //initialzie List
        units = new List<Unit>();
        unitsAtTurnPanel = new List<GameObject>();
    }

    void Start()
    {
        lm = LevelManager._instance;
        //Link to Level Manager
        if (lm.turn == null)
            lm.turn = this;
        else
            Destroy(this);
    }

    public void AddPlayerToTurnList(Player p)
    {
        units.Add(p);
        turnOrder = new Queue<Unit>();
        //Reorganiza por ordem de velocidade
        foreach (Unit u in units.OrderByDescending(unit => unit.speed).ToList())
        {
            turnOrder.Enqueue(u);
        }
    }

    public void SetupNextTurn()
    {
        //Verifica se a lista contém alguma unidade
        if (turnOrder.Count > 0)
        {
            //Pega o primeiro jogador da Fila e prepara para ele iniciar o turno.
            Unit nextUnit = turnOrder.Dequeue();

            //Enfileira novamente o jogador que está realizando o turno.
            turnOrder.Enqueue(nextUnit);
            nextUnit.action = Unit.ActionType.Ready;
            nextUnit.onTurn = true;
            if (!lm.turnHighlightArrow.activeInHierarchy)
                lm.turnHighlightArrow.SetActive(true);
            lm.turnHighlightArrow.transform.SetParent(nextUnit.transform);

            lm.hud.UpdateTurnCharacter(nextUnit);

            switch (nextUnit.unitType)
            {
                case Unit.UnitType.Player:
                    //
                    break;

                case Unit.UnitType.Enemy:
                    //
                    break;

                case Unit.UnitType.Guest:
                    //
                    break;
            }

            lm.turnPlayer = nextUnit;
            UpdateTurnOrderPanel();

        }
    }//END SetupNextTurn

    void UpdateTurnOrderPanel()
    {
        if(unitsAtTurnPanel.Count() > 0)
        {
            for (int i = 0; i < unitsAtTurnPanel.Count(); i++)
            {
                Destroy(unitsAtTurnPanel[i]);
            }  
        }

        unitsAtTurnPanel.Clear();

        for (int i = 0; i < 4 && i < turnOrder.Count(); i++)
        {
            GameObject gObj = Instantiate(turnUnitInfoPrefab);
            gObj.GetComponentInChildren<TurnUnitInfo>().unitImage.sprite = turnOrder.ElementAt(i).portraitSprite;
            gObj.transform.SetParent(turnOrderPanel.transform);
            unitsAtTurnPanel.Add(gObj);
        }

        UpdateNextUnitInfo();
    }

    void UpdateNextUnitInfo()
    {
        if (!nextTurnUnitInfo.activeInHierarchy)
            nextTurnUnitInfo.SetActive(true);
        Unit next = turnOrder.Peek();
        nextTurnUnitImage.sprite = next.portraitSprite;
    }

}//END Turn Manager
