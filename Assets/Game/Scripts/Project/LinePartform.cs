using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinePartform : MonoBehaviour
{
    public Transform leftPart;
    public Transform rightPart;
    public SpriteRenderer leftLine;
    public SpriteRenderer rightLine;

    [Header("平衡参数")]
    public float moveSpeed = 2f; // 移动速度
    public float maxMovement = 0.5f; // 最大移动距离
    public float recoverySpeed = 1f; // 恢复平衡速度

    [Header("掉落参数")]
    public float fallForce = 5f; // 掉落时的力

    // 当前平衡状态 (-1到1, 0为平衡)
    private float currentBalance = 0f;
    // 马里奥站在哪个平台 (-1:左侧, 0:无, 1:右侧)
    private int marioOnPlatform = 0;
    // 是否已经掉落
    private bool isFalling = false;

    // 初始位置
    private Vector3 leftInitialPos;
    private Vector3 rightInitialPos;
    // 初始旋转
    private Quaternion leftInitialRot;
    private Quaternion rightInitialRot;

    // 绳索初始高度
    private float lineInitialHeight = 3f;

    // 组件引用
    private Rigidbody2D leftRb;
    private Rigidbody2D rightRb;

    void Start()
    {
        EventManager.Instance.AddListener(Events.OnRestBreakBrick, ResetToInitialState);
        RecordInitialState();
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnRestBreakBrick, ResetToInitialState);
    }

    /// <summary>
    /// 记录初始状态
    /// </summary>
    private void RecordInitialState()
    {
        // 记录初始位置和旋转
        leftInitialPos = leftPart.position;
        rightInitialPos = rightPart.position;
        leftInitialRot = leftPart.rotation;
        rightInitialRot = rightPart.rotation;

        // 记录绳索初始高度
        if (leftLine != null)
        {
            lineInitialHeight = leftLine.size.y;
        }

        // 获取或添加刚体组件
        leftRb = GetOrAddRigidbody(leftPart);
        rightRb = GetOrAddRigidbody(rightPart);
    }

    private Rigidbody2D GetOrAddRigidbody(Transform part)
    {
        Rigidbody2D rb = part.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = part.gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Kinematic;
        return rb;
    }

    void Update()
    {
        if (isFalling) return;

        // 根据马里奥的位置更新平衡状态
        UpdateBalance();

        // 更新平台位置
        UpdatePlatformPositions();

        // 更新绳索
        UpdateRopes();

        // 检查是否过度倾斜
        if (Mathf.Abs(currentBalance) >= 1f && !isFalling)
        {
            StartFalling();
        }
    }

    void UpdateBalance()
    {
        if (marioOnPlatform != 0)
        {
            // 马里奥站在平台上，改变平衡
            float targetBalance = marioOnPlatform; // -1 或 1
            currentBalance = Mathf.MoveTowards(currentBalance, targetBalance, moveSpeed * Time.deltaTime);
        }
    }

    void UpdatePlatformPositions()
    {
        // 使用Lerp平滑移动
        leftPart.position = Vector3.Lerp(leftPart.position, leftInitialPos + new Vector3(0, -currentBalance * maxMovement, 0), recoverySpeed * Time.deltaTime);

        rightPart.position = Vector3.Lerp(rightPart.position, rightInitialPos + new Vector3(0, currentBalance * maxMovement, 0), recoverySpeed * Time.deltaTime);
    }

    void UpdateRopes()
    {
        if (leftLine != null && rightLine != null)
        {
            // 计算绳索高度
            float leftLineHeight = Mathf.Clamp(lineInitialHeight + currentBalance * 3f, 0f, 6f);
            float rightLineHeight = Mathf.Clamp(lineInitialHeight - currentBalance * 3f, 0f, 6f);

            // 更新绳索尺寸
            leftLine.size = new Vector2(leftLine.size.x, leftLineHeight);
            rightLine.size = new Vector2(rightLine.size.x, rightLineHeight);
        }
    }

    void StartFalling()
    {
        isFalling = true;

        // 切换到动态刚体
        leftRb.isKinematic =false;
        rightRb.isKinematic = false;

        // 施加掉落力
        //Vector2 fallDirection = new Vector2(Random.Range(-0.3f, 0.3f), -1f).normalized;
        //leftRb.AddForce(fallDirection * fallForce, ForceMode2D.Impulse);
        //rightRb.AddForce(new Vector2(-fallDirection.x, -1f).normalized * fallForce, ForceMode2D.Impulse);

        //// 添加旋转
        //leftRb.AddTorque(Random.Range(-5f, 5f));
        //rightRb.AddTorque(Random.Range(-5f, 5f));

        // 禁用脚本
        //enabled = false;
    }

    /// <summary>
    /// 重置到初始状态
    /// </summary>
    public void ResetToInitialState(object msg)
    {
        // 1. 重置状态变量
        currentBalance = 0f;
        marioOnPlatform = 0;
        isFalling = false;

        // 3. 重置刚体属性
        ResetRigidbody(leftRb, leftInitialPos, leftInitialRot);
        ResetRigidbody(rightRb, rightInitialPos, rightInitialRot);

        // 4. 立即更新位置和旋转
        leftPart.position = leftInitialPos;
        rightPart.position = rightInitialPos;
        leftPart.rotation = leftInitialRot;
        rightPart.rotation = rightInitialRot;

        // 5. 重置绳索
        ResetRopes();

        // 6. 停止所有正在进行的协程（如果有的话）
        StopAllCoroutines();
    }

    /// <summary>
    /// 重置刚体到初始状态
    /// </summary>
    private void ResetRigidbody(Rigidbody2D rb, Vector3 targetPosition, Quaternion targetRotation)
    {
        if (rb == null) return;


        // 切换回运动学刚体
        rb.bodyType = RigidbodyType2D.Kinematic;

        // 重置所有物理属性
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // 重置位置和旋转
        rb.transform.position = targetPosition;
        rb.transform.rotation = targetRotation;
        rb.gameObject.SetActive(true);
        // 重置质量、阻力等（如果有特殊设置的话）
        // rb.mass = 1f;
        // rb.drag = 0f;
        // rb.angularDrag = 0.05f;
    }

    /// <summary>
    /// 重置绳索到初始状态
    /// </summary>
    private void ResetRopes()
    {
        if (leftLine != null)
        {
            leftLine.size = new Vector2(leftLine.size.x, lineInitialHeight);
        }

        if (rightLine != null)
        {
            rightLine.size = new Vector2(rightLine.size.x, lineInitialHeight);
        }
    }

    // 触发检测方法保持不变
    public void OnMarioEnterLeftPlatform()
    {
        marioOnPlatform = -1;
    }

    public void OnMarioExitLeftPlatform()
    {
        if (marioOnPlatform == -1)
            marioOnPlatform = 0;
    }

    public void OnMarioEnterRightPlatform()
    {
        marioOnPlatform = 1;
    }

    public void OnMarioExitRightPlatform()
    {
        if (marioOnPlatform == 1)
            marioOnPlatform = 0;
    }

    // 用于调试
    public void SetDebugBalance(float balance)
    {
        currentBalance = Mathf.Clamp(balance, -1f, 1f);
    }
}