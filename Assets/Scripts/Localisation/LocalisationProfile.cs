using UnityEngine;

namespace GGJ21.Localisation
{
    [CreateAssetMenu(fileName = "LocalisationProfile_", menuName = "Configs/LocalisationProfile", order = 1)]
    public class LocalisationProfile : ScriptableObject
    {
        [System.Serializable]
        public struct LocalisationProfileElement
        {
            public string variable;
            [TextArea(1,10)]public string value;
        }

        public SystemLanguage language;
        public LocalisationProfileElement[] variables;
    }
}