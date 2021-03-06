﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Words
{
    [System.Serializable]
    public struct WordPuzzleCollection
    {
        public WordPuzzle[] wordPuzzles;
        public Vector2Int coordinate;

        public WordPuzzleCollection(Vector2Int coordinate, string[] words)
        {
            this.coordinate = coordinate;
            wordPuzzles = new WordPuzzle[words.Length];

            for(int i = 0; i < words.Length; i++)
                wordPuzzles[i] = new WordPuzzle(words[i]);
        }
    }

    [System.Serializable]
    public struct WordPuzzle
    {
        public string word;
        public string validationWord;
        public string[] fragments;

        public WordPuzzle(string word)
        {
            this.word = word;
            validationWord = this.word.Replace(" ", "");
            int fragmetModulo = validationWord.Length % 2;

            fragments = new string[(int)(validationWord.Length/2f)];

            for(int i = 0; i < fragments.Length; i++)
            {
                bool isLast = i == fragments.Length - 1;
                fragments[i] = validationWord.Substring(i * 2, isLast ? 2 + fragmetModulo : 2);
            }
        }

        public bool Validate(string word)
        {
            return word == validationWord;
        }
    }
}