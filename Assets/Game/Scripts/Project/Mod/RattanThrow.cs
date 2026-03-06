using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class RattanThrow : MonoBehaviour
{
    [Header("藤条设置")]
    [SerializeField] private SpriteRenderer rattanSprite;
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float normaRattanSpeed = 2f;  // 正常伸缩速度
    [SerializeField] private float pullSpeed = 3f;     // 拉回速度
    [SerializeField] private float raycastOffset = 0.1f; // 射线发射位置偏移

    [Header("玩家设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask playerLayer;    // 玩家所在的层
    [SerializeField] private List<GameObject> modAnimations;

    public GameObject showObj;
    public GameObject hideObj;
    public GameObject rattanObj;
    public GameObject hangObj;
    public GameObject player;

    // 状态变量
    private float currentHeight;
    private float targetHeight;
    private bool isExtending = true;


    private enum RattanState
    {
        None,
        Idle,           // 闲置状态，正常伸缩
        Detected,       // 检测到玩家
        Pulling,        // 拉动玩家
        Releasing       // 释放玩家
    }

    private RattanState currentState = RattanState.None;

    private void Start()
    {
        // 初始化藤条高度
        currentHeight = minHeight;
        rattanSprite.size = new Vector2(rattanSprite.size.x, currentHeight);

        // 初始化目标高度
        targetHeight = maxHeight;
        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(false);
        }
        currentState = RattanState.None;
    }

    private void Update()
    {
        // 根据状态执行不同逻辑
        switch (currentState)
        {
            case RattanState.Idle:
                HandleIdleState();
                break;
            case RattanState.Detected:
                HandleDetectedState();
                break;
            case RattanState.Pulling:

                break;
        }
        UpdateRattanHeight();
        // 只有在闲置状态才检测玩家
        if (currentState == RattanState.Idle)
        {
            CheckForPlayer();
        }
    }
    private void UpdateRattanHeight()
    {
        if (rattanSprite != null)
        {
            rattanSprite.size = new Vector2(rattanSprite.size.x, currentHeight);
        }
    }
    /// <summary>处理闲置状态（正常伸缩）</summary>
    private void HandleIdleState()
    {
        // 计算新的高度
        float speed = normaRattanSpeed * Time.deltaTime;

        if (isExtending)
        {
            currentHeight += speed;
            if (currentHeight >= targetHeight)
            {
                currentHeight = targetHeight;
            }
        }
    }

    public void OnShowModAnimation()
    {
        int index = 0;

        if (GameStatusController.IsDaoPlayer)
            index = 3;
        else if (GameStatusController.IsQiangPlayer)
            index = 4;
        else if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
            index = 2;
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
            index = 1;

        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(i == index);
        }
    }

    /// <summary>检测玩家</summary>
    private void CheckForPlayer()
    {
        // 计算射线长度（藤条当前高度）
        float rayLength = currentHeight;

        // 发射左侧射线
        RaycastHit2D leftHit = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            rayLength,
            playerLayer
        );

        // 绘制调试射线
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);

        // 检查是否检测到玩家
        RaycastHit2D hit = leftHit;

        if (hit.collider != null && hit.collider.gameObject.tag.Contains(playerTag))
        {
            currentState = RattanState.Detected;
        }
    }

    void OnBeginCheck()
    {
        showObj.SetActive(false);
        hideObj.SetActive(true);
        rattanObj.SetActive(true);
        hangObj.SetActive(false);
        currentState = RattanState.Idle;
    }

    /// <summary> 处理检测到玩家状态 </summary>
    private void HandleDetectedState()
    {
        PFunc.Log(PlayerController.Instance.transform.position);
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, false);
        }
        player.SetActive(true);
        OnShowModAnimation();
        rattanObj.SetActive(false);
        hangObj.SetActive(true);
        currentState = RattanState.Pulling;
        OnRotatePlayer();
        Invoke("OnThrowPlayer",0.4f);
    }

    void OnRotatePlayer()
    {
        hangObj. transform.DORotate(-new Vector3(0, 0, 780), 0.75f, RotateMode.LocalAxisAdd).SetEase(Ease.OutQuart)
        .onComplete += () =>
        {
            player.SetActive(false); 
            Invoke("OnClose",2);
        };
    }
    void OnThrowPlayer()
    {
        Vector3 vector = player.transform.position;
        PlayerController.Instance.transform.position = vector;
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(-3f, 1f), 30, 0.5f, true, false);
        }
    }

    public void OnClose()
    {
        rattanObj.SetActive(false);
        hangObj.SetActive(false);
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }
        currentState = RattanState.None;
    }

    bool check = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.tag.Contains("Player") && check)
        {
            check = false;
            OnBeginCheck();
        }
    }
}
