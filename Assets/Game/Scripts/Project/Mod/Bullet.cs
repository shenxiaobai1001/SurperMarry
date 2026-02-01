
using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("子弹参数")]
    [SerializeField] private float speed = 10f; // 子弹速度
    [SerializeField] private float lifeTime = 2f; // 子弹存在时间

    public Animator animator;

    private Vector2 moveDirection; // 子弹移动方向
    private float lifeTimer; // 生命周期计时器
    bool canFly = false;
    Vector2 flyVec;
    Vector2 startPos;
    void Start()
    {
        lifeTimer = lifeTime;
    }

    void Update()
    {
        if (canFly)
        {
            // 移动子弹
            transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);

            transform.Rotate(new Vector3(0,0,-180)*10*Time.deltaTime);

            // 生命周期管理
            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0)
            {
                canFly = false;
                SimplePool.Despawn(gameObject);
            }
        }
      
    }

    // 设置子弹方向
    public void SetDirection(Vector2 direction)
    {
        if (animator)
        {
            animator.Rebind();
            animator.Update(0f);
        }
        lifeTimer = lifeTime;
        canFly = true;
        moveDirection = direction.normalized;

        // 设置子弹旋转角度，使其朝向运动方向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other!=null)
        {
            animator.SetTrigger("Destroy_t");
            StartCoroutine(Destroy());
        }

    }
    private IEnumerator Destroy()
    {
        canFly = false;
        yield return new WaitForSeconds(0.02f);
        SimplePool.Despawn(gameObject);
    }

}