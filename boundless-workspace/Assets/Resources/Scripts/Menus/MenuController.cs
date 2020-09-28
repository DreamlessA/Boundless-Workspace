using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class MenuController : MonoBehaviour {
    public Menu menu;
    private MLInputController _controller;

    void Awake()
    {
        MLInput.OnControllerTouchpadGestureStart += HandleOnTouchpadGestureStart;
        //MLInput.OnControllerTouchpadGestureContinue += HandleOnTouchpadGestureStart;
        //MLInput.OnControllerTouchpadGestureEnd += HandleOnTouchpadGestureStart;
    }

    private void OnDestroy()
    {
        MLInput.OnControllerTouchpadGestureStart -= HandleOnTouchpadGestureStart;
        //MLInput.OnControllerTouchpadGestureContinue -= HandleOnTouchpadGestureStart;
        //MLInput.OnControllerTouchpadGestureEnd -= HandleOnTouchpadGestureStart;
    }

    // Update is called once per frame
    void HandleOnTouchpadGestureStart(byte controllerId, MLInputControllerTouchpadGesture gesture) {
        bool found = true;
        //menu.Forward();
        if (gesture.Type == MLInputControllerTouchpadGestureType.Swipe)
        {
            
            switch (gesture.Direction)
            {
                case MLInputControllerTouchpadGestureDirection.Left:
                    menu.Backward();
                    break;
                case MLInputControllerTouchpadGestureDirection.Right:
                    menu.Forward();
                    break;
                case MLInputControllerTouchpadGestureDirection.Up:
                    menu.GetCurrentSection().Forward();
                    break;
                case MLInputControllerTouchpadGestureDirection.Down:
                    menu.GetCurrentSection().Backward();
                    break;
                default:
                    found = false;
                    break;
            }
            
        }
        else if (gesture.Type == MLInputControllerTouchpadGestureType.Tap)
        {
            menu.gameObject.SetActive(!menu.gameObject.activeSelf);
        }
        else
        {
            found = false;
        }

        if (found)
        {
            if (_controller == null)
            {
                try
                {
                    _controller = MLInput.GetController(MLInput.Hand.Left);
                } catch (InvalidInstanceException e)
                {
                    _controller = null;
                }
            }
            if (_controller != null)
            {
                _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Buzz, MLInputControllerFeedbackIntensity.High);
                _controller.StopFeedbackPatternVibe();
            }
        }
    }
    //private void HandleOnTouchpadGestureStart()
    //{
    //    if (_controllerConnectionHandler.IsControllerValid(controllerId) && gesture.Type == MLInputControllerTouchpadGestureType.Swipe)
    //    {
    //        if (gesture.Direction == MLInputControllerTouchpadGestureDirection.Right)
    //        {
    //            ++swipeRight;
    //        }
    //        else if (gesture.Direction == MLInputControllerTouchpadGestureDirection.Left)
    //        {
    //            ++swipeLeft;
    //        }
    //    }
    //}
}
