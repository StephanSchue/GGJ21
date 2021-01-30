﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PathMovement : MonoBehaviour
{
    public Vector3 characterLookOrign = Vector2.right;
    public float mps = 1f;
    public GameObject walkCursorPrefab;
    
    private GameObject walkCursor;
    private SpriteRenderer walkCursorRenderer;

    private Animator animator;
    private PathManager pathManager;
    private bool walkCursorUsed = false;

    private readonly static string A_Velocity = "Velocity";

    private void Awake()
    {
        if(walkCursorPrefab != null)
        {
            walkCursor = Instantiate(walkCursorPrefab);
            walkCursorRenderer = walkCursor.GetComponent<SpriteRenderer>();

            walkCursorRenderer.enabled = false;
            walkCursor.transform.position = transform.position;
            walkCursorUsed = true;
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        pathManager = GameObject.FindGameObjectWithTag("PathManager")?.GetComponent<PathManager>();
    }

    public void SetPosition(Vector3 position)
    {
        DOTween.Kill(transform);
        transform.position = position;
    }

    public void MoveTo(Vector3 destination)
    {
        if(pathManager.FindPathToPosition(transform.position, destination, out Vector3[] path, out float length))
        {
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

            if(walkCursorUsed)
            {
                walkCursorRenderer.enabled = true;
                walkCursor.transform.DOPath(path, mps * 5f, PathType.Linear).SetEase(Ease.Linear).SetSpeedBased();
            }
        }
    }

    private void MoveComplete()
    {
        animator.SetFloat(A_Velocity, 0f);

        if(walkCursorUsed)
            walkCursorRenderer.enabled = false;
    }
}
