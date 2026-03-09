using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageMenace : BarrageFuncBase
{
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        int number = Random.Range(1, 39);
        ModVideoPlayerCreater.Instance.OnCreateModVideoPlayer(new Vector3(-0.5f, 0.4f, 90), Vector3.one, Vector3.zero, $"Question/{number}", 2);
        OnEnterResult();
    }

    public override void OnEnterResult()
    {
        Invoke("OnClose", 1.5f);
    }

    public override void OnClose()
    {
        base.OnClose();
        PFunc.Log("BarrageMenaceOnClose", barrageController.OnCheckHasHighControl(barrageData.barrageFuncData));
        if (!barrageController.OnCheckHasHighControl()
           && !OnCheckHasLevel()
           && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
           &&!GameStatusController.isDead)
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        CancelInvoke();
        SimplePool.Despawn(gameObject);
    }
}
