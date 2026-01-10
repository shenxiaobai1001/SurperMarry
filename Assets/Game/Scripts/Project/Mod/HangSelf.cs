using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangSelf : MonoBehaviour
{
    public static HangSelf Instance;
    public Transform LinePos;
    public Transform lastPoint;
    public Animator animator;

    [Header("摆动设置")]
    [SerializeField] private float swingAngle = 25;  // 摆动角度
    [SerializeField] private float swingDuration = 1f; // 单程时间
    [SerializeField] private Ease swingEase = Ease.InOutSine; // 缓动函数
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
    private Tween swingTween;
    private void OnEnable()
    {
        StartSwing();
        Invoke("OnReadyDes",4.5f);
    }

    public void StartSwing()
    {
        if (lastPoint != null) lastPoint.gameObject.SetActive(true);
        // 停止已有的摆动
        if (swingTween != null && swingTween.IsActive()) swingTween.Kill();

        // 重置旋转
        LinePos.localEulerAngles = new Vector3(0, 0, -swingAngle);

        // 创建摆动动画
        swingTween = LinePos.DOLocalRotate(
            new Vector3(0, 0, swingAngle),  // 目标角度
            swingDuration,                   // 持续时间
            RotateMode.Fast                  // 旋转模式
        )
        .SetEase(swingEase)                  // 设置缓动
        .SetLoops(-1, LoopType.Yoyo)         // 无限循环，Yoyo模式
        .SetAutoKill(false); // 可选：完成回调
    }

    public void OnBreakeHang()
    {
        ItemCreater.Instance.isHang = false;
        if (swingTween != null && swingTween.IsActive()) swingTween.Kill();
        if (lastPoint != null) lastPoint.gameObject.SetActive(false);
    }
    void OnReadyDes()
    {
        if (ItemCreater.Instance.isHang)
        {
            PlayerController.Instance.isHit = false;
            ItemCreater.Instance.isHang = false;
            OnBreakeHang();
            PlayerModController.Instance.OnCancelHangSelf();
        }
        SimplePool.Despawn(gameObject);
    }
}
