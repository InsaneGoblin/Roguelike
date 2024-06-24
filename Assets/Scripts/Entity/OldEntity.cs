/* Old class
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class OldEntity : MonoBehaviour
{
    [SerializeField] private bool isSentient = false, blocksMovement;
    [SerializeField] private Vector2 currentPosition;
    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView;

    AdamMilVisibility algorithm;

    public bool IsSentient { get => isSentient; }
    public bool BlocksMovement { get => blocksMovement; }

    void Start()
    {
        if (GetComponent<Player>())
            GameManager.instance.InsertActor(this, 0);
        else if (IsSentient)
            GameManager.instance.AddEntity(this);

        if (IsSentient)
        {
            if (GetComponent<Player>())
                GameManager.instance.InsertActor(this, 0);
            else
                GameManager.instance.AddEntity(this);
        }

        fieldOfView = new List<Vector3Int>();
        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), false);
    }

    public void Move(Vector2 direction) 
    {
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), true);
        transform.position += (Vector3)direction;
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), false);
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        algorithm.Compute(gridPosition, fieldOfViewRange, fieldOfView);

        if (GetComponent <Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }

    }
}
*/
