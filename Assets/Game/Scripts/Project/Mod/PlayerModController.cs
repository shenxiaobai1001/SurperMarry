using DG.Tweening;
using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class PlayerModController : MonoBehaviour
{
    public static PlayerModController Instance;
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
    }
    private void Start()
    {
        EventManager.Instance.AddListener(Events.OnLazzerHit, OnLazzerHit);
        playerBreak.gameObject.SetActive(false);
        for (int i = 0; i < modAnimations.Count; i++)
        {
            modAnimations[i].gameObject.SetActive(false);
        }
    }

    public Animator animator;
    public Animator modAnimator;
    public Rigidbody2D rigidbody2D;
    public GameObject spriteTrans;
    public GameObject Center;
    public List<SpriteRenderer> spriteRenderers;
    public PlayerBreak playerBreak;
    public List<GameObject> modAnimations;
    public GameObject modAnimation;
    public SpriteBlinkController spriteBlinkController;

    int isPassivityMove;

    [HideInInspector] public bool isKinematic = false;
    public void OnChangeState(bool open)
    {
        if (open && (ModVideoPlayerCreater.Instance.isBury || ItemCreater.Instance.lockPlayer || isSuperMan||isDance)) return;
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = !open;
        isKinematic = !open;
        Center.SetActive(open);
    }
    public void OnChangeStateTrue()
    {
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = true;
        isKinematic = true;
        Center.SetActive(false);
    }
    public void OnChangeStateFalse()
    {
        if (ModVideoPlayerCreater.Instance.isBury || ItemCreater.Instance.lockPlayer || isSuperMan) return;
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(false);
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = false;
        isKinematic = false;
        Center.SetActive(true);
    }
    public void OnSetPlayerIns(bool show)
    {
        if (show && (ModVideoPlayerCreater.Instance.isBury|| ItemCreater.Instance.lockPlayer || isSuperMan || isDance || isZombie)) return;
        // PFunc.Log("OnSetPlayerIns", show);
       if(animator) animator.enabled = show;
       if(spriteRenderers!=null|| spriteRenderers.Count>0)
        {
            for (int i = 0; i < spriteRenderers.Count; i++)
            {
                Color color = show ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
                spriteRenderers[i].color = color;
            }
        }
    
    }

    public void OnToHitPos()
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            if (animator) animator.SetTrigger("fireSwim");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            if (animator) animator.SetTrigger("Swim");
        }
        else
        {
            if (animator) animator.SetTrigger("Swim");
        }
    }
    public void OnEndHitPos()
    {
        if (GameStatusController.IsBigPlayer)
        {
            if (animator) animator.Play("Idle_Big");
        }
        else
        {
            if (animator) animator.Play("Idle");
        }
    }
    public void OnToSwim()
    {
        // OnToHitPos();
        bool isRight = PlayerController.Instance._isFacingRight;
        if (isRight)
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(1, 0.5f), 10, 0.25f, true, false, 1, true);
        else
            PlayerModMoveController.Instance.TriggerModMove(MoveType.Normal, new Vector3(-1, 0.5f), 10, 0.25f, true, false, 1, true);
    }

    public void OnAddFourePlayer(Vector3 vector)
    {
        if (ItemCreater.Instance.isHang)
        {
            OnCancelHangSelf();
        }
        rigidbody2D.velocity = new Vector2(0, 0); // 重置水平速度
        rigidbody2D.AddForce(vector, ForceMode2D.Impulse); // 重置水平速度
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision == null) return;

        if (collision.gameObject.CompareTag("Banana"))
        {
            Sound.PlaySound("Mod/banana");
            OnToSwim();
        }
    }
    public void OnHangSelf()
    {
        OnChangeState(false);

        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        OnSetPlayerIns(false);
        isPassivityMove++;
    }

    public void OnCancelHangSelf()
    {
        animator.gameObject.SetActive(true);
        isPassivityMove--;
        if (isPassivityMove <= 0)
        {
            isPassivityMove = 0;
            OnChangeState(true);
        }

        if (HangSelf.Instance != null && HangSelf.Instance.lastPoint != null)
        {
            Vector3 vector = HangSelf.Instance.lastPoint.transform.position;
            if (vector != Vector3.zero)
            {
                if(!ItemCreater.Instance.lockPlayer && !ModVideoPlayerCreater.Instance.isBury)
                    transform.position = vector;
            }
            HangSelf.Instance.OnBreakeHang();
        }
        OnSetPlayerIns(true);
        if (isSuperMan)
        {
            OnShowModAnimation(7);
        }
        else if (isZombie)
        {
            OnShowModAnimation(15);
        }
    }

    public void OnTriggerModAnimator(string riggerName)
    {
        if (animator) animator.SetTrigger(riggerName);
    }

    void OnLazzerHit(object msg)
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("redLazzer");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.SetTrigger("bigLazzer");
        }
        else
        {
            animator.SetTrigger("smallLazzer");
        }
    }


    public void OnBigDao() => OnShowModAnimation(0);
    public void OnTiggerDao()
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(1);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(0);
        }
        else
        {
            OnTriggerModAnimator("dao");
        }
    }
    public void OnTiggerManace()
    {
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(3);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(2);
        }
        else {
            OnTriggerModAnimator("menace");
        }
    }

    public void OnChanleModAni()
    {
        OnShowModAnimation(-1);
        OnSetPlayerIns(true);

        animator.SetTrigger("endMenace");

        if(GameStatusController.IsDaoPlayer)
        {
            animator.SetTrigger("TDao");
        }
        else if (GameStatusController.IsQiangPlayer)
        {
            animator.SetTrigger("TQiang");
        }
        else if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.Play("PlayerFireBigIdle");
            animator.SetTrigger("endFireSwim");
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            animator.Play("Idle_Big");
            animator.SetTrigger("endSwim");
        }
        else
        {
            animator.StopPlayback();
            animator.Play("Idle", 0);
            animator.SetTrigger("endSwim");
        }
        if (isSuperMan)
        {
            OnShowModAnimation(7);
        }
        else if (isZombie)
        {
            OnShowModAnimation(15);
        }
        else if (isInvincible)
        {
            OnSetInvincileState();
        }
    }
    public void OnSetModAniIns(bool show)
    {
        if(modAnimation) modAnimation.SetActive(show);
    }

    public void OnShowModAnimation(int index)
    {
        if (isDance) return;
        OnSetPlayerIns(false);
        for (int i = 0; i < modAnimations.Count; i++) {
            modAnimations[i].gameObject.SetActive(i == index);
        }
    }

    public void OnMoveShowIcon()
    {
        OnSetPlayerIns(false);
        if (ItemCreater.Instance.lockPlayer) return;
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(6);
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(5);
        }
        else
        {
            OnShowModAnimation(4);
        }
    }

    public void OnFiler()
    {
        if (modAnimator) modAnimator.Play("Filer");
        // if (spriteBlinkController) spriteBlinkController.StartBlink();
    }
    public bool isSuperMan = false;
    public GameObject superEffert;
    public Transform superEffertTrans;
    float superManTime = 0;
    Coroutine superMainTime;
    float superSpeed = 10;
    public void OnSuperMan(float Time)
    {
        if (GameStatusController.isDead || Config.isLoading) return;
        Sound.PlaySound("smb_1-up");
        superManTime += Time;
        if (superMainTime == null && !isSuperMan)
        {
            if (PlayerController.Instance != null)
                PlayerController.Instance.OnChanleControl(true);
            isSuperMan = true;
            OnShowModAnimation(7);
            OnSetPlayerIns(false);
            OnChangeStateTrue();
            superMainTime = StartCoroutine(SuperManTime());
        }
    }

    IEnumerator SuperManTime()
    {
        while (superManTime > 0&& isSuperMan) {
            superManTime -= Time.deltaTime;
            yield return null;
        }
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(false);
        isSuperMan = false;
        superMainTime = null;
        OnShowModAnimation(-1);
        OnSetPlayerIns(true);
        OnChangeStateFalse();
    }
    float superEffectTime = 0;
    private void Update()
    {
        #region GM
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rigidbody2D.isKinematic = true;
            transform.Translate(Vector2.left * superSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rigidbody2D.isKinematic = true;
            transform.Translate(Vector2.right * superSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rigidbody2D.isKinematic = true;
            transform.Translate(Vector2.down * superSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rigidbody2D.isKinematic = true;
            transform.Translate(Vector2.up * superSpeed * Time.deltaTime);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            rigidbody2D.isKinematic = false;
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            rigidbody2D.isKinematic = false;
        }
        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            rigidbody2D.isKinematic = false;
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            rigidbody2D.isKinematic = false;
        }
        #endregion

        if (isSuperMan&&!isDance && !ItemCreater.Instance.isHang && !ItemCreater.Instance.lockPlayer&&!ModVideoPlayerCreater.Instance.isBury) {
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector2.right * superSpeed * Time.deltaTime);
                if (PlayerController.Instance._isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    PlayerController.Instance._isFacingRight = false;
                }
                OnCreateSuperEffect();
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector2.right * superSpeed * Time.deltaTime);
                if (!PlayerController.Instance._isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    PlayerController.Instance._isFacingRight = true;
                }
                OnCreateSuperEffect();
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector2.down * superSpeed * Time.deltaTime);
                OnCreateSuperEffect();
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector2.up * superSpeed * Time.deltaTime);
                OnCreateSuperEffect();
            }
        }
    }

    void OnCreateSuperEffect()
    {
        superEffectTime += Time.deltaTime;
        if (superEffectTime >= 0.02f)
        {
            GameObject obj = SimplePool.Spawn(superEffert, superEffertTrans.position, Quaternion.identity);
            obj.transform.SetParent(ModController.Instance.itemParent);
            obj.SetActive(true);
            superEffectTime = 0;
        }
    }

   public bool isDance = false;
    public void OnGuangDance()
    {
        if (GameStatusController.isDead || Config.isLoading || isDance) return;
        Sound.PlayMusic("Mod/guangbo");
        Config.EnemyStop = true;
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        OnSetPlayerIns(false);
        OnChangeStateTrue();
        Animator atrDance = null;
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(10);
            atrDance = modAnimations[10].GetComponent<Animator>();
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(9);
            atrDance = modAnimations[9].GetComponent<Animator>();
        }
        else
        {
            OnShowModAnimation(8);
            atrDance = modAnimations[8].GetComponent<Animator>();
        }
        if (atrDance)
        {
            atrDance.Rebind();
            atrDance.Update(0f);
        }
        isDance = true;
        Invoke("OnRestDance", 110);
    }
    void OnRestDance()
    {
        Sound.PlayMusic("background");
        Config.EnemyStop = false;
        isDance = false;

        OnShowModAnimation(-1);
        OnSetPlayerIns(true);

        OnChangeStateFalse();
    }
    bool isDaom = false;
    public void OnDMDance()
    {
        if (GameStatusController.isDead || Config.isLoading||isDance) return;
        Sound.PlaySound("Mod/dmdm");
        Config.EnemyStop = true;
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        OnSetPlayerIns(false);
        OnChangeStateTrue();
        Animator atrDance = null;
        if (GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(14);
            atrDance = modAnimations[14].GetComponent<Animator>();
        }
        else if (!GameStatusController.IsFirePlayer && GameStatusController.IsBigPlayer)
        {
            OnShowModAnimation(13);
            atrDance = modAnimations[13].GetComponent<Animator>();
        }
        else
        {
            OnShowModAnimation(12);
            atrDance = modAnimations[12].GetComponent<Animator>();
        }
        if (atrDance)
        {
            atrDance.Rebind();
            atrDance.Update(0f);
        }
        isDance = true;
        Invoke("OnRestDMDance", 4);
    }
    void OnRestDMDance()
    {

        Config.EnemyStop = false;
        isDance = false;
        OnShowModAnimation(-1);
        OnSetPlayerIns(true);
   
        OnChangeStateFalse();
    }
    bool isKickHead = false;
    Tween tween = null;
    public GameObject kickHeadObj;
    public void OnKickHead()
    {
        Sound.PlaySound("Mod/kickHead");
        GameObject obj = SimplePool.Spawn(kickHeadObj, superEffertTrans.position, Quaternion.identity);
        obj.transform.SetParent(ModController.Instance.itemParent);
        obj.SetActive(true);
        OnSetPlayerIns(false);

        Transform trans =  modAnimations[11].transform ;
        trans.localScale = Vector3.one;
        OnShowModAnimation(11);
        tween = trans.DOScale(new Vector3(1, 0.75f, 1), 0.05f).SetLoops(-1);
        tween.Play();
        Invoke("OnEndKickHead", 2.1f);
    }
    void OnEndKickHead()
    {
        if(tween!=null) tween.Kill();
        OnShowModAnimation(-1);
        OnSetPlayerIns(true);
    }
    private float moveTimer = 0;
    private float moveDuration = 0.5f; // 每个方向的移动时间，单位秒
    private float moveSpeed = 8;    // 移动速度
    private bool movingUp = true;    // 当前移动方向
    private Vector3 startPos;        // 起始位置

    bool isZombie = false;
    float allZombieTime = 0;
    Coroutine ZombieMainTime;

   public void OnShowZomBie(float Time)
    {
        if (GameStatusController.isDead || Config.isLoading) return;
        Sound.PlaySound("smb_1-up");
        allZombieTime += Time;
        if (ZombieMainTime == null && !isZombie)
        {
            //if (PlayerController.Instance != null)
            //    PlayerController.Instance.OnChanleControl(true);
            isZombie = true;
            OnShowModAnimation(15);
            OnSetPlayerIns(false);
            ZombieMainTime = StartCoroutine(UpDownMovement());
        }
    }

    public LayerMask groundLayer; // 障碍物所在的层（如Ground层）

    IEnumerator UpDownMovement()
    {
        float lastTriggerTime = 0f; // 记录上次触发的时间
        lastTriggerTime = Time.time; // 更新触发时间
        while (allZombieTime > 0 && isZombie)
        {
            allZombieTime -= Time.deltaTime;
            if (!GameStatusController.isDead && !Config.isLoading
                && !ItemCreater.Instance.lockPlayer && !ItemCreater.Instance.isHang&& !ModVideoPlayerCreater.Instance.isBury)
            {
                RaycastHit2D hit = Physics2D.Raycast(
                    transform.position,  // 起点
                    Vector2.down,        // 方向向下
                    0.6f, groundLayer    // 检测距离
                );

                // 如果检测到地面，并且距离上次触发已超过0.5秒
                if (hit.collider != null && Time.time - lastTriggerTime > 0.5f)
                {
                    rigidbody2D.AddForce(new Vector2(0f, 620));
                    lastTriggerTime = Time.time; // 更新触发时间
                }
            }
            yield return null;
        }
        isZombie = false;
        ZombieMainTime = null;
        OnShowModAnimation(-1);
        OnSetPlayerIns(true);
    }
    [SerializeField] private GameObject bulletPrefab; // 子弹预制体
    [SerializeField] private Transform firePoint; // 发射点
    [SerializeField] private float spreadAngle = 30f; // 扇形角度
    [SerializeField] private float bulletSpacing = 0.2f; // 子弹间距（上下偏移）
    public void OnCreateBullet()
    {
        // 获取玩家朝向
        float direction = PlayerController.Instance._isFacingRight ? 1f : -1f;

        // 发射五颗子弹
        for (int i = 0; i < 5; i++)
        {
            // 创建子弹
            GameObject bullet = SimplePool.Spawn(bulletPrefab, firePoint.position, Quaternion.identity);
            bullet.transform.SetParent(ModController.Instance.itemParent);
            // 计算每颗子弹的偏移角度
            // 索引: 0,1,2,3,4 对应 -2,-1,0,1,2
            int offsetIndex = i - 2; // 中间为0，上下各两个
            Vector2 bulletDirection = Vector2.zero;

            if (offsetIndex == 0)
            {
                // 中间子弹，正前方
                bulletDirection = Vector2.right * direction;
            }
            else
            {
                // 上下各两发，使用角度偏移
                float angle = spreadAngle * offsetIndex * 0.5f; // 调整角度
                bulletDirection = Quaternion.Euler(0, 0, angle * direction) * (Vector2.right * direction);

            }

            // 设置子弹方向
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDirection(bulletDirection);
            }
            bullet.SetActive(true);
        }
    }

    public void OnTiggerDaoPlayer()
    {
        GameStatusController.IsBigPlayer = true;
        GameStatusController.IsFirePlayer = true;
        if (PlayerController.Instance != null) PlayerController.Instance.OnDaoMario();
    }

    public void OnTiggerQPianglayer()
    {
        GameStatusController.IsBigPlayer = true;
        GameStatusController.IsFirePlayer = true;
        if (PlayerController.Instance != null) PlayerController.Instance.OnQiangMario();
    }

    public void OnRandromPlayer(int index)
    {
        if(index>4)
             index = UnityEngine.Random.Range(0, 3);

        if (isInvincible)
        {
            animator.SetFloat("UltimateDuration_f", 11);
            animator.SetBool("Ultimate_b", false);
            isInvincible = false;
            invincibleTime = 0;
            Physics2D.IgnoreLayerCollision(8, 9, false);
            StopCoroutine(OnInvincible());
        }

        switch (index)
        {
            case 0:
                OnTiggerDaoPlayer();
                break;
            case 1:         
                OnTiggerQPianglayer();
                break;
            case 2:
                GameStatusController.IsBigPlayer = true;
                GameStatusController.IsFirePlayer = true;
                if (PlayerController.Instance != null) PlayerController.Instance.OnFireMario();
                break;
        }
    }

    public void OnChangScale(float value)
    {
        if (Config.playerScale>0)
        {
            Config.playerScale += value;
        }
        transform.localScale = new Vector3(Config.playerScale, Config.playerScale,1);
    }

    float invincibleTime = 0;
   public  bool isInvincible = false;

    public void OnSetInvincible()
    {
        Sound.PlayMusic("wudixing");
        invincibleTime += 10;
     
        if (!isInvincible)
        {
            OnSetInvincileState();
            isInvincible = true;
  
            StartCoroutine(OnInvincible());
        }
    }
    public void OnSetInvincileState()
    {
        GameStatusController.IsDaoPlayer = false;
        GameStatusController.IsQiangPlayer = false;

        if (!GameStatusController.IsBigPlayer)
            tag = "UltimatePlayer";
        else
            tag = "UltimateBigPlayer";

        PlayerController.Instance._isEatable = false;

        if (GameStatusController.IsBigPlayer)
            animator.SetTrigger("modBigInvincible");
        else
            animator.SetTrigger("modInvincible");
        Physics2D.IgnoreLayerCollision(8, 9, true);
    }
    IEnumerator OnInvincible()
    {
        while (invincibleTime>0) {

            invincibleTime -= Time.deltaTime;

            yield return null;
        }
        Sound.PlayMusic("background");
        invincibleTime = 0;
        tag = GameStatusController.PlayerTag;
        isInvincible = false;
        Physics2D.IgnoreLayerCollision(8, 9, false);
        if (GameStatusController.IsDaoPlayer)
        {
            animator.SetTrigger("TDao");
        }
        else if (GameStatusController.IsQiangPlayer)
        {
            animator.SetTrigger("TQiang");
        }
        else
        {
            animator.SetFloat("UltimateDuration_f", 11);
            animator.SetBool("Ultimate_b", false);
        }
        yield return new WaitForEndOfFrame();
        animator.SetFloat("UltimateDuration_f", 0);
    }

    float invisibilityTime = 0;
    public bool isInvisibility = false;

    public void OnSetInvisibilityState()
    {
        Sound.PlaySound("smb_1-up");
        invisibilityTime += 10;

        if (!isInvisibility)
        {
            OnSetPlayerIns(false);
            isInvisibility = true;
            StartCoroutine(OnInvisibility());
        }
    }

    IEnumerator OnInvisibility()
    {
        while (invisibilityTime > 0)
        {
            invisibilityTime -= Time.deltaTime;
            OnSetPlayerIns(false);
            yield return null;
        }
        OnSetPlayerIns(true);
        invisibilityTime = 0;
        isInvisibility = false;
    }
    private void OnDestroy()
    {
        isInvincible = false;
        StopAllCoroutines();
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
    }
}
