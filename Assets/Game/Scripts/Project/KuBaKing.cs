using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KuBaKing : MonoBehaviour
{
    [Header("巡逻设置")]
    public Vector3 pointA;      // 巡逻点A
    public Vector3 pointB;      // 巡逻点B
    public float moveSpeed = 3f; // 移动速度
    public float patrolSwitchDistance = 0.1f; // 切换巡逻点的距离阈值

    [Header("时间设置")]
    public float attackInterval = 5f;     // 攻击间隔（秒）
    public float jumpInterval = 8f;       // 跳跃间隔（秒）
    public float jumpHeight = 1.5f;       // 跳跃高度
    public float jumpDuration = 0.5f;     // 单次跳跃持续时间

    // 私有变量
    private Vector3 _currentTarget;      // 当前巡逻目标
    private Vector3 _originalPosition;   // 原始位置（用于恢复）
    private Coroutine _attackCoroutine;  // 攻击协程
    private Coroutine _jumpCoroutine;    // 跳跃协程
    private bool _isJumping = false;     // 是否正在跳跃

    public Animator animator;

    bool fallling = false;
    private void Start()
    {
        // 保存初始位置
        _originalPosition = transform.position;

        // 设置初始巡逻目标
        _currentTarget = pointA;

        // 开始计时器
        StartTimers();
    }

    private void Update()
    {
        if (fallling) {
            Fallling();
            return;
        }
        if (!_isJumping)
        {
            PatrolMovement();
        }
    }

    void Fallling()
    {
        transform.Translate(Vector3.down*5*Time.deltaTime);
        if (transform.position.y < -5)
        {
            Destroy(gameObject);
        }
    }

    // 巡逻移动
    private void PatrolMovement()
    {
        // 向当前目标点移动
        transform.position = Vector3.MoveTowards(transform.position, _currentTarget, moveSpeed * Time.deltaTime);
        CheckGroundBelow();
        // 检查是否到达目标点
        if (Vector3.Distance(transform.position, _currentTarget) < patrolSwitchDistance)
        {
            // 切换目标点
            _currentTarget = (_currentTarget == pointA) ? pointB : pointA;
        }
    }
    public Transform rayPos;
    // 检查下方是否有地面
    private void CheckGroundBelow()
    {
        // 向下发射射线
        RaycastHit2D hit = Physics2D.Raycast(
            rayPos.position,
            Vector2.down,
            1.5f,
            -1
        );

        // 绘制射线用于调试
        Debug.DrawRay(transform.position, Vector2.down * 1.5f, Color.red);
        // 如果检测到地面
        if (hit.collider == null)
        {
            fallling = true;
        }
    }
    // 开始所有计时器
    private void StartTimers()
    {
        // 启动攻击计时器
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _attackCoroutine = StartCoroutine(AttackTimer());

        // 启动跳跃计时器
        if (_jumpCoroutine != null)
        {
            StopCoroutine(_jumpCoroutine);
        }
        _jumpCoroutine = StartCoroutine(JumpTimer());
    }

    // 攻击计时器
    private IEnumerator AttackTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);
            OnAttack();
        }
    }

    // 跳跃计时器
    private IEnumerator JumpTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(jumpInterval);
            yield return StartCoroutine(PerformJump());
        }
    }

    // 执行跳跃
    private IEnumerator PerformJump()
    {
        _isJumping = true;

        Vector3 startPos = transform.position;
        Vector3 highPos = new Vector3(startPos.x, startPos.y + jumpHeight, startPos.z);
        float elapsedTime = 0f;

        // 上升阶段
        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            transform.position = Vector3.Lerp(startPos, highPos, t);
            yield return null;
        }

        // 下降阶段
        elapsedTime = 0f;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(currentPos.x, _originalPosition.y, currentPos.z);

        while (elapsedTime < jumpDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / jumpDuration;
            transform.position = Vector3.Lerp(currentPos, targetPos, t);
            yield return null;
        }

        _isJumping = false;
    }
    public GameObject fireball;
    public Transform atkPos;
    // 攻击方法（由你实现具体逻辑）
    private void OnAttack()
    {
        // 这里可以调用你的攻击逻辑
        Debug.Log($"{gameObject.name} 发动攻击！");
        Sound.PlaySound("smb_bowserfire");
        animator.SetTrigger("Atk");
        Instantiate(fireball, atkPos.position,Quaternion.identity);
    }

    // 在Scene视图中显示巡逻点
    private void OnDrawGizmosSelected()
    {
        if (pointA != Vector3.zero && pointB != Vector3.zero)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(pointA, 0.2f);
            Gizmos.DrawSphere(pointB, 0.2f);
            Gizmos.DrawLine(pointA, pointB);
        }
    }

    // 公共方法：立即攻击（用于测试）
    public void TriggerAttack()
    {
        OnAttack();
    }

    // 公共方法：立即跳跃（用于测试）
    public void TriggerJump()
    {
        if (!_isJumping)
        {
            StartCoroutine(PerformJump());
        }
    }
}
