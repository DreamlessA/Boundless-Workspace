using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Section : MonoBehaviour {
    public GameObject title;
    public GameObject content;
    public GameObject indicator;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    abstract public void Forward();

    abstract public void Backward();
}
