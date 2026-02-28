using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RopeSkip : MonoBehaviour
{
    public static RopeSkip Instance;
    public Animator animator;
    public Transform playerTrans;
    public GameObject topCollider;
    public GameObject bottomCollider;

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
    // Start is called before the first frame update
    void Start()
    {
        OnCloseCollider();
    }
    float time = 0;
    bool autoJump = false;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z)&&!autoJump)
        {
            animator.speed = 2;
            autoJump= true;
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            animator.speed = 1;
            autoJump = false;
        }
    }

    public void OnStart()
    {
        PlayerModController.Instance.OnSetModAniIns(false);
        //PlayerModController.Instance.OnSetPlayerIns(false);
        //PlayerModController.Instance.OnChangeState(false);
        PlayerController.Instance.transform.position = new Vector3(playerTrans.transform.position.x, playerTrans.transform.position.y, playerTrans.transform.position.z);

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
            PlayerController.Instance.OnJumpAuto();
        }
    }
    public void OnMinCount()
    {
        if (!triggerPlayer)
        {
            Config.succRopeCount++;
            Config.ropeCount--;
        }
        if (Config.ropeCount<=0)
        {
            EventManager.Instance.SendMessage(Events.OnShowRope, false);
            if (PlayerController.Instance != null)
                PlayerController.Instance.OnHorLock(true);
            Sound.PlayMusic("background");
            Sound.PauseOrPlayVolumeMusic(false);
            PlayerModController.Instance.OnSetModAniIns(true);
            ItemCreater.Instance.lockPlayer = false;
            PlayerController.Instance.isHit = false;
            SimplePool.Despawn(gameObject);
            PlayerModController.Instance.OnSetPlayerIns(true);
            PlayerModController.Instance.OnChangeState(true);
            PlayerModController.Instance.OnEndHitPos();
            PlayerModController.Instance.OnChanleModAni();
        }
        triggerPlayer = false;
    }

    public bool triggerPlayer = false;
}
