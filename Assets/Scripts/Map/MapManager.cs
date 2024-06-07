
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager instance;

    [Header("Map Settings")]
    [SerializeField] private int width = 80;
    [SerializeField] private int height = 45;
    [SerializeField] private int roomMaxSize = 10;
    [SerializeField] private int roomMinSize = 6;
    [SerializeField] private int maxRooms = 30;

    [Header("Colors")]
    [SerializeField] private Color32 darkColor = new Color32(0, 0, 0, 0);
    [SerializeField] private Color32 lightColor = new Color32(255, 255, 255, 255);

    [Header("Tiles and Tilemaps")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap obstacleMap;

    [Header("Features")]
    [SerializeField] private List<RectangularRoom> rooms = new List<RectangularRoom>();

    public TileBase FloorTile {  get { return floorTile; } }
    public TileBase WallTile { get { return wallTile; } }
    public Tilemap FloorMap { get { return floorMap; } }
    public Tilemap ObstacleMap { get { return obstacleMap; } }
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
        procGen.GenerateDungeon(width, height, roomMaxSize, roomMinSize, maxRooms, rooms);

        Instantiate(Resources.Load<GameObject>("Prefabs/NPC"), new Vector3(40 - 5.5f, 25 + 0.5f, 0), Quaternion.identity).name = "NPC";

    }

    public bool InBounds(int x, int y) => 0 <= x && x < width && 0 <= y && y < height;

    public void CreatePlayer(Vector2 position)
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/Player"), new Vector3(position.x + 0.5f, position.y + 0.5f, 0), Quaternion.identity).name = "Player";
    }

}
