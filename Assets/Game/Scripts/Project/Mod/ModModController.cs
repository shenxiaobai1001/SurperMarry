using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class ModModController : MonoBehaviour
{
    public static ModModController Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public UIPrankContro uPrankContro;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        BarrageController.Instance.LoadDataFromJson();
    }
    bool systemShow = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uPrankContro.ChangePrank();
        }
    }
}
