using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Entity : MonoBehaviour
{
    [SerializeField] private bool blocksMovement;
    public bool BlocksMovement { get => blocksMovement; set => blocksMovement = value; }

    void Start()
    {
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), false);
    }

    public void AddToGameManager()
    {
        GameManager.instance.AddEntity(this);
    }

    public void Move(Vector2 direction) 
    {
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), true);
        transform.position += (Vector3)direction;
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), false);
    }
}
