using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using UnityEngine.UIElements;

public class MoveEffectData
{
    public MoveType moveType;
    public Vector3 direction;
    public Vector3 originalDirection; // 原始方向，用于射线检测后的恢复
    public float moveSpeed;
    public float duration;
    public float remainingTime;
    public bool canFight;
    public bool shouldRotate;
    public int effectID;
    public int giftLevel;
    public float startTime; // 开始时间

    public MoveEffectData()
    {
        startTime = Time.time;
    }

    public int GetMoveLevel()
    {
        switch (moveType)
        {
            case MoveType.Normal: return 1;
            case MoveType.HighLeft:
            case MoveType.HighRight: return 2;
            case MoveType.MaxLeft:
            case MoveType.MaxRight: return 3;
            default: return 0;
        }
    }
}

public class PlayerModMoveController : MonoBehaviour
{
    public static PlayerModMoveController Instance;
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform rotationTarget;

    [Header("边界设置")]
    [SerializeField] private float minX = -7.5f;
    [SerializeField] private float maxX = 192;
    [SerializeField] private float minY = 0;
    [SerializeField] private float maxY = 9;

    [Header("射线检测设置")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask obstacleLayer = 0; // 默认检测所有层
    [SerializeField] private float upwardMoveTime = 0.3f; // 向上移动的时间

    private Dictionary<MoveType, int> moveLevels = new Dictionary<MoveType, int>
    {
        {MoveType.Normal, 1},
        {MoveType.HighLeft, 2},
        {MoveType.HighRight, 2},
        {MoveType.MaxLeft, 3},
        {MoveType.MaxRight, 3}
    };

    // 当前移动状态
    private MoveEffectData currentMoveEffect;
    private Coroutine mainMoveCoroutine;
    private float rotationSpeed = 180f;

    // 僵持状态
    private bool isInStalemate = false;
    private float stalemateRemainingTime = 0f;
    private MoveEffectData stalemateEffect1;
    private MoveEffectData stalemateEffect2;

    // 射线检测相关
    private Vector3 lastSafeDirection; // 上次安全的方向
    private float upwardMoveTimer = 0f; // 向上移动计时器
    private bool isAvoidingObstacle = false; // 是否正在躲避障碍物

    // 缓存
    private Queue<MoveEffectData> effectQueue = new Queue<MoveEffectData>();
    private WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();

    public int CurrentEffectLevel => currentMoveEffect != null ?
        moveLevels[currentMoveEffect.moveType] : 0;

    // 属性用于UI显示
    public string CurrentStatus
    {
        get
        {
            if (isInStalemate)
                return $"僵持中... {stalemateRemainingTime:F1}秒";

            if (currentMoveEffect != null)
                return $"{currentMoveEffect.moveType} - 剩余:{currentMoveEffect.remainingTime:F1}秒";

            return "静止";
        }
    }

    public bool IsMoving => currentMoveEffect != null || isInStalemate;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (playerTransform == null)
            playerTransform = transform;
    }

    private void Start()
    {
        // 启动主移动协程
        if (mainMoveCoroutine != null)
            StopCoroutine(mainMoveCoroutine);
        mainMoveCoroutine = StartCoroutine(MainMovementCoroutine());
    }

    private void OnDestroy()
    {
        if (mainMoveCoroutine != null)
        {
            StopCoroutine(mainMoveCoroutine);
            mainMoveCoroutine = null;
        }
    }
    public void OnSetMinValue(float mx,float max)
    {
        minX = mx;
        maxX = max;
    }
    public void TriggerModMove(MoveType type, Vector3 dir, float speed = 5f, float time = 2f,
        bool canFight = true, bool rotate = false, int effectId = 0)
    {
        if (ItemCreater.Instance != null && ItemCreater.Instance.isHang)
        {
            PlayerModController.Instance.OnCancelHangSelf();
        }
        if (!PlayerModController.Instance.isKinematic)
        {
            PlayerModController.Instance.OnChangeState(false);
        }

        PlayerController.Instance.OnChanleControl(true);
        if (!ItemCreater.Instance.lockPlayer)
             PlayerModController.Instance.OnMoveShowIcon();
        // 创建移动数据
        MoveEffectData newEffect = new MoveEffectData
        {
            moveType = type,
            direction = dir,
            originalDirection = dir.normalized,
            moveSpeed = speed,
            duration = time,
            remainingTime = time,
            canFight = canFight,
            shouldRotate = rotate,
            effectID = effectId
        };
        newEffect.giftLevel = moveLevels[type];

        // 播放特效
        PlayEffect(newEffect.effectID);

        // 处理移动优先级
        HandleMovementPriority(newEffect);

    }

