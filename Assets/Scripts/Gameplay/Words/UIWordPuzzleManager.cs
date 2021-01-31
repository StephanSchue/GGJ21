using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ21.Gameplay.Words
{
    public class UIWordPuzzleManager : MonoBehaviour
    {
        public UIWordPiece wordPiecePrefab;
        public Transform wordPieceContainer;
        public int wordPieceCount = 25;

        public UIWorldField[] uIWorldFields = new UIWorldField[3];

        private WordPuzzleCollection wordPuzzleCollection;
        private UIWordPiece[] wordPieces;

        private UIWorldField currentWordField => uIWorldFields[wordFieldIndex];
        private int wordFieldIndex = 0;

        public UnityEvent OnMoveDone { get; private set; }
        public UnityEvent OnPuzzleSolved { get; private set; }

        private void Awake()
        {
            OnPuzzleSolved = new UnityEvent();

            // --- Word Pices ---
            wordPieces = new UIWordPiece[wordPieceCount];

            for(int i = 0; i < wordPieceCount; i++)
            {
                wordPieces[i] = Instantiate(wordPiecePrefab, wordPieceContainer);
                wordPieces[i].SetPositionInParent();

                int index = i;
                wordPieces[i].OnClick.AddListener(() => TileClicked(index));
            }
        }

        private void Start()
        {
            for(int i = 0; i < uIWorldFields.Length; i++)
                uIWorldFields[i].OnReoder.AddListener(ValidateInput);
        }

        public void ShowPuzzle(WordPuzzleCollection wordPuzzleCollection)
        {
            this.wordPuzzleCollection = wordPuzzleCollection;
        }

        public void ValidateInput()
        {
            Debug.Log("ValidateInput");
        }
    
        private void TileClicked(int index)
        {
            UIWordPiece uiWordPiece = wordPieces[index];

            //Debug.Log($"TileClicked: {index}");
            if(uiWordPiece.status == WordPieceStatus.Free)
            {
                if(currentWordField.HasFreePosition())
                {
                    // Sort to next free field
                    currentWordField.Register(uiWordPiece, out Vector3 position);
                    uiWordPiece.MoveToWordField(position, ValidateInput);
                }
            }
            else
            {
                // Sort Back
                currentWordField.UnRegister(uiWordPiece);
                uiWordPiece.MoveBackToField(ValidateInput);
            }
        }
    }
}