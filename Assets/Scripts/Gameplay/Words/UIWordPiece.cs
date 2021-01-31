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

        public WordPieceStatus status;

        private void Awake()
        {
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
            rectTransform = GetComponent<RectTransform>();

            dimesions = new Vector2(rectTransform.rect.width, rectTransform.rect.height);
        }

        public void Initialize(string text)
        {
            textField.text = text;
            background.sprite = variations[Random.Range(0, variations.Length)];
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
    
        public void MoveToWordField(Vector3 nextPosition)
        {
            transform.DOMove(nextPosition, mps).SetSpeedBased();
            transform.DOScale(new Vector3(scaleDownValue, scaleDownValue, scaleDownValue), scaleDownDuration);
            status = WordPieceStatus.Sorted;
        }

        public void MoveBackToField()
        {
            transform.DOMove(basePosition, mps).SetSpeedBased();
            transform.DOScale(new Vector3(1f, 1f, 1f), scaleDownDuration);
            status = WordPieceStatus.Free; 
            transform.parent = parentRectTransform;
        }
    }
}