    /// <summary>所有移动都完成时调用的方法 </summary>
    private void OnAllMovementCompleted(MoveEffectData lastFinishedEffect = null)
    {
        Debug.Log("所有移动已完全结束");

   
        // 如果有最后完成的移动效果，可以基于它做一些特殊处理
        if (lastFinishedEffect != null)
        {
            Debug.Log($"最后一个完成的移动效果: {lastFinishedEffect.moveType}");
        }

        if (ItemCreater.Instance.lockPlayer)
        {
            if (!PlayerModController.Instance.isKinematic)
            {
                PlayerModController.Instance.OnChangeState(false);
            }
            PlayerController.Instance.transform.position = ChainPlayer.Instance. animator.transform.position;
        }
        else
        {
            if (PlayerModController.Instance.isKinematic)
            {
                PlayerModController.Instance.OnChangeState(true);
            }
            PlayerController.Instance.isHit = false;
            PlayerModController.Instance.OnEndHitPos();
            PlayerModController.Instance.OnChanleModAni();
        }

    }

    private void HandleMovementPriority(MoveEffectData newEffect)
    {
        // 如果当前处于僵持状态
        if (isInStalemate)
        {
            // Max级别可以打断僵持
            if (newEffect.giftLevel > stalemateEffect1.giftLevel)
            {
                StopStalemate();
                SetCurrentMoveEffect(newEffect);
            }
            else
            {
                HandleSameLevelMovement(newEffect);
            }
            return;
        }

        // 如果没有当前移动效果，直接设置
        if (currentMoveEffect == null)
        {
            SetCurrentMoveEffect(newEffect);
            return;
        }

        int newLevel = newEffect.giftLevel;
        int currentLevel = CurrentEffectLevel;

        // 新效果等级低于当前效果，忽略
        if (newLevel < currentLevel)
        {
            Debug.Log($"新礼物等级{newLevel}低于当前等级{currentLevel}，忽略");
            return;
        }

        // 新效果等级高于当前效果，覆盖
        if (newLevel > currentLevel)
        {
            Debug.Log($"新礼物等级{newLevel}高于当前等级{currentLevel}，覆盖");
            SetCurrentMoveEffect(newEffect);
            return;
        }

        // 等级相同的情况
        if (newLevel == currentLevel)
        {
            HandleSameLevelMovement(newEffect);
        }
    }

    private void HandleSameLevelMovement(MoveEffectData newEffect)
    {
        MoveType currentType = currentMoveEffect.moveType;
        MoveType newType = newEffect.moveType;

        // 如果是Normal类型，总是覆盖
        if (currentType == MoveType.Normal && newType == MoveType.Normal)
        {
            SetCurrentMoveEffect(newEffect);
            return;
        }

        // 相同类型的高等级移动，覆盖
        if ((currentType == MoveType.HighLeft && newType == MoveType.HighLeft) ||
            (currentType == MoveType.HighRight && newType == MoveType.HighRight) ||
            (currentType == MoveType.MaxLeft && newType == MoveType.MaxLeft) ||
            (currentType == MoveType.MaxRight && newType == MoveType.MaxRight))
        {
            SetCurrentMoveEffect(newEffect);
            return;
        }

        // 相反类型的High级别，进入僵持
        if ((currentType == MoveType.HighLeft && newType == MoveType.HighRight) ||
            (currentType == MoveType.HighRight && newType == MoveType.HighLeft))
        {
            StartStalemate(currentMoveEffect, newEffect);
            return;
        }

        // 相反类型的Max级别，进入僵持
        if ((currentType == MoveType.MaxLeft && newType == MoveType.MaxRight) ||
            (currentType == MoveType.MaxRight && newType == MoveType.MaxLeft))
        {
            StartStalemate(currentMoveEffect, newEffect);
            return;
        }

        // Max级别覆盖High级别
        if ((currentType == MoveType.HighLeft || currentType == MoveType.HighRight) &&
            (newType == MoveType.MaxLeft || newType == MoveType.MaxRight))
        {
            SetCurrentMoveEffect(newEffect);
        }
    }

    private void SetCurrentMoveEffect(MoveEffectData effect)
    {
        // 设置新的移动效果
        currentMoveEffect = effect;
        upwardMoveTimer = 0f;
        isAvoidingObstacle = false;
        Debug.Log($"设置新移动效果: {effect.moveType}, 方向: {effect.direction}");
    }

