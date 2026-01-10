using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Banana : MonoBehaviour
{
    public PolygonCollider2D polygonCollider2D;
    public Rigidbody2D rigidbody2D;
    private void OnEnable()
    {
        if (polygonCollider2D) polygonCollider2D.enabled = false;
        rigidbody2D.isKinematic = false;
        polygonCollider2D.isTrigger = false;
        Invoke("OnShowCollider",0.5f);
    }
    void OnShowCollider()
    {
       if(polygonCollider2D) polygonCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            rigidbody2D.isKinematic = true;
            //polygonCollider2D.isTrigger = true;
        }
    }
    private void OnDestroy()
    {
            if (IsInvoking("OnShowCollider"))
        {
            CancelInvoke("OnShowCollider");
        }
    }
}
