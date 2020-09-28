using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

[DisallowMultipleComponent]
public class UIComponent : MonoBehaviour
{
    #region Public Events
    public System.Action<Vector3> OnRaycastEnter;
    public System.Action<Vector3> OnRaycastContinue;
    public System.Action<Vector3> OnRaycastExit;
    public System.Action<MLInputController, MLInputControllerButton, Vector3> OnControllerButtonDown;
    public System.Action<MLInputController, MLInputControllerButton, Vector3> OnControllerButtonUp;
    public System.Action<MLInputController, float, Vector3> OnControllerTriggerDown;
    public System.Action<MLInputController, float, Vector3> OnControllerTriggerUp;
    public System.Action<MLInputController, bool, Vector3> OnControllerDrag;
    public System.Action<Collider> OnIntersectionEnter;
    public System.Action<Collider> OnIntersectionExit;
    #endregion

    #region Public Variables
    public Color EnabledColor;
    public Color DisabledColor;

    public Renderer[] Renderers;
    #endregion

    #region Public Properties
    public Material Material
    {
        get
        {
            return GetComponent<Renderer>().material;
        }
        set
        {
            Renderer r = GetComponent<Renderer>();
            if (r != null)
            {
                r.material = value;
                //if (enabled)
                //{
                //    r.material.color = EnabledColor;
                //}
                //else
                //{
                //    r.material.color = DisabledColor;
                //}
            }
        }
    }
    #endregion

    #region Private Methods
    protected void SetColor(Color color)
    {
        // Debug.Log("Setting the color!");
        int i = 0;
        foreach (Renderer renderer in Renderers)
        {
            i += 1;
            if (renderer.material != null)
            {
                renderer.material.SetColor("_Color", color);
            }         
        }
        //Debug.Log(string.Format("Set {0} materials!", i));
    }
    #endregion Private Methods

    #region Unity Methods
    protected virtual void OnDisable()
    {
        Collider buttonCollider = GetComponent<Collider>();
        if (buttonCollider != null)
        {
            buttonCollider.enabled = false;
        }

        SetColor(DisabledColor);
    }

    protected virtual void OnEnable()
    {
        Collider buttonCollider = GetComponent<Collider>();
        if (buttonCollider != null)
        {
            buttonCollider.enabled = true;
        }

        SetColor(EnabledColor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (OnIntersectionEnter != null)
        {
            OnIntersectionEnter(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnIntersectionEnter != null)
        {
            OnIntersectionEnter(other);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (OnIntersectionExit != null)
        {
            OnIntersectionExit(other);
        }
    }
    #endregion
}