using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GGJ21.Gameplay.Objects
{
    public class InteractObjectComponent : MonoBehaviour
    {
        public bool debug = false;

        private Animator animator;
        private UnityAction interactionCompleteCallback;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        public void Interact(ObjectComponent objectComponent, UnityAction callback)
        {
            Debug.Log($"Interact with: {objectComponent.name}");

            interactionCompleteCallback = callback;
            animator.SetTrigger("Interact");

            if(debug)
                OnInteractComplete();
        }

        public void OnInteractComplete()
        {
            if(interactionCompleteCallback != null)
                interactionCompleteCallback.Invoke();
        }
    }
}