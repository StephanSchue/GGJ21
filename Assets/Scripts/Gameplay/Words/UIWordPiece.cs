using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using DG.Tweening;

namespace GGJ21.Gameplay.Words
{
    [System.Serializable]
    public enum WordPieceStatus
    {
        Free,
        Sorted,
        Finished
    }

    public class UIWordPiece : MonoBehaviour
    {
        [Header("Visuals")]
        public Image background;
        public Sprite[] variations;
        public float mps = 500f;
        public float scaleDownValue = 0.75f;
        public float scaleDownDuration = 0.25f;

        [Header("References")]
        public Button button;
        public TextMeshProUGUI textField;

        public UnityEvent OnClick => button.onClick;

        private RectTransform parentRectTransform;
        private RectTransform rectTransform;

        private Vector3 basePosition;
        private Vector2 dimesions = new Vector2(100f, 100f);

        private UnityAction moveToCallback;

        public string content { get; private set; }
        public WordPieceStatus status;

        private void Awake()
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();

            dimesions = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
            ChangeVisual();
        }

        public void Initialize(string text)
        {
            content = text;
            textField.text = text;
            ChangeVisual();
        }

        public void SetPositionInParent()
        {
            Rect area = parentRectTransform.rect;

            float xSize = Mathf.Abs(area.width * 0.5f) - dimesions.x;
            float ySize = Mathf.Abs(area.height * 0.5f) - dimesions.y;

            float positionX = Random.Range(-xSize, xSize);
            float positionY = Random.Range(-ySize, ySize);

            rectTransform.anchoredPosition = new Vector2(positionX, positionY);

            basePosition = transform.position;
        }
    
        public void MoveToWordField(Vector3 nextPosition, UnityAction callback)
        {
            moveToCallback = callback;

            transform.DOMove(nextPosition, mps).SetSpeedBased().OnComplete(MoveComplete);
            transform.DOScale(new Vector3(scaleDownValue, scaleDownValue, scaleDownValue), scaleDownDuration);
            SetStatus(WordPieceStatus.Sorted);
        }

        public void MoveBackToField(UnityAction callback)
        {
            moveToCallback = callback;

            transform.DOMove(basePosition, mps).SetSpeedBased().OnComplete(MoveComplete);
            transform.DOScale(new Vector3(1f, 1f, 1f), scaleDownDuration);
            SetStatus(WordPieceStatus.Free); 
            transform.parent = parentRectTransform;
        }

        private void MoveComplete()
        {
            if(moveToCallback != null)
                moveToCallback.Invoke();
        }
    
        private void ChangeVisual()
        {
            background.sprite = variations[Random.Range(0, variations.Length)];
        }
    
        public void SetStatus(WordPieceStatus newStatus)
        {
            switch(newStatus)
            {
                case WordPieceStatus.Free:
                    button.interactable = true;
                    break;
                case WordPieceStatus.Sorted:
                    button.interactable = true;
                    break;
                case WordPieceStatus.Finished:
                    button.interactable = false;
                    break;
            }

            status = newStatus;
        }

        public void SetLocked()
        {
            SetStatus(WordPieceStatus.Finished);
        }
    }
}