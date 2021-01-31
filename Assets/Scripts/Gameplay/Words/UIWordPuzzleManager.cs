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

        private int[] puzzleSolvedIndexes;

        private UIWorldField currentWordField => uIWorldFields[wordFieldIndex];
        private int wordFieldIndex = 0;
        private int puzzlePieceIndex = 0;

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

                Vector2Int coordinates = new Vector2Int();
                wordPieces[i].SetPositionInParent(coordinates);

                int index = i;
                wordPieces[i].OnClick.AddListener(() => TileClicked(index));
            }
        }

        private void Start()
        {
            for(int i = 0; i < uIWorldFields.Length; i++)
                uIWorldFields[i].OnReoder.AddListener(ValidateInput);
        }

        public void InitializePuzzle(WordPuzzleCollection wordPuzzleCollection)
        {
            this.puzzlePieceIndex = 0;
            this.wordPuzzleCollection = wordPuzzleCollection;
            this.puzzleSolvedIndexes = new int[this.wordPuzzleCollection.wordPuzzles.Length];

            // --- Setup word Puzzles ---
            for(int i = 0; i < wordPuzzleCollection.wordPuzzles.Length; i++)
            {
                WordPuzzle wordPuzzle = wordPuzzleCollection.wordPuzzles[i];
                //Debug.Log($"SetWord: {wordPuzzle.word}");

                for(int x = 0; x < wordPuzzle.fragments.Length; x++)
                {
                    wordPieces[puzzlePieceIndex].Initialize(wordPuzzle.fragments[x]);
                    ++puzzlePieceIndex;
                }
            }

            for(int i = puzzlePieceIndex; i < wordPieceCount; i++)
            {
                wordPieces[i].Initialize("X");
            }
        }

        public void ValidateInput()
        {
            // --- Collect Word ---
            string validateWord = "";

            for(int i = 0; i < currentWordField.linkedPieces.Count; i++)
            {
                UIWordPiece worldPiece = currentWordField.linkedPieces[i];
                validateWord += worldPiece.content;
            }

            // --- Validate if Word is Working ---
            bool found = false;

            for(int i = 0; i < wordPuzzleCollection.wordPuzzles.Length; i++)
            {
                WordPuzzle wordPuzzle = wordPuzzleCollection.wordPuzzles[i];

                if(validateWord == wordPuzzle.word)
                {
                    if(wordFieldIndex < uIWorldFields.Length)
                    {
                        // Still things to solve
                        found = true;
                        puzzleSolvedIndexes[wordFieldIndex] = i;
                        currentWordField.SetCompleted();
                        ++wordFieldIndex;
                    }
                    
                    if(wordFieldIndex == uIWorldFields.Length)
                    {
                        // Finished
                        FinishedPuzzle();
                    }
                    
                    break;
                }
            }
        }

        private void FinishedPuzzle()
        {
            Debug.Log("FinishedPuzzle");

            if(OnPuzzleSolved != null)
                OnPuzzleSolved.Invoke();
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