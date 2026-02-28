using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePartform : MonoBehaviour
{
    public Vector3 targetPos;
    public float moveTime;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(targetPos, moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo); // 无限循环
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.tag.Contains("Player"))
        {
            PlayerController.Instance.transform.parent = transform;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        // 添加空值检查和有效性验证
        if (collision != null && collision.gameObject != null)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                // 先检查玩家对象是否仍然有效
                if (collision.gameObject.activeInHierarchy)
                {
                    // 将角色从平台子对象中移除
                    collision.transform.SetParent(null);
                }
                else
                {
                    // 如果玩家对象已被销毁或禁用，不执行任何操作
                    Debug.LogWarning("玩家对象已失效，跳过父级移除操作");
                }
            }
        }
    }
}
