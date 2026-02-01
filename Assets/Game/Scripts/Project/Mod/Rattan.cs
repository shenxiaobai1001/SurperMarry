using PlayerScripts;
using System;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class Rattan : MonoBehaviour
{
    [Header("藤条设置")]
    [SerializeField] private SpriteRenderer rattanSprite;
    [SerializeField] private float minHeight = 1f;
    [SerializeField] private float maxHeight = 10f;
    [SerializeField] private float normalSpeed = 2f;  // 正常伸缩速度
    [SerializeField] private float pullSpeed = 3f;     // 拉回速度
    [SerializeField] private float raycastOffset = 0.1f; // 射线发射位置偏移

    [Header("参考点设置")]
    [SerializeField] private Transform endPoint;      // 藤条末端跟随点
    [SerializeField] private Transform fixedPoint;     // 最终放置点

    [Header("玩家设置")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private LayerMask playerLayer;    // 玩家所在的层
    [SerializeField] private List<GameObject> modAnimations;
    [SerializeField] private GameObject moveObj;

    // 状态变量
    private float currentHeight;
    private float targetHeight;
    private bool isExtending = true;
    private bool isPulling = false;
    private GameObject player;
    private Vector3 rattanTopPosition;  // 藤条顶部位置
    private Vector3 rattanLeftRayStart; // 左侧射线起点
    private Vector3 rattanRightRayStart; // 右侧射线起点

    private enum RattanState
    {
        None,
        Idle,           // 闲置状态，正常伸缩
        Detected,       // 检测到玩家
        Pulling,        // 拉动玩家
        Releasing       // 释放玩家
    }

    private RattanState currentState = RattanState.Idle;

    private void Start()
    {
        if (rattanSprite == null)
        {
            rattanSprite = GetComponent<SpriteRenderer>();
        }

        // 初始化藤条高度
        currentHeight = minHeight;
        rattanSprite.size = new Vector2(rattanSprite.size.x, currentHeight);

        // 初始化目标高度
        targetHeight = maxHeight;

        // 更新射线位置
        UpdateRaycastPositions();
        // 更新末端点位置
        UpdateEndPointPosition();

        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(false);
        }
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
                HandlePullingState();
                break;
        }

        // 更新藤条高度
        UpdateRattanHeight();


        // 只有在闲置状态才检测玩家
        if (currentState == RattanState.Idle)
        {
            CheckForPlayer();
        }
    }

    /// <summary>
    /// 处理闲置状态（正常伸缩）
    /// </summary>
    private void HandleIdleState()
    {
        // 计算新的高度
        float speed = normalSpeed * Time.deltaTime;

        if (isExtending)
        {
            currentHeight += speed;
            if (currentHeight >= targetHeight)
            {
                currentHeight = targetHeight;
                isExtending = false;
            }
        }
        else
        {
            currentHeight -= speed;
            if (currentHeight <= minHeight)
            {
                currentHeight = minHeight;
                isExtending = true;
            }
        }
    }

    /// <summary>
    /// 处理检测到玩家状态
    /// </summary>
    private void HandleDetectedState()
    {
        // 可以在这里添加一些效果，比如震动、变色等
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        PlayerModController.Instance.OnSetModAniIns(false);
        PlayerModController.Instance.OnSetPlayerIns(false);
        PlayerModController.Instance.OnChangeState(false);
        OnShowModAnimation();
        isPulling = true;
        targetHeight = 0.1f;
        currentState = RattanState.Pulling;
    }
    public void OnShowModAnimation( )
    {
        int index = 0;

        if (GameStatusController.IsDaoPlayer)
            index = 3;
        else if (GameStatusController.IsQiangPlayer)
            index = 4;
        else if (GameStatusController.IsFirePlayer&& GameStatusController.IsBigPlayer)
            index = 2;
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
            index = 1;

        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(i == index);
        }
    }

    /// <summary>
    /// 处理拉动状态
    /// </summary>
    private void HandlePullingState()
    {
        // 收缩藤条
        float speed = pullSpeed * Time.deltaTime;
        currentHeight -= speed;

        if (currentHeight <= 0.1f)
        {
            currentHeight = 0.1f;
            ReleasePlayer();
        }
    }

    /// <summary>
    /// 检测玩家
    /// </summary>
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

        //// 发射右侧射线
        //RaycastHit2D rightHit = Physics2D.Raycast(
        //    rattanRightRayStart,
        //    Vector2.down,
        //    rayLength,
        //    playerLayer
        //);

        // 绘制调试射线
        Debug.DrawRay(transform.position, Vector2.down * rayLength, Color.red);
        //Debug.DrawRay(rattanRightRayStart, Vector2.down * rayLength, Color.red);

        // 检查是否检测到玩家
        RaycastHit2D hit = leftHit  ;

        if (hit.collider != null && hit.collider.gameObject.tag.Equals(playerTag))
        {
            player = hit.collider.gameObject;
            currentState = RattanState.Detected;
        }
    }

    /// <summary> 释放玩家</summary>
    private void ReleasePlayer()
    {
        if ( fixedPoint != null)
        {
            // 将玩家放到固定位置
            PlayerController.Instance.transform.position = fixedPoint.position;
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
        currentState = RattanState.None;
        moveObj.SetActive(false);
    }
    /// <summary> 更新藤条高度 </summary>
    private void UpdateRattanHeight()
    {
        if (rattanSprite != null)
        {
            rattanSprite.size = new Vector2(rattanSprite.size.x, currentHeight);
            UpdateEndPointPosition();
        }
    }

    /// <summary>更新末端点位置</summary>
    private void UpdateEndPointPosition()
    {
        if (endPoint != null)
        {
            // 计算藤条顶部位置（基于Sprite的pivot）
            // 假设pivot在底部中心
            float topY = transform.position.y - currentHeight;
            rattanTopPosition = new Vector3(transform.position.x, topY, transform.position.z);
            endPoint.position = rattanTopPosition;
        }
    }

    /// <summary> 更新射线发射位置</summary>
    private void UpdateRaycastPositions()
    {
        // 获取藤条的半宽
        float halfWidth = rattanSprite.size.x / 2f;

        // 计算射线起点（在藤条顶部，左右各偏移一点）
        rattanLeftRayStart = new Vector3(
            transform.position.x - halfWidth + raycastOffset,
                transform.position.y,
            transform.position.z
        );

        rattanRightRayStart = new Vector3(
            transform.position.x + halfWidth - raycastOffset,
          transform.position.y,
            transform.position.z
        );
    }

    /// <summary> 重置藤条 </summary>
    private void ResetRattan()
    {
        currentHeight = minHeight;
        isExtending = true;
        targetHeight = maxHeight;
        // 重置状态
        player = null;
        isPulling = false;
        currentState = RattanState.Idle;
    }

}