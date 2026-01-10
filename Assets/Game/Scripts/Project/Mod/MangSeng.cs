using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MangSeng : MonoBehaviour
{
    public GameObject seng1;    
    public GameObject seng2;
    public float moveSpeed = 3f;
    public bool isLeft;
    bool kickPlayer = true;
    Transform playerTarget;
    bool isMove = false;
    void Start()
    {
       // playerTarget = PlayerController.Instance.transform;
    }

    void Update()
    {
        if (!isMove) return;
        if (kickPlayer)
        {
            ChasePlayer();
        }
    }

    // 外部方法：设置初始位置
    public void StartMove(bool isLeft)
    {
        this.isLeft = isLeft;
        seng1.SetActive(isLeft);
        seng2.SetActive(!isLeft);

        playerTarget = PlayerController.Instance.transform;
        PFunc.Log(PlayerController.Instance.transform.position, transform.position, isLeft);
        isMove = true;
        kickPlayer = true;
    }

    void ChasePlayer()
    {
        
        // 向玩家移动
        transform.position = Vector3.MoveTowards(transform.position, playerTarget.position, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerTarget.position) < 1.5f)
        {
            kickPlayer = false;
            if (ItemCreater.Instance.lockPlayer)
            {
                ChainPlayer.Instance.transform.DOShakePosition(0.5f, 0.2f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                ChainPlayer.Instance.transform.position = new Vector3(Camera.main.transform.position.x, 5, 0);
            });
            }
            int x=isLeft ? 1 : -1;   
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(x,0.5f), 15,0.25f,true,false,1);
            OnClose();
        }
    }

    void OnClose()
    {
        SimplePool.Despawn(gameObject);
        gameObject.SetActive(false);
    }

}
