using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathComponent : MonoBehaviour
{
    [System.Serializable]
    public struct PathLink
    {
        public string label;
        public bool enabled;
        public Transform[] path;
    }

    public PathLink[] pathLinks;

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
