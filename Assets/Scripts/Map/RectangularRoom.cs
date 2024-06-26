using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class RectangularRoom
{
    [SerializeField] private int x, y, width, height, overflow = 2;

    public int X { get => x; set { x = value; } }
    public int Y { get => y; set { y = value; } }
    public int Width { get =>  width; set { width = value; } }
    public int Height { get =>  height; set { height = value; } }

    public RectangularRoom (int x, int y, int width, int height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }

    // Return the center of the room
    public Vector2Int Center() => new Vector2Int(x+width/2, y+height/2);

    // Return the area of the room as a Bounds
    public Bounds GetBounds() => new Bounds(new Vector3(x, y, 0), new Vector3(width + overflow, height + overflow, 0));

    // Return the area of the room as a BoundsInt
    public BoundsInt GetBoundsInt() => new BoundsInt(new Vector3Int(x, y, 0), new Vector3Int(width + overflow, height + overflow, 0));

    // True if this new room overlaps another one
    public bool Overlaps(List<RectangularRoom> otherRooms)
    {
        foreach (RectangularRoom otherRoom in otherRooms)
        {
            if (GetBounds().Intersects(otherRoom.GetBounds()))
            {
                // Debug.Log("New room overlapped with " + otherRoom.GetBounds());
                return true;
            }
        }

        return false;
    }
}
