using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class ShoeShine : MonoBehaviour
{
    public static ShoeShine Instance;
    public Animator animator;

    string aniType = "";
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
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)|| Input.GetKeyDown(KeyCode.J)
            )
        {
            if (animator != null) animator.SetTrigger(aniType);
            Config.shineCount--;
            if (Config.shineCount <= 0)
            {
                EventManager.Instance.SendMessage(Events.OnShowShine, false);
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
        }
    }

    public void OnStart()
    {
        PlayerModController.Instance.OnSetModAniIns(false);
        PlayerModController.Instance.OnSetPlayerIns(false);
        PlayerModController.Instance.OnChangeState(false);
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
}
