using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageFuncBase : MonoBehaviour
{
    [HideInInspector] public BarrageValue barrageData;
    [HideInInspector] public BarrageFuncController barrageController;
    [HideInInspector] public int barrageIndex;
    [HideInInspector] public bool isInit;

    private void Awake()
    {
      
    }

    public virtual void OnStart(BarrageValue barrageFuncData, int index)//开始执行
    {
        isInit = false;
        barrageController = BarrageFuncController.Instance;
        this. barrageData=barrageFuncData;
        this.barrageIndex = index;
        barrageData.BarrageState = BarrageState.Ready;
        isInit = true;
    }

    public virtual void OnPause() { barrageData.BarrageState = BarrageState.Pause; }//暂停执行
    public virtual void OnContinue() { barrageData.BarrageState = BarrageState.Underway; }//继续执行
    public virtual void OnEnterResult() { }

    public bool OnCheckHasLevel()
    {
        bool ishaigh = false;
        if (barrageController)
        {
            ishaigh = barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData);
        }
        return ishaigh;
    }

    public virtual void OnClose()//执行完毕
    {
        isInit = false;
        barrageData.BarrageState = BarrageState.Finsh;
        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, barrageIndex);
    }
}
