
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;
using static UnityEngine.EventSystems.EventTrigger;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Debug")]
    [SerializeField] public bool showFog = true;

    [Header("Map Settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;
    [SerializeField] private int maxMonstersPerRoom = 2;
    [SerializeField] private int maxItemsPerRoom = 2;

    [Header("Tiles and Tilemaps")]
    [SerializeField] private TileBase emptyTile;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase fogTile;
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;
    [SerializeField] private Tilemap fogMap;

    [Header("Features")]
    [SerializeField] private List<Vector3Int> visibleTiles = new List<Vector3Int>();
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();
    [SerializeField] private Dictionary<Vector3Int, TileData> tiles = new Dictionary<Vector3Int, TileData>();
    [SerializeField] private Dictionary<Vector2Int, Node> nodes = new Dictionary<Vector2Int, Node>();

    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public TileBase EmptyTile { get { return emptyTile; } }
    public TileBase FloorTile {  get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
    public Tilemap FogMap { get { return fogMap; } }
    public List<RectangularRoom> Rooms { get { return rooms; } }
    public Dictionary<Vector2Int, Node> Nodes { get { return nodes; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, maxItemsPerRoom, rooms);

        AddTileMapToDictionary(floorMap);
        AddTileMapToDictionary(obstacleMap);

        if (showFog)
            SetupFogMap();
    }

    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public void CreateEntity(string entity, Vector2 position)
    {
        /* Old Switch to instantiate prefabs
        switch (entity)
        {
            case "Player":
            {
                Instantiate(Resources.Load<GameObject>("Prefabs/Player"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Player";
                  break;
            }
            case "Orc":
                {
                    Instantiate(Resources.Load<GameObject>("Prefabs/Orc"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Orc";
                    break;
                }
            case "Troll":
                {
                    Instantiate(Resources.Load<GameObject>("Prefabs/Troll"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Troll";
                    break;
                }
            default:
                Debug.LogError("Entity not found");
                break;
        }
        */

        //Instantiate(Resources.Load<GameObject>("Prefabs/"+entity), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = entity;

        try
        {
            GameObject entityToCreate;
            entityToCreate = Resources.Load<GameObject>("Prefabs/" + entity);
            if (entityToCreate != null)
                Instantiate(entityToCreate, new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = entity;
            else
                Debug.LogError("Prefab '" + entity + "' not found in Resources folder!");
        }
        catch (System.Exception e)
        { 
            Debug.LogError("Error loading prefab: " + e.Message);
        }

        Vector3Int pos3 = new Vector3Int((int)position.x, (int)position.y, 0);
    }
    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        // Checks all tiles. If explored but not in FOV, reduce alpha
        foreach (Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].IsExplored)
            {
                tiles[pos].IsExplored = true;
            }

            tiles[pos].IsVisible = false;
            fogMap.SetColor(pos, new Color(1, 1, 1, 0.5f));
        }

        visibleTiles.Clear();

        // If visible, show
        foreach (Vector3Int pos in playerFOV)
        {
            //tiles[pos].isVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }
    }

    // If en entity is not in Player's FOV, hide it
    public void SetEntitiesVisibilities()
    {
        foreach (Entity entity in GameManager.instance.Entities)
        {
            if (entity.GetComponent<Player>()) continue;

            Vector3Int entityPosition = floorMap.WorldToCell(entity.transform.position);

            if (!showFog)
                entity.GetComponent<SpriteRenderer>().enabled = true;
            else
                entity.GetComponent<SpriteRenderer>().enabled = visibleTiles.Contains(entityPosition);
        }
    }

    // Fills dictionary with info about tiles
    private void AddTileMapToDictionary (Tilemap tilemap)
    {
        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin)
        {
            if (!tilemap.HasTile(pos)) continue;

            TileData tile = new TileData();
            tiles.Add(pos, tile);
        }

    }

    // Applies the dict info to the fogMap
    private void SetupFogMap()
    {
        foreach (Vector3Int pos in tiles.Keys)
        {
            fogMap.SetTile(pos, fogTile);
            fogMap.SetTileFlags(pos, TileFlags.None);
        }
    }

    public void ShowFloorTile(Vector3Int pos, bool show)
    {
        FloorMap.SetTile(pos, show ? floorTile : emptyTile);
    }
    public bool IsValidPosition(Vector3 futurePosition)
    {
        /* old code
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(futurePosition);

        if (!MapManager.instance.InBounds(gridPosition.x, gridPosition.y))
        {
            Debug.Log("Cell " + gridPosition.x + ", " + gridPosition.y + " is out of bounds!");
            return false;
        }

        else if (MapManager.instance.ObstacleMap.HasTile(gridPosition))
        {
            //Debug.Log("Cell " + gridPosition.x + ", " + gridPosition.y + " is blocked by an obstacle!");
            return false;
        }

        else if (futurePosition == transform.position)
        {
            return false;
        }

        */

        Vector3Int gridPosition = floorMap.WorldToCell(futurePosition);

        if (!InBounds(gridPosition.x, gridPosition.y) || obstacleMap.HasTile(gridPosition))
            return false;

        return true;
    }
}
