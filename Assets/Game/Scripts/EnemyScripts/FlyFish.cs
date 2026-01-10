using EnemyScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyFish : MonoBehaviour
{
    [System.Serializable]
    public enum FlightDirection
    {
        Left,
        Right
    }

    [Header("飞行参数")]
    [SerializeField] private FlightDirection flightDirection = FlightDirection.Left; // 飞行方向
    [SerializeField] private float flightSpeed = 5f; // 水平飞行速度
    public float maxHeight = 3f;   // 最高点高度
    [SerializeField] private float horizontalDistance = 10f; // 水平飞行总距离
    [SerializeField] private AnimationCurve verticalCurve; // 垂直高度曲线
    public SpriteRenderer spriteRenderer;

    [Header("调试")]
    [SerializeField] private bool drawTrajectory = true;
    [SerializeField] private Color trajectoryColor = Color.cyan;

    private Coroutine flightCoroutine;
    private Vector3 initialPosition;
    public EnemyController enemyController;

    void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }

    /// <summary>
    /// 开始飞行
    /// </summary>
    public void StartFlight()
    {
        initialPosition = transform.position;
        if (flightCoroutine != null)
        {
            StopCoroutine(flightCoroutine);
        }
        flightCoroutine = StartCoroutine(FlightRoutine());
    }

    /// <summary>飞行协程 </summary>
    private IEnumerator FlightRoutine()
    {
        // 重置位置
        transform.position = initialPosition;

        // 计算飞行参数
        float flightDuration = horizontalDistance / flightSpeed;

        // 记录开始位置
        Vector3 startPos = transform.position;
        float elapsedTime = 0f;

        // 计算水平移动的方向
        int horizontalDirection = flightDirection == FlightDirection.Left ? -1 : 1;
        spriteRenderer.flipX = flightDirection != FlightDirection.Left;
        // 方法1：使用二次函数抛物线
        while (elapsedTime < flightDuration)
        {
            // 计算水平位移
            float t = elapsedTime / flightDuration;
            float x = startPos.x + (t * horizontalDistance * horizontalDirection);

            // 使用抛物线方程：y = -a(x - 0.5)² + 1，再乘以maxHeight
            // 这个方程在t=0.5时达到最大值1
            float normalizedX = (t - 0.5f) * 2f; // 转换为-1到1的范围
            float parabola = -normalizedX * normalizedX + 1; // 抛物线形状
            float y = startPos.y + parabola * maxHeight;

            transform.position = new Vector3(x, y, startPos.z);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保到达终点
        Vector3 finalPosition = new Vector3(
            startPos.x + horizontalDistance * horizontalDirection,
            startPos.y,
            startPos.z
        );
        transform.position = finalPosition;

        if (enemyController != null)
        {
            enemyController.Die();
        }
    }

    /// <summary>
    /// 计算并绘制轨迹
    /// </summary>
    private void OnDrawGizmos()
    {
        if (!drawTrajectory)
            return;

        Gizmos.color = trajectoryColor;

        int segments = 50;
        Vector3 startPos = Application.isPlaying ? initialPosition : transform.position;

        // 计算水平移动的方向
        int horizontalDirection = flightDirection == FlightDirection.Left ? -1 : 1;

        for (int i = 0; i < segments; i++)
        {
            float t1 = (float)i / segments;
            float t2 = (float)(i + 1) / segments;

            // 计算水平位置
            float x1 = startPos.x + (t1 * horizontalDistance * horizontalDirection);
            float x2 = startPos.x + (t2 * horizontalDistance * horizontalDirection);

            // 使用抛物线方程计算垂直位置
            float normalizedX1 = (t1 - 0.5f) * 2f;
            float parabola1 = -normalizedX1 * normalizedX1 + 1;
            float y1 = startPos.y + parabola1 * maxHeight;

            float normalizedX2 = (t2 - 0.5f) * 2f;
            float parabola2 = -normalizedX2 * normalizedX2 + 1;
            float y2 = startPos.y + parabola2 * maxHeight;

            Vector3 point1 = new Vector3(x1, y1, startPos.z);
            Vector3 point2 = new Vector3(x2, y2, startPos.z);

            Gizmos.DrawLine(point1, point2);
        }

        // 标记起点和终点
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPos, 0.3f);

        Vector3 endPos = new Vector3(
            startPos.x + horizontalDistance * horizontalDirection,
            startPos.y,
            startPos.z
        );
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPos, 0.3f);

        // 绘制方向箭头
        DrawDirectionArrow(startPos, horizontalDirection, Color.yellow);
    }

    /// <summary>
    /// 绘制方向箭头
    /// </summary>
    private void DrawDirectionArrow(Vector3 startPos, int direction, Color color)
    {
        Vector3 directionVector = new Vector3(direction, 0, 0);
        Vector3 arrowStart = startPos + directionVector * 1.0f;
        Vector3 arrowEnd = arrowStart + directionVector * 1.0f;

        Gizmos.color = color;
        Gizmos.DrawLine(arrowStart, arrowEnd);

        // 绘制箭头头部
        float arrowHeadLength = 0.5f;
        float arrowHeadAngle = 20f;

        Vector3 right = Quaternion.LookRotation(directionVector) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(directionVector) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;

        Gizmos.DrawRay(arrowEnd, right * arrowHeadLength);
        Gizmos.DrawRay(arrowEnd, left * arrowHeadLength);
    }

    /// <summary>
    /// 重新开始飞行
    /// </summary>
    public void RestartFlight()
    {
        StartFlight();
    }

    /// <summary>
    /// 停止飞行
    /// </summary>
    public void StopFlight()
    {
        if (flightCoroutine != null)
        {
            StopCoroutine(flightCoroutine);
            flightCoroutine = null;
        }
    }

    /// <summary>
    /// 设置飞行参数
    /// </summary>
    public void SetFlightParameters(float height, FlightDirection direction = FlightDirection.Left)
    {
        maxHeight = height;
        flightDirection = direction;
        StartFlight();
    }

    /// <summary>
    /// 设置飞行方向
    /// </summary>
    public void SetFlightDirection(FlightDirection direction)
    {
        flightDirection = direction;
    }

    /// <summary>
    /// 获取飞行状态
    /// </summary>
    public bool IsFlying()
    {
        return flightCoroutine != null;
    }
}