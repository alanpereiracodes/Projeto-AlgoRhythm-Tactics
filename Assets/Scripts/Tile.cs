using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableTile {

    public Tile tile;
    public int cost;
    public Tile previousTile;                                                   //Usado para identificar qual foi o Tile que encontrou o caminho até este Tile.
    //Direção tomada?

    public MovableTile(Tile t, int moveCost)
    {
        tile = t;
        cost = moveCost;
        previousTile = null;
    }

    public MovableTile(Tile t, int moveCost, Tile p)
    {
        tile = t;
        cost = moveCost;
        previousTile = p;
    }
}

public class Tile : MonoBehaviour {

    public enum ObjectType
    {
        Unit = 1,
        Obstacle,
        Collectable,
        Treasure,
        Interactive
    }

    /// <summary>
    /// Determina a posição x e y que este Tile está no mapa.
    /// </summary>
    public Vector2 coord;

    /// <summary>
    /// The height of the Tile.
    /// </summary>
    public int height;

    /// <summary>
    /// Determina se o tile pode ser atravessado ou bloqueia a passagem.
    /// </summary>
    public bool isBlocked;

    public SpriteRenderer cellSprite;

    /// <summary>
    /// Determina se possui algum objeto no Tile, podendo ser:
    /// 1 - Unidade (Personagem jogável, Ajudante, Inimigo, Chefão, etc)
    /// 2 - Obstáculo (Árvore, Pedra, Arbusto, Cercado, etc),
    /// 3 - Item coletável* (Moeda, saco com itens, maçã, etc),
    /// 4 - Item tesouro (Báu de Tesouro, Caixa com itens),
    /// 5 - Objeto interativo (Alavancas, Botões, Cristais Mágicos, Relíquias, etc),
    /// 
    /// * Somente o item coletável não bloqueia o caminho, pois ele será coletado quando um personagem jogável pisar no tile onde está)
    /// </summary>
    public GameObject unit, obstacle, itemCollectable, itemTreasure, interactiveObject;


    private LevelManager lm;

    void Start()
    {
        lm = LevelManager._instance;
    }


    /// <summary>
    /// Int type:
    /// 1 - Personagem (Personagem jogável, Ajudante, Inimigo, Chefão, etc)
    /// 2 - Obstáculo (Árvore, Pedra, Arbusto, Cercado, etc),
    /// 3 - Item coletável* (Moeda, saco com itens, maçã, etc),
    /// 4 - Item tesouro (Báu de Tesouro, Caixa com itens),
    /// 5 - Objeto interativo (Alavancas, Botões, Cristais Mágicos, Relíquias, etc)
    ///</summary>
    public void AddObject(GameObject obj, ObjectType type)
    {
        isBlocked = type != ObjectType.Collectable;
        switch(type)
        {
            case ObjectType.Unit:
                unit = obj;
                break;
            case ObjectType.Obstacle:
                obstacle = obj;
                break;
            case ObjectType.Collectable:
                itemCollectable = obj;
                break;
            case ObjectType.Treasure:
                itemTreasure = obj;
                break;
            case ObjectType.Interactive:
                interactiveObject = obj;
                break;
        }
    }

    public bool CheckExistence(ObjectType type)
    {
        switch (type)
        {
            case ObjectType.Unit:
                return unit != null;
            case ObjectType.Obstacle:
                return obstacle != null;
            case ObjectType.Collectable:
                return itemCollectable != null;
            case ObjectType.Treasure:
                return itemTreasure != null;
            case ObjectType.Interactive:
                return interactiveObject != null;
        }
        return false;
    }

    public void RemoveObject(ObjectType type)
    {
        isBlocked = false;
        switch (type)
        {
            case ObjectType.Unit:
                unit = null;
                break;
            case ObjectType.Obstacle:
                obstacle = null;
                break;
            case ObjectType.Collectable:
                itemCollectable = null;
                break;
            case ObjectType.Treasure:
                itemTreasure = null;
                break;
            case ObjectType.Interactive:
                interactiveObject = null;
                break;
        }
    }

    //Green
    public void CellMovable()
    {
        cellSprite.gameObject.SetActive(true);
        cellSprite.color = new Color(0.55f, 1f, 0.625f, 0.6f);
    }

    //Yellow
    public void CellOnPlayerSelect()
    {
        cellSprite.gameObject.SetActive(true);
        cellSprite.color = new Color(1f, 1f, 0.3f, 0.6f);
    }

    //Red
    public void CellOnRedTarget()
    {
        cellSprite.gameObject.SetActive(true);
        cellSprite.color = new Color(1f, 0.3f, 0.3f, 0.6f);
    }

    //Blue
    public void CellOnBlueTarget()
    {
        cellSprite.gameObject.SetActive(true);
        cellSprite.color = new Color(0f, 0.8f, 1f, 0.6f);
    }

    public void CellDispose()
    {
        if(cellSprite.gameObject.activeInHierarchy)
            cellSprite.gameObject.SetActive(false);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))                                        //Captura o botao de click esquerod do Mouse
        {
            if (lm.turnPlayer != null && lm.currentStatus == LevelManager.Status.Moving)                 //Verifica se a ação na qual o jogador está realziando é de movimento
            {
                MovableTile mT = lm.turnPlayer.FindTileToMove(this);
                if (lm.turnPlayer.tilesToMove.Contains(mT))                   //Verifica se esse Tile é um dos Tiles para qual o personagem pode se mover;
                    MoveCharacter();

            }
                
        }
    }

    //Informa ao Level Manager o Tile de destino do movimento do jogador.
    void MoveCharacter()
    {
        if (coord != lm.turnPlayer.coord)
            lm.MoveCharacterToTile(this);
    }

}
