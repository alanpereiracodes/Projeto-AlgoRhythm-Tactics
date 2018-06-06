using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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

    //Recebe os tiles ao redor de uma posição
    public List<Tile> TilesAround(Vector2 _coord, int jumpHeight)
    {
        List<Tile> around = new List<Tile>();

        Tile currentTile = FindTile(_coord);

        //Clockwise search
        Tile up = FindTile(new Vector2(_coord.x, _coord.y - 1));
        if(up != null && Mathf.Abs(currentTile.height - up.height) < jumpHeight)
            around.Add(up);
        Tile right = FindTile(new Vector2(_coord.x + 1, _coord.y));
        if (right != null && Mathf.Abs(currentTile.height - right.height) < jumpHeight)
            around.Add(right);
        Tile down = FindTile(new Vector2(_coord.x, _coord.y + 1));
        if (down != null && Mathf.Abs(currentTile.height - down.height) < jumpHeight)
            around.Add(down);
        Tile left = FindTile(new Vector2(_coord.x - 1, _coord.y));
        if (left != null && Mathf.Abs(currentTile.height - left.height) < jumpHeight)
            around.Add(left);

        return around;
    }

}//Board END
