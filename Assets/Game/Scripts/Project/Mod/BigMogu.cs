using PlayerScripts;
using System.Collections;
using UnityEngine;

public class BigMogu : MonoBehaviour
{
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public GameObject Center;

    [Header("Movement Settings")]
    [SerializeField] private float fallSpeed = 5f;           // 下落速度
    [SerializeField] private float jumpDuration = 1.5f;     // 单次跳跃持续时间
    [SerializeField] private float jumpHeight = 3f;         // 跳跃高度
    [SerializeField] private float horizontalSpeed = 2f;    // 水平移动速度
    [SerializeField] private float waitTime = 1f;          // 跳跃间隔等待时间
    [SerializeField] private float totalLifeTime = 10f;    // 总生存时间

    private Vector3 startPosition;
    private Coroutine behaviorCoroutine;
    private float MaxXDistance = 0;
    private Transform playerTrans;
    private float elapsedTime = 0f;

    bool hasUp = false;
    bool hasDown = false;

    public void OnBeginMove()
    {
        playerTrans = PlayerController.Instance.transform;
        MaxXDistance = GameModController.Instance.OnGetLevelEndPos() - 14;

        if (behaviorCoroutine == null)
        {
            ResetMovementState();
            startPosition = transform.position;
            behaviorCoroutine = StartCoroutine(BehaviorRoutine());
        }
    }

    private IEnumerator BehaviorRoutine()
    {
        elapsedTime = 0f;
        animator.SetTrigger("Jump");

        // 阶段1: 下落至Y轴为0
        yield return FallToGround();

        // 阶段2: 等待一段时间
        yield return new WaitForSeconds(waitTime);

        // 阶段3: 朝玩家方向跳跃
        yield return ChasePlayerRoutine();

        behaviorCoroutine = null;
        SimplePool.Despawn(gameObject);
    }

    private IEnumerator FallToGround()
    {
        while (transform.position.y > 0f)
        {
            yield return WaitForResetCondition();

            float newY = transform.position.y - fallSpeed * Time.deltaTime;
            if (newY < 0f) newY = 0f;

            transform.position = new Vector3(
                transform.position.x,
                newY,
                transform.position.z
            );
        }

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);
    }

    private IEnumerator ChasePlayerRoutine()
    {
        while (elapsedTime < totalLifeTime)
        {
            yield return WaitForResetCondition();

            yield return PerformJumpTowardsPlayer();

            elapsedTime += waitTime;
            if (elapsedTime >= totalLifeTime) break;

            yield return new WaitForSeconds(waitTime);
        }
    }

    private IEnumerator PerformJumpTowardsPlayer()
    {
        hasUp = false;
        hasDown = false;

        // 获取跳跃方向
        float direction = GetJumpDirection();
        spriteRenderer.flipX = direction == 1;

        // 计算跳跃参数
        float startXPos = transform.position.x;
        float targetX = Mathf.Clamp(
            startXPos + horizontalSpeed * jumpDuration * direction,
            -MaxXDistance,
            MaxXDistance
        );

        animator.SetTrigger("Jump");
        OnSound(true);

        // 执行跳跃
        float jumpElapsed = 0f;
        while (jumpElapsed < jumpDuration && elapsedTime < totalLifeTime)
        {
            yield return WaitForResetCondition();

            float t = jumpElapsed / jumpDuration;
            float parabolaHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            float x = Mathf.Lerp(startXPos, targetX, t);

            transform.position = new Vector3(x, parabolaHeight, transform.position.z);

            jumpElapsed += Time.deltaTime;
            elapsedTime += Time.deltaTime;
        }

        OnSound(false);
        transform.position = new Vector3(targetX, 0f, transform.position.z);
    }

    private float GetJumpDirection()
    {
        if (playerTrans == null)
            return Random.value > 0.5f ? 1f : -1f;

        float direction = Mathf.Sign(playerTrans.position.x - transform.position.x);

        if (Mathf.Approximately(direction, 0f))
            direction = Random.value > 0.5f ? 1f : -1f;

        return direction;
    }

    private IEnumerator WaitForResetCondition()
    {
        if (ShouldResetPosition())
        {
            yield return ResetPositionRoutine();
        }
    }

    private bool ShouldResetPosition()
    {
        return PlayerController.Instance != null &&
               PlayerController.Instance._isFinish &&
               Config.isLoading;
    }

    private IEnumerator ResetPositionRoutine()
    {
        PFunc.Log("开始重置1");
        Center.SetActive(false);
        yield return new WaitUntil(() => !Config.isLoading);
        PFunc.Log("开始重置2");
        Center.SetActive(true);

        if (PlayerController.Instance != null)
        {
            Vector3 playerPos = PlayerController.Instance.transform.position;
            PFunc.Log("开始重置",playerPos);
            transform.position = new Vector3(playerPos.x, playerPos.y + 10, playerPos.z);
        }

        StopCurrentRoutineAndRestart();
    }

    private void StopCurrentRoutineAndRestart()
    {
        if (behaviorCoroutine != null)
        {
            StopCoroutine(behaviorCoroutine);
            behaviorCoroutine = null;
        }

        ResetMovementState();
        OnBeginMove();
    }

    private void ResetMovementState()
    {
        hasUp = false;
        hasDown = false;
    }

    void OnSound(bool up)
    {
        if (up && !hasUp)
        {
            hasUp = true;
            Sound.PlaySound("Mod/MGJump");
        }
        else if (!up && !hasDown)
        {
            hasDown = true;
            Sound.PlaySound("Mod/MGDown");
        }
    }

    private void OnDestroy()
    {
        if (behaviorCoroutine != null)
        {
            StopCoroutine(behaviorCoroutine);
        }
    }
}