using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Localisation
{
    public class LocalisationManager : MonoBehaviour
    {
        #region Settings/Variables

        public LocalisationProfile[] profiles;

        // --- Variables ---
        private SystemLanguage language = SystemLanguage.English;
        private int profileIndex = 0;

        private string[] variables = new string[0];
        private string[] values = new string[0];
        private List<LocalizeStringComponent>[] components = new List<LocalizeStringComponent>[0];

        // --- Properties ---
        public SystemLanguage Language => language;

        #endregion

        private void Awake()
        {
            SelectLanguageBySystemLanguage(Application.systemLanguage);
        }

        #region Setup

        private void SelectLanguageBySystemLanguage(SystemLanguage systemLanguage)
        {
            if(!FindLocalisationProfile(systemLanguage, out profileIndex))
                profileIndex = 0;
            
            // --- Initialize List ---
            if(variables.Length == 0)
                SetupVariables(profiles[profileIndex]);
            else
                RefreshData(profiles[profileIndex]);
        }

        private void SelectLanguageByProfile(int profileIndex)
        {
            if(profileIndex < 0 && profileIndex >= profiles.Length)
                return;

            this.profileIndex = profileIndex;

            // --- Initialize List ---
            if(variables.Length == 0)
                SetupVariables(profiles[profileIndex]);
            else
                RefreshData(profiles[profileIndex]);
        }

        private void SetupVariables(LocalisationProfile profile)
        {
            language = profile.language;

            int length = profile.variables.Length;
            variables = new string[length];
            values = new string[length];
            components = new List<LocalizeStringComponent>[length];

            for(int i = 0; i < length; i++)
            {
                variables[i] = profile.variables[i].variable;
                values[i] = profile.variables[i].value;
                components[i] = new List<LocalizeStringComponent>();
            }
        }

        private void RefreshData(LocalisationProfile profile)
        {
            language = profile.language;

            for(int i = 0; i < variables.Length; i++)
                values[i] = profile.variables[i].value;
        }

        #endregion

        #region Refresh

        public void ChangeLanguage(int profileIndex)
        {
            SelectLanguageByProfile(profileIndex);
            Refresh();
        }

        public void ChangeLanguage(SystemLanguage systemLanguage)
        {
            SelectLanguageBySystemLanguage(systemLanguage);
            Refresh();
        }

        private void Refresh()
        {
            for(int i = 0; i < variables.Length; i++)
            {
                string value = values[i];

                for(int x = 0; x < components[i].Count; x++)
                {
                    if(components[i][x].OnRefresh != null)
                        components[i][x].OnRefresh.Invoke(value);
                }
            }
        }

        #endregion
        
        #region Profile

        private bool FindLocalisationProfile(SystemLanguage systemLanguage, out int profileIndex)
        {
            for(int i = 0; i < profiles.Length; i++)
            {
                if(systemLanguage == profiles[i].language)
                {
                    profileIndex = i;
                    return true;
                }
            }

            profileIndex = -1;
            return false;
        }

        #endregion

        #region LocalizeStringComponent

        public void RegisterLocalizeStringComponent(LocalizeStringComponent component)
        {
            if(FindVariableIndexByString(component.variable, out int index))
            {
                components[index].Add(component);

                if(components[index][components[index].Count - 1].OnRefresh != null)
                    components[index][components[index].Count-1].OnRefresh.Invoke(values[index]);
            }
        }

        public void UnRegisterLocalizeStringComponent(LocalizeStringComponent component)
        {
            if(FindVariableIndexByString(component.variable, out int index))
                components[index].Remove(component);
        }

        #endregion

        #region Utils

        private bool FindVariableIndexByString(string search, out int index)
        {
            for(int i = 0; i < variables.Length; i++)
            {
                if(search == variables[i])
                {
                    index = i;
                    return true;
                }
            }
            
            index = -1;
            return false;
        }

        #endregion
    }
}