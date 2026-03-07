using DG.Tweening;
using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class BarrageChainPlayer : BarrageFuncBase
{
    public List<GameObject> gameObjects;
    public GameObject uiCenter;
    public GameObject objCenter;
    public Text tx_number;

    public Animator animator;
    public Transform parent;

    void Awake()
    {
        EventManager.Instance.AddListener(Events.OnLazzerHit, OnLazzerHit);
        EventManager.Instance.AddListener(Events.OnMangSengKick, OnMangSengKick);
    }
   

    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        OnRest();

    }
   void OnRest()
    {
        if (animator) animator.gameObject.SetActive(true);
        PlayerController.Instance.transform.position =
          new Vector3(animator.transform.position.x, animator.transform.position.y, animator.transform.position.z);
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            parent.GetChild(0).gameObject.SetActive(false);
            parent.GetChild(1).gameObject.SetActive(true);
            animator.SetTrigger("redLock");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            parent.GetChild(0).gameObject.SetActive(false);
            parent.GetChild(1).gameObject.SetActive(true);
            animator.SetTrigger("bigLock");
        }
        else
        {
            parent.GetChild(0).gameObject.SetActive(true);
            parent.GetChild(1).gameObject.SetActive(false);
            animator.SetTrigger("smallLock");
        }
        Config.EnemyStop = false;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
        uiCenter.SetActive(true);
        Sound.PauseOrPlayVolumeMusic(true);
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        barrageData.BarrageState = BarrageState.Underway;
    }

    void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                if (Config.chainCount <= 0)
                {
                    OnClose();
                    return;
                }
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))
                {
                    Sound.PlaySound("Mod/paopao");
                    OnRande();
                    Config.chainCount--;
                    OnSnake();
                }
                if (OnCheckHasLevel())
                    OnPause();
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                    OnContinue();
                break;
        }
        
        tx_number.text = $"{Config.chainCount}";
    }

    void OnMangSengKick(object msg)
    {
        OnSnake();
    }
    void OnSnake()
    {
        objCenter.transform.DOShakePosition(0.5f, 0.2f)
                  .SetEase(Ease.OutQuad)
                  .OnComplete(() => {
                      objCenter.transform.localPosition = Vector3.zero;
                  });
    }
    public override void OnPause()
    {
        base.OnPause();
        if (animator) animator.gameObject.SetActive(false);
    }

    public override void OnContinue()
    {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("…œµı");
        OnRest();
    }

    public void OnRande()
    {
        int value = UnityEngine.Random.Range(0, gameObjects.Count);
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(value == i);
        }
    }

    void OnLazzerHit(object msg)
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("redLockLazzer");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("bigLockLazzer");
        }
        else
        {
            animator.SetTrigger("smallLockLazzer");
        }
        OnSnake();
    }

    public override void OnClose()
    {
        base.OnClose();
        Sound.PlayMusic("background");
        Sound.PauseOrPlayVolumeMusic(false);
        PFunc.Log("Ã˙¡¥Ω· ¯£∫", barrageController.OnCheckHasHighControl(barrageData.barrageFuncData));
        if (!barrageController.OnCheckHasHighControl()
        && !OnCheckHasLevel()
        && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
        && !GameStatusController.isDead) { 
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        SimplePool.Despawn(gameObject);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
        EventManager.Instance.RemoveListener(Events.OnMangSengKick, OnMangSengKick);
    }
}
