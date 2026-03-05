using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageWuSaqi : BarrageFuncBase
{
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        int number = Random.Range(1, 13);
        float scaleValue = Random.Range(0.25f, 1);
        int rotateValue = Random.Range(0, 360);
        Vector3 scale = new Vector3(scaleValue, scaleValue, 1);
        Vector3 rotate = new Vector3(0, 0, rotateValue);
        ModVideoPlayerCreater.Instance. OnCreateModVideoPlayer(Vector3.zero, scale, rotate, "GreenScreen/wusaqi", 2);
        Invoke("OnEnterResult", 1);
    }

    public override void OnEnterResult()
    {
        if (barrageController.OnCheckHasHighControl(barrageData.barrageFuncData))
        {
            OnClose();
        }
        else
        {
            PlayerModController.Instance.OnTiggerDao();
            if (barrageData.name != "ÎÚÈøÆæ")
            {
                PlayerModController.Instance.OnSetPlayerContro(false,true, true);
            }
            Invoke("OnClose", 1.5f);
        }
    }

    public override void OnClose()
    {
        if (!barrageController.OnCheckHasHighControl(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }

        base.OnClose();
        CancelInvoke();
        SimplePool.Despawn(gameObject);
    }
}
