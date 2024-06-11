using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Burst.Intrinsics.Arm;

sealed class ProcGen
{
    public void GenerateDungeon(int mapWidth, int mapHeight, int roomMaxSize, int roomMinSize, int maxRooms, List<RectangularRoom> rooms)
    {
        for (int roomNum = 0; roomNum < maxRooms; roomNum++)
        {
            int roomWidth = Random.Range(roomMinSize, roomMaxSize);
            int roomHeight = Random.Range(roomMinSize, roomMaxSize);

            int roomX = Random.Range(0, mapWidth - roomWidth - 1);
            int roomY = Random.Range(0, mapHeight - roomHeight - 1);

            RectangularRoom newRoom = new RectangularRoom(roomX, roomY, roomWidth, roomHeight);
            //StartCoroutine(BuildDelay());

            // Check if there's overlap
            if (newRoom.Overlaps(rooms))
            {
                continue;
            }

            for (int x = roomX; x < roomX + roomWidth; x++)
            {
                for (int y = roomY; y < roomY + roomHeight; y++)
                {
                    if (x == roomX || x == roomX + roomWidth - 1 || y == roomY || y == roomY + roomHeight - 1)
                    {
                        if (SetWallTileIfEmpty(new Vector3Int(x, y, 0))) 
                        {
                            continue;
                        }
                    }
                    else
                    {
                        SetFloorTile(new Vector3Int(x, y));
                    }
                }
            }

            if (rooms.Count == 0)
            {
                // The first room, where the player starts
                MapManager.instance.CreatePlayer(newRoom.Center());
            }
            else
            {
                // Dig tunnel between this room and the previous one
                TunnelBetween(rooms[rooms.Count - 1], newRoom);
            }

            rooms.Add(newRoom);
        }

    }

    private void TunnelBetween(RectangularRoom oldRoom, RectangularRoom newRoom)
    {
        Vector2Int oldRoomCenter = oldRoom.Center();
        Vector2Int newRoomCenter = newRoom.Center();
        Vector2Int tunnelCorner;

        if (Random.value < 0.5f) // Horizonal, then vertical
            tunnelCorner = new Vector2Int(newRoomCenter.x, oldRoomCenter.y);
        else // Vertical, then horizontal
            tunnelCorner = new Vector2Int(oldRoomCenter.x, newRoomCenter.y);

        List<Vector2Int> tunnelCoords = new List<Vector2Int>();

        // Generate tunnel logic
        BresenhamLine.Compute(oldRoomCenter, tunnelCorner, tunnelCoords);
        BresenhamLine.Compute(tunnelCorner, newRoomCenter, tunnelCoords);

        // Set tunnel tiles
        for (int i = 0; i < tunnelCoords.Count; i++)
        {
            SetFloorTile(new Vector3Int(tunnelCoords[i].x, tunnelCoords[i].y));

            // Set tiles around as walls
            for (int x = tunnelCoords[i].x - 1; x <= tunnelCoords[i].x + 1; x++)
            {
                for (int y = tunnelCoords[i].y - 1; y <= tunnelCoords[i].y + 1; y++)
                {
                    if (SetWallTileIfEmpty(new Vector3Int(x, y, 0)))
                    {
                        continue;
                    }
                }
            }
        }
    }

    private bool SetWallTileIfEmpty(Vector3Int pos)
    {
        if (MapManager.instance.FloorMap.GetTile(pos))
        {
            return true;
        }
        else
        {
            MapManager.instance.ObstacleMap.SetTile(pos, MapManager.instance.WallTile);
            return false;
        }
    }

    private void SetFloorTile(Vector3Int pos)
    {
        if (MapManager.instance.ObstacleMap.GetTile(pos))
        {
            MapManager.instance.ObstacleMap.SetTile(pos, null);
        }

        MapManager.instance.FloorMap.SetTile(pos, MapManager.instance.FloorTile);
    }


    private IEnumerator BuildDelay()
    {
        yield return new WaitForSeconds(0.25f);
    }
}
