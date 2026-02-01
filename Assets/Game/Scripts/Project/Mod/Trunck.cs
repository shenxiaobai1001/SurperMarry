using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class Trunck : MonoBehaviour
{
    [SerializeField] private float speed = 10f; // 子弹速度
    [SerializeField] private float lifeTime = 2f; // 子弹存在时间

    public float moveSpeed = 3f;
    public bool isLeft;
    bool kickPlayer = true;
    Transform playerTarget;
    bool isMove = false;


    private Vector2 moveDirection=Vector2.left; // 子弹移动方向
    private float lifeTimer; // 生命周期计时器
    bool canFly = false;

    void Update()
    {
        if (!isMove) return;
        if (kickPlayer)
        {
            ChasePlayer();
        }
    }

    // 外部方法：设置初始位置
    public void StartMove()
    {
        Invoke("OnBegin",3.5f);
    }
    void OnBegin()
    {
        if (GameStatusController.isDead || Config.isLoading)
        {
            OnClose();
        }else
        {
            lifeTimer = lifeTime;
            isMove = true;
            kickPlayer = true;
        }
    }
    void ChasePlayer()
    {     
        // 移动子弹
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
        // 生命周期管理
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0)
        {
            kickPlayer = false;
            SimplePool.Despawn(gameObject);
        }
    }

    void OnClose()
    {
        SimplePool.Despawn(gameObject);
        gameObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")|| collision.gameObject.CompareTag("BigPlayer") || collision.gameObject.CompareTag("UltimateBigPlayer"))
        {
            kickPlayer = false;
            if (ItemCreater.Instance.lockPlayer)
            {
                ChainPlayer.Instance.transform.DOShakePosition(0.5f, 0.2f).SetEase(Ease.OutQuad).OnComplete(() => {
                    ChainPlayer.Instance.transform.position = new Vector3(Camera.main.transform.position.x, 5, 0);
                });
            }
            CameraShaker.Instance.StartShake(0.2f);
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(-1, 0.5f), 15, 0.25f, true, false, 1);
            OnClose();
        }
    }
}
