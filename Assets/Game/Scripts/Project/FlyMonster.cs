using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyMonster : MonoBehaviour
{
    [Header("追踪设置")]
    public Transform player;              // 玩家Transform
    public float normalSpeed = 3f;        // 正常追踪速度
    public float chaseSpeed = 6f;         // 加速追踪速度
    public float chaseDistance = 6f;      // 加速追击距离阈值
    public float attackInterval = 3f;     // 攻击间隔（秒）
    public float stopDistance = 1.5f;     // 停止距离（与玩家的最小距离）

    [Header("组件引用")]
    public Rigidbody2D rb;                // 刚体组件
    public SpriteRenderer spriteRenderer; // 精灵渲染器（用于翻转）

    [Header("调试")]
    public float currentSpeed;            // 当前速度
    public float distanceToPlayer;        // 与玩家的距离
    public bool isChasing = false;        // 是否正在追击
    public bool isAttacking = false;      // 是否正在攻击

    private Vector3 _originalPosition;    // 初始位置
    private Coroutine _attackCoroutine;   // 攻击协程
    private float _lastAttackTime;        // 上次攻击时间

    private void Start()
    {
        // 初始化组件
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // 保存初始位置
        _originalPosition = transform.position;

        // 如果玩家未指定，尝试查找玩家
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }

        // 开始攻击计时器
        StartAttackTimer();
    }

    private void Update()
    {
        if (player == null) return;

        // 计算与玩家的X轴距离
        distanceToPlayer = Mathf.Abs(player.position.x - transform.position.x);

        // 更新追击状态
        UpdateChaseState();

        // 更新朝向
        UpdateFacing();

    }

    private void FixedUpdate()
    {
        if (player == null || !isChasing) return;

        // 移动怪物
        MoveTowardsPlayer();
    }

    // 更新追击状态
    private void UpdateChaseState()
    {
        if (player == null) return;

        // 始终追击玩家
        isChasing = true;

        // 根据距离调整速度
        if (distanceToPlayer > chaseDistance)
        {
            currentSpeed = chaseSpeed;  // 距离远，加速
        }
        else
        {
            currentSpeed = normalSpeed; // 距离近，正常速度
        }
    }

    // 向玩家移动
    private void MoveTowardsPlayer()
    {
        if (player == null || isAttacking) return;

        // 如果距离大于停止距离，才移动
        if (distanceToPlayer > stopDistance)
        {
            // 计算移动方向（仅X轴）
            float moveDirection = Mathf.Sign(player.position.x - transform.position.x);

            // 移动怪物
            Vector2 velocity = rb.velocity;
            velocity.x = moveDirection * currentSpeed;
            rb.velocity = velocity;
        }
        else
        {
            // 到达停止距离，停止移动
            Vector2 velocity = rb.velocity;
            velocity.x = 0;
            rb.velocity = velocity;
        }
    }

    // 更新怪物朝向
    private void UpdateFacing()
    {
        if (player == null || spriteRenderer == null) return;

        // 玩家在右边，面朝右
        if (player.position.x > transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        // 玩家在左边，面朝左
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    // 开始攻击计时器
    private void StartAttackTimer()
    {
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
        }
        _attackCoroutine = StartCoroutine(AttackTimer());
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
    public GameObject chongzi;
    // 攻击方法（由你实现具体逻辑）
    private void OnAttack()
    {
        Debug.Log($"{gameObject.name} 对玩家发动攻击！");
        bool isRight = player.position.x > transform.position.x;
        Vector3 createPos = isRight ? new Vector3(transform.position.x+2, transform.position.y, transform.position.z)
            : new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);

        Instantiate(chongzi,createPos, Quaternion.identity);
    }

    // 设置目标玩家
    public void SetTarget(Transform target)
    {
        player = target;
    }

    // 开始追击
    public void StartChasing()
    {
        isChasing = true;
        StartAttackTimer();
    }

    // 停止追击
    public void StopChasing()
    {
        isChasing = false;
        isAttacking = false;

        // 停止移动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        // 停止攻击计时器
        if (_attackCoroutine != null)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }

    // 返回初始位置
    public void ReturnToOriginalPosition()
    {
        StartCoroutine(MoveToOriginalPosition());
    }

    // 移动回初始位置
    private IEnumerator MoveToOriginalPosition()
    {
        isChasing = false;

        while (Vector3.Distance(transform.position, _originalPosition) > 0.1f)
        {
            Vector3 direction = (_originalPosition - transform.position).normalized;
            rb.velocity = new Vector2(direction.x * normalSpeed, rb.velocity.y);
            yield return null;
        }

        rb.velocity = Vector2.zero;
    }


    // 公共方法：设置追击参数
    public void SetChaseParameters(float newNormalSpeed, float newChaseSpeed, float newChaseDistance)
    {
        normalSpeed = newNormalSpeed;
        chaseSpeed = newChaseSpeed;
        chaseDistance = newChaseDistance;
    }
}
