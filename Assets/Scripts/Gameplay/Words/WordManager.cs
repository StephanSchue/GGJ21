using GGJ21.Gameplay.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Words
{
    public class WordManager : MonoBehaviour
    {
        public WordPuzzleCollection[] wordPuzzleCollections;
        private int currentWordPuzzleIndex = -1;

        public int CurrentWordPuzzleIndex => currentWordPuzzleIndex;
        public WordPuzzleCollection CurrentWordPuzzle => wordPuzzleCollections[currentWordPuzzleIndex];

        public void CreateWordPuzzles(ObjectTileComponent[] tiles, int wordCount, SystemLanguage language)
        {
            currentWordPuzzleIndex = 0;

            int count = tiles.Length;
            wordPuzzleCollections = new WordPuzzleCollection[count];

            for(int i = 0; i < tiles.Length; i++)
            {
                // --- Build WordPuzzleCollection ---
                ObjectInfo[] objectInfoArray = tiles[i].GetObjectWords(wordCount);
                string[] localizedWords = new string[wordCount];

                for(int x = 0; x < 3; x++)
                    localizedWords[x] = objectInfoArray[x].GetLocalizedString(language);

                wordPuzzleCollections[i] = new WordPuzzleCollection(tiles[i].Coordinate, localizedWords);
            }
        }

        public bool NextWordPuzzle()
        {
            if(currentWordPuzzleIndex + 1 < wordPuzzleCollections.Length)
            {
                ++currentWordPuzzleIndex;
                return true;
            }

            return false;
        }
    }
}