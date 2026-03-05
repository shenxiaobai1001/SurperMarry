using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMask : BarrageFuncBase
{
    public GameObject Mask;

    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);

        barrageData.BarrageState = BarrageState.Underway;
    }

    void Update()
    {
        if (barrageData.BarrageState != BarrageState.Underway)
            return;

        Config.maskTime -= Time.deltaTime;

        if (Mask) Mask.SetActive (Config.maskTime > 0);

        if (Config.maskTime<=0)
        {
            OnClose();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}
