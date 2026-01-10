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
        int index= Random.Range(0, gameObjects.Count);
        for (int i = 0; i < gameObjects.Count; i++) {
            gameObjects[i].gameObject.SetActive(index==i);
        }
        // 让物体的正右方（X轴正方向）指向玩家
        Vector2 direction = PlayerController.Instance.transform.position - transform.position;
        // 计算角度（从右侧开始，所以要减去90度）
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle );

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
