using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStone : MonoBehaviour
{
    [Header("跟随设置")]
    [SerializeField] private Transform playerTarget;  // 玩家目标
    [SerializeField] private float followSpeed = 15;  // 跟随速度
    [SerializeField] private float stopDistance = 0.5f;  // 停止距离

    [Header("射线检测设置")]
    [SerializeField] private float raycastDistance = 1f;  // 射线检测距离
    [SerializeField] private float raycastOffset = 0.5f;  // 射线起始点偏移
    [SerializeField] private LayerMask detectionLayer;  // 检测的层级


    [Header("调试设置")]
    [SerializeField] private bool drawDebugRays = true;  // 是否绘制调试射线


    public bool isLive = false;
    public bool isFollowing = true;  // 是否正在跟随
    private SpriteRenderer spriteRenderer;  // 可选：用于可视化

    void Start()
    {
        // 获取SpriteRenderer（可选）
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 如果没指定层级，默认检测所有层级
        if (detectionLayer == 0)
        {
            detectionLayer = LayerMask.GetMask("Default");
        }
    }

    void Update()
    {
        if (playerTarget == null)
        {
            playerTarget = PlayerController.Instance.transform;
        }

        if (playerTarget == null) return;
        if (isLive)
        {
            bool flow = PlayerController.Instance._isFacingRight ? PlayerController.Instance.transform.position.x > transform.position.x :
                PlayerController.Instance.transform.position.x < transform.position.x;

            isFollowing = flow && CheckBothSides();
        }
        if (!isFollowing ) return;
 
        // 检查左右两侧是否有物体
        bool canMove = CheckBothSides();

        if (canMove)
        {
            // 仅跟随X轴
            Vector3 targetPosition = new Vector3(
                playerTarget.position.x,  // 跟随玩家的X坐标
                transform.position.y,     // 保持自己的Y坐标
                transform.position.z      // 保持自己的Z坐标
            );

            // 如果距离大于停止距离，则移动

            transform.position = targetPosition;

            // 可选：改变颜色表示正在移动
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.green;
            }
        }
        else
        {
            // 停止移动
            isFollowing = false;

            // 可选：改变颜色表示已停止
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.red;
            }

            Debug.Log("检测到边缘，已停止移动！");
        }
    }

    /// <summary>
    /// 检查左右两侧是否有物体
    /// </summary>
    /// <returns>true=两侧都有物体，false=任意一侧没有检测到物体</returns>
    private bool CheckBothSides()
    {
        Vector3 currentPos = transform.position;

        // 向左发射射线
        Vector3 leftRayOrigin = new Vector3(
            currentPos.x - raycastOffset,
            currentPos.y,
            currentPos.z
        );

        // 向右发射射线
        Vector3 rightRayOrigin = new Vector3(
            currentPos.x + raycastOffset,
            currentPos.y,
            currentPos.z
        );

        // 绘制调试射线
        if (drawDebugRays)
        {
            Debug.DrawRay(leftRayOrigin, Vector3.left * raycastDistance, Color.red);
            Debug.DrawRay(rightRayOrigin, Vector3.right * raycastDistance, Color.blue);
        }

        // 检测左侧
        RaycastHit2D hitLeft = Physics2D.Raycast(
            leftRayOrigin,
            Vector3.left,
            raycastDistance,
            detectionLayer
        );

        // 检测右侧
        RaycastHit2D hitRight = Physics2D.Raycast(
            rightRayOrigin,
            Vector3.right,
            raycastDistance,
            detectionLayer
        );

        bool canMove = hitLeft.collider != null && !PlayerController.Instance._isFacingRight 
            || hitRight.collider != null && PlayerController.Instance._isFacingRight;

        return canMove;
    }

    /// <summary>
    /// 重新开始跟随
    /// </summary>
    public void ResumeFollowing()
    {
        isFollowing = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.white;
        }
    }


    /// <summary>
    /// 在Scene视图中绘制检测范围
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (!drawDebugRays)
            return;

        Gizmos.color = Color.yellow;

        // 绘制检测范围
        Vector3 pos = transform.position;

        // 左侧检测范围
        Vector3 leftStart = new Vector3(pos.x - raycastOffset, pos.y, pos.z);
        Gizmos.DrawLine(leftStart, leftStart + Vector3.left * raycastDistance);
        Gizmos.DrawSphere(leftStart, 0.1f);

        // 右侧检测范围
        Vector3 rightStart = new Vector3(pos.x + raycastOffset, pos.y, pos.z);
        Gizmos.DrawLine(rightStart, rightStart + Vector3.right * raycastDistance);
        Gizmos.DrawSphere(rightStart, 0.1f);

        // 绘制跟随距离范围
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
        
             isLive = true;
            
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.CompareTag("Player"))
        {
            isLive = false;
        }
    }
}