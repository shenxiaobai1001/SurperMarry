using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using SystemScripts;
using UnityEngine;

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
        public GameObject deadEnableCollider;
        public SpriteRenderer spriteTrans;
        public Rigidbody2D rigidbody2D;
        public FlyKoopa flyKoopa;
        public Beatles beatles;
        public FlyFish flyFish;
        public KoopaShell koopaShell;
        public List<GameObject> bodys;
        public bool checkDie = true;
        public bool canMove = true;

        public bool isDead = false;

        public bool isCreate = true;
        public GameObject bodyCollider;

        private Animator _enemyAnim;
        private Vector3 _currentPatrolTarget;  // 当前巡逻目标点
        public Vector3 _moveDirection = Vector3.left;  // 移动方向
        bool isCanMove = false;
        Coroutine destoryObj=null;
        public bool checkDic = true;


        private static readonly int DieB = Animator.StringToHash("Die_b");

        private void Awake()
        {
            //isCanMove = canMove;
            _enemyAnim = GetComponent<Animator>();
        }
        private void Start()
        {
            OnBeginMove();
        }
        private void OnEnable()
        {
           
        }

        public void OnBeginMove()
        {
            if (IsInvoking("OnShowDeath"));
            {
                CancelInvoke("OnShowDeath");
            }
            dieByShell = false;
            if(koopaShell) koopaShell.gameObject.layer = LayerMask.NameToLayer("Koopa");
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
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
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
                deadEnableCollider.SetActive(false);
            }
            canMove = true;
            isDead = false;

            if (_enemyAnim)
            {
                _enemyAnim.Rebind();
                _enemyAnim.Update(0f);
            }
            if (spriteTrans != null)
            {
                spriteTrans.flipX = _moveDirection.x > 0;
                spriteTrans.flipY = false;
            }
            if (koopaShell) koopaShell._isPlayerKillable = false;
            if (flyFish) flyFish.StartFlight();
            if (flyKoopa) flyKoopa.OnStartFly();
            if(beatles) beatles.OnBeginCheck();
            destoryObj = null;
            checkGround = true;
            if(isCreate) bodyCollider.SetActive(false);
            //OnCheckHitGround();
        }
        public LayerMask groundLayer; // 障碍物所在的层（如Ground层）
        bool checkGround = true;
        private void Update()
        {
            OnCheckHitGround();
            if (transform.position.y < -3 && checkDie)
            {
                Die();
                if (destoryObj == null)
                {
                    destoryObj = StartCoroutine(Destroy());
                }
            }
            if (transform.position.y < -0.1f && destoryObj== null&&CompareTag("FlyFish"))
            {
                transform.position = new Vector3(transform.position.x,0,90);
            }
            if (canMove&&!Config.EnemyStop)
                Move();

            //if (canMove&&!isPatrolling)
            //{
            //    if (_moveDirection.x > 0)
            //    {
            //        RaycastHit2D hit3 = Physics2D.Raycast(   transform.position, Vector2.right,0.7f, groundLayer );
            //        Debug.DrawRay(transform.position, Vector3.right * 0.7f, Color.red);
            //        if (hit3.collider != null)
            //        {
            //            ChangeDirection();
            //        }
            //    }
            //    else if(_moveDirection.x < 0)
            //    {
            //        RaycastHit2D hit3 = Physics2D.Raycast(transform.position, Vector2.left, 0.7f, groundLayer);
            //        Debug.DrawRay(transform.position, Vector3.left * 0.7f, Color.red);
            //        if (hit3.collider != null)
            //        {
            //            ChangeDirection();
            //        }
            //    }
            //}
        }

        void OnCheckHitGround()
        {
            RaycastHit2D hit = Physics2D.Raycast(
              transform.position,  // 起点
              Vector2.down,        // 方向向下
              1.5f, groundLayer    // 检测距离
          );
            RaycastHit2D hit2 = Physics2D.Raycast(
              transform.position,  // 起点
              Vector2.left,        // 方向向下
              1, groundLayer    // 检测距离
          );
            RaycastHit2D hit3= Physics2D.Raycast(
              transform.position,  // 起点
              Vector2.right,        // 方向向下
              1, groundLayer    // 检测距离
          );
            bool isHit = hit.collider != null /*|| hit2.collider != null || hit3.collider != null*/;
            if (!isHit && isCreate && checkGround)
            {
                if (bodyCollider != null)
                {
                    bodyCollider.SetActive(false);
                }
            }
            else if (isHit && isCreate && checkGround)
            {
                checkGround = false;
                if (bodyCollider != null)
                {
                    bodyCollider.SetActive(true);
                }
            }

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
                //PFunc.Log("碰撞时转向", _moveDirection);
                // 原模式：向左移动，碰撞时转向
                transform.Translate(speed * Time.deltaTime * _moveDirection);
            }   // 更新精灵朝向
            if (spriteTrans != null)
            {
                spriteTrans.flipX = _moveDirection.x > 0;
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
            rigidbody2D.isKinematic = false;
            isPatrolling = false;
            canMove = false;
            isTouchByPlayer = true;
            GameStatusController.Score += 200;
            for (var i = 0; i < deadDisableCollider.Count; i++)
            {
                deadDisableCollider[i].enabled = false;
            }
            Invoke("OnShowDeath",0.05f);

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
        void OnShowDeath()
        {
            if (deadEnableCollider != null)
            {
                deadEnableCollider.SetActive(true);
            }
        }
        bool dieByShell = false;
        private void OnCollisionEnter2D(Collision2D other)
        {
            // 被火球或龟壳击中
            if (other.gameObject.CompareTag("KoopaShell") || other.gameObject.CompareTag("Fireball")
                || other.gameObject.CompareTag("UltimatePlayer")
                || other.gameObject.CompareTag("UltimateBigPlayer"))
            {
                rigidbody2D.isKinematic = false;
                isPatrolling = false;
                canMove = false;
                dieByShell = true;
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                if (destoryObj == null)
                {
                    _enemyAnim.SetBool(DieB, true);
                    destoryObj = StartCoroutine(Destroy());
                }

            }
            // 巡逻模式下不通过碰撞改变方向
            if (isPatrolling) return;

            // 简化碰撞检测逻辑
            if (CompareTag("KoopaShell"))
            {
                // 龟壳特殊处理
                if (!IsValidCollisionTarget(other.gameObject.tag) && checkDic)
                {
                    ChangeDirection();
                }
            }
            else
            {
                // 普通敌人处理
                if (!IsValidCollisionTarget(other.gameObject.tag) && checkDic)
                {
                    ChangeDirection();
                }
            }
        }

        // 检查是否为有效的碰撞目标（不会导致转向的物体）
        private bool IsValidCollisionTarget(string otherTag)
        {
            if (CompareTag("KoopaShell"))
            {
                return otherTag == "Player" || otherTag == "Ground" ||
                       otherTag == "Brick" || otherTag == "ScreenBorder" ||
                       otherTag == "Goomba" || otherTag == "Koopa";
            }
            else
            {
                return otherTag == "Player" || otherTag == "Ground" ||
                       otherTag == "Brick" || otherTag == "ScreenBorder";
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
            if (IsInvoking("OnShowDeath"));
            {
                CancelInvoke("OnShowDeath");
            }
            Sound.PlaySound("smb_kick");
            if (bodys != null && bodys.Count > 0)
            {
                for (var i = 0; i < bodys.Count; i++)
                {
                    bodys[i].SetActive(false);
                }
            }

            if (CompareTag("Goomba" )&& !dieByShell)
            {
                rigidbody2D.isKinematic = true;
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            }
            else
            {
                rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
               // rigidbody2D.isKinematic = true;
                spriteTrans.flipY = true;
                Vector3 dropDir = _moveDirection == Vector3.left ? new Vector3(5, 5, 0) : new Vector3(-5, 5, 0);
                rigidbody2D.AddForce(dropDir,ForceMode2D.Impulse);
            }
            if (isCreate)
            {
                MonsterCreater.Instance.OnMinCreates();
            }
            yield return new WaitForSeconds(0.6f);

            if (isCreate)
            {
                SimplePool.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
             destoryObj = null;
        }
    }
}