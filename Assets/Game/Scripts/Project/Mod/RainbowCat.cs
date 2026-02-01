using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowCat : MonoBehaviour
{
    float currentSpeed = 1.5f;
    private Vector3 startPosition;
    public float destroyDistance = 10f; // 移动多远后销毁

    bool canMove = false;

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void OnEnable()
    {
        canMove = true;
        // 记录开始位置
        startPosition = transform.position;
    }

    // 巡逻移动
    private void Movement()
    {
        if (!canMove) return;
        // 向左移动
        transform.Translate(Vector2.left * currentSpeed * Time.deltaTime);

        // 检查移动距离是否达到销毁条件
        if (Vector3.Distance(startPosition, transform.position) >= destroyDistance)
        {
            canMove = false;
            SimplePool.Despawn(gameObject);
            gameObject.SetActive(false);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            if (!ItemCreater.Instance.lockPlayer && !ModVideoPlayerCreater.Instance.isBury)
            {
                PlayerController.Instance.OnHalfDieFunc();
            }
        }
    }
}