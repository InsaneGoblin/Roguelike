
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
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

    //[Header("Colors")]
    //[SerializeField] private Color32 darkColor = new Color32(0, 0, 0, 0);
    //[SerializeField] private Color32 lightColor = new Color32(255, 255, 255, 255);

    [Header("Tiles and Tilemaps")]
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


 public TileBase FloorTile {  get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
    public Tilemap FogMap { get { return fogMap; } }
    public List<RectangularRoom> Rooms { get { return rooms; } }

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        /* Old debug
        Vector3 centerTile = new Vector3Int(width / 2, height / 2, 0);

        BoundsInt wallBounds = new BoundsInt(new Vector3Int(29, 28, 0), new Vector3Int(3, 1, 0));

        for (int x = 0; x < wallBounds.size.x; x++)
        {
            for (int y = 0; y < wallBounds.size.y; y++)
            {
                Vector3Int wallPosition = new Vector3Int(wallBounds.min.x + x, wallBounds.min.y + y, 0);
                obstacleMap.SetTile(wallPosition, wallTile);
            }
        }

        Camera.main.transform.position = new Vector3(40, 20.25f, -10);
        Camera.main.orthographicSize = 27;
        */

        ProcGen procGen = new ProcGen();
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, maxMonstersPerRoom, rooms);

        AddTileMapToDictionary(floorMap);
        AddTileMapToDictionary(obstacleMap);

        if (showFog)
            SetupFogMap();

        // Instantiate(Resources.Load<GameObject>("Prefabs/NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";

    }

    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public void CreateEntity(string entity, Vector2 position)
    {
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
        //Debug.Log("Created " + entity + " at " + position);
    }
    public void UpdateFogMap(List<Vector3Int> playerFOV)
    {
        // Checks all tiles. If explored but not in FOV, reduce alpha
        foreach (Vector3Int pos in visibleTiles)
        {
            if (!tiles[pos].isExplored)
            {
                tiles[pos].isExplored = true;
            }

            tiles[pos].isVisible = false;
            fogMap.SetColor(pos, new Color(1, 1, 1, 0.5f));
        }

        visibleTiles.Clear();

        // If visible, show
        foreach (Vector3Int pos in playerFOV)
        {
            tiles[pos].isVisible = true;
            fogMap.SetColor(pos, Color.clear);
            visibleTiles.Add(pos);
        }

        // TODO Debug: remove fog, show everything
        if (showFog)
        {
            foreach (Vector3Int pos in playerFOV)
            {
                tiles[pos].isVisible = true;
                fogMap.SetColor(pos, Color.clear);
                visibleTiles.Add(pos);
            }
        }
    }

    // If something is not in Player's FOV, hide it
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

}
