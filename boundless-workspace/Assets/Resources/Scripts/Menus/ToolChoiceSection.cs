using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolChoiceSection : Section {
    public string[] toolNames;
    private int tool_index = 0;
    private int tool_count = 3;

    public string getCurrentToolName()
    {
        return this.toolNames[this.tool_index];
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Forward()
    {
        GameObject choice = this.content.transform.GetChild(tool_index).gameObject;
        choice.transform.GetChild(0).gameObject.SetActive(false);

        tool_index = tool_index < tool_count - 1
            ? tool_index + 1
            : 0;

        choice = this.content.transform.GetChild(tool_index).gameObject;
        choice.transform.GetChild(0).gameObject.SetActive(true);
    }

    public override void Backward()
    {
        GameObject choice = this.content.transform.GetChild(tool_index).gameObject;
        choice.transform.GetChild(0).gameObject.SetActive(false);

        tool_index = tool_index > 0
            ? tool_index - 1
            : tool_count - 1;

        choice = this.content.transform.GetChild(tool_index).gameObject;
        choice.transform.GetChild(0).gameObject.SetActive(true);
    }
}
