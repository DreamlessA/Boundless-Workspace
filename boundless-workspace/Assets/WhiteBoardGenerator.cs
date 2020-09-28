using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class WhiteBoardGenerator : MonoBehaviour {

    private MLInputController _controller;
    public Transform _pointerRay;

	// Use this for initialization
	void Start () {
        _controller = MLInput.GetController(MLInput.Hand.Left);
        MLInput.OnControllerButtonDown += OnControllerButtonDown;
    }

    private void OnDestroy()
    {
        MLInput.OnControllerButtonDown -= OnControllerButtonDown;
    }

    private void OnControllerButtonDown(byte id, MLInputControllerButton button)
    {
        if (button == MLInputControllerButton.Bumper)
        {
            float aspectRatio = 16f / 9f;
            float height = 0.5f;
            float width = height * aspectRatio;
            WindowController whiteBoard = WindowController.New2DWindow(width, height);
            whiteBoard.transform.position = _controller.Position + _pointerRay.forward.normalized;
            whiteBoard.transform.rotation = _controller.Orientation;
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}
