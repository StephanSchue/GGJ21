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
}
