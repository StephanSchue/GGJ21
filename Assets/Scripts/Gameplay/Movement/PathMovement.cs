using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathMovement : MonoBehaviour
{
    public Vector3 characterLookOrign = Vector2.right;
    public float mps = 1f;

    private Animator animator;
    private PathManager pathManager;

    private readonly static string A_Velocity = "Velocity";

    private void Start()
    {
        animator = GetComponent<Animator>();
        pathManager = GameObject.FindGameObjectWithTag("PathManager")?.GetComponent<PathManager>();
    }

    public void MoveTo(Vector3 destination)
    {
        if(pathManager.FindPathToPosition(transform.position, destination, out Vector3[] path, out float length))
        {
            Debug.Log("MoveTo");

            // Path
            DOTween.Kill(transform);
            transform.DOPath(path, mps, PathType.Linear).SetEase(Ease.Linear).SetSpeedBased().OnComplete(MoveComplete); // .SetLookAt(1f, characterLookOrign, Vector3.up)
            animator.SetFloat(A_Velocity, 1f);

            // LookAt
            Vector3 heading = path[path.Length - 1] - path[0];

            if(heading.x > 0f)
                transform.eulerAngles = new Vector3(0f, 180f);
            else
                transform.eulerAngles = new Vector3(0f, 0f);
        }
    }

    private void MoveComplete()
    {
        Debug.Log("MoveComplete");
        animator.SetFloat(A_Velocity, 0f);
    }
}
