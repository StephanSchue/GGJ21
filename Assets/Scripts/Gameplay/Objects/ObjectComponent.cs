using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    [System.Serializable]
    public struct ObjectInfo
    {
        public string lang_de;
        public string lang_en;

        public string GetLocalizedString(string language)
        {
            switch(language)
            {
                case "de":
                    return lang_de;
                case "en":
                    return lang_en;
            }

            return "";
        }
    }

    public class ObjectComponent : MonoBehaviour
    {
        public ObjectInfo[] objectInfoArray;

        public ObjectInfo GetOneObjectInfo()
        {
            return objectInfoArray[Random.Range(0, objectInfoArray.Length)];
        }
    }
}