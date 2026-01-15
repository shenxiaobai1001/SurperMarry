using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meteorite : MonoBehaviour
{
    public List<GameObject> gameObjects;
    public Transform spriteTrans;
    private Coroutine mainMoveCoroutine;
    public void OnBeginMove()
    {
        moveCount = 0;
        moveSpeed = Random.Range(15, 25);
        int index = Random.Range(0, gameObjects.Count);
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(index == i);
        }

        // 获取玩家位置
        Vector2 targetPos = PlayerController.Instance.transform.position;

        // 方法1：在计算的角度上直接添加随机偏移
        Vector2 direction = targetPos - (Vector2)transform.position;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 添加随机偏移（-5到+5度）
        float randomOffset = Random.Range(-30f, 30f);
        float finalAngle = baseAngle + randomOffset;

        transform.rotation = Quaternion.Euler(0, 0, finalAngle);

        mainMoveCoroutine = StartCoroutine(OnMove());
    }

    float moveCount = 0;
    float moveSpeed = 0;
    IEnumerator OnMove()
    {
        while (transform.position.y>-15)
        {
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            spriteTrans.Rotate(new Vector3(0,0,1), 180 * Time.fixedDeltaTime);
            yield return null;
        }
        ReadyDestory();
    }
    void ReadyDestory()
    {
        if (mainMoveCoroutine != null)
        {
            StopCoroutine(mainMoveCoroutine);
        }

        SimplePool.Despawn(gameObject);
    }
}
