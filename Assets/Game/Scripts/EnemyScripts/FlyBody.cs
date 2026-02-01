using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class FlyBody : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    //private void OnCollisionEnter2D(Collision2D other)
    //{
    //    if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
    //    {
    //        GameStatusController.IsEnemyDieOrCoinEat = true;
    //        Sound.PlaySound("smb_stomp");
    //        if (other.rigidbody != null)
    //        {
    //            other.rigidbody.velocity = new Vector2(0, 0);
    //            other.rigidbody.AddForce(new Vector2(0f, 13), ForceMode2D.Impulse);
    //        }

    //        flyMonster.OnDIe();
    //    }
    //}
}
