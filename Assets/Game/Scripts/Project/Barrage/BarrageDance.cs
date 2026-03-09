using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageDance : BarrageFuncBase
{
    Animator animator;
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        time = 0;
        OnRest();
    }
    float time = 0;
    float allTime = 109;
    private void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                time += Time.deltaTime;
                if (time>= allTime)
                {
                    OnClose();
                }
                if (OnCheckHasLevel())
                {
                    OnClose();
                    return;
                }
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                {
                    OnContinue();
                    return;
                }
                break;
            case BarrageState.Finsh:
                break;
            case BarrageState.Close:
                break;
        }
    }
    public override void OnContinue()
    {
        base.OnContinue();
        OnRest();
    }
    public override void OnPause()
    {
        base.OnPause();
    }
    void OnRest()
    {
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        Config.EnemyStop = true;
        animator = PlayerModController.Instance.OnGuangDance();
        barrageData.BarrageState = BarrageState.Underway;
    }

    public override void OnClose()
    {
        base.OnClose();
        Config.EnemyStop = false;
        if (!barrageController.OnCheckHasHighControl()
          && !OnCheckHasLevel()
          && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
          && !GameStatusController.isDead) {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        Sound.PlayMusic("background");
        SimplePool.Despawn(gameObject);
    }
}
