using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class ChainPlayer : MonoBehaviour
{
    public static ChainPlayer Instance;
    public Animator animator;
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
    private void OnEnable()
    {
        PlayerModController.Instance.OnSetPlayerIns(false);
        PlayerModController.Instance.OnChangeState(false);
        PlayerController.Instance.transform.position = new Vector3(animator.transform.position.x, animator.transform.position.y, animator.transform.position.z);
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.transform.GetChild(0).gameObject.SetActive(false);
            animator.transform.GetChild(1).gameObject.SetActive(true);
            animator.SetTrigger("redLock");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.transform.GetChild(0).gameObject.SetActive(false);
            animator.transform.GetChild(1).gameObject.SetActive(true);
            animator.SetTrigger("bigLock");
        }
        else
        {
            animator.transform.GetChild(0).gameObject.SetActive(true);
            animator.transform.GetChild(1).gameObject.SetActive(false);
            animator.SetTrigger("smallLock");
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
