using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap;



/// <summary>
/// This class represents the controller and its interactions.
/// This works hand-in-hand with WorldButton.
/// </summary>
public class ARPointer : MonoBehaviour
{
    #region Private Variables
    [SerializeField, Tooltip("ControllerConnectionHandler reference.")]
    private ControllerConnectionHandler _controllerConnectionHandler;

    [SerializeField, Space, Tooltip("Pointer Ray")]
    public Transform _pointerRay;

    [SerializeField, Tooltip("Pointer Light")]
    private Light _pointerLight;

    [SerializeField, Tooltip("Color of the pointer light when no hit is detected")]
    private Color _pointerLightColorNoHit = Color.black;
    [SerializeField, Tooltip("Color of the pointer light when a hit is detected")]
    private Color _pointerLightColorHit = Color.yellow;
    [SerializeField, Tooltip("Color of the pointer light when a button is pressed while a hit is detected")]
    private Color _pointerLightColorHitPress = Color.green;

    private UIComponent _lastItemHit;
    private RaycastHit _lastHit; 
    private bool _isGrabbing = false;
    #endregion // Private Properties

    #region Unity Methods
    private void Awake()
    {
        if (_controllerConnectionHandler == null)
        {
            Debug.LogError("Error: VirtualPointer._controllerConnectionHandler is not set, disabling script.");
            enabled = false;
            return;
        }

        MLInput.OnControllerButtonDown += HandleControllerButtonDown;
        MLInput.OnControllerButtonUp += HandleControllerButtonUp;
        MLInput.OnTriggerDown += HandleTriggerDown;
        MLInput.OnTriggerUp += HandleTriggerUp;
    }

   
    private void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= HandleControllerButtonDown;
        MLInput.OnControllerButtonUp -= HandleControllerButtonUp;
        MLInput.OnTriggerDown -= HandleTriggerDown;
        MLInput.OnTriggerUp -= HandleTriggerUp;
    }

    private void Update()
    {
        if (!_isGrabbing)
        {
            RaycastHit hit;
            if (Physics.Raycast(_pointerRay.position, _pointerRay.forward, out hit))
            {
                UIComponent wb = hit.transform.GetComponent<UIComponent>();

                if (wb != null)
                {
                    _lastHit = hit;
                    if (_lastItemHit == null)
                    {                           
                        if (wb.OnRaycastEnter != null)
                        {
                            wb.OnRaycastEnter(hit.point);
                        }
                        _lastItemHit = wb;
                        _pointerLight.color = _pointerLightColorHit;
                    }
                    else if (_lastItemHit == wb)
                    {
                        if (_lastItemHit.OnRaycastContinue != null)
                        {
                            _lastItemHit.OnRaycastContinue(hit.point);
                        }
                    }
                    else
                    {
                        if (_lastItemHit.OnRaycastExit != null)
                        {
                            _lastItemHit.OnRaycastExit(hit.point);
                        }
                        _lastItemHit = null;
                    }
                }
                else
                {
                    if (_lastItemHit != null)
                    {
                        if (_lastItemHit.OnRaycastExit != null)
                        {
                            _lastItemHit.OnRaycastExit(hit.point);
                        }
                        _lastItemHit = null;
                    }
                    _pointerLight.color = _pointerLightColorNoHit;
                }
                UpdatePointer(hit.point);
            }
            else
            {
                _lastItemHit = null;
                ClearPointer();
            }
        }
        else if (_isGrabbing)
        {
            // _isGrabbing already guarantees that _lastItemHit is not null
            // but just in case the actual button gets destroyed in
            // the middle of the grab, let's still check

            Vector3 dragPosition = new Vector3();
            RaycastHit hit;
            bool onDragged = false;
            if (Physics.Raycast(_pointerRay.position, _pointerRay.forward, out hit))
            {
                UIComponent wb = hit.transform.GetComponent<UIComponent>();
                if (wb == _lastItemHit)
                {
                    dragPosition = hit.point;
                    onDragged = true;
                }
                UpdatePointer(hit.point);
            }

            if (_lastItemHit != null && _lastItemHit.OnControllerDrag != null)
            {
                _lastItemHit.OnControllerDrag(_controllerConnectionHandler.ConnectedController, onDragged, dragPosition);
            }
        }
    }
    #endregion // Unity Methods

    #region Private Methods
    private void UpdatePointer(Vector3 hitPosition)
    {
        Vector3 pointerScale = _pointerRay.localScale;
        pointerScale.z = Vector3.Distance(_pointerRay.position, hitPosition);
        _pointerRay.localScale = pointerScale;

        _pointerLight.transform.position = hitPosition;
    }

    private void ClearPointer()
    {
        Vector3 pointerScale = _pointerRay.localScale;
        pointerScale.z = 1.0f;
        _pointerRay.localScale = pointerScale;

        _pointerLight.transform.position = transform.position;
        _pointerLight.color = _pointerLightColorNoHit;
    }
    #endregion // Private Methods

    #region Event Handlers
    private void HandleControllerButtonDown(byte controllerId, MLInputControllerButton button)
    {
        if (_controllerConnectionHandler.IsControllerValid(controllerId) && _lastItemHit != null && !_isGrabbing)
        {
            if (_lastItemHit.OnControllerButtonDown != null)
            {
                _lastItemHit.OnControllerButtonDown(_controllerConnectionHandler.ConnectedController, button, _lastHit.point);
            }
            _pointerLight.color = _pointerLightColorHitPress;
            _isGrabbing = true;
            Debug.Log(">>> HELLO! The button is " + button);
            Debug.Log(button);
        }
    }

    private void HandleControllerButtonUp(byte controllerId, MLInputControllerButton button)
    {
        if (_controllerConnectionHandler.IsControllerValid(controllerId))
        {
            if (_lastItemHit != null)
            {
                if (_lastItemHit.OnControllerButtonUp != null)
                {
                    _lastItemHit.OnControllerButtonUp(_controllerConnectionHandler.ConnectedController, button, _lastHit.point);
                }
                _pointerLight.color = _pointerLightColorHit;
                _isGrabbing = false;
            }
            else
            {
                _pointerLight.color = _pointerLightColorNoHit;
            }
        }
    }

    private void HandleTriggerDown(byte controllerId, float triggerValue)
    {
        if (_controllerConnectionHandler.IsControllerValid(controllerId) && _lastItemHit != null && !_isGrabbing)
        {
            if (_lastItemHit.OnControllerTriggerDown != null)
            {
                _lastItemHit.OnControllerTriggerDown(_controllerConnectionHandler.ConnectedController, triggerValue, _lastHit.point);
            }
            _pointerLight.color = _pointerLightColorHitPress;
            _isGrabbing = true;
        }
    }

    private void HandleTriggerUp(byte controllerId, float triggerValue)
    {
        if (_controllerConnectionHandler.IsControllerValid(controllerId))
        {
            if (_lastItemHit != null)
            {
                if (_lastItemHit.OnControllerTriggerUp != null)
                {
                    _lastItemHit.OnControllerTriggerUp(_controllerConnectionHandler.ConnectedController, triggerValue, _lastHit.point);
                }
                _pointerLight.color = _pointerLightColorHit;
                _isGrabbing = false;
            }
            else
            {
                _pointerLight.color = _pointerLightColorNoHit;
            }
        }
    }
    #endregion // Event Handlers
}