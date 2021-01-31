using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

namespace GGJ21.Gameplay.Words
{
    public class UIWordList : MonoBehaviour
    {
        [Header("Settings")]
        public float fadeInDuration;
        public float fadeOutDuration;

        [Header("Textfields")]
        public TextMeshProUGUI[] textfields = new TextMeshProUGUI[3];

        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(WordPuzzleCollection wordPuzzleCollection)
        {
            for(int i = 0; i < textfields.Length; i++)
                textfields[i].text = $"- {wordPuzzleCollection.wordPuzzles[i].word}";

            canvasGroup.DOFade(1f, fadeInDuration);
        }

        public void Clear()
        {
            for(int i = 0; i < textfields.Length; i++)
                textfields[i].text = "";
        }

        public void Hide()
        {
            canvasGroup.DOFade(0f, fadeInDuration);
        }
    }
}