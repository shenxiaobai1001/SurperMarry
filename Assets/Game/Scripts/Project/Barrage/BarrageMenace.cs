using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
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
        PFunc.Log("BarrageMenaceOnClose", barrageController.OnCheckHasHighControl(barrageData.barrageFuncData));
        if (!barrageController.OnCheckHasHighControl(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }

        base.OnClose();
        CancelInvoke();
        SimplePool.Despawn(gameObject);
    }
}
