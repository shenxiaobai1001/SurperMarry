using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageHangSelf : BarrageFuncBase
{
    public Transform LinePos;
    public Transform lastPoint;
    public Animator animator;

    [Header("摆动设置")]
    [SerializeField] private float swingAngle = 25;  // 摆动角度
    [SerializeField] private float swingDuration = 1f; // 单程时间
    [SerializeField] private Ease swingEase = Ease.InOutSine; // 缓动函数
    [SerializeField] private float normalSpeed = 0.25f; // 常态摆动速度
    [SerializeField] private float boostSpeed = 0.5f; // 加速摆动速度
    [SerializeField] private float boostDistance = 60f; // 加速摆动距离

    private float currentAngle = 120f; // 当前角度(12点为0°)
    private float swingDirection = 1f; // 摆动方向: 1为逆时针(角度增加), -1为顺时针(角度减少)
    private float swingSpeed; // 当前摆动速度
    private float boostRemaining = 0f; // 剩余加速距离
    private float normalRangeMin = 120f; // 常态最小角度
    private float normalRangeMax = 240f; // 常态最大角度
    private float targetAngle; // 目标角度
    private Tween swingTween;

    private void Awake()
    {
        EventManager.Instance.AddListener(Events.HangSelfByKick, OnKick);
    }
    float time = 0;
    float allTime = 4.5f;

    public override void OnStart(BarrageValue barrageFuncData, int index)
    {
        base.OnStart(barrageFuncData, index);
        if (!OnCheckHasLevel())
        {
            PlayerModController.Instance.OnSetPlayerContro(false, false, true);
        }
        if (lastPoint != null) lastPoint.gameObject.SetActive(true);
        Invoke("OnReadyDes", 4.5f);
        StartSwing();
        if (animator) animator.gameObject.SetActive(true);
        barrageData.BarrageState = BarrageState.Underway;
    }

    void OnKick(object msg)
    {
        if (barrageData.BarrageState != BarrageState.Ready 
            && barrageData.BarrageState != BarrageState.Underway) 
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

    public void StartSwing()
    {
        // 初始化参数
        currentAngle = 120f; // 从120°开始
        swingDirection = 1f; // 初始逆时针摆动(120° -> 240°)
        swingSpeed = normalSpeed;
        boostRemaining = 0f;
        targetAngle = normalRangeMax; // 初始目标240°

        // 设置初始旋转
        LinePos.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    private void Update()
    {
        switch (barrageData.BarrageState)
        {
            case BarrageState.Tigger:
                break;
            case BarrageState.Ready:
            case BarrageState.Underway:
                UpdateSwing();      // 更新摆动
                Vector3 vector = lastPoint.transform.position;
                PlayerController.Instance.transform.position = vector;
                if (OnCheckHasLevel())
                    OnPause();
                break;
            case BarrageState.Pause:
                if (!OnCheckHasLevel())
                    OnContinue();
                break;
        }
    }

    private void UpdateSwing()
    {
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
                // 加速结束
                swingSpeed = normalSpeed;
            }
        }
        else
        {
            // 常态摆荡
            currentAngle += angleDelta;

            // 角度归一化到0-360
            float normalizedAngle = NormalizeAngle(currentAngle);

            // 检查是否到达边界
            if (normalizedAngle <= normalRangeMin || normalizedAngle >= normalRangeMax)
            {
                // 到达边界，反转方向
                swingDirection *= -1f;

                // 确保角度不超出范围
                if (normalizedAngle <= normalRangeMin)
                    currentAngle = normalRangeMin;
                else if (normalizedAngle >= normalRangeMax)
                    currentAngle = normalRangeMax;
            }
        }

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

    public override void OnPause()
    {
        base.OnPause();
        if (animator) animator.gameObject.SetActive(false);
    }

    public override void OnContinue()
    {
        base.OnContinue();
        transform.position = BarrageFuncCreater.Instance.OnCreatePos("上吊");
        if (animator) animator.gameObject.SetActive(true);
    }

    public void OnBreakeHang()
    {
        if (lastPoint != null) lastPoint.gameObject.SetActive(false);
    }

    public override void OnClose()
    {
        base.OnClose();
        SimplePool.Despawn(gameObject);
    }

    void OnReadyDes()
    {
        barrageData.BarrageState = BarrageState.Finsh;
        OnBreakeHang();
        if (!barrageController.OnCheckHasHighControl()
         && !OnCheckHasLevel()
         && !BarrageFuncController.Instance.OnCheckHighLevelFunc(barrageData.barrageFuncData)
         && !GameStatusController.isDead)
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
            Vector3 vector = lastPoint.transform.position;
            if (vector != Vector3.zero)
            {
                PlayerController.Instance.transform.position = vector;
            }
            PlayerController.Instance.OnHalfDieFunc();
            Invoke("OnClose", 1.6f);
        }
        else
        {
            OnClose();
        }
    }
}