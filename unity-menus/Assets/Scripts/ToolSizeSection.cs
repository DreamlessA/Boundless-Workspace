using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToolSizeSection : Section {
    private int tool_size_current = 12;
    private int tool_size_min = 1;
    private int tool_size_max = 20;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Forward()
    {
        tool_size_current = tool_size_current < tool_size_max
                            ? tool_size_current + 1
                            : tool_size_current;

        float percentage = ((float)tool_size_current) / tool_size_max;

        GameObject bar = this.content.transform.GetChild(0).GetChild(0).gameObject;
        GameObject fill = bar.transform.GetChild(0).gameObject;
        fill.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
            axis: RectTransform.Axis.Horizontal,
            size: percentage * bar.GetComponent<RectTransform>().rect.width
        );

        Text reading = this.content.transform.GetChild(1).gameObject.GetComponent<Text>();
        reading.text = string.Format("{0}px", tool_size_current);
    }

    public override void Backward()
    {
        tool_size_current = tool_size_current > tool_size_min
                            ? tool_size_current - 1
                            : tool_size_current;

        float percentage = ((float)tool_size_current) / tool_size_max;

        GameObject bar = this.content.transform.GetChild(0).GetChild(0).gameObject;
        GameObject fill = bar.transform.GetChild(0).gameObject;
        fill.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
            axis: RectTransform.Axis.Horizontal,
            size: percentage * bar.GetComponent<RectTransform>().rect.width
        );

        Text reading = this.content.transform.GetChild(1).gameObject.GetComponent<Text>();
        reading.text = string.Format("{0}px", tool_size_current);
    }
}
