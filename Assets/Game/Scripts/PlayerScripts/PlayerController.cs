using System.Collections;
using System.Xml.Serialization;
using SystemScripts;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance;

        public float speed = 410f;
        public float slideDownSpeed = 410f;
        public float jumpForce = 1050;
        public float _flagPos;
        public float _startInvincible;
        public float _invincibleTime;
        [Range(0, 1)] public float smoothTime = 0.6f;
        public bool isDead;
        public bool _isOnGround;
        public bool _isEatable;
        public bool _isFinish;
        public bool _isNotHugPole;
        public bool _isFacingRight;
        public bool _isGoingDownPipeAble;
        public bool _isAboveSpecialPipe;
        public bool isWalkingToCastle;
        public bool isInCastle;
        public bool isStopTime;
        public bool isInvulnerable;
        public bool isInvincible;
        private Vector3 _velocity;

        // 新增：输入状态变量
        private bool _isSprinting = false;
        private bool _isCrouching = false;

        [Header("GameObject Settings")] public GameObject playerSprite;
        public GameObject bigPlayer;
        public GameObject bigPlayerCollider;
        public GameObject smallPlayer;
        public GameObject smallPlayerCollider;
        public GameObject playerCol;
        public GameObject fireBallPrefab;
        public Transform fireBallParent;
        private Animator _playerAnim;
        [HideInInspector] public Rigidbody2D _playerRb;
        private AudioSource _playerAudio;

        [Header("AudioClip Settings")] public AudioClip jumpSound;
        public AudioClip jumpBigSound;
        public AudioClip flagPoleSound;
        public AudioClip pipeSound;
        public AudioClip dieSound;
        public AudioClip oneUpSound;
        public AudioClip turnBigSound;
        public AudioClip coinSound;
        public AudioClip kickSound;
        public AudioClip endGameSound;
        public AudioClip fireballSound;

        public  bool ishit;
        [HideInInspector]public bool isHit { get { return ishit; } set { /*Debug.Log(value); */ishit = value; } }

        private static readonly int IdleB = Animator.StringToHash("Idle_b");
        private static readonly int WalkB = Animator.StringToHash("Walk_b");
        private static readonly int RunB = Animator.StringToHash("Run_b");
        private static readonly int SpeedF = Animator.StringToHash("Speed_f");
        private static readonly int JumpTrig = Animator.StringToHash("Jump_trig");
        private static readonly int DieB = Animator.StringToHash("Die_b");
        private static readonly int BigB = Animator.StringToHash("Big_b");
        private static readonly int HugB = Animator.StringToHash("Hug_b");
        private static readonly int UltimateB = Animator.StringToHash("Ultimate_b");
        private static readonly int UltimateDurationF = Animator.StringToHash("UltimateDuration_f");
        private static readonly int CrouchB = Animator.StringToHash("Crouch_b");
        private static readonly int VulnerableB = Animator.StringToHash("Vulnerable_b");
        private static readonly int FireB = Animator.StringToHash("Fire_b");

        Vector3 startPos;
        void Awake()
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
            EventManager.Instance.AddListener(Events.ToGetPoleFlagPlayer,OnGetPoleFunc);
            isSpecialDie = false;
            _isFacingRight = true;
            isInvulnerable = false;
            if (GameStatusController.PlayerTag != null)
            {
                tag = GameStatusController.PlayerTag;
            }
            GameStatusController.IsDead = false;
            GameStatusController.IsGameFinish = false;
            GameStatusController.IsStageClear = false;
            GameStatusController.IsShowMessage = false;
            GameStatusController.IsBossBattle = false;
            _playerAudio = GetComponent<AudioSource>();
            _velocity = Vector3.zero;
            _playerAnim = GetComponent<Animator>();
            _playerRb = GetComponent<Rigidbody2D>();
            _isFinish = false;
            _isOnGround = true;
            isInCastle = false;
            getPole = false;
            startPos =transform.position;
            DieCoroutine = null;
            if (GameStatusController.IsDaoPlayer)
            {
                OnDaoMario();
            }
            else if(GameStatusController.IsQiangPlayer)
            {
                OnQiangMario();
            }
            else
            {
                TurnIntoBigPlayer();
            }
            if(Config.playerScale!=0)
                transform.localScale = new Vector3(Config.playerScale, Config.playerScale, 1);
        }
        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(Events.ToGetPoleFlagPlayer, OnGetPoleFunc);
        }

        bool isJumping = false;
        private void Update()
        {
            if (isHit) return;
            if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                // 修改1：J键 - 攻击/加速
                HandleJKeyInput();
                // 修改2：K键 - 跳跃
                if (Input.GetKeyDown(KeyCode.K) && _isOnGround || Input.GetKeyDown(KeyCode.Space) && _isOnGround)
                {
                    _playerAudio.PlayOneShot(GameStatusController.IsBigPlayer ? jumpSound : jumpBigSound);
                    _isOnGround = false;
                    _playerRb.velocity = Vector3.zero;
                   _playerRb.AddForce(new Vector2(0f, jumpForce));
                    _playerAnim.SetBool(IdleB, false);
                    _playerAnim.SetBool(WalkB, false);
                    _playerAnim.SetBool(RunB, false);
                    _playerAnim.SetTrigger(JumpTrig);
                    isJumping = true;
                }

                DenyMidAirJump();

                // 修改3：S键 - 下蹲
                HandleSKeyInput();

                // 修改4：AD键 - 左右移动和转向
                HandleMovementInput();

                if (_isGoingDownPipeAble)
                {
                    if (CompareTag("Player"))
                    {
                        smallPlayerCollider.SetActive(false);
                    }
                    else if (CompareTag("BigPlayer"))
                    {
                        bigPlayerCollider.SetActive(false);
                    }

                    _playerRb.isKinematic = true;
                    transform.Translate(slideDownSpeed / 2.5f * Time.deltaTime * Vector3.down);
                }
                if (transform.position.y <= -2 && checkYpos&&!isDead)
                {
                    checkYpos = false;
                    GameStatusController.IsDead = true;
                    isRest = true;
                }

                if (GameStatusController.IsHidden&& transform.position.y<25)
                {
                    GameStatusController.IsHidden = false;
                    GameStatusController.HiddenMove = false;
                }
            }
        
        }
        bool isRest = false;
        bool checkYpos = true;
        // 新增方法：处理J键输入（攻击/加速）
        private void HandleJKeyInput()
        {
            // J键按下：发射火球（攻击）
            if (Input.GetKeyDown(KeyCode.J) && GameStatusController.IsFirePlayer || Input.GetMouseButtonDown(0) && GameStatusController.IsFirePlayer)
            {
                if (GameStatusController.IsDaoPlayer)
                {
                    Sound.PlaySound("Mod/fire");
                    _playerAnim.SetTrigger("DaoAtk");
                    ItemCreater.Instance.OnCreateDaoQI(1);
                }
                else if (GameStatusController.IsQiangPlayer)
                {
                    _playerAnim.SetTrigger("QiangAtk");
                    Sound.PlaySound("Mod/fire");
                    PlayerModController.Instance.OnCreateBullet();
                }
                 else
                {
                    Instantiate(fireBallPrefab, fireBallParent.position, fireBallParent.rotation);
                    _playerAudio.PlayOneShot(fireballSound);
                }
            }

            // J键按住：加速
            if (Input.GetKey(KeyCode.J)|| Input.GetKey(KeyCode.LeftShift))
            {
                _isSprinting = true;
                speed = 310;
               jumpForce = 720;
            }
            else if (Input.GetKeyUp(KeyCode.J) || Input.GetKey(KeyCode.LeftShift))
            {
                _isSprinting = false;
                speed = 240;
               jumpForce = 620;
            }
        }

        // 新增方法：处理S键输入（下蹲）
        private void HandleSKeyInput()
        {
            // S键按下：下蹲或进入管道
            if (Input.GetKeyDown(KeyCode.S) && _isAboveSpecialPipe || Input.GetKeyDown(KeyCode.S) && isHidden)
            {
                PlayerModController.Instance.OnChangeStateFalse();
                _playerAudio.PlayOneShot(pipeSound);
                _isGoingDownPipeAble = true;
                _playerRb.velocity = Vector2.zero;
                StartCoroutine(StopGoingDownPipe());
            }

            // S键按住：持续下蹲（仅限大马里奥）
            _isCrouching = Input.GetKey(KeyCode.S);
        }

        // 新增方法：处理移动输入（AD键）
        private void HandleMovementInput()
        {
            // A键：向左移动和转向
            if (Input.GetKey(KeyCode.A) && !_isCrouching)
            {
                if (_isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    _isFacingRight = false;
                }
            }
            // D键：向右移动和转向
            else if (Input.GetKey(KeyCode.D) && !_isCrouching)
            {
                if (!_isFacingRight)
                {
                    transform.Rotate(0, 180, 0);
                    _isFacingRight = true;
                }
            }
        }

        public float jumpingForce;

        float jumpTime=0;
        private void FixedUpdate()
        {
            if (GameStatusController.IsGameFinish)
            {
                transform.Translate(slideDownSpeed / 1.25f * Time.deltaTime * Vector3.right);
            }
            if ((Input.GetKey(KeyCode.K) && isJumping || Input.GetKey(KeyCode.Space) && isJumping) && jumpTime < 1)
            {
                _playerAnim.SetTrigger(JumpTrig);
                _playerRb.AddForce(new Vector2(0f, jumpingForce), ForceMode2D.Force);
                jumpTime += Time.deltaTime;
            }
            else
            {
                isJumping = false;
                jumpTime = 0;
            }

            if (Input.GetKeyUp(KeyCode.K) && isJumping || Input.GetKeyUp(KeyCode.Space) && isJumping)
            {
                isJumping = false;
                jumpTime = 0;
            }

            isDead = GameStatusController.IsDead;

            if (isInvincible)
            {
                _invincibleTime = Time.time - _startInvincible;
                _playerAnim.SetFloat(UltimateDurationF, _invincibleTime);
                Physics2D.IgnoreLayerCollision(8, 9, true);
                if (Time.time - _startInvincible > 10)
                {
                    StartCoroutine(BeNormal());
                }
            }

            if (isInvulnerable)
            {
                Physics2D.IgnoreLayerCollision(8, 9, true);
                StartCoroutine(BeVulnerable());
            }
            if (isDead)
            {
                checkYpos = false;
                Die();
            }
            else if (!isDead && !_isFinish && !GameStatusController.IsGameFinish)
            {
                //_playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
                //_playerAnim.SetBool(FireB, GameStatusController.IsFirePlayer);
                //ChangeAnim();
                MovePlayer();
                GetPlayerSpeed();
            }

            if (_isFinish)
            {
                if (transform.position.y >= 1)
                {
                    _playerAnim.SetBool(HugB, true);
                    _playerAnim.SetFloat(SpeedF, 0);
                    transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.down);
                }
                else
                {
                    if (!getFlag) return;
                    if (transform.position.x < _flagPos + 0.8f)
                    {
                        _playerAnim.SetBool(HugB, false);
                        transform.localScale = new Vector3(-1, 1, 1);
                        transform.position = new Vector3(_flagPos + 0.8f, transform.position.y);
                    }

                    StartCoroutine(HugPole());
           
                    if (_isNotHugPole)
                    {
                        _playerRb.isKinematic = false;
                        playerCol.SetActive(true);
                        transform.localScale = Vector3.one;
                        _playerAnim.SetFloat(SpeedF, 3f);
                        transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.right);
                    }
                }
            }
        }
        public void OnChanleControl(bool toHit)
        {
            _playerRb.velocity = Vector2.zero;
            horizontalInput = 0;
            isHit = toHit;
        }
        float horizontalInput = 0f;
        private void MovePlayer()
        {
            horizontalInput = 0;
            if (!_isCrouching && !_isGoingDownPipeAble && !isHit)
            {
                // 修改：使用AD键获取水平输入
                if (Input.GetKey(KeyCode.A) && !isHit) horizontalInput = -1f;
                if (Input.GetKey(KeyCode.D) && !isHit) horizontalInput = 1f;
                // 直接设置固定的水平速度，移除平滑效果
                Vector2 playerVelocity = _playerRb.velocity;
                playerVelocity.x = horizontalInput * speed * Time.fixedDeltaTime;
                _playerRb.velocity = playerVelocity;
            }
            // 修改：使用S键下蹲
            if (_isCrouching && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) && !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, true);
                smallPlayerCollider.SetActive(true);
                bigPlayerCollider.SetActive(false);
            }
            else if (!_isCrouching && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) && !_isAboveSpecialPipe)
            {
                _playerAnim.SetBool(CrouchB, false);
                smallPlayerCollider.SetActive(false);
                bigPlayerCollider.SetActive(true);
            }
        }

        bool getFlag = false;
        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("EnemyBody")&&!isInvincible&&!PlayerModController.Instance.isInvincible)
            {
                OnDieNFunc();
                return;
            }
            else if (other.gameObject.CompareTag("Arrow"))
            {
                PFunc.Log("OnCollisionEnter2DArrow", isSpecialDie);
                isSpecialDie = true;
                OnDieNFunc();
                return;
            }
            if (other.gameObject.CompareTag("Ground") || other.gameObject.CompareTag("Pipe") ||
                other.gameObject.CompareTag("Brick") || other.gameObject.CompareTag("Stone") ||
                other.gameObject.CompareTag("SpecialPipe"))
            {
                _isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }

            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = false;
            }
            //PFunc.Log(other.gameObject.tag, getPole);
            if (other.gameObject.CompareTag("Pole"))
            {
                OnGetPole(other.gameObject);
            }

            if (other.gameObject.CompareTag("Castle"))
            {
                isInCastle = true;
                isWalkingToCastle = false;
                playerSprite.SetActive(false);
            }

            if (other.gameObject.CompareTag("DeathAbyss"))
            {
                //_playerAudio.PlayOneShot(dieSound);
                OnDieNFunc();
                return;
            }

            if (other.gameObject.CompareTag("1UpMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(oneUpSound);
                GameStatusController.Live += 1;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("BigMushroom") && _isEatable)
            {
                OnChanleControl(true);
                _playerAnim.SetFloat(SpeedF, 0);
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true;
                _playerAudio.PlayOneShot(turnBigSound);
                GameStatusController.IsBigPlayer = true;
                GameStatusController.PlayerTag = "BigPlayer";
                tag = GameStatusController.PlayerTag;
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("UltimateStar") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                if (CompareTag("Player"))
                {
                    tag = "UltimatePlayer";
                }
                else
                {
                    tag = "UltimateBigPlayer";
                }

                isInvincible = true;
                _playerAnim.SetBool(UltimateB, isInvincible);
                _startInvincible = Time.time;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("Player") || CompareTag("UltimatePlayer")) &&
                _isEatable)
            {
                OnChanleControl(true);
                _playerAnim.SetFloat(SpeedF, 0);
                _playerAudio.PlayOneShot(turnBigSound);
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true;
                if (GameStatusController.IsBigPlayer)
                {
                    GameStatusController.IsFirePlayer = true;
                    tag = "UltimateBigPlayer";
                }
                else
                {
                    GameStatusController.IsBigPlayer = true;
                    GameStatusController.PlayerTag = "BigPlayer";
                    tag = GameStatusController.PlayerTag;
                }
               
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                _isEatable)
            {
                OnChanleControl(true);
                _playerAnim.SetFloat(SpeedF, 0);
                _playerAudio.PlayOneShot(turnBigSound);
                _playerRb.velocity = Vector2.zero; 
                _playerRb.isKinematic = true;
                GameStatusController.IsFirePlayer = true;
                TurnIntoBigPlayer();
                tag = "UltimateBigPlayer";
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("SpecialPipe"))
            {
                _isAboveSpecialPipe = true;
            }

            if (other.gameObject.CompareTag("KoopaShell"))
            {
                _playerAudio.PlayOneShot(kickSound);
            }
            if (other.gameObject.CompareTag("NPC"))
            {
                findNpc = true;
            }
            if (other.gameObject.CompareTag("HorSpecialPipe"))
            {
                ModController.Instance.OnModPause();
                OnInHorSpecialPipe();
            }
       
        }
        bool getPole = false;

        void OnGetPoleFunc(object  msg)
        {
            if(msg == null|| getPole) return;
            getFlag = true;
            OnGetPole((GameObject)msg);
        }
        void OnGetPole(GameObject other)
        {
            PlayerModController.Instance.isSuperMan = false;
            getPole = true;
            Sound.PauseOrPlayVolumeMusic(true);
            ModController.Instance.OnModPause();
            _playerAudio.PlayOneShot(flagPoleSound);
            _flagPos = other.gameObject.transform.position.x;
            _isFinish = true;
            _playerRb.velocity = Vector2.zero;
            _playerRb.isKinematic = true;
            isWalkingToCastle = true;
            isStopTime = true;
            transform.localEulerAngles = Vector3.zero;
            transform.position = new Vector3(_flagPos, transform.position.y, transform.position.z);
            EventManager.Instance.SendMessage(Events.ToGetPoleFlag);
            StartCoroutine(PlayStageClearSound());
        }

        void OnDieNFunc()
        {
            if (!GameStatusController.IsBigPlayer)
            {
                // StartCoroutine(Die(other.gameObject));
                PFunc.Log("OnCollisionEnter2D", isInvulnerable);
                if (!isInvulnerable)
                {
                    Die();
                }
                else
                {
                    if(GetComponent<Collider2D>()&& smallPlayerCollider.GetComponent<Collider2D>())
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), smallPlayerCollider.GetComponent<Collider2D>());
                }
            }
            else if (CompareTag("BigPlayer")|| CompareTag("UltimateBigPlayer"))
            {
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                gameObject.tag = GameStatusController.PlayerTag;
                TurnIntoBigPlayer();
                isInvulnerable = true;
            }
        }

        public void OnFireMario(){

            if (GameStatusController.IsDaoPlayer || GameStatusController.IsQiangPlayer)
            {
                _playerAnim.SetTrigger("TFire");
                GameStatusController.IsDaoPlayer = false;
                GameStatusController.IsQiangPlayer = false;
            }
            GameStatusController.PlayerTag = "BigPlayer";
            tag = GameStatusController.PlayerTag;
            _playerAudio.PlayOneShot(turnBigSound);
            TurnIntoBigPlayer();
        _isEatable = false;
        }
        public void OnDaoMario()
        {
            GameStatusController.IsQiangPlayer = false;
            GameStatusController.IsDaoPlayer = true;
            _playerAnim.SetTrigger("Dao_b");
            GameStatusController.PlayerTag = "BigPlayer";
            tag = GameStatusController.PlayerTag;
            _playerAudio.PlayOneShot(turnBigSound);
            TurnIntoBigPlayer();
            _isEatable = false;
        }
        public void OnQiangMario()
        {
            GameStatusController.IsDaoPlayer = false;
            GameStatusController.IsQiangPlayer = true;
            _playerAnim.SetTrigger("Qiang_b");
            GameStatusController.PlayerTag = "BigPlayer";
            tag = GameStatusController.PlayerTag;
            _playerAudio.PlayOneShot(turnBigSound);
            TurnIntoBigPlayer();
            _isEatable = false;
        }
        private void OnCollisionExit2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("PowerBrick"))
            {
                _isEatable = true;
            }
        }
        public Vector3 bossPkPos;
        public bool isHidden = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Princess"))
            {
                slideDownSpeed = 0;
                GameStatusController.IsShowMessage = true;
                _playerAnim.SetFloat(SpeedF, 0);
            }
            if (other.gameObject.CompareTag("BoosPK"))
            {
                bossPkPos= other.transform.position;
                GameStatusController.IsBossBattle = true;
            }
            if (other.gameObject.CompareTag("Axe"))
            {
                _playerAudio.PlayOneShot(endGameSound);
                Destroy(other.gameObject);
                GameStatusController.IsBossBattle = false;
                GameStatusController.IsGameFinish = true;
                _playerAnim.SetFloat(SpeedF, 3f);
                _playerRb.velocity = Vector2.zero;
            }

            if (other.gameObject.CompareTag("Coin"))
            {
                _playerAudio.PlayOneShot(coinSound);
            }
            if (other.gameObject.CompareTag("Hidden"))
            {
           
                isHidden = true;
            }
            if (other.gameObject.CompareTag("outHidden"))
            {
                StartCoroutine(OutHiddenPass());
            }
            if (other.gameObject.CompareTag("Flag"))
            {
                getFlag = true;
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Hidden")&&! _isGoingDownPipeAble)
            {
                isHidden = false;
            }
        }

        public void TurnIntoBigPlayer()
        {
            if (GameStatusController.IsBigPlayer)
            {
                bigPlayer.SetActive(GameStatusController.IsBigPlayer);
                smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
            }
            _playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
            _playerAnim.SetBool(FireB, GameStatusController.IsFirePlayer);
            
        
           // ChangeAnim();
            if (CompareTag("Player"))
            {
                isInvulnerable = true;
                OnStartFilker();
            }
            else
            {     
                isInvulnerable = true;
                OnStartFilker();
            }
        }
        public GameObject aniParent;
        Vector3 deadPos=Vector3.zero;
        public void OnStartFilker()
        {
            StartCoroutine(OnFliker());
        }

        IEnumerator OnFliker()
        {
            yield return new WaitForSeconds(0.7f);
            if (!GameStatusController.IsBigPlayer)
            {
                GameStatusController.IsDaoPlayer = false;
                GameStatusController.IsQiangPlayer = false;
                bigPlayer.SetActive(GameStatusController.IsBigPlayer);
                smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
            }
            _playerRb.velocity = Vector2.zero;
            _playerRb.isKinematic = false;
            ChangeCollider();
            OnChanleControl(false);
            if (PlayerModController.Instance.isInvincible)
            {
                PlayerModController.Instance.OnSetInvincileState();
            }
        }

        private void Die()
        {
            //playerCol.SetActive(false);
            //_playerRb.isKinematic = true;
            //_playerAnim.SetBool(DieB, isDead);
            GameStatusController.IsDead = true;
            OnDieFunc();
        }
        Coroutine DieCoroutine = null;
        bool isSpecialDie = false;
        void OnDieFunc()
        {
            GameStatusController.IsDaoPlayer = false;
            GameStatusController.IsQiangPlayer = false;
            GameStatusController.IsBigPlayer = false;
            GameStatusController.IsFirePlayer = false;

            if (isSpecialDie)
            {
                if (DieCoroutine == null)
                {
                    Sound.PlaySound("smb_mariodie");
                    PlayerModController.Instance.OnSetPlayerIns(false);
                    PlayerModController.Instance.OnChangeState(false);
                    deadPos = isRest ? new Vector3(-2, 0) : new Vector3(transform.position.x, transform.position.y+2, transform.position.z);
                    DieCoroutine = StartCoroutine(DieAnim());
                }
            }
            else
            {
                if (DieCoroutine == null)
                {
                    Sound.PlaySound("smb_mariodie");
                    PlayerModController.Instance.OnChangeState(false);
                    deadPos = isRest ? new Vector3(-2, 4) : new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                    DieCoroutine = StartCoroutine(NormalDie());
                }
            }

        }
       public void OnHalfDieFunc()
        {
            GameStatusController.IsBigPlayer = false;
            GameStatusController.IsFirePlayer = false;
            isSpecialDie = true;

            if (DieCoroutine == null)
            {
                Sound.PlaySound("smb_mariodie");
                PlayerModController.Instance.OnSetPlayerIns(false);
                PlayerModController.Instance.OnChangeState(false);
                deadPos = isRest ? new Vector3(-2, 0) : new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                DieCoroutine = StartCoroutine(DieAnim());
                GameStatusController.isDead = true;
            }
        }

        private void GetPlayerSpeed()
        {
            if (!hasToFindNpc)
            {
                _playerAnim.SetFloat(SpeedF, Mathf.Abs(_playerRb.velocity.x));
            }
        }

        public void ChangeAnim()
        {
            bigPlayer.SetActive(GameStatusController.IsBigPlayer);
            smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
        }

        public void ChangeCollider()
        {
            bigPlayerCollider.SetActive(GameStatusController.IsBigPlayer);
            smallPlayerCollider.SetActive(!GameStatusController.IsBigPlayer);
        }

        private void DenyMidAirJump()
        {
            if (_playerRb.velocity.y > 0 || _playerRb.velocity.y < 0)
            {
                //_isOnGround = false;
                _playerAnim.SetBool(IdleB, false);
                _playerAnim.SetBool(WalkB, false);
                _playerAnim.SetBool(RunB, false);
            }
            else
            {
                //_isOnGround = true;
                _playerAnim.SetBool(IdleB, true);
                _playerAnim.SetBool(WalkB, true);
                _playerAnim.SetBool(RunB, true);
            }
        }

        private IEnumerator SetBoolEatable()
        {
            yield return new WaitForSeconds(0.5f);
            _isEatable = true;
        }

        private IEnumerator HugPole()
        {
            yield return new WaitForSeconds(1.5f);
            _isNotHugPole = true;
        }
        public PlayerBreak playerBreak;
        private IEnumerator DieAnim()
        {
            playerCol.SetActive(false);
            if (playerBreak)
            {
                Sound.PlaySound("Mod/die");
                playerBreak.gameObject.SetActive(true);
                yield return playerBreak.OnAddAllForceIE();
            }
            ModData.mLife--;

            if (ModData.mLife < 0)
            {
                yield return StartCoroutine(LoadingScene());
            }
            else if (isRest || GameManager.Instance.time<=0)
            {
                ModController.Instance.OnModPause();
                EventManager.Instance.SendMessage(Events.OnRestBreakBrick);
                GameStatusController.IsDead = false;
                ResetPlayerState(startPos);
                isInvulnerable = true;
                _playerAnim.SetTrigger("toDead");
                Camera.main.transform.position = new Vector3(1.5f, 5, -10);
                Config.isLoading = false;
            }
            else
            {
                Sound.PlaySound("Mod/life");
                yield return playerBreak.OnHuifuIE();
                GameStatusController.IsDead = false;
                ResetPlayerState(deadPos);
                isInvulnerable = true;
                _playerAnim.SetTrigger("toDead");

            }
            DieCoroutine = null;
        }

        IEnumerator NormalDie()
        {
            ModController.Instance.OnModPause();
            PFunc.Log("玩家死亡");
            playerCol.SetActive(false);
            _playerAnim.SetBool(DieB,true);
            float y = transform.position.y + 3;
            while (transform.position.y < y)
            {
                transform.Translate(Vector3.up * 10 * Time.deltaTime);
                yield return null;
            }

            while (transform.position.y>-3)
            {
                transform.Translate(Vector3.down * 10 * Time.deltaTime);
                yield return null;
            }
      
            ModData.mLife--;
            if (ModData.mLife < 0)
            {
                yield return StartCoroutine(LoadingScene());
            }
            else {
                EventManager.Instance.SendMessage(Events.OnRestBreakBrick);
              
                yield return new WaitForSeconds(1);
                GameStatusController.IsDead = false;
                ResetPlayerState(startPos);
                isInvulnerable=true;
                _playerAnim.SetTrigger("toDead");
                Camera.main.transform.position = new Vector3(1.5f, 5, -10);
           
                Config.isLoading = false;
            }
            DieCoroutine = null;
        }


        private IEnumerator HalfDieAnim()
        {
            playerCol.SetActive(false);

            ModData.mLife--;

            if (ModData.mLife < 0)
            {
                yield return StartCoroutine(LoadingScene());
            }
            else if (isRest || GameManager.Instance.time <= 0)
            {
                ModController.Instance.OnModPause();
                EventManager.Instance.SendMessage(Events.OnRestBreakBrick);
                GameStatusController.IsDead = false;
                ResetPlayerState(startPos);
                isInvulnerable = true;
                _playerAnim.SetTrigger("toDead");
                Camera.main.transform.position = new Vector3(1.5f, 5, -10);
                Config.isLoading = false;
            }
            else
            {
                Sound.PlaySound("Mod/life");
                yield return playerBreak.OnHuifuIE();
                GameStatusController.IsDead = false;
                ResetPlayerState(deadPos);
                isInvulnerable = true;
                DieCoroutine = null;
            }

        }

        private static IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(0.1f);
            if (ModData.mLife <= 0)
            {
                ModController.Instance.statusController.mLife = 30;
                ModData.mLife = 30;
                GameModController.Instance.OnLoadScene("1-1");
            }
            else
            {
                string passName= GameModController.Instance.nowPos;
                GameModController.Instance.OnLoadScene(passName);
            }

        }

        private IEnumerator PlayStageClearSound()
        {
            yield return new WaitForSeconds(1.5f);
            GameStatusController.IsStageClear = true;
        }

        private IEnumerator BeVulnerable()
        {
            yield return new WaitForSeconds(2);
            _playerAnim.SetBool(VulnerableB, true);
            Physics2D.IgnoreLayerCollision(8, 9, false);
            isInvulnerable = false;
        }

        private IEnumerator BeNormal()
        {
            yield return new WaitForSeconds(2);
            tag = GameStatusController.PlayerTag;
            isInvincible = false;
            _playerAnim.SetBool(UltimateB, isInvincible);
            Physics2D.IgnoreLayerCollision(8, 9, false);
        }

        private IEnumerator StopGoingDownPipe()
        {
            yield return new WaitForSeconds(1.5f);
            _isGoingDownPipeAble = false;

            if (_isAboveSpecialPipe)
            {
                _isAboveSpecialPipe = false;
                Loaded.OnLoadScence("Excessive");
            }
            else if(isHidden)
            {
                GameStatusController.IsHidden = true;
                GameStatusController. HiddenMove = Config.passIndex > 1;
                 isHidden = false;
                transform.position = GameManager.Instance.hiddenEnterPos.position;
                Camera.main.transform.position = new Vector3(20, 37, -10);
                PlayerModController.Instance.OnChangeStateFalse();
                ChangeCollider();
            }
        }
        private IEnumerator OutHiddenPass()
        {
            _playerRb.velocity = Vector2.zero;
            _playerAudio.PlayOneShot(pipeSound);
            PlayerModController.Instance.OnChangeStateTrue();

            float allTime = 1f;
            float time = 0;

            // 第一部分：水平移动
            while (time < allTime)
            {
                transform.Translate(Vector2.right * 1 * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }

            transform.position = GameManager.Instance.hiddenOutPos.position;
            GameStatusController.IsHidden = false;
            GameStatusController.HiddenMove = false;

            // 第二部分：向上移动（修改部分）
            float addValue = GameStatusController.IsBigPlayer ? 2.5f : 2;
            Vector3 startPos = transform.position;  // 记录起始位置
            Vector3 targetPos = new Vector3(transform.position.x, transform.position.y + addValue, transform.position.z);

            time = 0;  // 重置时间

            while (time < allTime)
            {
                // 使用线性插值，time从0到1，t也从0到1
                float t = time / allTime;  // 计算插值比例
                transform.position = Vector3.Lerp(startPos, targetPos, t);
                time += Time.deltaTime;
                yield return null;
            }

            // 确保最终位置精确
            transform.position = targetPos;

            isHidden = false;
            PlayerModController.Instance.OnChangeStateFalse();
        }
        void OnInHorSpecialPipe()
        {
            StartCoroutine(OnInHorSpecialPipeIE());
        }
        private IEnumerator OnInHorSpecialPipeIE()
        {
            _playerRb.velocity = Vector2.zero;
            _playerAudio.PlayOneShot(pipeSound);
            PlayerModController.Instance.OnChangeStateTrue();

            float allTime = 1f;
            float time = 0;

            // 第一部分：水平移动
            while (time < allTime)
            {
                transform.Translate(Vector2.right * 1 * Time.deltaTime);
                time += Time.deltaTime;
                yield return null;
            }
            Loaded.OnLoadScence("Excessive");

        }
        // 在PlayerController类中添加以下方法
        public void ResetPlayerState(Vector3? startPosition = null)
        {
            PlayerModController.Instance.OnSetPlayerIns(true);
            PlayerModController.Instance.OnChangeState(true);
            PlayerModController.Instance.OnChanleModAni();
            PlayerModController.Instance.isSuperMan = false;

            checkYpos = true;
            // 1. 停止所有正在运行的协程
            StopAllCoroutines();
            isRest = false;
            // 2. 重置核心状态变量
            isDead = false;
            isHit = false;
            _isOnGround = true;
            _isEatable = false;
            _isFinish = false;
            _isNotHugPole = false;
            _isFacingRight = true;
            _isGoingDownPipeAble = false;
            _isAboveSpecialPipe = false;
            isWalkingToCastle = false;
            isInCastle = false;
            isStopTime = false;
            isInvulnerable = false;
            isInvincible = false;
            isSpecialDie = false;
            isRest = false;
            getFlag = false;
            // 重置输入状态变量
            _isSprinting = false;
            _isCrouching = false;

            // 重置速度相关变量
            speed = 240;
            slideDownSpeed = 5;
            jumpForce = 620;

    
            // 确保面向右侧
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            if (!_isFacingRight)
            {
                transform.Rotate(0, 180, 0);
            }
            _isFacingRight = true;

            // 4. 重置物理组件
            if (_playerRb != null)
            {
                _playerRb.velocity = Vector2.zero;
                _playerRb.angularVelocity = 0f;
                _playerRb.isKinematic = false;
                _playerRb.simulated = true;
            }                  
            // 6. 重置碰撞体和游戏对象状态
            playerSprite.SetActive(true);
            playerCol.SetActive(true);
            // 3. 重置位置和旋转
            if (startPosition.HasValue)
            {
                transform.position = startPosition.Value;
            }

            if (_playerAnim)
            {
                _playerAnim.Rebind();
                _playerAnim.Update(0f);
            }

            // 根据当前标签设置正确的碰撞体
            if (GameStatusController.PlayerTag != null)
            {
                tag = GameStatusController.PlayerTag;
            }

            // 根据标签激活/停用大小玩家碰撞体
            bigPlayer.SetActive(GameStatusController.IsBigPlayer);
            smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
            ChangeCollider();
            findNpc = false;
            // 7. 重置层碰撞
            Physics2D.IgnoreLayerCollision(8, 9, false);

            // 8. 重置计时器
            _startInvincible = 0f;
            _invincibleTime = 0f;

            // 9. 重置音频（如果需要）
            if (_playerAudio != null && _playerAudio.isPlaying)
            {
                _playerAudio.Stop();
            }

            // 10. 重置速度插值
            _velocity = Vector3.zero;

            // 11. 重置全局状态（如果这些是玩家相关的状态）
            // 注意：这些可能会在全局状态控制器中处理
            GameStatusController.IsDead = false;
            GameStatusController.IsBigPlayer = false;
            GameStatusController.IsFirePlayer = false;
            GameStatusController.IsGameFinish = false;
            GameStatusController.IsStageClear = false;
            GameStatusController.IsShowMessage = false;
            GameStatusController.IsBossBattle = false;
            GameStatusController.IsDaoPlayer = false;
            GameStatusController.IsQiangPlayer = false;
            GameManager.Instance.time = 400;

            // 12. 设置玩家标签为初始状态
            GameStatusController.PlayerTag = "Player";
            tag = "Player";
            PlayerModController.Instance.OnChangeState(true);
            PlayerModController.Instance.OnSetPlayerIns(true);
            // 13. 启用脚本（如果被禁用）
            enabled = true;
            getPole = false;
            hasToFindNpc = false;
            transform.localScale = Vector3.one;
            if (PlayerModController.Instance.isInvincible)
            {
                PlayerModController.Instance.OnSetInvincileState();
            }
            ModData.deadCount++;
        }

        // 可选：添加一个重载方法，使用默认位置
        public void ResetPlayerState()
        {
            ResetPlayerState(startPos);
        }
        bool hasToFindNpc = false;
        bool findNpc = false;
        public void OnToFindNpc()
        {
            hasToFindNpc = true;
            _playerRb.velocity = Vector2.zero;
            findNpc = false;
            _playerAnim.SetFloat(SpeedF, 0);
            StartCoroutine(OnFindNpc());
        }
        IEnumerator OnFindNpc()
        {
            if (!_isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                _isFacingRight = true;
            }
            PlayerModController.Instance.OnChangeState(true);
            _playerAnim.SetFloat(SpeedF, 3f);
           
            _playerAnim.SetBool(RunB, true);
            while (!findNpc)
            {
                transform.Translate(Vector3.right * 10 * Time.deltaTime);
                yield return null;  
            }
            _playerRb.velocity = Vector2.zero;
            _playerAnim.SetFloat(SpeedF, 0);
        }
    }
}