using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Direction = Unit.Facing;


[System.Serializable]
public class Column
{
    public List<int> row;                                                       //Important to note that all Rows in the map has to have the same lenght.
}

[System.Serializable]
public class Map
{
    public List<Column> tilemap;

    public static void SaveMapData(Map data, string filename)
    {
        string path = Application.streamingAssetsPath + "/Maps/" + filename + ".json";
        string jsonString = JsonUtility.ToJson(data, true);
        System.IO.File.WriteAllText(path, jsonString);

        Debug.Log("Mapa salvo em:" + path + "\n" + jsonString);
    }

    public static Map LoadMapData(string filename)
    {
        string path = Application.streamingAssetsPath + "/Maps/" + filename + ".json";
        string jsonString = System.IO.File.ReadAllText(path);

        Debug.Log("Mapa carregado:\n" + jsonString);

        Map loadedMap = JsonUtility.FromJson<Map>(jsonString);
        return loadedMap;
    }
}


//Board Script Beginning
public class Board : MonoBehaviour 
{
    //Public
    public List<GameObject> tileSet;                                            //Set of the Tiles Prefabs that will compound our board.
    public Vector2 boardSize;


    //Private
    private Map map;
    private List<Tile> tileList;

    public void Setup(string mapName)
    {
        //Load Map from a JSON File
        map = Map.LoadMapData(mapName);

        //Get Board Size from the Map loaded
        boardSize.x = map.tilemap[0].row.Count;
        boardSize.y = map.tilemap.Count;

        //Generate Board
        GenerateMap();
    }

    void GenerateMap()
    {
        //Initialize tile list
        tileList = new List<Tile>();

        //Travels the map and instantiate the tile based on the value in the current coordinate[x, y]
        for (int col = 0; col < map.tilemap.Count; col++)
        {
            for (int row = 0; row < map.tilemap[col].row.Count; row++)
            {
                //Tile Position
                float posY = (tileSet[map.tilemap[col].row[row]].transform.localScale.y - 1) / 2;
                Vector3 pos = new Vector3(-col + boardSize.y / 2, posY, -row + boardSize.x / 2);

                //Create our Tile
                GameObject gObj = Instantiate(tileSet[map.tilemap[col].row[row]], pos, Quaternion.identity);

                //Setup the Tile parameters
                Tile tile = gObj.GetComponent<Tile>();

                tile.coord.x = row;
                tile.coord.y = col;

                //Add the Tile to the List
                tileList.Add(tile);

                //Set the new Tile as an Child of Board Manager
                gObj.transform.SetParent(transform);
            }
        }
    }

    public void PlayerPlacement(GameObject player, Vector2 _coord)
    {
        Tile t = FindTile(_coord);
        GameObject tileObject = t.gameObject;

        Vector3 pos = new Vector3(tileObject.transform.position.x, tileObject.transform.position.y + 1.5f, tileObject.transform.position.z); 
        Player newPlayer = Instantiate(player, pos, Quaternion.identity).GetComponent<Player>();
        newPlayer.coord = _coord;

        //Adiciona a referencia do jogador ao Tile
        t.AddObject(newPlayer.gameObject, Tile.ObjectType.Unit);

        //Adiciona o novo jogador a lista de Unidades no Jogo
        LevelManager._instance.AddPlayer(newPlayer);

    }

    public Tile FindTile(Vector2 _coord)
    {
        foreach(Tile t in tileList)
        {
            if(t.coord == _coord)
            {
                return t; 
            }
        }
        Debug.Log("Nenhum Tile pode ser encontrado na posição: " + _coord);
        return null;
    }

    //Recebe os tiles ao redor de uma posição que possam se rmovimentados (levando em consideração a altura)
    public List<Tile> TilesAround(Vector2 _coord, int jumpHeight)
    {
        List<Tile> around = new List<Tile>();

        Tile currentTile = FindTile(_coord);

        //Clockwise search
        Tile up = GetUpTile(_coord);
        if(up != null && Mathf.Abs(currentTile.height - up.height) < jumpHeight)
            around.Add(up);
        Tile right = GetRightTile(_coord);
        if (right != null && Mathf.Abs(currentTile.height - right.height) < jumpHeight)
            around.Add(right);
        Tile down = GetDownTile(_coord);
        if (down != null && Mathf.Abs(currentTile.height - down.height) < jumpHeight)
            around.Add(down);
        Tile left = GetLeftTile(_coord);
        if (left != null && Mathf.Abs(currentTile.height - left.height) < jumpHeight)
            around.Add(left);

        return around;
    }


    //Get Direction Tiles
    public Tile GetUpTile(Vector2 _coord)
    {
        return FindTile(new Vector2(_coord.x, _coord.y - 1));
    }

