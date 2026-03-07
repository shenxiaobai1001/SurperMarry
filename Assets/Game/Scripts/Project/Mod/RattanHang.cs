using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class RattanHang : MonoBehaviour
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

    [Header("摆动设置")]
    [SerializeField] private float swingAngle = 25;  // 摆动角度
    [SerializeField] private float swingDuration = 1f; // 单程时间
    [SerializeField] private Ease swingEase = Ease.InOutSine; // 缓动函数
    [SerializeField] private float normalSpeed = 0.25f; // 常态摆动速度
    [SerializeField] private float boostSpeed = 0.5f; // 加速摆动速度
    [SerializeField] private float boostDistance = 60f; // 加速摆动距离
    public Transform LinePos;
    public Transform lastPoint;

    private float currentAngle = 150; // 当前角度(12点为0°)
    private float swingDirection = 1f; // 摆动方向: 1为逆时针(角度增加), -1为顺时针(角度减少)
    private float swingSpeed; // 当前摆动速度
    private float boostRemaining = 0f; // 剩余加速距离
    private float normalRangeMin = 150; // 常态最小角度
    private float normalRangeMax = 210; // 常态最大角度
    private float targetAngle; // 目标角度
    private Tween swingTween;
    // 状态变量
    private float currentHeight;
    private float targetHeight;
    private bool isExtending = true;
    private GameObject player;
    float hangTime;
    float maxHangTime=4;
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
        EventManager.Instance.AddListener(Events.HangSelfByKick, OnKick);
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
        StartSwing();

    }

    void OnKick(object msg)
    {
        if (currentState != RattanState.Pulling)
            return;

        bool toRight = (bool)msg;
        // 判断踢击方向与当前摆动方向是否相反
        // 假设向右踢是顺时针(角度减少，方向-1)
        float kickDirection = toRight ? 1f : -1f;

        if (Mathf.Sign(kickDirection) != Mathf.Sign(swingDirection))
        {
            // 方向相反，改变摆动方向
            swingDirection *= -1f;
        }

        // 开始加速摆荡
        swingSpeed = boostSpeed;
        boostRemaining = boostDistance;
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.HangSelfByKick, OnKick);
    }

    public void StartSwing()
    {
        // 初始化参数
        currentAngle = 150; // 从120°开始
        swingDirection = 1f; // 初始逆时针摆动(120° -> 240°)
        swingSpeed = normalSpeed;
        boostRemaining = 0f;
        targetAngle = normalRangeMax; // 初始目标240°

        // 设置初始旋转
        LinePos.localEulerAngles = new Vector3(0, 0, currentAngle);
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
                UpdateSwing();
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
        //Debug.DrawRay(rattanRightRayStart, Vector2.down * rayLength, Color.red);

        // 检查是否检测到玩家
        RaycastHit2D hit = leftHit;

        if (hit.collider != null && hit.collider.gameObject.tag.Contains(playerTag))
        {
            player = hit.collider.gameObject;
            currentState = RattanState.Detected;
        }
    }


    /// <summary> 更新藤条高度 </summary>
    private void UpdateRattanHeight()
    {
        if (rattanSprite != null)
        {
            rattanSprite.size = new Vector2(rattanSprite.size.x, currentHeight);
        }
    }


    private void UpdateSwing()
    {
        if (BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            OnClose();
        }
        hangTime += Time.deltaTime;
        if(hangTime>=maxHangTime)
        {
            OnClose();
        }
        // 计算本次帧的角度变化
        float angleDelta = swingSpeed * swingDirection * Time.deltaTime * 120f; // 根据swingDuration=1s计算速度

        if (boostRemaining > 0f)
        {
            // 加速摆荡状态
            float actualDelta = Mathf.Min(angleDelta, boostRemaining);
            currentAngle += actualDelta;
            boostRemaining -= Mathf.Abs(actualDelta);

            if (boostRemaining <= 0f)
            {
                swingSpeed = normalSpeed;
            }
        }
        else
        {
            currentAngle += angleDelta;

            float normalizedAngle = NormalizeAngle(currentAngle);

            if (normalizedAngle <= normalRangeMin || normalizedAngle >= normalRangeMax)
            {
                swingDirection *= -1f;

                if (normalizedAngle <= normalRangeMin)
                    currentAngle = normalRangeMin;
                else if (normalizedAngle >= normalRangeMax)
                    currentAngle = normalRangeMax;
            }
        }
        Vector3 vector = lastPoint.transform.position;
        PlayerController.Instance.transform.position = vector;
        // 应用旋转
        LinePos.localEulerAngles = new Vector3(0, 0, currentAngle);
    }
    private float NormalizeAngle(float angle)
    {
        // 将角度归一化到0-360度
        angle = angle % 360f;
        if (angle < 0) angle += 360f;
        return angle;
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
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, false);
        }
        Sound.PlaySound("Mod/hangself");
        OnShowModAnimation();
        rattanObj.SetActive(false);
        hangObj.SetActive(true);
        Config.isHang = true;
        currentState = RattanState.Pulling;
    }
    public void OnClose()
    {
        rattanObj.SetActive(false);
        hangObj.SetActive(false);
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
            PlayerController.Instance.OnHalfDieFunc();
        }
        Config.isHang = false;
        currentState = RattanState.None;
    }
    bool check = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.tag.Contains("Player")&& check)
        {
            check = false;
            OnBeginCheck();
        }
    }
}
