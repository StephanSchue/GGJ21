using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PathManager : MonoBehaviour
{
    public GameObject[] rows;
    private PathComponent[,] pathGrid;

    private void Awake()
    {
        int rowCount = rows.Length;
        int columnCount = rows[0].transform.childCount; 
        pathGrid = new PathComponent[columnCount, rowCount];
        //Debug.Log($"pathGrid: {rowCount}x{columnCount}");

        for(int y = 0; y < rowCount; y++)
        {
            PathComponent[] rowColumns = rows[y].GetComponentsInChildren<PathComponent>();

            // --- Connect Links ---
            for(int x = 0; x < columnCount; x++)
            {
                pathGrid[x, y] = rowColumns[x];
                pathGrid[x, y].name = $"{x}.{y}";

                // Up
                if(y > 0)
                {
                    pathGrid[x, y].pathLinks[0].pathComponent = pathGrid[x, y - 1];
                    pathGrid[x, y - 1].pathLinks[2].pathComponent = pathGrid[x, y];
                }   

                // Left
                if(x > 0 && x < columnCount)
                {
                    pathGrid[x, y].pathLinks[3].pathComponent = pathGrid[x - 1, y];
                    pathGrid[x - 1, y].pathLinks[1].pathComponent = pathGrid[x, y];
                }

                pathGrid[x, y].Initialize();
            }
        }
    }

    public bool FindPathToPosition(Vector3 start, Vector3 destination, out Vector3[] path, out float pathLength)
    {
        bool foundPath = false;
        List<Vector3> pathList = new List<Vector3>();
        pathLength = 0f;

        (Vector2Int startTile, PathComponent pathComponent1, PathLink originPath1, int pathIndex1) = GetPathPosition(start);
        (Vector2Int endTile, PathComponent pathComponent2, PathLink originPath2, int pathIndex2) = GetPathPosition(destination);

        Vector2Int tileHeading = endTile - startTile;
        Vector2Int tileHeadingAbsolute = new Vector2Int(Mathf.Abs(tileHeading.x), Mathf.Abs(tileHeading.y));

        foundPath = true;
        pathList.Add(pathComponent1.center.position);

        Vector2Int coordinates = startTile;

        for(int x = 0; x < tileHeadingAbsolute.x; x++)
        {
            if(tileHeading.x > 0) // Right
            {
                coordinates.x += 1;
                pathList.Add(pathGrid[coordinates.x, coordinates.y].pathLinks[1].pathPositions.Last());
            }
            else if(tileHeading.x < 0) // Left
            {
                coordinates.x -= 1;
                pathList.Add(pathGrid[coordinates.x, coordinates.y].pathLinks[3].pathPositions.Last());
            }
        }

        for(int y = 0; y < tileHeadingAbsolute.y; y++)
        {
            if(tileHeading.y > 0) // Top
            {
                coordinates.y += 1;
                pathList.Add(pathGrid[coordinates.x, coordinates.y].pathLinks[0].pathPositions.Last());
            }
            else if(tileHeading.y < 0) // Bottom
            {
                coordinates.y -= 1;
                pathList.Add(pathGrid[coordinates.x, coordinates.y].pathLinks[2].pathPositions.Last());
            }
        }

        pathList.Add(pathComponent2.center.position);
        
        //Debug.DrawLine(originPath1.path[pathIndex1].position, originPath2.path[pathIndex2].position, Color.green, 10f);

        path = pathList.ToArray();
        return foundPath;
    }

    private (Vector2Int, PathComponent, PathLink, int) GetPathPosition(Vector2 position)
    {
        Vector2Int coordinates = new Vector2Int();
        PathComponent pathComponent = new PathComponent();
        PathLink pathLink = new PathLink();
        int pathIndex = 0;
        float minDistance = 20f;

        for(int x = 0; x < pathGrid.GetLength(0); x++)
        {
            for(int y = 0; y < pathGrid.GetLength(1); y++)
            {
                float _distance = Vector2.Distance(position, pathGrid[x,y].center.position);

                if(_distance < minDistance)
                {
                    coordinates = new Vector2Int(x, y);
                    pathComponent = pathGrid[x,y];
                    (pathLink, pathIndex) = pathComponent.GetPathIndexByDistance(position);

                    minDistance = _distance;
                }
            }
        }

        return (coordinates, pathComponent, pathLink, pathIndex);
    }
}
