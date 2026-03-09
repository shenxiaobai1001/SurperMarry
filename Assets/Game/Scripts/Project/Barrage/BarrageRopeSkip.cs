using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.UI;

public class BarrageRopeSkip : BarrageFuncBase
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public Transform playerTrans;
    public GameObject topCollider;
    public GameObject bottomCollider;
    public RopePlayer player;

    public GameObject center;
    public Text tx_number;
    public Text tx_miss;
    public Text tx_succ;

    bool autoJump = false;
    // Update is called once per frame
    void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Ready:
            case BarrageState.Underway:
                if (Input.GetKey(KeyCode.Z) && !autoJump)
                {
                    animator.speed = 4;
                    autoJump = true;
                }
                if (Input.GetKeyUp(KeyCode.Z))
                {
                    animator.speed = 2;
                    autoJump = false;
                }
                if (OnCheckHasLevel())
                    OnPause();
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                    OnContinue();
                break;
        }
        if (tx_number) tx_number.text = $"剩余次数{Config.ropeCount}";
        if (tx_succ) tx_succ.text = $"成功次数{Config.succRopeCount}";
        if (tx_miss) tx_miss.text = $"失败次数{Config.missRopeCount}";
    }
    public override void OnPause()
    {
        base.OnPause();
        center.SetActive(false);
        //spriteRenderer.enabled = false;
        autoJump=false;
        player.gameObject.SetActive(false); 
    }
    public override void OnContinue()
    {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("跳绳");
        OnRest();
    }
    
    public override void OnStart(BarrageValue barrageFuncData, int index)//开始执行
    {
        base.OnStart(barrageFuncData, index);
        OnRest();
        barrageData.BarrageState = BarrageState.Underway;
    }

    public void OnShowTopCollider()
    {
        if (topCollider) topCollider.SetActive(true);
    }
    public void OnShowButtonCollider()
    {
        if (bottomCollider) bottomCollider.SetActive(true);
    }
    public void OnCloseCollider()
    {
        if (topCollider) topCollider.SetActive(false);
        if (bottomCollider) bottomCollider.SetActive(false);
    }
    public void OnAutoJump()
    {
        if (autoJump)
        {
            player.OnJumpAuto(true);
        }
    }
    public void OnRest()
    {
        autoJump = false;
        player.gameObject.SetActive(true);
        animator.speed = 2;
        OnCloseCollider();
        center.SetActive(true); 
        player.transform.localPosition = Vector3.zero;
        //spriteRenderer.enabled = true;
        if (!OnCheckHasLevel())
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        PlayerController.Instance.transform.position =
            new Vector3(playerTrans.transform.position.x, playerTrans.transform.position.y, playerTrans.transform.position.z);
    }
    public void OnMinCount()
    {
        if (!triggerPlayer)
        {
            Config.succRopeCount++;
            Config.ropeCount--;
            if (tx_succ) tx_succ.transform.DOScale(1.1f, 0.025f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                tx_succ.transform.localScale = Vector3.one;
            });
        }
        if (Config.ropeCount <= 0)
        {
            OnClose();
        }
        triggerPlayer = false;
    }

    public bool triggerPlayer = false;

    public override void OnClose()
    {
        base.OnClose();
        Sound.PlayMusic("background");
        Sound.PauseOrPlayVolumeMusic(false);
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
