using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPlacement : MonoBehaviour
{
    [System.Serializable]
    public struct ObjectInfo
    {
        public string label;
        public bool status;
    }

    public ObjectInfo[] objects;
}
