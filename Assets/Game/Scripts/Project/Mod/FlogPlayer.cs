using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlogPlayer : MonoBehaviour
{
    public static FlogPlayer Instance;
    public Animator animator;
    public GameObject flogEffet;
    public Transform smokePos;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        
    }
    public void OnStartHit()
    {
        if (!checkFlogTime)
        {
            PlayerModController.Instance.isDance = false;
            PlayerController.Instance.transform.position = new Vector3(smokePos.position.x,
            smokePos.position.y, smokePos.position.z);
            checkFlogTime = true; 
            if (PlayerController.Instance != null)
                PlayerController.Instance.OnChanleControl(true);
            PlayerModController.Instance.OnSetModAniIns(false);
            PlayerModController.Instance.OnSetPlayerIns(false);
            PlayerModController.Instance.OnChangeState(false);
        }
    }

    public bool checkFlogTime = false;
    bool hasChangePlaySpeed = false;
    private void Update()
    {
        if (checkFlogTime)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.OnChanleControl(true);
            if (!hasChangePlaySpeed  && Config.FlogCount>10) {
                animator.speed = 4;
                hasChangePlaySpeed = true;
            }
            if (hasChangePlaySpeed && Config.FlogCount < 10)
            {
                animator.speed = 2;
                hasChangePlaySpeed = false;
            }  
        }
    }

    public void OnHitPlayer()
    {
        Sound.PlaySound("Mod/pa");
        Config.FlogCount--;
        GameObject obj = SimplePool.Spawn(flogEffet, smokePos.position, Quaternion.identity);
        obj.transform.SetParent(smokePos);
        obj.SetActive(true);
        if (Config.FlogCount<=0)
        {
            OnClose();
        }
    }

    public void OnClose()
    {
        ItemCreater.Instance.lockPlayer = false;
        PlayerModController.Instance.OnSetPlayerIns(true);
        PlayerModController.Instance.OnChangeState(true);
        PlayerModController.Instance.OnEndHitPos();
        PlayerModController.Instance.OnChanleModAni();
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(false);
        checkFlogTime = false;
        SimplePool.Despawn(gameObject);
    }
}
