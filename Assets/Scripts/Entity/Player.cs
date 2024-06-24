using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Controls.IPlayerActions
{
    private Controls controls;

    [SerializeField] private bool moveKeyHeld;

    private void Awake()
    {
        controls = new Controls();
    }

    private void OnEnable()
    {
        controls.Player.SetCallbacks(this);
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Player.SetCallbacks(null);
        controls.Disable();
    }

    void Controls.IPlayerActions.OnMovement(InputAction.CallbackContext context)
    {
        if (context.started)
            moveKeyHeld = true;
        else if (context.canceled)
            moveKeyHeld = false;
    }

    void Controls.IPlayerActions.OnExit(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (context.performed)
            Action.EscapeAction();
    }

    void Controls.IPlayerActions.OnHideFog(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        // TODO : add live key
        if (context.performed)
            MapManager.instance.showFog = !MapManager.instance.showFog;
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.IsPlayerTurn && moveKeyHeld && GetComponent<Actor>().IsAlive)
            MovePlayer();
    }

    private void MovePlayer()
    {
        Vector2 direction = controls.Player.Movement.ReadValue<Vector2>();
        Vector2 roundedDirection = new Vector2(Mathf.Round(direction.x), Mathf.Round(direction.y));
        Vector3 futurePosition = transform.position + (Vector3)roundedDirection;

        if (IsValidPosition(futurePosition))
            moveKeyHeld = Action.BumpAction(GetComponent<Actor>(), roundedDirection);
    }

    private bool IsValidPosition (Vector3 futurePosition)
    {
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

        return true;
    }

}
