using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.UI;

public class BarrageShoeShine : BarrageFuncBase
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject center;
    public Text tx_number;

    string aniType = "";
    public override void OnStart(BarrageValue barrageFuncData, int index)//开始执行
    {
        base.OnStart(barrageFuncData, index);
        OnRest();
        barrageData.BarrageState = BarrageState.Underway;
    }

    private void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Ready:
            case BarrageState.Underway:
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.J))
                {
                    Sound.PlaySound("Mod/ca");
                    if (animator != null) animator.SetTrigger(aniType);
                    Config.shineCount--;
                    if (tx_number) tx_number.transform.DOScale(1.1f, 0.025f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
                    {
                        tx_number.transform.localScale = Vector3.one;
                    });
                    if (Config.shineCount <= 0)
                    {
                        OnClose();
                        return;
                    }
                }
                if (OnCheckHasLevel())
                    OnPause();
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                    OnContinue();
                break;
        }

        if (tx_number) tx_number.text = $"剩余次数{Config.shineCount}";
    }

    public override void OnPause() 
    {
        base.OnPause();
        spriteRenderer.enabled = false;
        center.SetActive(false);
    }
    public override void OnContinue() {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("擦皮鞋");
        spriteRenderer.enabled = true;
        center.SetActive(true); 
        OnRest();
    }

    public override void OnEnterResult() { }

    public void OnRest()
    {
        if (!OnCheckHasLevel())
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }

        spriteRenderer.enabled = true;
        center.SetActive(true);
        PlayerController.Instance.transform.position = new Vector3(animator.transform.position.x, animator.transform.position.y, animator.transform.position.z);

        if (GameStatusController.IsQiangPlayer)
        {
            if (animator != null) animator.SetTrigger("blackIdle");
            aniType = "black";
        }
        else if ((GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer) || GameStatusController.IsDaoPlayer)
        {
            if (animator != null) animator.SetTrigger("redIdle");
            aniType = "red";
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            if (animator != null) animator.SetTrigger("bigIdle");
            aniType = "big";
        }
        else
        {
            if (animator != null) animator.SetTrigger("smallIdle");
            aniType = "small";
        }
    }

    public override void OnClose()//执行完毕
    {
        barrageData.BarrageState = BarrageState.Finsh;
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
