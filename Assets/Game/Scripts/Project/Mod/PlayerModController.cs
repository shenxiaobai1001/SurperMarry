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
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = !open;
        isKinematic = !open;
        Center.SetActive(open);
    }

    //设置玩家状态：是否被控制、是否显示主要角色
    public void OnSetPlayerContro(bool CanControl,bool show,bool closeModAni)
    {
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = !CanControl;
        PlayerController.Instance.OnChanleControl(!CanControl);
        Center.SetActive(CanControl);
        OnSetPlayerIns(show);
        if (closeModAni)
        {
            OnChanleModAni();
             OnShowModAnimation(-1);
        }
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
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(false);
        rigidbody2D.velocity = Vector3.zero;
        rigidbody2D.isKinematic = false;
        isKinematic = false;
        Center.SetActive(true);
    }

    public void OnSetPlayerIns(bool show)
    {
        PFunc.Log("OnSetPlayerIns", show);
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
        //OnShowModAnimation(-1);
       // OnSetPlayerIns(true);

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
        if (isInvincible)
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
       // if (isDance) return;
       // OnSetPlayerIns(index==-1);
        for (int i = 0; i < modAnimations.Count; i++) {
            modAnimations[i].gameObject.SetActive(i == index);
        }
    }

    public void OnMoveShowIcon()
    {
        OnSetPlayerIns(false);
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
    float superSpeed = 10;
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
    }
    public Animator OnGuangDance()
    {
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
        return atrDance;
    }
    bool isDaom = false;
    public Animator OnDMDance()
    {
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
        return atrDance;
    }
    Tween tween = null;
    public GameObject kickHeadObj;
    public Transform superEffertTrans;
    int kickHeadIndex = 0;
    public void OnKickHead(int index)
    {
        Sound.PlaySound("Mod/kickHead");
        GameObject obj = SimplePool.Spawn(kickHeadObj, superEffertTrans.position, Quaternion.identity);
        obj.transform.SetParent(ModController.Instance.itemParent);
        obj.SetActive(true);
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            OnSetPlayerContro(true, false, true);
            OnShowModAnimation(11);
        }
        kickHeadIndex = index;
        Transform trans =  modAnimations[11].transform ;
        trans.localScale = Vector3.one;
        tween = trans.DOScale(new Vector3(1, 0.75f, 1), 0.05f).SetLoops(-1);
        tween.Play();
        Invoke("OnEndKickHead", 2.1f);
    }
    void OnEndKickHead()
    {
        if(tween!=null) tween.Kill();
        if (!BarrageFuncController.Instance.OnCheckHasHighControl())
        {
            OnSetPlayerContro(true, true, true);
        }
        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, kickHeadIndex);
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

    public void OnChangScale(float value,int index)
    {
        if (Config.playerScale<0.1f)
            Config.playerScale = 0.1f;
        else
            Config.playerScale += value;
        PFunc.Log("OnChangScale", value, Config.playerScale);
        transform.localScale = new Vector3(Config.playerScale, Config.playerScale,1);
        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
    }

    float invincibleTime = 0;
    public  bool isInvincible = false;
    int invincibleIndex = 0;
    public void OnSetInvincible(int callIndex)
    {
        Sound.PlayMusic("wudixing");
        invincibleTime += 10;
     
        if (!isInvincible)
        {
            invincibleIndex= callIndex;
            OnSetInvincileState();
            isInvincible = true;
  
            StartCoroutine(OnInvincible());
        }
        else
        {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, callIndex);
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
        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, invincibleIndex);
        invincibleIndex = 0;
    }

    float invisibilityTime = 0;
    public bool isInvisibility = false;
    int invisibilityIndex = 0;

    public void OnSetInvisibilityState(int callIndex)
    {
        Sound.PlaySound("smb_1-up");
        invisibilityTime += 10;

        if (!isInvisibility)
        {
            invisibilityIndex = callIndex;
            OnSetPlayerIns(false);
            isInvisibility = true;
            StartCoroutine(OnInvisibility());
        }
        else
        {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, callIndex);
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
        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, invisibilityIndex);
    }

    private void OnDestroy()
    {
        isInvincible = false;
        StopAllCoroutines();
        EventManager.Instance.RemoveListener(Events.OnLazzerHit, OnLazzerHit);
    }
}
