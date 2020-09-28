using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

public class ScaleManager : MonoBehaviour {

    public Transform _pointerRay;
    private MLInputController _controller;
    private Vector3 MIN_SCALE = new Vector3(0.1f,0.1f,0.1f);
    private Vector3 MAX_SCALE = new Vector3(3.0f,3.0f,3.0f);

	// Use this for initialization
	void Start () {
        _controller = MLInput.GetController(MLInput.Hand.Left);
        MLInput.OnControllerTouchpadGestureStart += OnControllerTouchpadGestureStart;

	}

    private void OnDestroy()
    {
        MLInput.OnControllerTouchpadGestureStart -= OnControllerTouchpadGestureStart;
    }

   
    private void OnControllerTouchpadGestureStart(byte id, MLInputControllerTouchpadGesture gesture)
    {
        

        if (gesture.Type == MLInputControllerTouchpadGestureType.ForceTapDown)
        {
            //bool isHit = false;
            
            RaycastHit hit;
            if (Physics.Raycast(_pointerRay.position, _pointerRay.forward, out hit))
            {
                //isHit = true;
                //WindowController window = hit.transform.GetComponentInParent<WindowController>();
                UIComponent ui = hit.transform.GetComponent<UIComponent>();
                //Debug.Log(window);
                if (ui != null)
                {
                    Transform transform = ui.transform.parent;
                    //hit.transform.GetComponentInParent<Transform>().Find("Layers");
                    
                    //Debug.Log(transform);
                    //transform.localScale *= 2;// new Vector3(0.1f, 0.1f, 0.1f);
                    //transform = hit.transform.GetComponentInParent<Transform>();

                    //transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
                    //hit.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);

                    
                    if (gesture.PosAndForce.Value.x >= 0)
                    {
                        transform.localScale *= 1.1f; // increase the scale by 10%
                        if (transform.localScale.sqrMagnitude > MAX_SCALE.sqrMagnitude)
                            transform.localScale.Set(MAX_SCALE.x,MAX_SCALE.y,MAX_SCALE.z); 
                    }
                    else
                    {
                        transform.localScale *= 0.9f; // decrease the scale by 10%
                        if (transform.localScale.sqrMagnitude < MIN_SCALE.sqrMagnitude)
                            transform.localScale.Set(MIN_SCALE.x, MIN_SCALE.y, MIN_SCALE.z);
                    }
                    _controller.StartFeedbackPatternVibe(MLInputControllerFeedbackPatternVibe.Tick, MLInputControllerFeedbackIntensity.High);
                    _controller.StopFeedbackPatternVibe();

                }

                //Debug.Log("Raycast hit an object");
            }
            
            
        }
    }

}
