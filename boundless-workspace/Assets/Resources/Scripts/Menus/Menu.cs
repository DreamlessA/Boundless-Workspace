using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
    public Section[] sections;

    private int section_index = 0;
    private int section_count = 3;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Forward()
    {
        this.sections[section_index].indicator.SetActive(false);

        section_index = section_index < section_count - 1 ? section_index + 1 : 0;

        this.sections[section_index].indicator.SetActive(true);
    }

    public void Backward()
    {
        this.sections[section_index].indicator.SetActive(false);

        section_index = section_index > 0 ? section_index - 1 : section_count - 1;

        this.sections[section_index].indicator.SetActive(true);
    }

    public Section GetCurrentSection()
    {
        return this.sections[section_index];
    }
}
