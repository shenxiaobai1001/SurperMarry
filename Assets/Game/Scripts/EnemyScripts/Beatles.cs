using EnemyScripts;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beatles : MonoBehaviour
{
    public Animator animator;
    public EnemyController enemyController;
    bool check = true;

    public void OnBeginCheck()
    {
        check = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other != null&& check) {
            check = false;
            animator.SetBool("Idel",true);
            enemyController.canMove = true;
            enemyController._moveDirection = OnGetMoveDirection();
        }
    }

    Vector3 OnGetMoveDirection()
    {
        Vector3 vector3 = Vector3.zero;
        vector3 = transform.position.x > PlayerController.Instance.transform.position.x ? Vector3.right : Vector3.left;
        return vector3;
    }
}
