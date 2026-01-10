using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpFire : MonoBehaviour
{
    public void OnStarMove()
    {
        StartCoroutine(OnMovwDown());
    }

    IEnumerator OnMovwDown()
    {
        while (transform.position.y > -10)
        {
            transform.Translate(Vector3.down * 15 * Time.deltaTime);
            yield return null;
        }
        SimplePool.Despawn(gameObject);
    }
}
