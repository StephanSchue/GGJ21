﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Game
{
    [System.Serializable]
    public enum WinCondition
    {
        TreasureHunt,
        Endless
    }

    [System.Serializable]
    public struct MatchWinCondition
    {
        public WinCondition condition;
        public int puzzleCount;
        public int wordCount;

        public MatchWinCondition(MatchWinCondition otherCondition)
        {
            this.condition = otherCondition.condition;
            this.puzzleCount = otherCondition.puzzleCount;
            this.wordCount = otherCondition.wordCount;
        }
    }

    [CreateAssetMenu(fileName = "MatchConditionsProfile_", menuName = "Configs/MatchConditionsProfile", order = 1)]
    public class MatchConditionsProfile : ScriptableObject
    {
        public float helpDuration = 5f;
        public MatchWinCondition winCondtion;

        public bool ValidateWinCondition(int score, int remainingMoves)
        {
            switch(winCondtion.condition)
            {
                case WinCondition.TreasureHunt:
                    return score == winCondtion.puzzleCount;
            }

            return false;
        }
        
        public bool ValidateLooseCondition(int remainingMoves)
        {
            switch(winCondtion.condition)
            {
                case WinCondition.TreasureHunt:
                    return (remainingMoves == 0);
            }

            return false;
        }
    }
}
