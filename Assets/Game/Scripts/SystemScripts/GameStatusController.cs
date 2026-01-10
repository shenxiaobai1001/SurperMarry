using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace SystemScripts
{
    public class GameStatusController : MonoBehaviour
    {
        public TextMeshProUGUI playerScoreText;
        public TextMeshProUGUI playerHighScoreText;
        public TextMeshProUGUI collectedCoinText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI secondsText;
        public TextMeshProUGUI livesText;
        public GameObject score200Prefab;
        public GameObject score1000Prefab;
        public GameObject pausePopup;
        public GameObject instructionPopup;
        public GameObject creditPopup;
        public GameObject firstMessagePopup;
        public GameObject secondMessagePopup;
        public Transform scoreParent;
        private AudioSource _gameStatusAudio;
        public GameObject LiveStart;

        public AudioClip pauseSound;
        public AudioClip stageClearSound;

        public GameObject NpcTalk;

        private bool _pauseTrigger;

        public static int CollectedCoin;
        public static int Score;
        private static int _highScore;
        public static int Live;
        public static int CurrentLevel;

        public static bool isDead;
        public static bool IsDead;
        public static bool IsGameOver;
        public static bool IsStageClear;
        public static bool IsBigPlayer;
        public static bool IsFirePlayer;
        public static bool IsBossBattle;
        public static bool IsGameFinish;
        public static bool IsEnemyDieOrCoinEat;
        public static bool IsPowerUpEat;
        public static bool IsShowMessage;
        public static string PlayerTag;
        public static bool IsHidden;
        public static bool HiddenMove;
        private float _second;

        private void Awake()
        {
            SetScore(playerHighScoreText, _highScore);
            _gameStatusAudio = GetComponent<AudioSource>();
            _pauseTrigger = false;
            mLife = ModData.mLife;
            tx_life.text = mLife.ToString();
            LiveStart.SetActive(false);
            NpcTalk.SetActive(false);
        }

        private void Update()
        {
            if (IsStageClear)
            {
                _gameStatusAudio.PlayOneShot(stageClearSound);
                IsStageClear = false;
            }

            if (IsShowMessage)
            {
                StartCoroutine(DisplayFirstMessage());
            }

            if (IsEnemyDieOrCoinEat)
            {
                IsEnemyDieOrCoinEat = false;
                UpdateScorePopup(score200Prefab);
            }

            if (IsPowerUpEat)
            {
                IsPowerUpEat = false;
                UpdateScorePopup(score1000Prefab);
            }

            if (Score > _highScore)
            {
                _highScore = Score;
            }

            if (ModData.mTrap)
            {
                tx_trap.text = $"{ModData.tiggerTrapCount}/{ModData.allTrapCount}";
            }
            else
            {
                tx_trap.text = "0";
            }
            SetCoin();
            SetLevel();
            SetScore(playerScoreText, Score);
            SetLive();
            Pause();
        }
        public TextMeshProUGUI tx_trap;
        private void SetScore(TextMeshProUGUI scoreText, int score)
        {
            switch (score.ToString().Length)
            {
                case 0:
                    scoreText.SetText("000000");
                    break;
                case 3:
                    scoreText.SetText($"000{score}");
                    break;
                case 4:
                    scoreText.SetText($"00{score}");
                    break;
                case 5:
                    scoreText.SetText($"0{score}");
                    break;
                case 6:
                    scoreText.SetText($"{score}");
                    break;
            }
        }

        private void SetCoin()
        {
            if (CollectedCoin > 0)
            {
                collectedCoinText.SetText($"x0{CollectedCoin}");
                if (CollectedCoin <= 9) return;
                collectedCoinText.SetText($"x{CollectedCoin}");
                if (CollectedCoin > 99)
                {
                    collectedCoinText.SetText("x00");
                }
            }
            else
            {
                collectedCoinText.SetText("x00");
            }
        }

        public void SetTime(float second)
        {
            _second = second;
            if (_second > 0)
            {
                if (_second > 99.5f)
                {
                    secondsText.SetText(Mathf.RoundToInt(_second).ToString());
                }
                else if (_second > 9.5f)
                {
                    secondsText.SetText($"0{Mathf.RoundToInt(_second).ToString()}");
                }
                else
                {
                    secondsText.SetText($"00{Mathf.RoundToInt(_second).ToString()}");
                }
            }
            else
            {
                secondsText.SetText("000");
            }
        }

        private void SetLevel()
        {
            levelText.SetText(SceneManager.GetActiveScene().name);
        }

        private void SetLive()
        {
            livesText.SetText($"x {ModData.mLife.ToString()}");
        }

        private void Pause()
        {
            if (SceneManager.GetActiveScene().buildIndex > 1)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    _gameStatusAudio.PlayOneShot(pauseSound);
                    _pauseTrigger = !_pauseTrigger;
                    pausePopup.SetActive(_pauseTrigger);
                    Time.timeScale = _pauseTrigger ? 0 : 1;
                }
            }
        }

        public void StartGame()
        {
            Config.passIndex = 0;
            GameModController.Instance.OnLoadScene("1-1");
            CurrentLevel = 2;
            Live = 3;
            Score = 0;
            CollectedCoin = 0;
            PlayerTag = "Player";
        }

        public void OpenInstructionPopup()
        {
            instructionPopup.SetActive(true);
        }

        public void OpenCreditPopup()
        {
            creditPopup.SetActive(true);
        }

        public void CloseInstructionPopup()
        {
            instructionPopup.SetActive(false);
        }

        public void CloseCreditPopup()
        {
            creditPopup.SetActive(false);
        }

        public void ExitGame()
        {
            SceneManager.LoadScene(0);
            Time.timeScale = 1;
        }

        private void UpdateScorePopup(GameObject scorePrefab)
        {
            GameObject score = Instantiate(scorePrefab, scoreParent);
            StartCoroutine(DestroyScorePrefab(score));
        }

        private IEnumerator DestroyScorePrefab(GameObject prefab)
        {
            yield return new WaitForSeconds(1);
            Destroy(prefab);
        }

        private IEnumerator DisplayFirstMessage()
        {
            yield return new WaitForSeconds(1);
            firstMessagePopup.SetActive(true);
            StartCoroutine(DisplaySecondMessage());
        }

        private IEnumerator DisplaySecondMessage()
        {
            yield return new WaitForSeconds(1.5f);
            secondMessagePopup.SetActive(true);
        }

        private void Start()
        {
            EventManager.Instance.AddListener(Events.NpcTalkShow, OnShowNpcTalk);
            if (currentLifeCoroutine != null)
            {
                StopCoroutine(currentLifeCoroutine);
            }

            // 启动新的协程处理生命值变化
            currentLifeCoroutine = StartCoroutine(ChangeLifeCoroutine());

            if (currentCretateine != null)
            {
                StopCoroutine(currentCretateine);
            }
            currentLifeCoroutine = StartCoroutine(OnShowCreateCount());
        }
        public TextMeshProUGUI tx_life;
        [HideInInspector] public int mLife = 30;
        private Coroutine currentLifeCoroutine; // 保存当前运行的协程引用
        private Coroutine currentCretateine; // 保存当前运行的协程引用
        public void OnChangeLife(object msg)
        {
            // 如果已有协程在运行，先停止它
      
        }

        IEnumerator ChangeLifeCoroutine()
        {
            // 持续变化直到当前生命值等于目标值
            while (true)
            {
                // 实时判断变化方向
                if (mLife < ModData.mLife)
                {
                    Sound.PlaySound("smb_1-up");
                    mLife++;
                }
                else if(mLife > ModData.mLife)
                {
                    mLife--;
                }
                //PFunc.Log("实时判断变化方向", mLife, ModData.mLife);
                tx_life.text = mLife.ToString();
                yield return new WaitForSeconds(0.1f);
            }
        }
        public TextMeshProUGUI tx_createCount;

        IEnumerator OnShowCreateCount()
        {
            // 持续变化直到当前生命值等于目标值
            while (true)
            {
                string showValue = "00";
              if (ItemCreater.Instance.qlCount>0 || ItemCreater.Instance.tcCount > 0)
                {
                    showValue = $"Q:{ItemCreater.Instance.qlCount}/T:{ItemCreater.Instance.tcCount}";
                }
                else if (MonsterCreater.Instance.MonsterCount>0)
                {
                    showValue = $"{MonsterCreater.Instance.MonsterCount}";
                }
                else if (ItemCreater.Instance.allReadyCreateDuck > 0)
                {
                    showValue = $"DUCK:{ItemCreater.Instance.allReadyCreateDuck}";
                }
                tx_createCount.text = showValue;
                yield return new WaitForSeconds(0.1f);
            }
        }

        public void OnShowNpcTalk(object msg)
        {
            NpcTalk.SetActive(true);
        }
        private void OnDestroy()
        {
            EventManager.Instance.RemoveListener(Events.NpcTalkShow, OnShowNpcTalk);
        }
    }
}