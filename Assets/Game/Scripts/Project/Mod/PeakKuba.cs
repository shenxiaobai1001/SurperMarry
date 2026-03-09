using DG.Tweening;
using EnemyScripts;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class PeakKuba : MonoBehaviour
{
    public Transform kubaPos;
    public Transform kubaCreateos;
    public GameObject KUBA;
    public Transform uiNumber;

    private void Start()
    {
     
    }
    private void OnEnable()
    {
        transform.position = new Vector3(0, 15);
        oldPos = transform.position;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.tag.Contains("Player"))
        {
            Config.kubaCount--;
            Config.hasKubaCount++;
            if (uiNumber) uiNumber.transform.DOScale(1.1f, 0.025f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                uiNumber.transform.localScale = Vector3.one;
            });
            Sound.PlaySound("smb_1-up");
            kubaPos.DOShakePosition(0.2f, 0.2f).onComplete+=()=>{ kubaPos.localPosition = Vector3.zero; } ;
            GameObject kuba = MonsterCreater.Instance.InstantiateSingleMonster(KUBA, kubaCreateos.position);
            kuba.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 8), ForceMode2D.Impulse);
            kuba.GetComponent<EnemyController>().OnBeginMove();
        }
    }

    [Header("跟随目标")]
    public Transform target; // 要跟随的目标

    [Header("偏移量设置")]
    public Vector3 offset; // 可自定义的跟随偏移量

    [Header("Y轴跟随限制")]
    public float minY = -10f; // Y轴下降的最低限度

    Vector3 oldPos;
    bool isRest = false;
    private void LateUpdate()
    {
        if (target == null)
        {
            if (PlayerController.Instance != null)
            {
                target = PlayerController.Instance.transform;

                transform.position = new Vector3(0,15);
                oldPos= transform.position;
                isRest=true;
            }
        }
        if (GameStatusController.isDead)
        {
            transform.position = new Vector3(0, 15);
            oldPos = transform.position;
            return;
        }
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;

        float newX = targetPosition.x;

        if (target.position.y > minY)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = new Vector3(newX, minY, transform.position.z);
        }

   
        if (transform.position.y > oldPos.y)
        {
            Vector3 vec = new Vector3(newX, oldPos.y, targetPosition.z);
            transform.position = vec;
        }
        else
            oldPos = transform.position;
    }
}