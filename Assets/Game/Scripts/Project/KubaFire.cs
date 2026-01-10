using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KubaFire : MonoBehaviour
{
    private void Start()
    {
        OnStarMove();
    }
    public void OnStarMove()
    {
        StartCoroutine(OnMovwDown());
    }

    IEnumerator OnMovwDown()
    {
        while (transform.position.x > 85)
        {
            transform.Translate(Vector3.left * 8 * Time.deltaTime);
            yield return null;
        }
        SimplePool.Despawn(gameObject);
    }
}
