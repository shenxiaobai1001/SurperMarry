using EnemyScripts;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class KoopaShellHead : MonoBehaviour
{
    public KoopaShell koopaShell;
    public EnemyController enemyController;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") ||
        collision.gameObject.CompareTag("BigPlayer") ||
        collision.gameObject.CompareTag("UltimateBigPlayer"))
        {
            if (koopaShell._isPlayerKillable)
            {
                koopaShell._isPlayerKillable = false;
                enemyController.canMove = false;
                if (enemyController.gameObject.CompareTag("KoopaShell"))
                {
                    enemyController.gameObject.tag = "Koopa";
                    enemyController.gameObject.layer = LayerMask.NameToLayer("Koopa");
                    koopaShell.gameObject.tag = "KoopaShell";
                    koopaShell.gameObject.layer = LayerMask.NameToLayer("Koopa");
                }
            }
            else
            {
                koopaShell.OnHitByPlayer(collision);
            }
        }
    }
}
