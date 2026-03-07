using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarrageFlogPlayer : BarrageFuncBase
{
    public Animator animator;
    public GameObject flogEffet;
    public Transform smokePos;
    public SpriteRenderer spriteRenderer;

    public List<GameObject> gameObjects;
    public Text tx_number;
    public GameObject UIObbj;

    float changTime = 0;
    float allChangeTime = 1;
    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);

        PlayerController.Instance.transform.position =
        new Vector3(smokePos.position.x, smokePos.position.y, smokePos.position.z);
        if (!barrageController.OnCheckHasHighControlLevel(barrageData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
        barrageData.BarrageState = BarrageState.Underway;
    }

    private void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
                break;
            case BarrageState.Underway:
                if (OnCheckHasLevel())
                {
                    OnPause();
                    return;
                }
                OnChangeAniSpeed();
                changTime += Time.deltaTime;

                if (changTime >= allChangeTime)
                {
                    OnShowOneObj(Random.Range(0, gameObjects.Count));
                    changTime = 0;
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
        }

        tx_number.text = $"{Config.FlogCount}";
    }
    void OnShowOneObj(int index)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(index == i);
        }
    }
    public override void OnPause()
    {
        base.OnPause();
        animator.speed = 0;
        spriteRenderer.enabled=false;
        UIObbj.SetActive(false);
    }

    public override void OnContinue()
    {
        base.OnContinue();
        OnChangeAniSpeed();
        spriteRenderer.enabled = true;
        UIObbj.SetActive(true);
    }

    void OnChangeAniSpeed()
    {
        if (Config.FlogCount > 10)
        {
            animator.speed = 4;
        }
        if (Config.FlogCount < 10)
        {
            animator.speed = 2;
        }
    }

    public void OnHitPlayer()
    {
        Sound.PlaySound("Mod/pa");
        Config.FlogCount--;
        GameObject obj = SimplePool.Spawn(flogEffet, smokePos.position, Quaternion.identity);
        obj.transform.SetParent(smokePos);
        obj.SetActive(true);
        if (Config.FlogCount <= 0)
        {
            OnClose();
            barrageData.BarrageState = BarrageState.Finsh;
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
