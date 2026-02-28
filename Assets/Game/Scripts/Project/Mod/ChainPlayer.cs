using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.UI;

public class ChainPlayer : BarrageFuncBase
{
    public static ChainPlayer Instance;
    public List<GameObject> gameObjects;
    public GameObject center;
    public Text tx_number;

    public Animator animator;
    public Transform parent;

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
        EventManager.Instance.AddListener(Events.OnLazzerHit,OnLazzerHit);
    }

    private void OnInitState()
    {
        PlayerModController.Instance.OnSetModAniIns(false);
        PlayerModController.Instance.OnSetPlayerIns(false);
        PlayerModController.Instance.OnChangeState(false);
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
    }
    public override void OnStart()
    {
        base.OnStart();
    //    barrageType = BarrageType.Ready;
        Config.EnemyStop = false;
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
        center.SetActive(true);
        PlayerModController.Instance.isDance = false;
        Sound.PauseOrPlayVolumeMusic(true);
    }
    // Update is called once per frame
    void Update()
    {
        if (Config.chainCount <= 0) return;
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.K))
        {
            Sound.PlaySound("Mod/paopao");
            OnRande();
            Config.chainCount--;
            if (Config.chainCount <= 0)
            {
                OnChekcMinZero();
                return;
            }
            transform.DOShakePosition(0.5f, 0.2f)
              .SetEase(Ease.OutQuad)
              .OnComplete(() => {transform.position = new Vector3(Camera.main.transform.position.x, 5, 0);
              });
        }
        tx_number.text = $"{Config.chainCount}";
    }

    public void OnChekcMinZero()
    {
        Sound.PlayMusic("background");
        Sound.PauseOrPlayVolumeMusic(false);
        PlayerModController.Instance.OnSetModAniIns(true);
        ItemCreater.Instance.lockPlayer = false;
        center.SetActive(false);
        PlayerController.Instance.isHit = false;
        SimplePool.Despawn(gameObject);
        PlayerModController.Instance.OnSetPlayerIns(true);
        PlayerModController.Instance.OnChangeState(true);
        PlayerModController.Instance.OnEndHitPos();
        PlayerModController.Instance.OnChanleModAni();
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
        if (GameStatusController.IsFirePlayer&& GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("redLockLazzer");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("bigLockLazzer");
        }else
        {
            animator.SetTrigger("smallLockLazzer");  
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
    }
}
