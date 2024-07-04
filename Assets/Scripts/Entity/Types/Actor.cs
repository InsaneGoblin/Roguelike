using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : Entity
{
    [SerializeField] private bool isAlive = true;
    [SerializeField] private int fieldOfViewRange = 8;
    [SerializeField] private List<Vector3Int> fieldOfView;
    [SerializeField] private AI ai;
    [SerializeField] private Inventory inventory;

    AdamMilVisibility algorithm;

    public int FieldOfViewRange { get => fieldOfViewRange; }
    public bool IsAlive { get => isAlive; set => isAlive = value; }
    public List<Vector3Int> FieldOfView { get => fieldOfView; }
    public Inventory Inventory { get => inventory; }

    private void OnValidate()
    {
        if (GetComponent<AI>())
            ai = GetComponent<AI>();

        if (GetComponent <Inventory>())
            inventory = GetComponent<Inventory>();
    }

    void Start()
    {
        AddToGameManager();

        if (GetComponent<Player>())
            GameManager.instance.InsertActor(this, 0);
        else if (isAlive)
            GameManager.instance.AddActor(this);

        algorithm = new AdamMilVisibility();
        UpdateFieldOfView();
        MapManager.instance.ShowFloorTile(MapManager.instance.FloorMap.WorldToCell(transform.position), false);
    }

    public void UpdateFieldOfView()
    {
        Vector3Int gridPosition = MapManager.instance.FloorMap.WorldToCell(transform.position);

        fieldOfView.Clear();
        algorithm.Compute(gridPosition, fieldOfViewRange, fieldOfView);

        if (GetComponent<Player>())
        {
            MapManager.instance.UpdateFogMap(fieldOfView);
            MapManager.instance.SetEntitiesVisibilities();
        }

    }
}
