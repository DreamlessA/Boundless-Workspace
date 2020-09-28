// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Creator Agreement, located
// here: https://id.magicleap.com/creator-terms
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// This class handles setting the position and rotation of the
    /// transform to match the camera's based on input distance and height
    /// </summary>
    public class PlaceAtCamera : MonoBehaviour
    {
        #region Private Variables
        [SerializeField, Tooltip("The distance from the camera through its forward vector.")]
        private float _distance = 0.0f;

        [SerializeField, Tooltip("The distance on the Y axis from the camera.")]
        private float _height = 0.0f;

        [SerializeField, Tooltip("The approximate time it will take to reach the current position.")]
        private float _positionSmoothTime = 5f;
        private Vector3 _positionVelocity = Vector3.zero;

        [SerializeField, Tooltip("The approximate time it will take to reach the current rotation.")]
        private float _rotationSmoothTime = 5f;

        [SerializeField, Tooltip("The script need to behave different to work on simulator")]
        private bool _runOnSimulator = false;

        [SerializeField, Tooltip("Toggle to set position on awake.")]
        private bool _placeOnAwake = false;

        [SerializeField, Tooltip("Toggle to set position on update.")]
        private bool _placeOnUpdate = false;

        private bool _updateOnce = false;
        #endregion

        #region Unity Methods
        /// <summary>
        /// Set the transform from latest position if flag is checked.
        /// </summary>
        private void Awake()
        {
            if (_placeOnAwake)
            {
                UpdateTransform();
            }
        }

        private void Start()
        {
            if (_placeOnAwake)
            {
                UpdateTransform();
            }
        }

        private void Update()
        {
            //this is for simulator
            if (_runOnSimulator)
            {
                if (_placeOnAwake && !_updateOnce)
                {
                    _updateOnce = UpdateTransform();
                }
            }

            if (_placeOnUpdate && Camera.main.transform.hasChanged)
            {
                _updateOnce = UpdateTransform();
            }
        }

        private void OnValidate()
        {
            _positionSmoothTime = Mathf.Max(0.01f, _positionSmoothTime);
            _rotationSmoothTime = Mathf.Max(0.01f, _rotationSmoothTime);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Reset position and rotation to match current camera values.
        /// Return weather or not the transformation is finished
        /// </summary>
        public bool UpdateTransform()
        {
            Camera camera = Camera.main;

            // Move the object CanvasDistance units in front of the camera.
            Vector3 upVector = new Vector3(0.0f, _height, 0.0f);
            Vector3 targetPosition = camera.transform.position + upVector + (camera.transform.forward * _distance);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _positionVelocity, _positionSmoothTime);

            // Rotate the object to face the camera.
            RotateTowardCamera();

            return targetPosition == transform.position;
        }

        public void RotateTowardCamera()
        {
            Camera camera = Camera.main;

            Quaternion targetRotation = Quaternion.LookRotation(transform.position - camera.transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / _rotationSmoothTime);
        }
        #endregion
    }
}
