using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PathLink
{
    public string label;
    public bool enabled;
    public Transform[] path;

    private float length;

    public float Length => length;

    public void CalculateLength()
    {
        length = 0f;

        for(int i = 1; i < path.Length; i++)
            length += Vector3.Distance(path[i - 1].position, path[i].position);
    }
}

public class PathComponent : MonoBehaviour
{
    public Transform center;
    public PathLink[] pathLinks;

    private void Awake()
    {
        for(int i = 0; i < pathLinks.Length; i++)
            pathLinks[i].CalculateLength();
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
                Gizmos.DrawSphere(pathLink.path[x-1].position, 0.25f); 
                Gizmos.DrawSphere(pathLink.path[x].position, 0.25f);
            }
        }
    }
}
