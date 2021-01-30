using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ21.Gameplay.Objects
{
    public class ObjectTileComponent : MonoBehaviour
    {
        public Transform objectContainer;

        #if UNITY_EDITOR
        public SpriteRenderer debugRenderer;
        #endif
        
        private GameObject[] objectAnchors;
        private ObjectComponent[] objects;

        public GameObject[] ObjectAnchors => objectAnchors;
        public int ObjectAnchorCount { get; private set; }

        private void Awake()
        {
            // --- Create Anchors ---
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

            objects = new ObjectComponent[ObjectAnchorCount];
        }

        public void Initialize()
        {
            #if UNITY_EDITOR
            MarkGoalObject(false);
            #endif
        }

        public void AddObjectToAnchor(int index, ObjectComponent objectComponent)
        {
            objects[index] = objectComponent;
        }

        #if UNITY_EDITOR

        public void MarkGoalObject(bool mark)
        {
            debugRenderer.enabled = mark;
        }

        #endif
    }
}