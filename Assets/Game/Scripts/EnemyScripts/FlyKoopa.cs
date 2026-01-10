using System.Collections;
using UnityEngine;

public class FlyKoopa : MonoBehaviour
{
    [Header("飞行设置")]
    public float cycleTime = 2f;           // 一个完整上下循环的时间
    public float jumpHeight = 3f;         // 跳跃高度
    public AnimationCurve motionCurve;    // 运动曲线

    [Header("状态")]
    public bool canFly = true;
    public bool isFlying = false;

    public float originalY;
    private Coroutine flyCoroutine;
    private float currentCycleTime = 0f;
    private bool isRising = true;  // 标记当前是上升还是下降

    private void Start()
    {
        // 如果没有设置曲线，使用正弦波作为默认
        if (motionCurve == null || motionCurve.keys.Length == 0)
        {
            motionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        }
    }

    public void OnStartFly()
    {
        canFly = true;
        StartFlying();
    }

    public void StartFlying()
    {
        if (isFlying) return;

        canFly = true;
        isFlying = true;
        flyCoroutine = StartCoroutine(ContinuousFlyRoutine());
    }

    public void StopFlying()
    {
        canFly = false;
        isFlying = false;

        if (flyCoroutine != null)
        {
            StopCoroutine(flyCoroutine);
            flyCoroutine = null;
        }

        // 平滑回到起始位置
        StartCoroutine(SmoothReturnToOriginal());
    }

    IEnumerator ContinuousFlyRoutine()
    {
        // 记录开始位置
        Vector3 startPos = transform.position;
        originalY = startPos.y;
        currentCycleTime = 0f;
        isRising = true;

        while (canFly)
        {
            // 更新当前周期时间
            currentCycleTime += Time.deltaTime;

            // 如果超过一个周期，重置
            if (currentCycleTime > cycleTime)
            {
                currentCycleTime -= cycleTime;
                isRising = true;  // 开始新的上升周期
            }

            // 计算当前是上升还是下降阶段
            float halfCycle = cycleTime / 2f;
            if (currentCycleTime > halfCycle)
            {
                isRising = false;  // 进入下降阶段
            }

            // 计算归一化时间 (0到1)
            float normalizedTime = 0f;
            if (isRising)
            {
                // 上升阶段：0到0.5映射到0到1
                normalizedTime = currentCycleTime / halfCycle;
            }
            else
            {
                // 下降阶段：0.5到1映射到1到0
                normalizedTime = 2f - (currentCycleTime / halfCycle);
            }

            // 使用曲线控制运动
            float curveValue = motionCurve.Evaluate(Mathf.Clamp01(normalizedTime));

            // 计算新的Y位置
            float newY = originalY + (curveValue * jumpHeight);

            // 更新位置
            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );

            yield return null;
        }
    }

    IEnumerator SmoothReturnToOriginal()
    {
        float duration = 0.5f;
        float elapsed = 0f;
        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(startPos.x, originalY, startPos.z);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 使用缓动函数
            t = Mathf.SmoothStep(0, 1, t);

            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // 确保精确位置
        transform.position = targetPos;
    }

    // 可选：使用正弦波实现更平滑的乒乓运动
    IEnumerator SineWaveFlyRoutine()
    {
        float timer = 0f;
        Vector3 startPos = transform.position;
        originalY = startPos.y;

        while (canFly)
        {
            timer += Time.deltaTime;

            // 使用正弦波计算Y位置偏移
            // Mathf.Sin 的周期是 2π，所以除以π来匹配cycleTime
            float sineValue = Mathf.Sin((timer / cycleTime) * Mathf.PI * 2f);

            // 将正弦值从[-1, 1]映射到[0, 1]
            float normalizedValue = (sineValue + 1f) / 2f;

            // 应用曲线
            float curveValue = motionCurve.Evaluate(normalizedValue);

            // 计算新位置
            float newY = originalY + (curveValue * jumpHeight);

            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );

            yield return null;
        }
    }

    // 如果要使用正弦波版本，可以将StartFlying中的协程改为SineWaveFlyRoutine

    // 在Scene视图中显示辅助线
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        // 在编辑模式下显示
        Vector3 currentPos = transform.position;
        float displayY = Application.isPlaying ? originalY : transform.position.y;

        Vector3 originalPos = new Vector3(currentPos.x, displayY, currentPos.z);
        Vector3 maxPos = new Vector3(currentPos.x, displayY + jumpHeight, currentPos.z);

        Gizmos.DrawWireSphere(originalPos, 0.3f);
        Gizmos.DrawWireSphere(maxPos, 0.3f);
        Gizmos.DrawLine(originalPos, maxPos);

        // 显示运动路径
        Gizmos.color = new Color(1, 1, 0, 0.3f);
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            float t1 = (float)i / segments;
            float t2 = (float)(i + 1) / segments;

            float y1 = displayY + (motionCurve.Evaluate(t1) * jumpHeight);
            float y2 = displayY + (motionCurve.Evaluate(t2) * jumpHeight);

            Vector3 pos1 = new Vector3(currentPos.x - 0.5f, y1, currentPos.z);
            Vector3 pos2 = new Vector3(currentPos.x - 0.5f, y2, currentPos.z);

            Gizmos.DrawLine(pos1, pos2);
        }
    }

    // 公开方法，可以在其他脚本中调用
    public void ToggleFlying()
    {
        if (isFlying)
        {
            StopFlying();
        }
        else
        {
            StartFlying();
        }
    }

    public void SetParameters(float newHeight, float newCycleTime)
    {
        jumpHeight = newHeight;
        cycleTime = Mathf.Max(0.1f, newCycleTime);  // 防止时间为0
    }
}