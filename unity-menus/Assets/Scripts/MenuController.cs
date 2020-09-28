using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public Menu menu;

    void Awake()
    {

    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            menu.Backward();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            menu.Forward();
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            menu.GetCurrentSection().Forward();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            menu.GetCurrentSection().Backward();
        }
    }
}
