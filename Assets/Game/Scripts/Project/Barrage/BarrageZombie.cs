using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageZombie : BarrageFuncBase
{
    Transform trans;
    Rigidbody2D rigidbody2D;
    float lastTriggerTime = 0f; // 记录上次触发的时间
    public LayerMask groundLayer; // 障碍物所在的层（如Ground层）
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        OnRest();
        barrageData.BarrageState = BarrageState.Underway;
    }

    private void Update()
    {
        Config.allZombieTime -= Time.deltaTime;
        if (Config.allZombieTime <= 0)
        {
            OnClose();
            return;
        }
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                if (Config.isLoading)
                {
                    OnPause();
                    return;
                }
                RaycastHit2D hit = Physics2D.Raycast(
                trans.position,  // 起点
                Vector2.down,        // 方向向下
                0.6f, groundLayer);   // 检测距离
                PFunc.Log("BarrageZombie", Time.time - lastTriggerTime);
                // 如果检测到地面，并且距离上次触发已超过0.5秒
                if (hit.collider != null && Time.time - lastTriggerTime > 0.5f)
                {
                    PFunc.Log("如果检测到地面", rigidbody2D);
                    rigidbody2D.AddForce(new Vector2(0f, 620));
                    lastTriggerTime = Time.time; // 更新触发时间
                  
                }
                if (OnCheckHasLevel())
                {
                    OnPause();
                }
                break;
            case BarrageState.Pause:
                if (!Config.isLoading&&!OnCheckHasLevel())
                {
                    OnContinue();
                }
                break;
        }
    }

    public override void OnPause()
    {
        base.OnPause();
        PlayerModController.Instance.OnSetPlayerContro(true, true, true);
    }

    public override void OnContinue()
    {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("僵尸+10秒");
        OnRest();
    }

    void OnRest()
    {
        lastTriggerTime = Time.time; // 更新触发时间
        trans = PlayerController.Instance.transform;
        rigidbody2D = PlayerModController.Instance.rigidbody2D;
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(true, false, true);
            PlayerModController.Instance.OnShowModAnimation(15);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        if (!barrageController.OnCheckHasHighControl()
         && !OnCheckHasLevel()
         && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
         && !GameStatusController.isDead)
        {

            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        SimplePool.Despawn(gameObject);
    }
}
