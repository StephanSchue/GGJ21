using System;
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
        public SpriteRenderer[] spriteRenderers;

        public ObjectInfo GetOneObjectInfo()
        {
            if(objectInfoArray == null || objectInfoArray.Length == 0)
                throw new NotImplementedException($"ObjectInfo: {name} not implemented");
            else
                return objectInfoArray[UnityEngine.Random.Range(0, objectInfoArray.Length)];
        }

        public void IncreaseSpriteOrder(int add)
        {
            if(spriteRenderers != null && spriteRenderers.Length > 0)
            {
                for(int i = 0; i < spriteRenderers.Length; i++)
                {
                    if(spriteRenderers[i] != null)
                        spriteRenderers[i].sortingOrder += add;
                }
            }
        }
    }
}