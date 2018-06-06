using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ActionType = Unit.ActionType;

public class LevelManager : MonoBehaviour
{

    [HideInInspector]
    public static LevelManager _instance;

    [HideInInspector]
    public Board board;                                                         //Reference to our board

    public string mapName;
    public List<GameObject> playersPrefabs;
    public Unit turnPlayer;
    public bool isRunning;

    private List<Unit> units;
    private Queue<Unit> turnOrder;
    private int currentTurn;

    void Awake()
    {
        if (_instance == null)
            _instance = this;
        else
            Destroy(this);

        board = GameObject.FindWithTag("Board").GetComponent<Board>();
        units = new List<Unit>();
        currentTurn = 1;
        isRunning = false;
    }

    void Start()
    {
        //Inicializa o tabuleiro
        board.Setup(mapName);

        //Posiciona o Personagem 1
        board.PlayerPlacement(playersPrefabs[0], new Vector2(0, 0));

        //Posiciona o Personagem 2
        board.PlayerPlacement(playersPrefabs[1], new Vector2(4, 3));

        NextPlayerTurn();
    }

    public void AddPlayer(Player p)
    {
        p.action = ActionType.Wait;
        units.Add(p);
        turnOrder = new Queue<Unit>();
        //Reorganiza por ordem de velocidade
        foreach (Unit u in units.OrderByDescending(unit => unit.speed).ToList())
        {
            turnOrder.Enqueue(u);
        }
    }

    public void NextPlayerTurn()
    {
        //Verifica se a lita contém alguma unidade
        if(turnOrder.Count > 0)
        {
            turnPlayer = turnOrder.Dequeue();
            turnPlayer.action = ActionType.Ready;
            turnPlayer.onTurn = true;

            switch(turnPlayer.tag)
            {
                case Tag.PLAYER:
                    //
                    break;

                case Tag.ENEMY:
                    //
                    break;

                case Tag.GUEST:
                    //
                    break;
            }
        }
        else
        {
            currentTurn++;
            foreach (Unit u in units.OrderByDescending(unit => unit.speed).ToList())
            {
                turnOrder.Enqueue(u);
            }
            NextPlayerTurn();
        }
    }


    #region Movement Setup
    //Recebe o Tile para qual o personagem irá se movimenatr e cria o caminho a ser traçado usando uma pilha.
    public void MoveCharacterToTile(Tile destination)
    {
        isRunning = true;
        //Cria pilha dos Tiles do caminho com Backtracking através do
        //previousTile de cada MovableTile
        Stack<MovableTile> path = new Stack<MovableTile>();
        MovableTile temp = turnPlayer.FindTileToMove(destination);

        while (temp != null)
        {
            path.Push(temp);
            temp = turnPlayer.FindTileToMove(temp.previousTile);
        }

        //Movimenta o Personagem
        StartCoroutine(MoveCharacterToPath(path));
    }

    //Gerencia o tempo da animação e chama a função de conclusão do Movimento
    IEnumerator MoveCharacterToPath(Stack<MovableTile> path)
    {
        Tile toMove = null;

        while (path.Count > 0)
        {
            toMove = path.Pop().tile;
            Debug.Log(toMove.coord);
            Vector3 currentPos = turnPlayer.gameObject.transform.position;
            Vector3 destinationPos = new Vector3(toMove.gameObject.transform.position.x, toMove.gameObject.transform.position.y + 1.5f + (toMove.height - 1) * 0.25f, toMove.gameObject.transform.position.z);

            float timeOfMovement = (float)turnPlayer.movementSpeed / 10f;

            //Verifica se há uma diferência de altura do tile atual em comparação ao próximo tile
            if (System.Math.Abs(currentPos.y - destinationPos.y) < float.Epsilon)
            {
                StartCoroutine(MoveUnit(turnPlayer.gameObject, currentPos, destinationPos, timeOfMovement));
            }
            else
            {
                //Calculates a midway point
                Vector3 midwayPoint;
                if (destinationPos.y > currentPos.y)
                {
                    midwayPoint = new Vector3(currentPos.x, currentPos.y + (destinationPos.y - currentPos.y) / 1.25f, currentPos.z);
                    Vector3 midwayPoint2 = new Vector3(currentPos.x + (destinationPos.x - currentPos.x) / 3, destinationPos.y + 0.1f, currentPos.z + (destinationPos.z - currentPos.z) / 3);
                    StartCoroutine(MoveUnit(turnPlayer.gameObject, new Vector3[] { currentPos, midwayPoint, midwayPoint2, destinationPos }, timeOfMovement / 4));
                }
                else
                {
                    midwayPoint = new Vector3(currentPos.x + (destinationPos.x - currentPos.x) / 1.2f, currentPos.y, currentPos.z + (destinationPos.z - currentPos.z) / 1.2f);
                    StartCoroutine(MoveUnit(turnPlayer.gameObject, new Vector3[] { currentPos, midwayPoint, destinationPos }, timeOfMovement / 2.5f));
                }
            }

            yield return new WaitForSeconds(timeOfMovement);
        }

        //Atualiza as coordenadas do personagem que foi movimentado
        turnPlayer.Moved(toMove.coord);

        //Finaliza o movimento
        FinishMovement();

        yield break;
    }

    //Without Midpoints movement, just source and destination interpolation
    IEnumerator MoveUnit(GameObject unit, Vector3 source, Vector3 target, float overTime)
    {
        float startTime = Time.time;
        while (Time.time < startTime + overTime)
        {
            float t = (Time.time - startTime) / overTime;
            unit.transform.position = Vector3.Lerp(source, target, t);
            yield return null;
        }

        unit.transform.position = target;
    }

    //Movement with Midway points, generally used for jumping movement
    IEnumerator MoveUnit(GameObject unit, Vector3[] waypoints, float overTime)
    {
        float startTime;
        for (int i = 1; i < waypoints.Length; i++)
        {
            startTime = Time.time;
            while (Time.time < startTime + overTime)
            {
                float t = (Time.time - startTime) / overTime;
                unit.transform.position = Vector3.Lerp(waypoints[i - 1], waypoints[i], t);
                yield return null;
            }

            unit.transform.position = waypoints[i];
        }
    }

    //Realiza algumas configurações após o movimento.
    void FinishMovement()
    {
        if (turnPlayer.action == ActionType.Wait)
        {
            NextPlayerTurn();
        }
        isRunning = false;
    }

#endregion


}//END LevelManager
