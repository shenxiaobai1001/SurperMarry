using System;
using System.Collections;
using System.Collections.Generic;
using EnemyScripts;
using PlayerScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemScripts
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public PlayerController player;
        public GameStatusController gameStatusController;
        public List<EnemyController> enemyControllers;
        public List<GameObject> enemyGameObjects;
        public GameObject stairwayPrefab;
        public Transform stairwayDownParent;
        public Transform stairwayUpParent;
        public Transform hiddenEnterPos;
        public Transform hiddenOutPos;
        public bool isStairLevel;

        public float time = 3;
        public float finalTime;

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
            if (isStairLevel)
            {
                InvokeRepeating(nameof(SpawnStairway), 0, 3);
            }

            if (gameStatusController != null)
            {
                gameStatusController.SetTime(time);
            }
        }

        private void Update()
        {
            if (player == null)
            {
                if (PlayerController.Instance!=null)
                {
                    player = PlayerController.Instance;
                }
            }
            if (player != null)
            {
                //StopEnemiesFromMovingWhenPlayerDie();
                SetActiveEnemiesWhenSeePlayer();
                DestroyEnemiesOutOfBound();
                UpdateTime();
                UltimateDestroyAll();

                if (player.isStopTime)
                {
                    finalTime = time;
                    player.isStopTime = false;
                }
            }
        }

        private void StopEnemiesFromMovingWhenPlayerDie()
        {
            if (!GameStatusController.IsDead) return;
            for (var i = 0; i < enemyControllers.Count; i++)
            {
                if (enemyControllers[i] != null)
                {
                    enemyControllers[i].speed = 0;
                    enemyControllers[i].gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    enemyControllers[i].gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
                }
                else
                {
                    enemyControllers.Remove(enemyControllers[i]);
                }
            }
        }
        public float showCount = 12;

        private void SetActiveEnemiesWhenSeePlayer()
        {
            for (var i = 0; i < enemyGameObjects.Count; i++)
            {
                if (enemyGameObjects[i] != null)
                {
                    if (enemyGameObjects[i].transform.position.x - player.transform.position.x < showCount)
                    {
                        enemyGameObjects[i].SetActive(true);
                    }
                }
                else
                {
                    enemyGameObjects.Remove(enemyGameObjects[i]);
                }
            }
        }

        private void DestroyEnemiesOutOfBound()
        {
            for (var i = 0; i < enemyGameObjects.Count; i++)
            {
                if (enemyGameObjects[i] != null)
                {
                    if (enemyGameObjects[i].transform.position.x - player.transform.position.x < -15)
                    {
                        enemyGameObjects[i].SetActive(false);
                    }
                }
                else
                {
                    enemyGameObjects.Remove(enemyGameObjects[i]);
                }
            }
        }
        bool checkLevel = true;
        private void UpdateTime()
        {

            if (!GameStatusController.IsDead && !player.isWalkingToCastle && !player.isInCastle &&
                !GameStatusController.IsGameFinish)
            {
                ModController.Instance.statusController.SetTime(time -= Time.deltaTime * 2);
                if (time < 0)
                {
                    PFunc.Log("掉到坑里");
                    time = 0;
                    GameStatusController.IsDead = true;
                }
            }
            else if (player.isInCastle)
            {
                ModController.Instance.statusController.SetTime(time -= Time.deltaTime * 60);

                if (time < 0&& checkLevel)
                {
                    checkLevel = false;
                    time = 0;
                    StartCoroutine(NextLevel());
                }
                else
                {
                    if (finalTime - time >= 1f)
                    {
                        GameStatusController.Score += 50;
                        finalTime = time;
                    }
                }
            }
        }

        private void UltimateDestroyAll()
        {
            if (player.CompareTag("UltimatePlayer") || player.CompareTag("UltimateBigPlayer"))
            {
                for (var i = 0; i < enemyControllers.Count; i++)
                {
                    if (enemyControllers[i] != null)
                    {
                        if (Mathf.RoundToInt(enemyControllers[i].gameObject.transform.position.x -
                                             player.transform.position.x) == 0 && player.CompareTag("UltimatePlayer"))
                        {
                            KillAndRemoveEnemies(i, 0.2f);
                        }
                        else if (Mathf.RoundToInt(enemyControllers[i].gameObject.transform.position.x -
                                                  player.transform.position.x) == 0 &&
                                 player.CompareTag("UltimateBigPlayer"))
                        {
                            KillAndRemoveEnemies(i, 0.7f);
                        }
                    }
                    else
                    {
                        enemyControllers.Remove(enemyControllers[i]);
                    }
                }
            }
        }

        private void KillAndRemoveEnemies(int i, float distance)
        {
            if (enemyControllers[i].gameObject.transform.position.y - player.transform.position.y <
                distance &&
                enemyControllers[i].gameObject.transform.position.y - player.transform.position.y >
                -distance)
            {
                GameStatusController.Score += 200;
                GameStatusController.IsEnemyDieOrCoinEat = true;
                enemyControllers[i].Die();
                enemyControllers.Remove(enemyControllers[i]);
            }
        }

        private void SpawnStairway()
        {
            GameObject stairwayDown = Instantiate(stairwayPrefab, stairwayDownParent);
            GameObject stairwayUp = Instantiate(stairwayPrefab, stairwayUpParent);
            StartCoroutine(DestroyStair(stairwayDown));
            StartCoroutine(DestroyStair(stairwayUp));
        }

        private static IEnumerator NextLevel()
        {
            yield return new WaitForSeconds(1);
            Config.passIndex++;
            string name = Config.passName[Config.passIndex];
            PFunc.Log(Config.passIndex, name);
            GameModController.Instance.OnLoadScene(name);
        }

        private IEnumerator DestroyStair(GameObject stair)
        {
            yield return new WaitForSeconds(5.3f);
            Destroy(stair);
        }
    }
}