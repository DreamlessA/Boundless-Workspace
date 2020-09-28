using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class UIButton : UIComponent
{
    #region Public Variables
    public Color UsualColor = Color.black;
    public Color PressedColor = Color.gray;

    public Material[] MaterialSequence;
    #endregion

    #region Private Variables
    private int _state;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        OnControllerTriggerDown += OnPress;
        OnControllerTriggerUp += OnRelease;
        OnControllerTriggerUp += SetState;
    }

    private void OnDestroy()
    {
        OnControllerTriggerDown -= OnPress;
        OnControllerTriggerUp -= OnRelease;
        OnControllerTriggerUp -= SetState;
    }
    #endregion

    #region Private Methods
    private void OnPress(MLInputController controller, float triggerValue, Vector3 togglePosition)
    {
        this.Material.SetColor("_Color", PressedColor);
    }

    private void SetState(MLInputController controller, float triggerValue, Vector3 togglePosition)
    {
        _state = (_state + 1) % MaterialSequence.Length;
        this.Material = MaterialSequence[_state];
    }

    private void OnRelease(MLInputController controller, float triggerValue, Vector3 togglePosition)
    {
        this.Material.SetColor("_Color", UsualColor);
    }
    #endregion
}
