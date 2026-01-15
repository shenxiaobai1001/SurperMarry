using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleUpFire : MonoBehaviour
{
    Coroutine mainMoveCoroutine;
    public void OnStarMove()
    {
        if (mainMoveCoroutine != null)
        {
            StopAllCoroutines();
            mainMoveCoroutine = null;
        }
        mainMoveCoroutine = StartCoroutine(OnMovwDown());
    }

    IEnumerator OnMovwDown()
    {
        while (transform.position.y > -10)
        {
            transform.Translate(Vector3.down * 12 * Time.deltaTime);
            yield return null;
        }
        mainMoveCoroutine = null;
        SimplePool.Despawn(gameObject);
    }
}
