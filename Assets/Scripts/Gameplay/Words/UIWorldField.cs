using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ21.Gameplay.Words
{
    public class UIWorldField : MonoBehaviour
    {
        public RectTransform rectTransform;
        public List<UIWordPiece> linkedPieces = new List<UIWordPiece>();
        public Vector3 positionOffset = Vector3.zero;
        public Vector3[] positions = new Vector3[5];

        public UnityEvent OnReoder { get; private set; }

        private void Awake()
        {
            OnReoder = new UnityEvent();
        }

        public bool HasFreePosition()
        {
            return linkedPieces.Count < positions.Length;
        }

        public void Register(UIWordPiece wordPiece, out Vector3 position)
        {
            wordPiece.SetStatus(WordPieceStatus.Sorted);
            linkedPieces.Add(wordPiece);

            Vector3 startPosition = transform.position + positionOffset;
            startPosition += new Vector3(rectTransform.rect.x, rectTransform.rect.y);
            position = startPosition + positions[linkedPieces.Count-1];
            wordPiece.transform.parent = transform;
        }

        public void UnRegister(UIWordPiece wordPiece)
        {
            wordPiece.SetStatus(WordPieceStatus.Free);
            int indexRemoved = linkedPieces.IndexOf(wordPiece);
            linkedPieces.Remove(wordPiece);
            Reorder(indexRemoved);
        }

        private void Reorder(int indexRemoved)
        {
            Vector3 startPosition = transform.position + positionOffset;
            startPosition += new Vector3(rectTransform.rect.x, rectTransform.rect.y);

            for(int i = 0; i < linkedPieces.Count; i++)
            {
                Vector3 position = startPosition + positions[i];

                if(i == linkedPieces.Count)
                    linkedPieces[i].MoveToWordField(position, ReorderComplete);
                else
                    linkedPieces[i].MoveToWordField(position, null);
            }  
        }

        private void ReorderComplete()
        {
            if(OnReoder != null)
                OnReoder.Invoke();
        }

        public void SetCompleted()
        {
            for(int i = 0; i < linkedPieces.Count; i++)
                linkedPieces[i].SetLocked();
        }

        #region Gizmos

        private void OnDrawGizmos()
        {
            Vector3 startPosition = transform.position + positionOffset;
            startPosition += new Vector3(rectTransform.rect.x, rectTransform.rect.y);

            for(int i = 0; i < positions.Length; i++)
            {
                Vector3 position = startPosition + positions[i];
                Gizmos.DrawSphere(position, 100f);
            }
        }

        #endregion
    }
}