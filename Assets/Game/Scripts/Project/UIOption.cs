using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class UIOption : MonoBehaviour
{
    public GameStatusController gameStatus;
    bool click = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J)&&!click)
        {
            click = true;
            ModController.Instance.statusController.StartGame();
        }
    }
}
