using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.TextCore;

namespace EnemyScripts
{
    public class EnemyController : MonoBehaviour
    {
        [Header("移动设置")]
        public float speed = 2;
        public float Patrolspeed = 2;
        public float pushForce = 100;
        public bool isTouchByPlayer;

        [Header("巡逻模式设置")]
        public bool isPatrolling = false;  // 是否启用巡逻模式
        public Vector3 patrolPointA;     // 巡逻点A（世界坐标）
        public Vector3 patrolPointB;     // 巡逻点B（世界坐标）
        public float patrolSwitchDistance = 0.1f;  // 切换巡逻点的距离阈值

        [Header("组件引用")]
        public List<Collider2D> deadDisableCollider;
        public Collider2D deadEnableCollider;
        public SpriteRenderer spriteTrans;
        public Rigidbody2D rigidbody2D;
        public FlyKoopa flyKoopa;
        public Beatles beatles;
        public FlyFish flyFish;
        public KoopaShell koopaShell;
        public List<GameObject> bodys;
        public bool canMove = true;

        public bool isDead = false;

        private Animator _enemyAnim;
        private Vector3 _currentPatrolTarget;  // 当前巡逻目标点
        public Vector3 _moveDirection = Vector3.left;  // 移动方向
        bool isCanMove = false;
        Coroutine destoryObj=null;


        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            isCanMove = canMove;
            _enemyAnim = GetComponent<Animator>();
        }

        private void OnEnable()
        {
            OnBeginMove();
        }

        public void OnBeginMove()
        {
            if (CompareTag("KoopaShell"))
            {
                gameObject.tag = "Koopa";
            }
             speed = 2;
            if (bodys != null && bodys.Count > 0)
            {
                for (var i = 0; i < bodys.Count; i++)
                {
                    bodys[i].SetActive(true);
                }
            }
            rigidbody2D.isKinematic = isPatrolling;
            if (isPatrolling)
            {
                _currentPatrolTarget = patrolPointB;  // 直接使用世界坐标
                _moveDirection = GetMoveDirection(_currentPatrolTarget);
            }
            isTouchByPlayer = false;
            for (var i = 0; i < deadDisableCollider.Count; i++)
            {
                deadDisableCollider[i].enabled = true;
            }
            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = false;
            }
            canMove = isCanMove;
            isDead = false;

