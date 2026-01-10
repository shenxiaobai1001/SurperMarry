using System.Collections;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.IMGUI.Controls.PrimitiveBoundsHandle;

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

        [HideInInspector]public bool isHit = false;

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
            startPos=transform.position;
        }

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
                    _playerAnim.SetTrigger(JumpTrig);
                    _playerRb.AddForce(new Vector2(0f, jumpForce));
                    _playerAnim.SetBool(IdleB, false);
                    _playerAnim.SetBool(WalkB, false);
                    _playerAnim.SetBool(RunB, false);
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
                Instantiate(fireBallPrefab, fireBallParent.position, fireBallParent.rotation);
                _playerAudio.PlayOneShot(fireballSound);
            }

            // J键按住：加速
            if (Input.GetKey(KeyCode.J))
            {
                _isSprinting = true;
                speed = 600;
                jumpForce = 1180;
            }
            else if (Input.GetKeyUp(KeyCode.J))
            {
                _isSprinting = false;
                speed = 410;
                jumpForce = 1050;
            }
        }

        // 新增方法：处理S键输入（下蹲）
        private void HandleSKeyInput()
        {
            // S键按下：下蹲或进入管道
            if (Input.GetKeyDown(KeyCode.S) && _isAboveSpecialPipe|| Input.GetKeyDown(KeyCode.S) && isHidden)
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

        private void FixedUpdate()
        {
            if (isHit) return;
            if (GameStatusController.IsGameFinish)
            {
                transform.Translate(slideDownSpeed / 1.25f * Time.deltaTime * Vector3.right);
                //if (Input.GetKeyDown(KeyCode.Backspace))
                //{
                //    GameStatusController.IsGameFinish = false;
                //    GameStatusController.IsShowMessage = false;
                //    SceneManager.LoadScene(0);
                //}
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
                _playerAnim.SetBool(BigB, GameStatusController.IsBigPlayer);
                _playerAnim.SetBool(FireB, GameStatusController.IsFirePlayer);
                ChangeAnim();
                MovePlayer();
                GetPlayerSpeed();
            }

            if (_isFinish)
            {
                if (transform.position.y >= 1.5f)
                {
                    _playerAnim.SetBool(HugB, true);
                    _playerAnim.SetFloat(SpeedF, 0);
                    transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.down);
                }
                else
                {
                    if (transform.position.x < _flagPos + 0.8f)
                    {
                        _playerAnim.SetBool(HugB, false);
                        transform.localScale = new Vector3(-1, 1, 1);
                        transform.position = new Vector3(_flagPos + 0.8f, transform.position.y);
                    }

                    _playerRb.isKinematic = false;
                    StartCoroutine(HugPole());
           
                    if (_isNotHugPole)
                    {
                        transform.localScale = Vector3.one;
                        _playerAnim.SetFloat(SpeedF, 3f);
                        transform.Translate(slideDownSpeed * Time.deltaTime * Vector3.right);
                    }
                }
            }
        }
        float hugPoleTime = 1.5f;
        float hugTime = 0;

        private void MovePlayer()
        {
            if (!_isCrouching && !_isGoingDownPipeAble)
            {
                // 修改：使用AD键获取水平输入
                float horizontalInput = 0f;
                if (Input.GetKey(KeyCode.A)) horizontalInput = -1f;
                if (Input.GetKey(KeyCode.D)) horizontalInput = 1f;

                Vector2 playerVelocity = _playerRb.velocity;
                Vector3 targetVelocity = new Vector2(horizontalInput * speed * Time.fixedDeltaTime, playerVelocity.y);
                _playerRb.velocity = Vector3.SmoothDamp(playerVelocity, targetVelocity, ref _velocity, smoothTime);
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

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (other.gameObject.CompareTag("Arrow"))
            {
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

            if (other.gameObject.CompareTag("Pole"))
            {
                Sound.PauseOrPlayVolumeMusic(true);
                ModController.Instance.OnModPause();
                _playerAudio.PlayOneShot(flagPoleSound);
                _flagPos = other.gameObject.transform.position.x;
                _isFinish = true;
                _playerRb.velocity = Vector2.zero;
                _playerRb.isKinematic = true;
                isWalkingToCastle = true;
                isStopTime = true;
                StartCoroutine(PlayStageClearSound());
            }

            if (other.gameObject.CompareTag("Castle"))
            {
                isInCastle = true;
                isWalkingToCastle = false;
                playerSprite.SetActive(false);
            }

            //if (other.gameObject.CompareTag("DeathAbyss"))
            //{
            //    //_playerAudio.PlayOneShot(dieSound);
            //    GameStatusController.Live -= 1;
            //    GameStatusController.IsBigPlayer = false;
            //    GameStatusController.IsFirePlayer = false;
            //    GameStatusController.PlayerTag = "Player";
            //    GameStatusController.IsDead = true;
            //    _playerRb.isKinematic = true;
            //    _playerRb.velocity = Vector2.zero;
            //}

            if (other.gameObject.CompareTag("1UpMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(oneUpSound);
                GameStatusController.Live += 1;
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("BigMushroom") && _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
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
           
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                _isEatable = false;
            }

            if (other.gameObject.CompareTag("FireFlower") && (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer")) &&
                _isEatable)
            {
                _playerAudio.PlayOneShot(turnBigSound);
                TurnIntoBigPlayer();
                GameStatusController.IsFirePlayer = true;
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
        void OnDieNFunc()
        {
            if (!GameStatusController.IsBigPlayer)
            {
                // StartCoroutine(Die(other.gameObject));
                PFunc.Log("OnCollisionEnter2D", isInvulnerable);
                if (!isInvulnerable)
                {
              
                    GameStatusController.IsDead = true;
                    GameStatusController.Live -= 1;
                    _playerRb.isKinematic = true;
                    _playerRb.velocity = Vector2.zero;
                }
                else
                {
                    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), smallPlayerCollider.GetComponent<Collider2D>());
                }
            }
            else if (GameStatusController.IsBigPlayer)
            {
                GameStatusController.IsBigPlayer = false;
                GameStatusController.IsFirePlayer = false;
                GameStatusController.PlayerTag = "Player";
                gameObject.tag = GameStatusController.PlayerTag;
                ChangeAnim();
                isInvulnerable = true;
            }
        }

        public void OnFireMario(){
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
        public bool isHidden = false;
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Princess"))
            {
                slideDownSpeed = 0;
                GameStatusController.IsShowMessage = true;
                _playerAnim.SetFloat(SpeedF, 0);
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

        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Hidden")&&! _isGoingDownPipeAble)
            {
                isHidden = false;
            }
        }

        private void TurnIntoBigPlayer()
        {
            PFunc.Log("碰见大花", gameObject.tag);
            if (CompareTag("Player"))
            {
                GameStatusController.PlayerTag = "BigPlayer";
                tag = GameStatusController.PlayerTag;
            }
            else
            {
                tag = "UltimateBigPlayer";
            }
  
            GameStatusController.IsBigPlayer = true;
            PFunc.Log("TurnIntoBigPlayer", GameStatusController.IsBigPlayer);
            ChangeAnim();
        }
        Vector3 deadPos=Vector3.zero;
        
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
            if (isSpecialDie)
            {
                if (DieCoroutine == null)
                {
               
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
                    PlayerModController.Instance.OnChangeState(false);
                    deadPos = isRest ? new Vector3(-2, 4) : new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                    DieCoroutine = StartCoroutine(NormalDie());
                }
            }

        }

        private void GetPlayerSpeed()
        {
            _playerAnim.SetFloat(SpeedF, Mathf.Abs(_playerRb.velocity.x));
        }

        public void ChangeAnim()
        {
            bigPlayer.SetActive(GameStatusController.IsBigPlayer);
            bigPlayerCollider.SetActive(GameStatusController.IsBigPlayer);
            smallPlayer.SetActive(!GameStatusController.IsBigPlayer);
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
                Sound.PlaySound("smb_mariodie");
                yield return StartCoroutine(LoadingScene());
            }
            else if (isRest || GameManager.Instance.time<=0)
            {
                string name = Config.passName[Config.passIndex];
                GameModController.Instance.OnLoadScene(name);
            }
            else
            {
                Sound.PlaySound("Mod/life");
                yield return playerBreak.OnHuifuIE();
                GameStatusController.IsDead = false;
                ResetPlayerState(deadPos);
                isInvincible = true;
                isInvulnerable = true;
                DieCoroutine = null;
            }
       
        }

        IEnumerator NormalDie()
        {
            PFunc.Log("玩家死亡");
            playerCol.SetActive(false);
            _playerAnim.SetBool(DieB,true);
            yield return new WaitForSeconds(0.5f);
            while (transform.position.y>-3)
            {
                transform.Translate(Vector3.down * 6 * Time.deltaTime);
                yield return null;
            }
            ModData.mLife--;
            if (ModData.mLife < 0)
            {
                Sound.PlaySound("smb_mariodie");
                yield return StartCoroutine(LoadingScene());
            }
            else if (isRest || GameManager.Instance.time <= 0)
            {
                string name = Config.passName[Config.passIndex];
                GameModController.Instance.OnLoadScene(name);
            }
            else 
            {
          
                GameStatusController.IsDead = false;
                ResetPlayerState(deadPos);
                isInvincible = true;
                isInvulnerable=true;
                _playerAnim.SetTrigger("toDead");
            }
     
            DieCoroutine = null;
        }

        private static IEnumerator LoadingScene()
        {
            yield return new WaitForSeconds(0.1f);
            if (ModData.mLife <= 0)
            {
                PFunc.Log("LoadingScene");
                ModController.Instance.statusController.mLife = 30;
                ModData.mLife = 30;
                GameModController.Instance.OnLoadTargetScene("LoadingScene");
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
            PFunc.Log("StopGoingDownPipe", _isAboveSpecialPipe, isHidden);
            if (_isAboveSpecialPipe)
            {
                _isAboveSpecialPipe = false;
                Loaded.OnLoadScence("Excessive");
            }
            else if(isHidden)
            {
                GameStatusController.IsHidden = true;
                GameStatusController. HiddenMove = Config.passIndex!=0;
                 isHidden = false;
                transform.position = GameManager.Instance.hiddenEnterPos.position;
                Camera.main.transform.position = new Vector3(20, 37, -10);
                PlayerModController.Instance.OnChangeStateFalse();
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
            // 重置输入状态变量
            _isSprinting = false;
            _isCrouching = false;

            // 重置速度相关变量
            speed = 410f;
            slideDownSpeed = 5;
            jumpForce = 1050;

    
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
            if (CompareTag("Player") || CompareTag("UltimatePlayer"))
            {
                smallPlayerCollider.SetActive(true);
                bigPlayerCollider.SetActive(false);
                smallPlayer.SetActive(true);
                bigPlayer.SetActive(false);
            }
            else if (CompareTag("BigPlayer") || CompareTag("UltimateBigPlayer"))
            {
                smallPlayerCollider.SetActive(false);
                bigPlayerCollider.SetActive(true);
                smallPlayer.SetActive(false);
                bigPlayer.SetActive(true);
            }
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
            hugTime = 0;
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

            // 12. 设置玩家标签为初始状态
            GameStatusController.PlayerTag = "Player";
            tag = "Player";
            PlayerModController.Instance.OnChangeState(true);
            PlayerModController.Instance.OnSetPlayerIns(true);
            // 13. 启用脚本（如果被禁用）
            enabled = true;
        }

        // 可选：添加一个重载方法，使用默认位置
        public void ResetPlayerState()
        {
            ResetPlayerState(startPos);
        }
        bool findNpc = false;
        public void OnToFindNpc()
        {
            _playerRb.velocity = Vector2.zero;
            findNpc = false;
            StartCoroutine(OnFindNpc());
        }
        IEnumerator OnFindNpc()
        {
            if (!_isFacingRight)
            {
                transform.Rotate(0, 180, 0);
                _isFacingRight = true;
            }
            _playerAnim.SetFloat(SpeedF, 3f);
            PlayerModController.Instance.OnChangeState(true);
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