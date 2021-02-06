using DG.Tweening;
using GGJ21.Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICompass : MonoBehaviour
{
    [Header("Components")]
    public Canvas canvas;
    public CanvasGroup canvasGroup;
    public RectTransform needle;

    [Header("Settings")]
    public float minDistance = 20f;
    public float maxDistance = 100f;

    public float fadeInDuration = 0.25f;
    public float fadeOutDuration = 0.25f;

    // --- Variables ---
    private GameManager gameManager;
    private bool foundGameManager;

    private bool show = false;

    #region Editor

    private void Reset()
    {
        canvas = GetComponent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    #endregion

    #region Initialisation

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
        foundGameManager = gameManager != null;

        Hide();
    }

    #endregion

    #region Loop

    private void Update()
    {
        if(foundGameManager && gameManager.OnMap)
        {
            Vector3 heading = gameManager.TargetPosition - gameManager.PlayerPosition;
            float magnitude = heading.magnitude;

            if(magnitude > minDistance && magnitude < maxDistance)
            {
                float angle = Vector3.SignedAngle(Vector3.up, heading, Vector3.forward);
                needle.eulerAngles = new Vector3(0f, 0f, angle);

                if(!show)
                    Show(fadeInDuration);
            }
            else
            {
                if(show)
                    Hide(fadeOutDuration);
            }
        }
    }

    #endregion

    #region Show/Hide

    public void Show()
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        canvas.enabled = true;
        show = true;
    }

    public void Show(float duration, float delay = 0f)
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = true;
        canvasGroup.DOFade(1f, duration).SetDelay(delay);
        canvas.enabled = true;
        show = true;
    }

    public void Hide()
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        canvas.enabled = false;
        show = false;
    }

    public void Hide(float duration, float delay = 0f)
    {
        canvasGroup.interactable = canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0f, duration).SetDelay(delay).OnComplete(HideComplete);
        show = false;
    }

    private void HideComplete()
    {
        canvas.enabled = false;
    } 

    #endregion
}