            if (_enemyAnim)
            {
                _enemyAnim.Rebind();
                _enemyAnim.Update(0f);
            }
            spriteTrans.flipY = false;
            if (koopaShell) koopaShell._isPlayerKillable = false;
            if (flyFish) flyFish.StartFlight();
            if (flyKoopa) flyKoopa.OnStartFly();
            if(beatles) beatles.OnBeginCheck();
        }

        private void Update()
        {
            if (transform.position.y<-5)
            {
                Die();
                if(destoryObj==null)
                    destoryObj=StartCoroutine(Destroy());
            }
            if (transform.position.y < -0.1f && destoryObj== null&&CompareTag("FlyFish"))
            {
                transform.position = new Vector3(transform.position.x,0,90);
            }
            if (canMove)
                Move();
        }

        private void Move()
        {
            if (isPatrolling)
            {
                // 直接向目标点移动
                transform.position = Vector3.MoveTowards(transform.position, _currentPatrolTarget, Patrolspeed * Time.deltaTime);
                // 检查是否到达目标点
                if (Mathf.Abs(transform.position.x - _currentPatrolTarget.x) <= patrolSwitchDistance)
                {
                    // 切换目标点
                    _currentPatrolTarget = (_currentPatrolTarget == patrolPointA) ? patrolPointB : patrolPointA;
                    _moveDirection = GetMoveDirection(_currentPatrolTarget);
                }
            }
            else
            {
                // 原模式：向左移动，碰撞时转向
                transform.Translate(speed * Time.deltaTime * _moveDirection);
            }   // 更新精灵朝向
            if (spriteTrans != null)
            {
                spriteTrans.flipX = _moveDirection.x > 0;
            }
        }

        void OnChekYpos()
        {
            if (isDead)
            {
                return;
            }
        }

        // 获取移动到目标的方向
        private Vector3 GetMoveDirection(Vector3 target)
        {
            Vector3 direction = (target - transform.position).normalized;
            return direction;
        }

        // 外部调用改变移动方向
        public void ChangeDirection()
        {
            if (!isPatrolling)  // 巡逻模式下不通过碰撞改变方向
            {
                if (spriteTrans != null)
                {
                    spriteTrans.flipX = !spriteTrans.flipX;
                }
                _moveDirection = (_moveDirection == Vector3.left) ? Vector3.right : Vector3.left;
            }
        }

        public void Die()
        {
            isTouchByPlayer = true;
            GameStatusController.Score += 200;
            for (var i = 0; i < deadDisableCollider.Count; i++)
            {
                deadDisableCollider[i].enabled = false;
            }

            if (deadEnableCollider != null)
            {
                deadEnableCollider.enabled = true;
            }
            rigidbody2D.isKinematic = false;
            isPatrolling = false;
            canMove = false;
            isDead = true;
            if (flyKoopa) flyKoopa.StopFlying();
            if (flyFish) flyFish.StopFlight();
            _enemyAnim.SetBool(DieB, true);
       
            if (CompareTag("Goomba")|| CompareTag("FlyFish"))
            {
                if (destoryObj == null)
                    destoryObj = StartCoroutine(Destroy());
            }
        }

        private void OnCollisionEnter2D(Collision2D other)
        {
            // 巡逻模式下不通过碰撞改变方向
            if (isPatrolling) return;

            // 简化碰撞检测逻辑
            if (CompareTag("KoopaShell"))
            {
                // 龟壳特殊处理
                if (!IsValidCollisionTarget(other.gameObject.tag))
                {
                    ChangeDirection();
                }
            }
            else
            {
                // 普通敌人处理
                if (!IsValidCollisionTarget(other.gameObject.tag))
                {
                    ChangeDirection();
                }
            }

            // 被火球或龟壳击中
            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball"))
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                if (destoryObj == null)
                    destoryObj = StartCoroutine(Destroy());
            }
        }

        // 检查是否为有效的碰撞目标（不会导致转向的物体）
        private bool IsValidCollisionTarget(string otherTag)
        {
            if (CompareTag("KoopaShell"))
            {
                return otherTag == "Player" || otherTag == "Ground" ||
                       otherTag == "Brick" || otherTag == "ScreenBorder" ||
                       otherTag == "Goomba" || otherTag == "Koopa" ;
            }
            else
            {
                return otherTag == "Player" || otherTag == "Ground" ||
                       otherTag == "Brick" || otherTag == "ScreenBorder" ;
            }
        }

        // 在编辑器场景中绘制巡逻点
        private void OnDrawGizmosSelected()
        {
            if (isPatrolling && patrolPointA != Vector3.zero && patrolPointB != Vector3.zero)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(patrolPointA, 0.2f);
                Gizmos.DrawSphere(patrolPointB, 0.2f);
                Gizmos.DrawLine(patrolPointA, patrolPointB);

                // 绘制当前巡逻目标
                if (Application.isPlaying)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(_currentPatrolTarget, 0.3f);
                }
            }
        }

        IEnumerator Destroy()
        {
            Sound.PlaySound("smb_kick");
            if (bodys != null && bodys.Count > 0)
            {
                for (var i = 0; i < bodys.Count; i++)
                {
                    bodys[i].SetActive(false);
                }
            }
            spriteTrans.flipY = true;
            Vector3 dropDir = _moveDirection == Vector3.left ? new Vector3(5, 1, 0) : new Vector3(-5, 1, 0);
            rigidbody2D.AddForce(dropDir,ForceMode2D.Impulse);
            yield return new WaitForSeconds(0.6f);
            rigidbody2D.isKinematic = true;
            yield return new WaitForSeconds(0.6f);
            MonsterCreater.Instance.hasCreateMonster--;
            SimplePool.Despawn(gameObject);
            destoryObj = null;
        }
    }
}