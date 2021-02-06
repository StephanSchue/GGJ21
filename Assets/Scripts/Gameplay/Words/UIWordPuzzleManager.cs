using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ21.Gameplay.Words
{
    public class UIWordPuzzleManager : MonoBehaviour
    {
        #region Settings/Variables

        [Header("Components")]
        public UIWordPiece wordPiecePrefab;
        public Transform wordPieceContainer;
        public int wordPieceCount = 25;

        [Header("UI Elements")]
        public UIWordList wordList;
        public UIWorldField[] uIWorldFields = new UIWorldField[3];

        // --- Variables ---
        private WordPuzzleCollection wordPuzzleCollection;
        private UIWordPiece[] wordPieces;

        private int[] puzzleSolvedIndexes;

        private UIWorldField currentWordField => uIWorldFields[wordFieldIndex];
        private int wordFieldIndex = 0;
        private int puzzlePieceIndex = 0;

        private string[] randomWordList = new string[] { "zz", "xy", "mp", "qw", "lz", "öä", "uq", "cx", "ya", "wq", "hx", "rq" };

        public UnityEvent OnMoveDone { get; private set; }
        public UnityEvent OnPuzzleSolved { get; private set; }

        #endregion

        private void Awake()
        {
            OnPuzzleSolved = new UnityEvent();

            // --- Word Pices ---
            wordPieces = new UIWordPiece[wordPieceCount];
            Vector2Int[] placementGrid = new Vector2Int[wordPieceCount];
            int cowCount = 2;
            int wordPieceHalfCount = wordPieceCount / cowCount;
            Vector2Int position = new Vector2Int(125, 125);
            int index = 0;

            for(int y = 0; y < cowCount; y++)
            {
                for(int x = 0; x < wordPieceHalfCount; x++)
                {
                    placementGrid[index] = new Vector2Int(position.x * x, position.y * y);
                    ++index;
                }
            }

            int[] placementIndexes = GetArrayOfUniqueNumbers(wordPieceCount);

            for(int i = 0; i < wordPieceCount; i++)
            {
                wordPieces[i] = Instantiate(wordPiecePrefab, wordPieceContainer);
                wordPieces[i].SetPositionInParent(placementGrid[placementIndexes[i]]);

                int eventIndex = i;
                wordPieces[i].OnClick.AddListener(() => TileClicked(eventIndex));
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
            this.wordFieldIndex = 0;
            this.wordPuzzleCollection = wordPuzzleCollection;
            this.puzzleSolvedIndexes = new int[this.wordPuzzleCollection.wordPuzzles.Length];

            for(int i = 0; i < uIWorldFields.Length; i++)
                uIWorldFields[i].Clear();

            // --- Setup word Puzzles ---
            for(int i = 0; i < wordPuzzleCollection.wordPuzzles.Length; i++)
            {
                WordPuzzle wordPuzzle = wordPuzzleCollection.wordPuzzles[i];
                Debug.Log($"SetWord: {wordPuzzle.word}");

                for(int x = 0; x < wordPuzzle.fragments.Length; x++)
                {
                    wordPieces[puzzlePieceIndex].Initialize(wordPuzzle.fragments[x]);
                    ++puzzlePieceIndex;
                }
            }

            int index = 0;
            int[] randomWordIndexes = GetArrayOfUniqueNumbers(randomWordList.Length);

            for(int i = puzzlePieceIndex; i < wordPieceCount; i++)
            {
                wordPieces[i].Initialize(randomWordList[randomWordIndexes[index]]);
                ++index;
            }

            this.wordList.Clear();
        }

        public void ClearWordList()
        {
            this.wordList.Clear();
        }
        
        public void HideWordList()
        {
            this.wordList.Hide();
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

                if(wordPuzzle.Validate(validateWord))
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
            wordList.Show(wordPuzzleCollection);

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

        private int[] GetArrayOfUniqueNumbers(int maxValue)
        {
            int[] nums = Enumerable.Range(0, maxValue).ToArray();
            //System.Random rnd = new System.Random();

            // Shuffle the array
            for(int i = 0; i < nums.Length; ++i)
            {
                int randomIndex = Random.Range(0, maxValue);
                int temp = nums[randomIndex];
                nums[randomIndex] = nums[i];
                nums[i] = temp;
            }

            return nums;
        }

    }
}