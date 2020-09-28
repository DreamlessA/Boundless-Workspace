﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChoiceSection : Section {
    public Color[] choices;
    private int brush_color_index = 5;
    private int brush_color_count = 10;

    public Color getCurrentColor()
    {
        return this.choices[this.brush_color_index];
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Forward()
    {
        GameObject color = this.content.transform.GetChild(brush_color_index).gameObject;

        color.transform.GetChild(0).gameObject.SetActive(false);
        color.transform.GetChild(2).gameObject.SetActive(false);

        brush_color_index = brush_color_index < brush_color_count - 1
            ? brush_color_index + 1
            : 0;

        color = this.content.transform.GetChild(brush_color_index).gameObject;

        color.transform.GetChild(0).gameObject.SetActive(true);
        color.transform.GetChild(2).gameObject.SetActive(true);

        this.OnColorSet(this.getCurrentColor());
    }

    public override void Backward()
    {
        GameObject color = this.content.transform.GetChild(brush_color_index).gameObject;

        color.transform.GetChild(0).gameObject.SetActive(false);
        color.transform.GetChild(2).gameObject.SetActive(false);

        brush_color_index = brush_color_index > 0
            ? brush_color_index - 1
            : brush_color_count - 1;

        color = this.content.transform.GetChild(brush_color_index).gameObject;

        color.transform.GetChild(0).gameObject.SetActive(true);
        color.transform.GetChild(2).gameObject.SetActive(true);

        this.OnColorSet(this.getCurrentColor());
    }


    public MeshRenderer _arPointerRayMeshRenderer;
    private void OnColorSet(Color color)
    {
        //if (_arPointer == null)
        //{
        //    _arPointer = FindObjectOfType<ARPointer>();
        //}
        _arPointerRayMeshRenderer.material.color = color;
    }
}