    public Tile GetRightTile(Vector2 _coord)
    {
        return FindTile(new Vector2(_coord.x + 1, _coord.y));
    }

    public Tile GetDownTile(Vector2 _coord)
    {
        return FindTile(new Vector2(_coord.x, _coord.y + 1));
    }

    public Tile GetLeftTile(Vector2 _coord)
    {
        return FindTile(new Vector2(_coord.x - 1, _coord.y));
    }

    //TargetType Methods
    public Tile TargetFrontTile(Direction facing, Vector2 _coord)
    {
        switch (facing)
        {
            case Direction.Up:
                return GetUpTile(_coord);

            case Direction.Right:
                return GetRightTile(_coord);

            case Direction.Down:
                return GetDownTile(_coord);

            case Direction.Left:
                return GetLeftTile(_coord);
        }

        return null;
    }

    public List<Tile> TargetFrontTriangle(Direction facing, Vector2 _coord)
    {
        List<Tile> triangle = new List<Tile>();
        Tile t = null;
        Vector2 tempCoord = new Vector2();

        switch (facing)
        {
            case Direction.Up:
                //UP
                t = GetUpTile(_coord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }
                else
                {
                    tempCoord = new Vector2(_coord.x, _coord.y - 1);
                }

                //UP, UP
                t = GetUpTile(tempCoord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }

                else
                {
                    tempCoord = new Vector2(_coord.x, _coord.y - 2);
                }

                //UP, R
                t = GetRightTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);

                //UP, L
                t = GetLeftTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);
                break;

            case Direction.Right:
                //R
                t = GetRightTile(_coord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }
                else
                {
                    tempCoord = new Vector2(_coord.x + 1, _coord.y);
                }

                //R, R
                t = GetRightTile(tempCoord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }

                else
                {
                    tempCoord = new Vector2(_coord.x + 2, _coord.y);
                }

                //R, UP
                t = GetUpTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);

                //R, DOWN
                t = GetDownTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);
                break;

            case Direction.Down:
                //DOWN
                t = GetDownTile(_coord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }
                else
                {
                    tempCoord = new Vector2(_coord.x, _coord.y + 1);
                }

                //DOWN, DOWN
                t = GetDownTile(tempCoord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }

                else
                {
                    tempCoord = new Vector2(_coord.x, _coord.y + 2);
                }

                //DOWN, R
                t = GetRightTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);

                //DOWN, L
                t = GetLeftTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);
                break;

            case Direction.Left:
                //L
                t = GetLeftTile(_coord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }
                else
                {
                    tempCoord = new Vector2(_coord.x - 1, _coord.y);
                }

                //L, L
                t = GetLeftTile(tempCoord);
                if (t != null && !t.isBlocked)
                {
                    tempCoord = t.coord;
                    triangle.Add(t);
                }

                else
                {
                    tempCoord = new Vector2(_coord.x - 2, _coord.y);
                }

                //L, UP
                t = GetUpTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);

                //L, DOWN
                t = GetDownTile(tempCoord);
                if (t != null && !t.isBlocked)
                    triangle.Add(t);
                break;
        }

        return triangle;
    }

    public List<Tile> TargetAround(Vector2 _coord)
    {
        List<Tile> around = new List<Tile>();

        Tile up = GetUpTile(_coord);
        if (up != null && !up.isBlocked)
            around.Add(up);
        Tile right = GetRightTile(_coord);
        if (right != null && !right.isBlocked)
            around.Add(right);
        Tile down = GetDownTile(_coord);
        if (down != null && !down.isBlocked)
            around.Add(down);
        Tile left = GetLeftTile(_coord);
        if (left != null && !left.isBlocked)
            around.Add(left);

        return around;
    }

    //TODO:
    public Tile TargetCellAt(Direction facing, Vector2 _coord, int distance)
    {
        switch (facing)
        {
            case Direction.Up:
                return GetUpTile(_coord);

            case Direction.Right:
                return GetRightTile(_coord);

            case Direction.Down:
                return GetDownTile(_coord);

            case Direction.Left:
                return GetLeftTile(_coord);
        }

        return null;
    }

    //TODO:
    public List<Tile> TargetCrossAt(Direction facing, Vector2 _coord, int distance)
    {
        List<Tile> cross = new List<Tile>();

        switch (facing)
        {
            case Direction.Up:

                break;

            case Direction.Right:

                break;

            case Direction.Down:

                break;

            case Direction.Left:

                break;
        }

        return cross;
    }

    //TODO:
    public List<Tile> TargetLine(Direction facing, Vector2 _coord)
    {
        List<Tile> Line = new List<Tile>();

        switch (facing)
        {
            case Direction.Up:

                break;

            case Direction.Right:

                break;

            case Direction.Down:

                break;

            case Direction.Left:

                break;
        }

        return Line;
    }

}//Board END
