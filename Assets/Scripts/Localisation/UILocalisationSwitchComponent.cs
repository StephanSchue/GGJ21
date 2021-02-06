using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GGJ21.Localisation
{
    public class UILocalisationSwitchComponent : MonoBehaviour
    {
        private LocalisationManager localisationManager;
        private bool foundLocalisationManager;

        private void Awake()
        {
            localisationManager = GameObject.FindGameObjectWithTag("GameController")?.GetComponent<LocalisationManager>();
            foundLocalisationManager = localisationManager != null;
        }

        public void ChangeLanguage(int index)
        {
            if(foundLocalisationManager)
                localisationManager.ChangeLanguage(index);
        }
    }
}