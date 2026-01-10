using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    [Header("震动参数")]
    [SerializeField] private float shakeInterval = 0.2f;    // 震动间隔（秒）
    [SerializeField] private float shakeDuration = 2.0f;    // 单次震动持续时间（秒）
    [SerializeField] private Vector3 shakeStrength = new Vector3(0, 1.2f, 0); // 震动强度
    [SerializeField] private int shakeVibrato = 0;          // 震动频率
    [SerializeField] private float shakeRandomness = 0f;   // 随机性

    [Header("调试信息")]
    [SerializeField] private bool isShaking = false;        // 当前是否在震动
    [SerializeField] private float remainingTime = 0f;     // 剩余震动时间
    [SerializeField] private GameObject snakeObj;     // 剩余震动时间
    private Coroutine shakeCoroutine;
    private float currentShakeTime = 0f;

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

    /// <summary>
    /// 开始震动摄像头
    /// </summary>
    /// <param name="customDuration">可选的自定义震动总时间</param>
    public void StartShake(float customDuration = 0.2f)
    {
        // 如果已经有震动在进行，则延长震动时间
        if (isShaking)
        {
            if(remainingTime<customDuration)
            remainingTime += customDuration - remainingTime;
            return;
        }

        remainingTime += customDuration;
        isShaking = true;
        PerformSingleShake();
        // 启动震动协程
        if (shakeCoroutine != null)
        {
            StopCoroutine(shakeCoroutine);
        }
        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }


    /// <summary>
    /// 震动协程
    /// </summary>
    private IEnumerator ShakeRoutine()
    {
        while (remainingTime > 0)
        {
            // 等待间隔时间
            yield return new WaitForSeconds(shakeInterval);
            // 执行一次震动
            PerformSingleShake();

            remainingTime -= shakeInterval;
        }
        isShaking = false;
        shakeCoroutine = null;
        remainingTime = 0;
    }

    /// <summary>
    /// 执行单次震动
    /// </summary>
    public void PerformSingleShake()
    {
        if (snakeObj == null)
        {
            snakeObj = Camera.main.transform.parent.gameObject;
        }
        if (snakeObj != null)
        {
            snakeObj.transform.DOKill(); // 停止之前的震动避免叠加
            snakeObj.transform.DOShakePosition(
                0.2f,
                shakeStrength,
                shakeVibrato,
                shakeRandomness,
                false
            );
        }
    }

    /// <summary>立即停止震动</summary>
    public void StopShake()
    {
        if (isShaking)
        {
            isShaking = false;
            remainingTime = 0f;

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
                shakeCoroutine = null;
            }
        }
    }

    private void OnDestroy()
    {
        if (isShaking)
        {
            StopShake();
        }
    }
}
