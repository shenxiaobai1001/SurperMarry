using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaoFire : MonoBehaviour
{
    public Transform trans;
    public int distance = 10;
    public bool CheckPlayer = true;
    float speed = 20;
    bool canFly = false;
    Vector2 flyVec ;
    Vector2 startPos;

   public void OnCreate (  )
    {
        if(PlayerController.Instance&& CheckPlayer)
        {
            transform.position = PlayerController.Instance.transform.position;
        }
        if (PlayerController.Instance )
        {
            trans.localEulerAngles = !PlayerController.Instance._isFacingRight ? new Vector3(0, 0, 180) : Vector3.zero;
            flyVec = PlayerController.Instance._isFacingRight ? Vector2.right : Vector2.left;
        }
    
        startPos = transform.position;
        canFly = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (canFly)
        {
            transform.Translate(flyVec * speed * Time.deltaTime);
            if (Vector2.Distance(transform.position, startPos) > distance)
            {
                canFly = false;
                SimplePool.Despawn(gameObject);
            }
        }
    }
}
