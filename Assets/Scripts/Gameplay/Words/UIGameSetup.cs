using DG.Tweening;
using GGJ21.Game.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIGameSetup : MonoBehaviour
{
    [Header("Components")]
    public TMP_InputField inputField;

    // --- Variables ---
    private GameManager gameManager;
    private bool foundGameManager;

    private bool show = false;

    #region Editor

    private void Reset()
    {
        inputField = GetComponent<TMP_InputField>();
    }

    #endregion

    #region Initialisation

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<GameManager>();
        foundGameManager = gameManager != null;

        inputField.onValueChanged.AddListener(OnChangeValue);
    }

    private void Start()
    {
        if(foundGameManager)
            gameManager.OnGameSetup.AddListener(GetBaseValue);
    }

    #endregion

    #region Change Value

    private void GetBaseValue()
    {
        inputField.text = gameManager.GetMatchSetupValue().ToString();
    }

    private void OnChangeValue(string value)
    {
        int intValue = System.Convert.ToInt32(value);

        if(foundGameManager)
        {
            if(intValue > 0)
                gameManager.UpdateMatchSetupValue(intValue);
            else
                GetBaseValue();
        }
    }

    #endregion
}
