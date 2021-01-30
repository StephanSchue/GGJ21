﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace GGJ21.General
{
    public class CameraManager : MonoBehaviour
    {
        [Header("References")]
        public Transform origin;
        public Camera camera;

        [Header("Chinemachine Settings")]
        public CinemachineBrain cinemachineBrain;
        public CinemachineVirtualCamera cinemachineCharacterFollow;

        [Header("Aspect Settings")]
        public CameraSceneProfile cameraSceneProfile;

        private Vector3 originPosition;

        private void Reset()
        {
            origin = transform;
            camera = Camera.main;
        }

        // Start is called before the first frame update
        private void Awake()
        {
            if(origin == null)
                return;

            originPosition = origin.position;
            //AdjustToAspectRatio();
        }
        #region Initialize

        public void Initialize(Transform transform)
        {
            cinemachineCharacterFollow.Follow = transform;
        }

        public void Initialize(CameraSceneProfile cameraSceneProfile)
        {
            this.cameraSceneProfile = cameraSceneProfile;
            AdjustToAspectRatio();
        }

        #endregion

        #region Aspect Methods

        public void AdjustToAspectRatio()
        {

            if(origin == null || camera == null || cameraSceneProfile == null)
            {
                Debug.LogErrorFormat("'{0}': Is missing component reference. Function interrupted.", name);
                return;
            }
            bool isPortrait = Screen.width < Screen.height;
            float ratio = isPortrait ? Screen.height / (float)Screen.width : Screen.width / Screen.height;
            AspectRatioElement[] aspects = cameraSceneProfile.aspects;

            for(int i = 0; i < aspects.Length; i++)
            {
                if(ratio >= aspects[i].aspectRatio)
                {
                    UpdateTransform(aspects[i].offset);
                    UpdateOrthographicSize(aspects[i].orthographicSize);
                    break;
                }
            }
        }

        public void ResetCameraAspect()
        {
            camera.ResetAspect();
        }

        #endregion

        #region Cinemachine Methods

        public void SetCameraEnabled(bool enabled)
        {
            cinemachineCharacterFollow.enabled = enabled;
        }

        #endregion

        #region Property Methods

        private void UpdateTransform(Vector3 offset)
        {
            origin.transform.position = originPosition + offset;
        }

        private void UpdateOrthographicSize(float orthographicSize)
        {
            camera.orthographicSize = orthographicSize;
        }

        #endregion
    }

}