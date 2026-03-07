using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarragePeakKuba : BarrageFuncBase
{
    public GameObject peakObj;
    public GameObject uiObj;
    public Text tx_number;
    public Text tx_succ; 

    public override void OnStart(BarrageValue barrageFuncData, int index)//开始执行
    {
        base.OnStart(barrageFuncData, index);
        OnRest();
        barrageData.BarrageState = BarrageState.Underway;
    }

    private void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                if (Config.kubaCount <= 0)
                {
                    OnClose();
                }
                if (OnCheckHasLevel())
                    OnPause();
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                    OnContinue();
                break;
        }
        if (tx_number) tx_number.text = $"剩余数量：{Config.kubaCount}";
        if (tx_succ) tx_succ.text = $"乌龟数量：{Config.hasKubaCount}";
    }

    public override void OnPause()
    {
        base.OnPause();
        uiObj.SetActive(false);
        peakObj.SetActive(false);
    }

    public override void OnContinue()
    {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("顶乌龟");
        OnRest();
    }

    public void OnRest()
    {
        uiObj.SetActive(true);
        peakObj.SetActive(true);
    }

    public override void OnClose()
    {
        base.OnClose();
        SimplePool.Despawn(gameObject);
    }
}
