using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBreakBrick : MonoBehaviour
{
    public enum PieceType
    {
        leftPiece,
        rightPiece
    }

    public PieceType pieceType = PieceType.leftPiece;
    public float initialSpeed = 8f;          // 初始速度
    public float gravity = 9.8f;            // 重力加速度
    public float horizontalVelocity = 4f;   // 水平速度
    public BreakBrickController breakBrickController;

    private Vector3 velocity;
    private bool isTriggered = false;
    private Rigidbody2D rb;
    Vector3 startPos;
    private void Start()
    {
        startPos = transform.localPosition;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isTriggered && other.CompareTag("Meteorite"))
        {
            StartMovement();
        }
    }

    private void StartMovement()
    {
        isTriggered = true;

        // 禁用刚体物理模拟，改为手动控制
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        StartCoroutine(ParabolicMovement());
    }

    private IEnumerator ParabolicMovement()
    {
        // 设置初始速度
        velocity = Vector3.zero;

        // 根据类型确定初始水平方向
        if (pieceType == PieceType.leftPiece)
        {
            velocity.x = -horizontalVelocity;  // 向左
        }
        else
        {
            velocity.x = horizontalVelocity;   // 向右
        }

        velocity.y = initialSpeed;  // 向上

        float elapsedTime = 0f;

        while (transform.position.y > -10)
        {
            elapsedTime += Time.deltaTime;

            // 应用重力
            velocity.y -= gravity * Time.deltaTime;

            // 更新位置
            Vector3 newPosition = transform.position;
            newPosition.x += velocity.x * Time.deltaTime;
            newPosition.y += velocity.y * Time.deltaTime;
            transform.position = newPosition;

            // 如果物体已经隐藏，跳出循环
            if (!gameObject.activeSelf)
            {
                yield break;
            }
            yield return null;
        }
       if(breakBrickController) breakBrickController.OnToPool();
        gameObject.SetActive(false);
    }

    // 可选：添加一个方法来重置砖块状态
    public void ResetBrick()
    {
        isTriggered = false;
        gameObject.SetActive(true);

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Static;
        }

        transform.localPosition = startPos;
    }

}