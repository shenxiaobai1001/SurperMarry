using DG.Tweening;
using EnemyScripts;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeakKuba : MonoBehaviour
{
    public static PeakKuba Instance;
    public Transform kubaPos;
    public Transform kubaCreateos;
    public GameObject KUBA;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        currentY = transform.position.y;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.tag.Contains("Player"))
        {
            Config.kubaCount--;
            Config.hasKubaCount++;
            Sound.PlaySound("smb_1-up");
            kubaPos.DOShakePosition(0.2f, 0.2f).onComplete+=()=>{ kubaPos.localPosition = Vector3.zero; } ;
            GameObject kuba = MonsterCreater.Instance.InstantiateSingleMonster(KUBA, kubaCreateos.position);
            kuba.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 8), ForceMode2D.Impulse);
            kuba.GetComponent<EnemyController>().OnBeginMove();
            if (Config.kubaCount <=0)
            {
                EventManager.Instance.SendMessage(Events.OnShowKubaCount, false);
                SimplePool.Despawn(gameObject);
            }
        }
    }

    [Header("跟随目标")]
    public Transform target; // 要跟随的目标

    [Header("偏移量设置")]
    public Vector3 offset; // 可自定义的跟随偏移量

    [Header("Y轴跟随限制")]
    public float minY = -10f; // Y轴下降的最低限度
    private float currentY; // 当前物体的Y轴位置
    private bool hasReachedMinY = false; // 是否已经达到最低Y值


    private void LateUpdate()
    {
        if (target == null)
        {
            if (PlayerController.Instance != null)

                target = PlayerController.Instance.transform;
        }
        if (target == null) return;

        // 计算目标位置（应用偏移量）
        Vector3 targetPosition = target.position + offset;

        // X轴始终跟随
        float newX = targetPosition.x;

        // Y轴逻辑
        if (targetPosition.y < currentY) // 如果目标低于当前Y值
        {
            // 跟随下降，但不低于最低限度
            currentY = Mathf.Max(targetPosition.y, minY);
            hasReachedMinY = (currentY <= minY); // 检查是否达到最低Y值
        }
        else if (targetPosition.y > currentY && !hasReachedMinY) // 如果目标高于当前Y值且未达到最低Y值
        {
            // 放开上升跟随
            currentY = targetPosition.y;
        }
        // 如果已经达到最低Y值，则不再跟随上升

        // 更新位置
        transform.position = new Vector3(newX, currentY, transform.position.z);
        if (target.position.y > minY + 1) {
            hasReachedMinY = false;
            transform.position = targetPosition;
            
        }
    }
}