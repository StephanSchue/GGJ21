using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PathLink
{
    public string label;
    public bool enabled;
    public Transform[] path;
    public Vector3[] pathPositions;
    public PathComponent pathComponent;

    private float length;

    public float Length => length;

    public void CalculatePath()
    {
        length = 0f;
        pathPositions = new Vector3[path.Length];

        for(int i = 0; i < path.Length; i++)
        {
            pathPositions[i] = path[i].position;

            if(i > 0)
                length += Vector3.Distance(pathPositions[i-1], pathPositions[i]);
        }
    }
}

public class PathComponent : MonoBehaviour
{
    public Transform center;
    public PathLink[] pathLinks;

    public List<PathLink> availablePathLinks = new List<PathLink>();

    public void Initialize()
    {
        for(int i = 0; i < pathLinks.Length; i++)
        {
            pathLinks[i].CalculatePath();

            if(pathLinks[i].enabled)
                availablePathLinks.Add(pathLinks[i]);
        }
    }
    
    public (PathLink, int) GetPathIndexByDistance(Vector2 position)
    {
        PathLink pathLink = new PathLink();
        int pathIndex = 0;

        float minDistance = 20f;

        for(int i = 0; i < pathLinks.Length; i++)
        {
            for(int x = 0; x < pathLinks[i].path.Length; x++)
            {
                float _distance = Vector2.Distance(position, pathLinks[i].path[x].position);

                if(_distance < minDistance)
                {
                    pathLink = pathLinks[i];
                    pathIndex = x;

                    minDistance = _distance;
                }
            }  
        }

        return (pathLink, pathIndex);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for(int i = 0; i < pathLinks.Length; i++)
        {
            PathLink pathLink = pathLinks[i];

            if(!pathLink.enabled)
                continue;

            for(int x = 1; x < pathLink.path.Length; x++)
            {
                Gizmos.DrawLine(pathLink.path[x-1].position, pathLink.path[x].position);
                //Gizmos.DrawSphere(pathLink.path[x-1].position, 0.25f); 
                Gizmos.DrawSphere(pathLink.path[x].position, 0.25f);
            }
        }
    }
}
