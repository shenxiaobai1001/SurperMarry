using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageSuperMan : BarrageFuncBase
{
    public GameObject superEffert;

    Transform trans ;
    Transform suuperTrans;
    float superEffectTime = 0;
    float superSpeed = 10;
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        OnRest();

    }

    private void Update()
    {
        Config.superManTime-=Time.deltaTime;
        if (Config.superManTime <= 0)
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
                if (Config.isLoading|| BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
                    || OnCheckHasLevel())
                {
                    OnPause();
                    return;
                }
                if (Input.GetKey(KeyCode.A))
                {
                    trans.Translate(Vector2.right * superSpeed * Time.deltaTime);
                    if (PlayerController.Instance._isFacingRight)
                    {
                        trans.Rotate(0, 180, 0);
                        PlayerController.Instance._isFacingRight = false;
                    }
                    OnCreateSuperEffect();
                }
                if (Input.GetKey(KeyCode.D))
                {
                    trans.Translate(Vector2.right * superSpeed * Time.deltaTime);
                    if (!PlayerController.Instance._isFacingRight)
                    {
                        trans.Rotate(0, 180, 0);
                        PlayerController.Instance._isFacingRight = true;
                    }
                    OnCreateSuperEffect();
                }
                if (Input.GetKey(KeyCode.S))
                {
                    trans.Translate(Vector2.down * superSpeed * Time.deltaTime);
                    OnCreateSuperEffect();
                }
                if (Input.GetKey(KeyCode.W))
                {
                    trans.Translate(Vector2.up * superSpeed * Time.deltaTime);
                    OnCreateSuperEffect();
                }
                break;
            case BarrageState.Pause:
                if (!Config.isLoading && !OnCheckHasLevel()
                    &&!BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData))
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
        OnRest();
    }
    void OnRest()
    {
        trans = PlayerController.Instance.transform;
        suuperTrans = PlayerModController.Instance.superEffertTrans;
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
            PlayerModController.Instance.OnShowModAnimation(7);
            barrageData.BarrageState = BarrageState.Underway;
        }
        else
            barrageData.BarrageState = BarrageState.Pause;
    }

    void OnCreateSuperEffect()
    {
        superEffectTime += Time.deltaTime;
        if (superEffectTime >= 0.02f)
        {
            GameObject obj = SimplePool.Spawn(superEffert, suuperTrans.position, Quaternion.identity);
            obj.transform.SetParent(ModController.Instance.itemParent);
            obj.SetActive(true);
            superEffectTime = 0;
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        if (!barrageController.OnCheckHasHighControl() 
            && !OnCheckHasLevel()
            && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData))
        {

            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        SimplePool.Despawn(gameObject);
    }
}
