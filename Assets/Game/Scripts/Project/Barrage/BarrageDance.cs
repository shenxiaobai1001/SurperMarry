using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageDance : BarrageFuncBase
{
    Animator animator;
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        if (!barrageController.OnCheckHasHighControl(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        Config.EnemyStop = true;
        animator = PlayerModController.Instance.OnGuangDance();
        barrageData.BarrageState = BarrageState.Underway;
        Invoke("OnClose",109);
    }

    public override void OnClose()
    {
        Config.EnemyStop = false;
        if (!barrageController.OnCheckHasHighControl(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        Sound.PlayMusic("background");
        base.OnClose();
        SimplePool.Despawn(gameObject);
    }
}
