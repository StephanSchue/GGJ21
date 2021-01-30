using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    [System.Serializable]
    public struct ObjectInfo
    {
        public string lang_de;
    }

    public class ObjectComponent : MonoBehaviour
    {
        public ObjectInfo[] objectInfoArray;
    }
}