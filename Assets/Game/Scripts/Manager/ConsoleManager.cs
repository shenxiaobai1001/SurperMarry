using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleManager : MonoBehaviour
{
    public GameObject obj;
    private void Start()
    {
        DontDestroyOnLoad(this);
        obj.SetActive(false);
    }

    int count = 0;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
            OnShowConsole();
    
    }
    void OnShowConsole()
    {
        count++;
        if (count >= 5)
        {
            obj.SetActive(true);
        }
    }

    private void OnDestroy()
    {
    

    }
}
