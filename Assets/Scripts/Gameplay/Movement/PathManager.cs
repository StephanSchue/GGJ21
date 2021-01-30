using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public PathComponent[] pathComponents;

    private void Awake()
    {
        pathComponents = GetComponentsInChildren<PathComponent>();
    }

    public bool FindPathToPosition(Vector3 start, Vector3 destination, out Vector3[] path, out float pathLength)
    {
        bool foundPath = false;
        List<Vector3> pathList = new List<Vector3>();
        pathLength = 0f;

        (PathComponent pathComponent1, PathLink originPath1, int pathIndex1) = GetPathPosition(start);
        (PathComponent pathComponent2, PathLink originPath2, int pathIndex2) = GetPathPosition(destination);

        foundPath = true;
        pathList.Add(originPath1.path[pathIndex1].position);
        pathList.Add(originPath2.path[pathIndex2].position);

        Debug.DrawLine(originPath1.path[pathIndex1].position, originPath2.path[pathIndex2].position, Color.green, 10f);

        path = pathList.ToArray();
        return foundPath;
    }

    private (PathComponent, PathLink, int) GetPathPosition(Vector2 position)
    {
        PathComponent pathComponent = new PathComponent();
        PathLink pathLink = new PathLink();
        int pathIndex = 0;
        float minDistance = 20f;

        for(int i = 0; i < pathComponents.Length; i++)
        {
            float _distance = Vector2.Distance(position, pathComponents[i].center.position);

            if(_distance < minDistance)
            {
                pathComponent = pathComponents[i];
                (pathLink, pathIndex) = pathComponent.GetPathIndexByDistance(position);

                minDistance = _distance;
            }
        }

        return (pathComponent, pathLink, pathIndex);
    }
}
