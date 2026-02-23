
using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class ItemCreater : MonoBehaviour
{
    public static ItemCreater Instance;
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
    public Transform CreatePos3;
    public Transform modItemPatent;
    public GameObject banana;
    public GameObject manyArrow;
    public GameObject duck;
    public GameObject psyDuck;
    public GameObject hangself;
    public GameObject mangseng;
    public GameObject rollStone;
    public GameObject rollArrow;
    public GameObject Meteorite;
    public GameObject QiLinBi;
    public GameObject TCJiao;
    public GameObject UPFire; 
    public GameObject singleUPFire;
    public GameObject downFire;
    public GameObject chainPlayer;
    public GameObject Electricity;
    public GameObject ElectricityLeft;
    public GameObject ElectricityRight;
    public Transform moguParent;

    [SerializeField] private float batchInterval = 0.1f;

    // 统一管理生成状态
    private class ItemSpawnData
    {
        public string type;
        public int count = 0;
        public int batchSize = 10;
        public float batchInterval = 0.1f;
        public bool isCreating = false;
        public Action<object> endAction;

        public ItemSpawnData(string type, int batchSize, float batchInterval,Action<object> endAction=null)
        {
            this.type = type;
            this.batchSize = batchSize;
            this.endAction = endAction;
            this.batchInterval = batchInterval;
        }
    }

    private Dictionary<GameObject, ItemSpawnData> spawnDataDict = new Dictionary<GameObject, ItemSpawnData>();

    /// <summary>统一生成怪物方法 </summary>
    public void CreateItem(GameObject itemPrefab, int count, string type, int batchSize, float batchInterval=0.1f, Action<object> endAction = null)
    {
        if (!spawnDataDict.ContainsKey(itemPrefab))
        {
            spawnDataDict[itemPrefab] = new ItemSpawnData(type, batchSize, batchInterval, endAction);
        }

        ItemSpawnData data = spawnDataDict[itemPrefab];

        data.count += count;
        if (!data.isCreating)
        {
            data.isCreating = true;
            StartCoroutine(CreateItemBatch(itemPrefab));
        }
        
    }

    /// <summary> 批量生成协程</summary>
    private IEnumerator CreateItemBatch(GameObject monsterPrefab)
    {
        ItemSpawnData data = spawnDataDict[monsterPrefab];
        while (data.count > 0)
        {
            if (Config.isLoading)
            {
                yield return new WaitUntil(() => !Config.isLoading);
            }
            if (GameStatusController.isDead)
            {
                yield return new WaitUntil(() => !GameStatusController.isDead);
            }
            int spawnCount = Mathf.Min(data.batchSize, data.count);
            for (int i = 0; i < spawnCount; i++)
            {
                if (Config.isLoading)
                {
                    yield return new WaitUntil(() => !Config.isLoading);
                }
                if (GameStatusController.isDead)
                {
                    yield return new WaitUntil(() => !GameStatusController.isDead);
                }
                Vector3 createPos = CreatePos1.position;
                createPos = OnGetCreatePos(data);
                GameObject obj = InstantiateSingleMonster(monsterPrefab, createPos);
                OnCreateEnd(data, obj);
                PFunc.Log("创建", obj.name);
                obj.SetActive(true);
            }
            data.count -= spawnCount;
            yield return new WaitForSeconds(data.batchInterval);
        }

        data.isCreating = false;
        if (data.endAction != null)
        {
            data.endAction.Invoke(false);
        }
    }
    bool mangleft;
    Vector3 OnGetCreatePos(ItemSpawnData data)
    {
        Vector3 createPos = CreatePos1.position;
        Vector3 vector = PlayerController.Instance.transform.position;
        float valueZ = PlayerController.Instance.transform.position.z;
        float value;
        switch (data.type)
        {
            case "banana":
                Sound.PlaySound("Mod/banana");
                value = UnityEngine.Random.Range(-7, 7);
                createPos = new Vector3(createPos.x + value, createPos.y, valueZ);
                break;
            case "manyArrow":
                createPos = new Vector3(vector.x, -10, valueZ);
                break;
            case "duck":
                float value1 = UnityEngine.Random.Range(0, 6);
                createPos = new Vector3(CreatePos2.position.x+5, CreatePos2.position.y+ value1, valueZ);
                break;
            case "psyDuck":
                float value3 = UnityEngine.Random.Range(0, 6);
                createPos = new Vector3(CreatePos2.position.x + 5, CreatePos2.position.y + value3, valueZ);
                break;
            case "mangseng":
                mangleft = UnityEngine.Random.Range(0, 2) == 0;
                int xx = mangleft ? -15 : 15;
                createPos = new Vector3(vector.x + xx, vector.y+10, valueZ);
                break;
            case "rollStone":
            case "rollArrow":
                int x = (int)vector.x - 12;
                if (x < -7) x = -7;
                createPos = new Vector3(x, 0, valueZ);
                break;
            case "Meteorite":
                value = UnityEngine.Random.Range(-8, 8);
                createPos = new Vector3(createPos.x + value, createPos.y, valueZ);
                break;
            case "QiLinBi":
                Sound.PlaySound("Mod/QLBi");
                createPos = vector;
                break;
            case "TCJiao":
                Sound.PlaySound("Mod/TCJiao");
                createPos = vector;
                break;
            case "UPFire":
                createPos = new Vector3(Camera.main.transform.position.x, createPos.y, valueZ);
                break;
            case "singleUPFire":
                value = UnityEngine.Random.Range(-5, 5);
                createPos = new Vector3(createPos.x+ value, createPos.y, valueZ);
                break;
            case "DownFire":
                value = UnityEngine.Random.Range(-7, 7);
                createPos = new Vector3(vector.x + value, -5, 0);
                break;
            case "chainPlayer":
                createPos = new Vector3(Camera.main.transform.position.x, 5, valueZ);
                break;
            case "Electricity":
                createPos = vector;
                break;
            case "bigMG":
                createPos = new Vector3(vector.x , vector.y + 10, valueZ);
                break;
            case "bigGear":
                int xValue = PlayerController.Instance._isFacingRight ? 10 : -10;
               createPos = new Vector3(vector.x- xValue, 0, valueZ);
                break;
            case "trunck":
                createPos = new Vector3(vector.x + 30, vector.y, valueZ);
                break;
            case "Flog":
                 createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                break;
            case "huoquan":
                value = UnityEngine.Random.Range(0, 7);
                createPos = new Vector3(vector.x + 7, vector.y+ value, valueZ);
                break;
            case "Rattan":
                int xxx = UnityEngine.Random.Range(-7, 7);
                int yyy = UnityEngine.Random.Range(7, 11);
                createPos = new Vector3(Camera.main.transform.position.x + xxx, yyy, valueZ);
                break;
            case "zhiqian":
                createPos = new Vector3(Camera.main.transform.position.x , Camera.main.transform.position.y, valueZ);
                break;
        }
        return createPos;
    }

    void OnCreateEnd(ItemSpawnData data,GameObject obj)
    {
        switch (data.type)
        {
            case "banana":
                Sound.PlaySound("smb_1-up");
                break;
            case "duck":
                Sound.PlaySound("Mod/Duck");
                Duck duck1 = obj.GetComponent<Duck>();
                duck1.StartMove();
                allReadyCreateDuck--;
                break;
            case "psyDuck":
                Sound.PlaySound("Mod/Duck");
                PsyDuck duck2 = obj.GetComponent<PsyDuck>();
                duck2.StartMove();
                allReadyCreateDuck--;
                break;
            case "mangseng":
                Sound.PlaySound("Mod/mangseng");
                MangSeng mangSeng = obj.GetComponent<MangSeng>();
                mangSeng.StartMove(mangleft);
                break;
            case "rollStone":
            case "rollArrow":
                RollingRockController rollingRockController = obj.GetComponent<RollingRockController>();
                rollingRockController.OnBeginShow();
                break;
            case "Meteorite":
                Sound.PlaySound("Mod/meteorite");
                Meteorite meteorite = obj.GetComponent<Meteorite>();
                meteorite.OnBeginMove();
                break;
            case "QiLinBi":
                QLBI qLBI = obj.GetComponent<QLBI>();
                qLBI.OnStarMove();
                qlCount--;
                break;
            case "TCJiao":
                TCJiao tCJiao = obj.GetComponent<TCJiao>();
                tCJiao.OnStarMove();
                tcCount--;
                break;
            case "UPFire":
                Sound.PlaySound("smb_1-up");
                Sound.PlaySound("smb_bowserfire");
                UpFire upFire = obj.GetComponent<UpFire>();
                upFire.OnStarMove();
                break;
            case "singleUPFire":
                Sound.PlaySound("smb_1-up");
                Sound.PlaySound("smb_bowserfire");
                SingleUpFire upFire1 = obj.GetComponent<SingleUpFire>();
                upFire1.OnStarMove();
                break;
            case "DownFire":
                Sound.PlaySound("smb_1-up");
                break;
            case "chainPlayer":
                Config.chainCount += 20;
                break;
            case "Electricity":
                Electricity electricity = obj.GetComponent<Electricity>();
                electricity.OnStartLazzer();
                break;
            case "bigMG":
                BigMogu BigMoguu = obj.GetComponent<BigMogu>();
                BigMoguu.OnBeginMove();
                BigMoguu.transform.SetParent(moguParent);
                break;
            case "DaoQI":
                DaoFire daoFire = obj.GetComponent<DaoFire>();
                daoFire.OnCreate();
                break;
            case "bigGear":
                DaoFire bbigGear = obj.GetComponent<DaoFire>();
                bbigGear.OnCreate();
                break;
            case "trunck":
                ModVideoPlayerCreater.Instance.OnPlayTrunck();
                Trunck tTrunck = obj.GetComponent<Trunck>();
                tTrunck.StartMove();
                break;
            case "Flog":
                PlayerController.Instance.isHit = true;
                lockPlayer = true;
                UIFlog.Instance.OnStartMove();
                break;
        }
    }

    /// <summary> 实例化单个怪物 </summary>
    private GameObject InstantiateSingleMonster(GameObject prefab, Vector3 trans)
    {
        GameObject obj = SimplePool.Spawn(prefab, trans, Quaternion.identity);
        obj.transform.SetParent(modItemPatent);
        obj.SetActive(true);
        return obj;
    }

    // 保留原有接口，内部调用统一方法
    public void OnCreateBanana(int count) => CreateItem(banana, count, "banana", 1);
    public void OnCreateManyArrow(int count) => CreateItem(manyArrow, count, "manyArrow", 1);
    public int allReadyCreateDuck = 0;
    public int allCreateDuck = 0;
    public void OnCreateDuck(int count)
    {
        //UIDuck.Instance.OnSetCenter(true);
        allReadyCreateDuck += count; 
        CreateItem(duck, count, "duck", 1,0.05f);
    }
    public void OnCreatePsyDuck(int count)
    {
        //UIDuck.Instance.OnSetCenter(true);
        allReadyCreateDuck += count;
        CreateItem(psyDuck, count, "psyDuck", 1, 0.05f);
    }
    public void OnCreateHangSelf()
    {
        Vector3 vectorPlayer = PlayerController.Instance.transform.position;
        float value = GameStatusController.IsHidden ? 32 : 0;
        Vector3 createPos = new Vector3(vectorPlayer.x-2, value);
        GameObject obj = SimplePool.Spawn(hangself, createPos, Quaternion.identity);
        obj.transform.SetParent(transform);
        Sound.PlaySound("Mod/hangself");
        isHang = true;
        PlayerModController.Instance.OnHangSelf();
    }
    public void OnCreateMangSeng(int count) {

        if (lockPlayer) Config.chainCount++;
        CreateItem(mangseng, count, "mangseng", 1);
    }
    public void OnCreateRollStone(int count) => CreateItem(rollStone, count, "rollStone", 1);
    public void OnCreateRollArrow(int count) => CreateItem(rollArrow, count, "rollArrow", 1);
    public void OnCreateMeteorite(int count) => CreateItem(Meteorite, count, "Meteorite", 1);

    public int qlCount=0;
    public void OnCreateQiLinBi(int count)
    {
        if (lockPlayer)
        {
            Config.chainCount -= 2;
            if (Config.chainCount <= 0)
            {
                UIChain.Instance.OnChekcMinZero();
            }
        }
        qlCount += count;
        CreateItem(QiLinBi, count, "QiLinBi", 1);
    }
    public int tcCount = 0;
    public void OnCreateTCJiao(int count)
    {
        if (lockPlayer) Config.chainCount += 2;
        tcCount += count;
        CreateItem(TCJiao, count, "TCJiao", 1);
    }
    
    public void OnCreateUPFire(int count) => CreateItem(UPFire, count, "UPFire", 1);
    public void OnCreateSingleUPFire(int count) => CreateItem(singleUPFire, count, "singleUPFire", 1);
    public void OnCreateDownFire(int count) => CreateItem(downFire, count, "DownFire", 1);
    public bool lockPlayer = false;
    public void OnCreateChainPlayer(int count) {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        lockPlayer = true;
        UIChain.Instance.OnStartMove();
        Sound.PlaySound("Mod/lock");
        CreateItem(chainPlayer, count, "chainPlayer", 1);
    }
    public void OnCreateLazzer(int count)
    {
        if ( lockPlayer) Config.chainCount++;
        Sound.PlaySound("Mod/lazzer");
        CreateItem(Electricity, count, "Electricity", 1);
    }
    public GameObject bigMG;
    public void OnCreateBigMG(int count)
    {
         CreateItem(bigMG, count, "bigMG", 1);
    }
    public bool isHang = false;

    public GameObject DaoQI;
    public void OnCreateDaoQI(int count)
    {
        CreateItem(DaoQI, count, "DaoQI", 1);
    }
    public GameObject bigGear;
    public void OnCreateBigGear(int count)
    {
        Sound.PlaySound("Mod/gear");
        CreateItem(bigGear, count, "bigGear", 1);
    }
    public GameObject trunck;
    public void OnCreateTrunck(int count)
    {
        CreateItem(trunck, count, "trunck", 1);
    }
    public GameObject Flog;
    public void OnCreateFlog(int count)
    {
        if (PlayerController.Instance != null)
            PlayerController.Instance.OnChanleControl(true);
        CreateItem(Flog, count, "Flog", 1);
    }
    public GameObject huoQuan;
    public void OnCreatehuoQuan(int count)
    {
        CreateItem(huoQuan, count, "huoquan", 1);
    }
    public GameObject Rattan;
    public void OnCreateRattan(int count)
    {
        Sound.PlaySound("smb_1-up");
        CreateItem(Rattan, count, "Rattan", 1);
    }
    public GameObject zhiqian;
    public void OnCreateZhiQian(int count)
    {
        CreateItem(zhiqian, count, "zhiqian", 1);
    }
}