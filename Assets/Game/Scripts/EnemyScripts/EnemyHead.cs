using SystemScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class EnemyHead : MonoBehaviour
    {
        private EnemyController _enemyController;
        public GameObject enemy;
        private AudioSource _enemyAudio;
        public AudioClip hitByPlayerSound;

        private void Awake()
        {
            _enemyAudio = GetComponent<AudioSource>();
            _enemyController = enemy.GetComponent<EnemyController>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer") 
                || other.gameObject.CompareTag("UltimateBigPlayer") )
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;
                _enemyAudio.PlayOneShot(hitByPlayerSound);
                if (other.rigidbody != null)
                {
                    other.rigidbody.velocity = new Vector2(0, 0);
                    other.rigidbody.AddForce(new Vector2(0f, _enemyController.pushForce), ForceMode2D.Impulse);
                }
               
                _enemyController.Die();
            }
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
           // PFunc.Log("OnTriggerEnter2D"collision.gameObject.tag);

            if (collision.gameObject.CompareTag("brickHit")&& _enemyController.isInit)
            {
                GameStatusController.IsEnemyDieOrCoinEat = true;
                _enemyAudio.PlayOneShot(hitByPlayerSound);
                _enemyController.dieByShell = true;
                _enemyController.Die();
            }
        }
    }
}
