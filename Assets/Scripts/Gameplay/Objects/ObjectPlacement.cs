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

    public bool IsLabelActive(string label)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            if(label == objects[i].label)
                return objects[i].status;
        }

        return true;
    }

    public int GetRandomFreeIndex()
    {
        List<int> availableIndex = new List<int>();

        for(int i = 0; i < objects.Length; i++)
        {
            if(objects[i].status)
                availableIndex.Add(i);
        }

        return availableIndex[Random.Range(0, availableIndex.Count)];
    }
}