    private IEnumerator MainMovementCoroutine()
    {
        while (true)
        {
            yield return null;
            if (currentMoveEffect == null) continue;

            if (!PlayerModController.Instance.isKinematic)
            {
                PlayerModController.Instance.OnChangeStateTrue();
                if(ItemCreater.Instance != null && ItemCreater.Instance.lockPlayer)
                {
                    PlayerModController.Instance.OnShowModAnimation(-1);
                    PlayerModController.Instance.OnSetPlayerIns(false);
                }
                else
                {
                    PlayerModController.Instance.OnSetPlayerIns(true);
                }
                yield return null;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                         
            }
            // 更新僵持状态
            if (isInStalemate)
            {
                UpdateStalemate();
                continue;
            }

            if(currentMoveEffect != null)
            {
                PlayerController.Instance.OnChanleControl(true);
                UpdateMovement();
            }
            
        }
    }

    private void UpdateMovement()
    {
        if (currentMoveEffect == null) return;
        currentMoveEffect.remainingTime -= Time.deltaTime;
        // 时间到，结束移动
        if (currentMoveEffect.remainingTime <= 0)
        {
            // 记录当前移动效果
            MoveEffectData finishedEffect = currentMoveEffect;

            // 清除当前移动效果
            currentMoveEffect = null;
            isAvoidingObstacle = false;

            // 检查是否需要调用所有移动完成方法
            CheckAndHandleAllMovementFinished(finishedEffect);
            return;
        }

        // 射线检测
        Vector3 moveDirection = GetAdjustedMoveDirection();

        // 计算移动
        Vector3 moveDelta = moveDirection * currentMoveEffect.moveSpeed * Time.fixedDeltaTime;
        Vector3 newPosition = playerTransform.position + moveDelta;
        tminX = GameStatusController.IsHidden ? 14 : minX;
        tminY=GameStatusController.IsHidden ? 31 : minY;
        tmaxX = GameStatusController.IsHidden ? 25 : maxX;
        tmaxY = GameStatusController.IsHidden ? 41 : maxY;

        // 边界限制
        newPosition.x = Mathf.Clamp(newPosition.x, tminX, tmaxX);

        bool up = currentMoveEffect.direction.y > 0;
        if (up)
        {
            // 向上移动：限制在 tminY 和 tmaxY 之间
            newPosition.y = Mathf.Clamp(newPosition.y, tminY, tmaxY);
        }
        else
        {
            // 向下移动：确保不低于 tminY，但不高于 tmaxY
            newPosition.y = newPosition.y <= tminY ? tminY : newPosition.y;
            // 或者更清晰的写法：
            // newPosition.y = Mathf.Max(newPosition.y, tminY);
            // 然后再确保不超过 tmaxY
            // newPosition.y = Mathf.Min(newPosition.y, tmaxY);
        }


        if(ItemCreater.Instance != null && !ItemCreater.Instance.lockPlayer)
        {
            // 应用移动
            playerTransform.position = newPosition;
        }

        // 处理旋转
        if (currentMoveEffect.shouldRotate && rotationTarget != null)
        {
            rotationTarget.Rotate(Vector3.up, rotationSpeed * Time.fixedDeltaTime);
        }
    }
    float tminX = 0;
    float tminY = 0;
    float tmaxX = 0;
    float tmaxY = 0;
    /// <summary>
    /// 检查并处理所有移动完成的情况
    /// </summary>
    private void CheckAndHandleAllMovementFinished(MoveEffectData finishedEffect = null)
    {
        // 如果当前没有移动效果，也不在僵持状态，说明所有移动都完成了
        if (currentMoveEffect == null && !isInStalemate)
        {
            OnAllMovementCompleted(finishedEffect);
        }
    }


    private Vector3 GetAdjustedMoveDirection()
    {
        if (currentMoveEffect == null) return Vector3.zero;

        // 如果需要躲避障碍物且计时未结束，继续向上移动
        if (isAvoidingObstacle)
        {
            upwardMoveTimer -= Time.fixedDeltaTime;
            if (upwardMoveTimer > 0)
            {
                return Vector3.up;
            }
            else
            {
                isAvoidingObstacle = false;
            }
        }

        // 进行射线检测
        Vector3 rayDirection = currentMoveEffect.direction.normalized;
        RaycastHit2D hit = Physics2D.Raycast(
            playerTransform.position,
            rayDirection,
            raycastDistance,
            obstacleLayer
        );

        // 在Scene视图中绘制射线
        Debug.DrawRay(playerTransform.position, rayDirection * raycastDistance,
            hit.collider != null ? Color.red : Color.green);

        // 如果检测到障碍物
        if (hit.collider != null)
        {
            Debug.Log($"检测到障碍物: {hit.collider.name}");

            // 改为向上移动
            isAvoidingObstacle = true;
            upwardMoveTimer = upwardMoveTime;
            return Vector3.up;
        }

        // 没有障碍物，使用原始方向
        return currentMoveEffect.direction.normalized;
    }

