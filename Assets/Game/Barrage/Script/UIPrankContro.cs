using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPrankContro : MonoBehaviour
{
    public GameObject prankPanel;
    public Button btn_setting;
    public Button btn_boxsetting;
    public Button btn_specialsetting;
    public Button btn_close;
    public Button btn_clear;
    public Button btn_add;

    private bool isPrankPanel;

    void Start()
    {
        btn_setting.onClick.AddListener(OnClickSetting);
        btn_boxsetting.onClick.AddListener(OnClickBoxSetting);
        btn_specialsetting.onClick.AddListener(OnClickSpecialSetting);
        btn_close.onClick.AddListener(ChangePrank);
        btn_clear.onClick.AddListener(OnClickClear);
        btn_add.onClick.AddListener(OnClickAdd);
        prankPanel.SetActive(false);
    }

    private void Update()
    {

    }

    void OnClickSetting()
    {
        BarrageController.Instance.ChangePrankType(0);
    }

    void OnClickBoxSetting()
    {
        BarrageController.Instance.ChangePrankType(1);
    }

    void OnClickSpecialSetting()
    {
        BarrageController.Instance.ChangePrankType(2);
    }

    void OnClickClear()
    {
        BarrageController.Instance.RemoveAllItem();
    }
    void OnClickAdd()
    {
        BarrageController.Instance.AddItem();
    }
    public void ChangePrank()
    {
        isPrankPanel = !isPrankPanel;
        if (isPrankPanel)
        {
            prankPanel.SetActive(true);
            BarrageController.Instance.LoadDataFromJson();
            BarrageController.Instance.ChangePrankType(0);
        }
        else
        {
            BarrageController.Instance.SaveDataToJson();
            prankPanel.SetActive(false);
        }
    }
}
