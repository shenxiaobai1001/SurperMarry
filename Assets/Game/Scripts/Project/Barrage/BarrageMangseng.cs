using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageMangseng : BarrageFuncBase
{
    public GameObject seng1;
    public GameObject seng2;
    public float moveSpeed = 3f;
    public bool isLeft;
    Transform playerTarget;
    void Start()
    {
        // playerTarget = PlayerController.Instance.transform;
    }

    void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                ChasePlayer();
                break;
            case BarrageState.Pause:
                break;
        }
    }

    // 外部方法：设置初始位置
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        this.isLeft = Random.Range(0,2) == 1;
        playerTarget = PlayerController.Instance.transform;
        float xx = isLeft ? playerTarget .position.x- 15 : playerTarget.position.x+ 15;

        transform.position = new Vector3(xx, transform.position.y, playerTarget.position.z);
        seng1.SetActive(isLeft);
        seng2.SetActive(!isLeft);
    }

    void ChasePlayer()
    {
        // 向玩家移动
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerTarget.position) < 1.5f)
        {
            barrageData.BarrageState = BarrageState.Finsh;
            if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("铁链"))
            {
                Config.chainCount++;
            }
            EventManager.Instance.SendMessage(Events.HangSelfByKick, isLeft);
            
            int x = isLeft ? 1 : -1;
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(x, 0.5f), 15, 0.25f, true, false, 1);
            OnClose();
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        SimplePool.Despawn(gameObject);
    }
}
