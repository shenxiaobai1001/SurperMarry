using SystemScripts;
using PlayerScripts;
using UnityEngine;

namespace EnemyScripts
{
    public class KoopaShell : MonoBehaviour
    {
        public GameObject koopa;
        public EnemyController controller;
        private bool _isMoveRight;
        private bool _isMove;
        public bool _isPlayerKillable;
        public float speed;

        [HideInInspector]public AudioSource _enemyAudio;

        public AudioClip hitPlayerSound;
        public AudioClip kickSound;
        public AudioClip turnSmallPlayerSound;

        private void Awake()
        {
            _isPlayerKillable = false;
            _enemyAudio = GetComponent<AudioSource>();
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")&&!other.gameObject.CompareTag("Ground"))
            {
                _enemyAudio.PlayOneShot(kickSound);
            }

            if (!_isPlayerKillable)
            {
                if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
                {

                    OnHitByPlayer(other);
                }
            }
            else
            {
           
                PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
                if (other.gameObject.CompareTag("Player"))
                {
                    //speed = 0;
                    // StartCoroutine(Die(other.gameObject));
                    if (!playerController.isInvulnerable)
                    {
                        //_enemyAudio.PlayOneShot(hitPlayerSound);
                        GameStatusController.IsDead = true;
                    }
                    else
                    {
                        Physics2D.IgnoreCollision(GetComponent<Collider2D>(),
                            playerController.smallPlayerCollider.GetComponent<Collider2D>());
                    }
                }
                else if (other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
                {
                    _enemyAudio.PlayOneShot(turnSmallPlayerSound);
                    GameStatusController.IsBigPlayer = false;
                    GameStatusController.IsFirePlayer = false;
                    GameStatusController.PlayerTag = "Player";
                    playerController.gameObject.tag = GameStatusController.PlayerTag;
                    playerController.isInvulnerable = true;
                    PlayerController.Instance.TurnIntoBigPlayer();
                    // StartCoroutine(Die(other.gameObject));
                }
            }
        }
        public void OnHitByPlayer(Collision2D other)
        {
            Vector3 relative = transform.InverseTransformPoint(other.transform.position);
            float angle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;

            if (other.gameObject.CompareTag("Player"))
            {
                if (angle > 0)
                {
                    controller._moveDirection = Vector3.left;
                }
                else
                {
                    controller._moveDirection = Vector3.right;
                }
            }
            else if (other.gameObject.CompareTag("BigPlayer") || other.gameObject.CompareTag("UltimateBigPlayer"))
            {
                if (angle < 0)
                {
                    controller._moveDirection = Vector3.left;
                }
                else
                {
                    controller._moveDirection = Vector3.right;
                }
            }
            if (other.rigidbody != null)
            {
                other.rigidbody.velocity = new Vector2(0, 0);
                other.rigidbody.AddForce(new Vector2(0f, controller.pushForce), ForceMode2D.Impulse);
            }
            controller.canMove = true;
            controller.speed = speed;
  
            gameObject.layer = LayerMask.NameToLayer("KoopaShell");
            koopa.tag = "KoopaShell";
            Invoke("OnCanHitPlayer",0.1f);
        }
        void OnCanHitPlayer()
        {
            _isPlayerKillable = true;
        }
    }
}