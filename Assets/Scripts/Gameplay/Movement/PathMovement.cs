using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathMovement : MonoBehaviour
{
    public Vector3 characterLookOrign = Vector2.right;
    public float mps = 1f;

    private PathManager pathManager;

    private void Start()
    {
        pathManager = GameObject.FindGameObjectWithTag("PathManager")?.GetComponent<PathManager>();
    }

    public void MoveTo(Vector3 destination)
    {
        if(pathManager.FindPathToPosition(transform.position, destination, out Vector3[] path, out float length))
        {
            transform.DOPath(path, mps, PathType.Linear).SetEase(Ease.Linear).SetSpeedBased(); // .SetLookAt(1f, characterLookOrign, Vector3.up)
        }
    }
}
