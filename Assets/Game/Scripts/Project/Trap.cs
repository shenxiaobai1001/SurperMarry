using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening; // 需要导入DOTween命名空间

public class Trap : MonoBehaviour
{
    [System.Serializable]
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Header("陷阱设置")]
    [SerializeField] private MoveDirection moveDirection = MoveDirection.Up;
    [SerializeField] private float moveDistance = 2f;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private bool enableSpikes = false;
    [SerializeField] private bool isPingPong = false; // 是否来回移动

    [Header("尖刺设置")]
    [SerializeField] private GameObject spikeObject; // 拖入尖刺的GameObject

    [Header("DOTween设置")]
    [SerializeField] private Ease easeType = Ease.Linear; // 移动缓动类型

    private bool isTriggered = false;
    private bool isMoving = false;
    private Vector3 originalPosition;
    private Vector3 targetPosition;
    private Sequence moveSequence; // DOTween序列，用于管理动画

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        CalculateTargetPosition();

        // 初始化尖刺状态
        if (spikeObject != null)
        {
            spikeObject.SetActive(false);
        }
    }

    // 计算目标位置
    private void CalculateTargetPosition()
    {
        Vector3 direction = GetDirectionVector(moveDirection);
        targetPosition = originalPosition + direction * moveDistance;
    }

    // 获取方向向量
    private Vector3 GetDirectionVector(MoveDirection direction)
    {
        switch (direction)
        {
            case MoveDirection.Up:
                return Vector3.up;
            case MoveDirection.Down:
                return Vector3.down;
            case MoveDirection.Left:
                return Vector3.left;
            case MoveDirection.Right:
                return Vector3.right;
            default:
                return Vector3.up;
        }
    }

    // 碰撞检测
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ModData.mTrap)  return;
        if (!isTriggered && !isMoving &&( other.CompareTag("Player")|| other.CompareTag("BigPlayer")))
        {
            TriggerTrap();
        }
    }

    // 也可以使用Collision检测
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ModData.mTrap) return;
        if (!isTriggered && !isMoving && (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("BigPlayer")))
        {
            TriggerTrap();
        }
    }

    // 触发陷阱
    private void TriggerTrap()
    {
        ModData.tiggerTrapCount++;
        isTriggered = true;
        isMoving = true;

        // 启用尖刺
        if (enableSpikes && spikeObject != null)
        {
            spikeObject.SetActive(true);
        }

        // 使用DOTween移动
        MoveWithDOTween();
    }

    // 使用DOTween移动
    private void MoveWithDOTween()
    {
        // 计算移动时间（距离除以速度）
        float moveTime = moveDistance / moveSpeed;

        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }

        moveSequence = DOTween.Sequence();

        if (isPingPong)
        {
            // 来回移动
            moveSequence.Append(transform.DOMove(targetPosition, moveTime)
                .SetEase(easeType))
                .Append(transform.DOMove(originalPosition, moveTime)
                .SetEase(easeType))
                .SetLoops(-1, LoopType.Restart) // 无限循环
                .OnUpdate(() =>
                {
                    // 可以在移动过程中执行一些操作
                })
                .OnComplete(() =>
                {
                    isTriggered = false;
                });
        }
        else
        {
            // 单次移动
            moveSequence.Append(transform.DOMove(targetPosition, moveTime)
                .SetEase(easeType))
                .OnComplete(() =>
                {
                    isMoving = false;
                });
        }
    }

    // 重置陷阱
    public void ResetTrap()
    {
        isTriggered = false;
        isMoving = false;

        // 停止DOTween动画
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }

        // 重置位置
        transform.position = originalPosition;

        if (enableSpikes && spikeObject != null)
        {
            spikeObject.SetActive(false);
        }
    }

    // 当物体被禁用或销毁时
    private void OnDisable()
    {
        if (moveSequence != null && moveSequence.IsActive())
        {
            moveSequence.Kill();
        }
    }

    // 调试用：在Scene视图中显示移动方向和距离
    private void OnDrawGizmosSelected()
    {
        Vector3 startPos = Application.isPlaying ? originalPosition : transform.position;

        Gizmos.color = Color.red;

        if (isPingPong)
        {
            // 绘制来回移动路径
            Vector3 direction = GetDirectionVector(moveDirection);
            Vector3 endPos = startPos + direction * moveDistance;

            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawWireSphere(startPos, 0.2f);
            Gizmos.DrawWireSphere(endPos, 0.2f);

            // 绘制箭头指示来回方向
            DrawArrow(startPos, direction, moveDistance, Color.red);
            DrawArrow(endPos, -direction, moveDistance, Color.red);
        }
        else
        {
            // 绘制单次移动路径
            Vector3 direction = GetDirectionVector(moveDirection);
            Vector3 endPos = startPos + direction * moveDistance;

            Gizmos.DrawLine(startPos, endPos);
            Gizmos.DrawWireSphere(endPos, 0.2f);

            // 绘制箭头指示移动方向
            DrawArrow(startPos, direction, moveDistance, Color.red);
        }
    }

    // 绘制箭头辅助函数
    private void DrawArrow(Vector3 start, Vector3 direction, float length, Color color)
    {
        float arrowHeadLength = 0.5f;
        float arrowHeadAngle = 20f;

        Vector3 end = start + direction * length;
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * Vector3.forward;
        Gizmos.DrawRay(end, right * arrowHeadLength);
        Gizmos.DrawRay(end, left * arrowHeadLength);
    }
}