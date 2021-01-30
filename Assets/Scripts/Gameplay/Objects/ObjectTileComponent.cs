using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    public class ObjectTileComponent : MonoBehaviour
    {
        public Transform objectContainer;

        private GameObject[] objectAnchors;

        public GameObject[] ObjectAnchors => objectAnchors;
        public int ObjectAnchorCount { get; private set; }

        private void Awake()
        {
            Transform parent = objectContainer;
            List<GameObject> anchorList = new List<GameObject>(); 

            for(int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if(child.tag == "ObjectAnchor")
                    anchorList.Add(child.gameObject);
            }

            objectAnchors = anchorList.ToArray();
            ObjectAnchorCount = objectAnchors.Length;
        }
    }
}