using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageDaom : BarrageFuncBase
{
    Animator animator;
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        Config.EnemyStop = true;
        animator = PlayerModController.Instance.OnDMDance();
        barrageData.BarrageState = BarrageState.Underway;
        Invoke("OnClose", 4);
    }
    private void Update()
    {
        if (OnCheckHasLevel())
        {
            OnClose();
        }
    }
    public override void OnClose()
    {
        base.OnClose();
        Config.EnemyStop = false;
        if (!barrageController.OnCheckHasHighControl()
                && !OnCheckHasLevel()
                && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData))
        {

            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        if (!BarrageFuncController.Instance.OnCheckBarrageFuncByName("Ήγ²₯Με²Ω"))
        {
            Sound.PlayMusic("background");
        }
        SimplePool.Despawn(gameObject);
    }
}
