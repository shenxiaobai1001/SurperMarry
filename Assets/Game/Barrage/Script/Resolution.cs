using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    public Dropdown dropdown;

    void Start()
    {
        dropdown.onValueChanged.AddListener(ChangeResolution);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChangeResolution(int index)
    {
        int x, y;
        string[] text =  dropdown.options[index].text.Split("X");
        x = int.Parse(text[0]);
        y = int.Parse(text[1]);
        switch(index)
        {
            case 0:
                Screen.SetResolution(x, y, false);
            break;
            case 1:
                Screen.SetResolution(x, y, false);
            break;
            case 2:
                Screen.SetResolution(x, y, false);
            break;
        }
        Debug.Log($"·Ö±æÂÊÎª {x} X {y}");
    }
}
