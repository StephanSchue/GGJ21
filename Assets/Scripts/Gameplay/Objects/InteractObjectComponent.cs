﻿using System.Collections;
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
        private UnityAction finishCompleteCallback;


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

        private void OnInteractComplete()
        {
            if(interactionCompleteCallback != null)
                interactionCompleteCallback.Invoke();
        }

        public void PlayFinish(UnityAction callback)
        {
            Debug.Log("Finish");
            finishCompleteCallback = callback;

            animator.SetTrigger("Finish");

            if(debug)
                OnFinishComplete();
        }

        private void OnFinishComplete()
        {
            if(finishCompleteCallback != null)
                finishCompleteCallback.Invoke();
        }
    }
}