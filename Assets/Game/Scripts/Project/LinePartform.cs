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

    // 绳索初始高度
    private float lineInitialHeight = 3f;

    // 组件引用
    private Rigidbody2D leftRb;
    private Rigidbody2D rightRb;

    void Start()
    {
        // 记录初始位置
        leftInitialPos = leftPart.position;
        rightInitialPos = rightPart.position;

        // 记录绳索初始高度
        if (leftLine != null)
        {
            lineInitialHeight = leftLine.size.y;
        }

        // 获取或添加刚体组件
        leftRb = leftPart.GetComponent<Rigidbody2D>();
        if (leftRb == null)
        {
            leftRb = leftPart.gameObject.AddComponent<Rigidbody2D>();
            leftRb.bodyType = RigidbodyType2D.Kinematic;
        }

        rightRb = rightPart.GetComponent<Rigidbody2D>();
        if (rightRb == null)
        {
            rightRb = rightPart.gameObject.AddComponent<Rigidbody2D>();
            rightRb.bodyType = RigidbodyType2D.Kinematic;
        }
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
        // 正常移动
         
        leftPart.position = Vector3.Lerp(leftPart.position ,leftInitialPos + new Vector3(0, -currentBalance * maxMovement, 0), recoverySpeed * Time.deltaTime) ;
        rightPart.position = Vector3.Lerp(rightPart.position, rightInitialPos + new Vector3(0, currentBalance * maxMovement, 0), recoverySpeed * Time.deltaTime);
       // rightPart.position = rightInitialPos + new Vector3(0, currentBalance * maxMovement, 0);
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
        leftRb.bodyType = RigidbodyType2D.Dynamic;
        rightRb.bodyType = RigidbodyType2D.Dynamic;

        // 施加掉落力
        Vector2 fallDirection = new Vector2(Random.Range(-0.3f, 0.3f), -1f).normalized;
        leftRb.AddForce(fallDirection * fallForce, ForceMode2D.Impulse);
        rightRb.AddForce(new Vector2(-fallDirection.x, -1f).normalized * fallForce, ForceMode2D.Impulse);

        // 添加旋转
        leftRb.AddTorque(Random.Range(-5f, 5f));
        rightRb.AddTorque(Random.Range(-5f, 5f));

        // 禁用脚本
        enabled = false;
    }

    // 触发检测（需要为两个平台添加BoxCollider2D和子物体用于触发）
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