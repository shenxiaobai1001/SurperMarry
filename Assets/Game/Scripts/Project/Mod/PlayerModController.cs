using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class PlayerModController : MonoBehaviour
{
    public static PlayerModController Instance;
    private void Awake()
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
        EventManager.Instance.AddListener(Events.OnLazzerHit, OnLazzerHit);
        playerBreak.gameObject.SetActive(false);
        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(false);
        }
    }

    public Animator animator;
    public Rigidbody2D rigidbody2D;
    public GameObject spriteTrans;
    public GameObject Center;
    public List<SpriteRenderer> spriteRenderers;
    public PlayerBreak playerBreak;
    public List<GameObject> modAnimations;

    int isPassivityMove;

    [HideInInspector]public bool isKinematic = false;
    public void OnChangeState(bool open)
    {
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = !open;
        isKinematic = !open;
        Center.SetActive(open);
    }
    public void OnChangeStateTrue()
    {
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = true;
        isKinematic = true;
        Center.SetActive(false);
    }
    public void OnChangeStateFalse()
    {
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = false;
        isKinematic = false;
        Center.SetActive(true);
    }
    public void OnSetPlayerIns(bool show)
    {
        PFunc.Log("OnSetPlayerIns");
        for (int i = 0; i < spriteRenderers.Count; i++)
        {
            Color color = show ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
            spriteRenderers[i].color = color;
        }
    }

    public void OnToHitPos()
    {
        if(animator) animator.SetTrigger("Swim");
    }
    public void OnEndHitPos()
    {
        if (animator) animator.SetTrigger("endSwim");
    }
    public void OnToSwim()
    {
        OnToHitPos();
        bool isRight = PlayerController.Instance._isFacingRight;
        if(isRight)
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(1, 0.5f), 20, 0.3f, true, false, 1);
        else
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(-1, 0.5f), 20, 0.3f, true, false, 1);
    }

    public void OnAddFourePlayer(Vector3 vector)
    {
        if (ItemCreater.Instance.isHang)
        {
            OnCancelHangSelf();
        }
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        rigidbody2D.AddForce (vector,ForceMode2D.Impulse); // 重置水平速度
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;
        
        if (collision.gameObject.CompareTag("Banana"))
        {
            PFunc.Log("碰到香蕉");
            OnToSwim();
        }
    }
    public void OnHangSelf()
    {
       OnChangeState(false);

        if (PlayerController.Instance != null)
            PlayerController.Instance.isHit = true;
        OnSetPlayerIns(false);
        isPassivityMove++;
    }

    public void OnCancelHangSelf()
    {
        spriteTrans.gameObject.SetActive(true);
        isPassivityMove--;
        if (isPassivityMove <= 0)
        {
            isPassivityMove = 0;
            OnChangeState(true);
        }

        if (HangSelf.Instance != null && HangSelf.Instance.lastPoint != null)
        {
            Vector3 vector = HangSelf.Instance.lastPoint.transform.position;
            if (vector != Vector3.zero)
            {
                transform.position = vector;
            }
            HangSelf.Instance.OnBreakeHang();
        }
        OnSetPlayerIns(true);

    }

    public void OnTriggerModAnimator(string riggerName)
    {
        if(animator) animator.SetTrigger(riggerName);
    }

    void OnLazzerHit(object msg)
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("redLazzer");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("bigLazzer");
        }
        else
        {
            animator.SetTrigger("smallLazzer");
        }
    }


    public void OnBigDao() => OnShowModAnimation(0);
    public void OnTiggerDao()
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(1);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(0);
        }
        else
        {
            OnTriggerModAnimator("dao");
        }
    }
    public void OnTiggerManace()
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(3);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(2);
        }
        else {
            OnTriggerModAnimator("menace");
        }
    }

    public void OnChanleModAni()
    {
        OnShowModAnimation(5);
        OnSetPlayerIns(true);

        animator.SetTrigger("endMenace");
    }

    void OnShowModAnimation(int index)
    {
        OnSetPlayerIns(false);
        for (int i = 0; i < modAnimations.Count; i++) {
            modAnimations[i].gameObject.SetActive(i==index);
        }
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
    }
}