    private void UpdateStalemate()
    {
        stalemateRemainingTime -= Time.deltaTime;
        if (stalemateEffect1 != null) stalemateEffect1.remainingTime-= Time.deltaTime;
        if (stalemateEffect2 != null) stalemateEffect2.remainingTime -= Time.deltaTime;
        if (stalemateRemainingTime <= 0)
        {
            EndStalemate();
            OnNormalDuiKangScence();
        }
    }
    float airMintime = 0.1f;
    float airtime = 0;
    float snakeTargetTime = 1;
    float snakeTime = 2;

    /// <summary>
    /// 新增：Normal对抗状态处理
    /// </summary>
    void OnNormalDuiKangScence()
    {
        // Normal对抗状态，摄像机抖动，不移动
        snakeTime += Time.deltaTime;
        if (snakeTime > snakeTargetTime)
        {
            CameraShaker.Instance.StartShake(snakeTargetTime);
            snakeTime = 0;
        }
    }
    private void StartStalemate(MoveEffectData effect1, MoveEffectData effect2)
    {
        isInStalemate = true;
        stalemateEffect1 = effect1;
        stalemateEffect2 = effect2;
        stalemateRemainingTime = Mathf.Min(effect1.remainingTime, effect2.duration);

        Debug.Log($"进入僵持状态，持续时间: {stalemateRemainingTime:F1}秒");
    }

    private void EndStalemate()
    {
        isInStalemate = false;

        // 僵持结束后，选择剩余时间更长的效果继续
        MoveEffectData nextEffect = null;

        if (stalemateEffect1 != null && stalemateEffect2 != null)
        {
            float effect1Remaining = stalemateEffect1.remainingTime ;
            float effect2Remaining = stalemateEffect2.remainingTime;

            if (effect1Remaining > 0 && effect1Remaining> effect2Remaining)
            {
                nextEffect = stalemateEffect1;
            }
            else if (effect2Remaining > 0 && effect2Remaining > effect1Remaining)
            {
                nextEffect = stalemateEffect2;
            }
        }

        stalemateEffect1 = null;
        stalemateEffect2 = null;

        if (nextEffect != null)
        {
            SetCurrentMoveEffect(nextEffect);
        }
        else
        {
            // 僵持结束后没有后续移动，检查是否所有移动都完成了
            CheckAndHandleAllMovementFinished();
        }
    }

    private void StopStalemate()
    {
        isInStalemate = false;
        stalemateRemainingTime = 0f;
        stalemateEffect1 = null;
        stalemateEffect2 = null;
    }

    public void ForceStopAllMovement()
    {
        // 停止当前移动
        currentMoveEffect = null;
        isInStalemate = false;
        StopStalemate();
        isAvoidingObstacle = false;

        // 所有移动被强制停止，调用完成方法
        OnAllMovementCompleted();
    }

    // 以下特效播放和辅助方法保持不变
    public GameObject boom1;
    public GameObject boom2;

    public void PlayEffect(int effectId)
    {
        Debug.Log($"播放特效: {effectId}");

        switch (effectId)
        {
            case 1:
                if (boom1 != null)
                {
                    GameObject boom11 = SimplePool.Spawn(boom1, transform.position, Quaternion.identity);
                    boom11.transform.SetParent(ItemCreater.Instance.modItemPatent);
                    boom11.SetActive(true);
                }
                break;
            case 2:
                if (boom2 != null)
                {
                    GameObject boom22 = SimplePool.Spawn(boom2, transform.position, Quaternion.identity);
                    boom22.transform.SetParent(ItemCreater.Instance.modItemPatent);
                    boom22.SetActive(true);
                }
                break;
        }
    }

    // 设置边界
    public void SetBoundary(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    // 在编辑器中可视化边界
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Vector3 bottomLeft = new Vector3(minX, minY, 0);
        Vector3 bottomRight = new Vector3(maxX, minY, 0);
        Vector3 topLeft = new Vector3(minX, maxY, 0);
        Vector3 topRight = new Vector3(maxX, maxY, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }

    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    public void SetRotationTarget(Transform target)
    {
        rotationTarget = target;
    }
}