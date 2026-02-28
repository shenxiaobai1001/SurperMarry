using EnemyScripts;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using TheFactory.Snappy;
using UnityEngine;
using UnityEngine.Video;

public class MonsterCreater : MonoBehaviour
{
    public static MonsterCreater Instance;
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

    public Transform CreatePos1; 
    public Transform CreatePos2;
    public Transform modMonsterPatent;
    public GameObject tortoise;
    public GameObject mushroom;
    public GameObject FlyKoopa;
    public GameObject flyFish;
    public GameObject Beatles;

    [SerializeField] private float batchInterval = 0.05f;

    public int MonsterCount = 0;
    public int MaxCanCreateMonster = 450;
    public int hasCreateMonster = 0;
    // 统一管理生成状态
    private class MonsterSpawnData
    {
        public string type;
        public int count = 0;
        public int batchSize = 10;
        public bool isCreating = false;

       public MonsterSpawnData(string type, int batchSize)
        {
            this.type = type;
            this.batchSize = batchSize; 
        }
    }

    private Dictionary<GameObject, MonsterSpawnData> spawnDataDict = new Dictionary<GameObject, MonsterSpawnData>();

    /// <summary>
    /// 统一生成怪物方法
    /// </summary>
    public void CreateMonster(GameObject monsterPrefab, int count,string type, int batchSize)
    {
        MonsterCount += count;
        if (!spawnDataDict.ContainsKey(monsterPrefab))
        {
            spawnDataDict[monsterPrefab] = new MonsterSpawnData(type, batchSize);
        }

        MonsterSpawnData data = spawnDataDict[monsterPrefab];
        data.count += count;
        if (!data.isCreating)
        {
            data.isCreating = true;
            StartCoroutine(CreateMonsterBatch(monsterPrefab));
        }
    }

    /// <summary> 批量生成协程</summary>
    private IEnumerator CreateMonsterBatch(GameObject monsterPrefab)
    {
        MonsterSpawnData data = spawnDataDict[monsterPrefab];

        while (data.count > 0)
        {
            int spawnCount = Mathf.Min(data.batchSize, data.count);
            if (Config.isLoading)
            {
                yield return new WaitUntil(() => !Config.isLoading);
            }
            if(hasCreateMonster>=MaxCanCreateMonster)
                yield return new WaitUntil(() => hasCreateMonster < MaxCanCreateMonster);

            if (Config.isLoading)
            {
                yield return new WaitUntil(() => !Config.isLoading);
            }
            if (hasCreateMonster >= MaxCanCreateMonster)
                yield return new WaitUntil(() => hasCreateMonster < MaxCanCreateMonster);
            Sound.PlaySound("smb_1-up");
            Vector3 createPos = CreatePos1.position;
            createPos = OnGetCreatePos(data);
            GameObject obj = InstantiateSingleMonster(monsterPrefab, createPos);
            EnemyController enemyController = null;
            switch (data.type)
            {
                case "FlyKoopa":
                    float hight = Random.Range(0.27f, 7);
                    obj.GetComponent<FlyKoopa>().originalY = hight;

                    break;
                case "flyFish":
                    float maxHight = Random.Range(2, 8);
                    obj.GetComponent<FlyFish>().maxHeight = maxHight;
                    break;
                case "tortoise":
                        enemyController = obj.GetComponent<EnemyController>();
                    bool left = Random.Range(0,2)==0;
                    enemyController._moveDirection = left ? Vector3.left : Vector3.right;

                    break;

            }
            if(enemyController==null)
            {
                enemyController = obj.GetComponent<EnemyController>();
            }
            enemyController.OnBeginMove();
            hasCreateMonster++;
            MonsterCount -= 1;

            data.count -= 1;
            yield return null;
        }

        data.isCreating = false;
    }

    Vector3 OnGetCreatePos(MonsterSpawnData data)
    {
        Vector3 createPos = CreatePos1.position;
        switch (data.type)
        {
             case "mushroom":
             case "tortoise":
                float value = Random.Range(0, 7);
                createPos = new Vector3(CreatePos1.position.x-value, CreatePos1.position.y, CreatePos1.position.z) ;
                break;
            case "FlyKoopa":
                float value1 = Random.Range(-8, 2);
                createPos = new Vector3(CreatePos1.position.x , CreatePos1.position.y+ value1, CreatePos1.position.z);
                break;
            case "flyFish":
                createPos = CreatePos2.position;
                break;
        }
        return createPos;
    }

    /// <summary> 实例化单个怪物 </summary>
    public GameObject InstantiateSingleMonster(GameObject prefab,Vector3 trans)
    {
        GameObject obj = SimplePool.Spawn(prefab, trans, Quaternion.identity);
        obj.transform.SetParent(modMonsterPatent);
        obj.SetActive(true);
        return obj;
    }
    public void OnMinCreates()
    {
        hasCreateMonster--;
        if (hasCreateMonster <= 0) 
            hasCreateMonster = 0;
    }

    // 保留原有接口，内部调用统一方法
    public void OnCreateTortoise(int count) => CreateMonster(tortoise, count, "tortoise",2);
    public void OnCreateMushroom(int count) => CreateMonster(mushroom, count, "mushroom", 2);
    public void OnCreateFlyKoopa(int count) => CreateMonster(FlyKoopa, count, "FlyKoopa", 1);
    public void OnCreateFlyFish(int count) => CreateMonster(flyFish, count, "flyFish", 1);
    public void OnCreateBeatles(int count) => CreateMonster(Beatles, count, "Beatles", 1);

}