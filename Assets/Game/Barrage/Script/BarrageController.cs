using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using PlayerScripts;
using SystemScripts;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class BarrageNormalSetting
{
    public string CallName; // 功能名称
    public string Type; // 数据类型名称
    public string Message; // 触发内容
    public string Tip; // 提示
    public int Count; // 倍率
    public float Delay; // 延迟
}

[Serializable]
public class BarrageBoxSetting
{
    public string BoxName; // 盲盒名称
    public string Type; // 数据类型名称
    public string Message; // 触发内容
    public string Tip; // 提示
    public int Count; // 倍率
    public float Delay; // 延迟
    public string videoName; // 选择的视频

    public List<string> Calls = new List<string>(); // 盲盒所有功能
}

[Serializable]
public class BarrageSpecialBoxSetting
{
    public string BoxName; // 盲盒名称
    public string Type; // 数据类型名称
    public string Message; // 触发内容
    public string Tip; // 提示
    public int Count; // 倍率
    public float Delay; // 延迟
    public string videoName; // 选择的视频

    public List<string> Calls = new List<string>(); // 盲盒所有功能
}

public enum PrankType
{
    normal,
    box,
    special
}

public class BarrageNormalWrapper
{
    public List<BarrageNormalSetting> NormalConfigs;
}

public class BarrageBoxWrapper
{
    public List<BarrageBoxSetting> BoxConfigs;
}

public class BarrageSpecialWrapper
{
    public List<BarrageSpecialBoxSetting> SpecialConfigs;
}

public class BarrageController : MonoBehaviour
{
    public static BarrageController Instance { get; set; }

    // 功能名称
    public List<string> Calls = new List<string>();

    [Tooltip("当前整蛊配置类型")]
    public PrankType prankType;

    public GameObject content;
    public GameObject item;
    public GameObject box;
    public GameObject special;

    public List<BarrageNormalSetting> barrageNormalSetting = new List<BarrageNormalSetting>();
    public List<BarrageBoxSetting> barrageBoxSetting = new List<BarrageBoxSetting>();
    public List<BarrageSpecialBoxSetting> barrageSpecialBoxSetting = new List<BarrageSpecialBoxSetting>();
    public bool isInit;

    private class ActionTask
    {
        public string user;
        public string avatar;
        public string callName;
        public int giftCount;
        public int times;
        public float delay;
    }

    private readonly Dictionary<string, Queue<ActionTask>> _queues = new Dictionary<string, Queue<ActionTask>>();
    private readonly Dictionary<string, Coroutine> _runners = new Dictionary<string, Coroutine>();
    private readonly Dictionary<string, float> _lastExec = new Dictionary<string, float>();

    public void EnqueueAction(string user, string avatar, string callName, int giftCount, int times, float delay, bool isBox = false)
    {
        if (string.IsNullOrEmpty(callName)) return;
        if (!_queues.TryGetValue(callName, out var q))
        {
            q = new Queue<ActionTask>();
            _queues[callName] = q;
        }
        int total = Mathf.Max(1, giftCount * times);
        for (int i = 0; i < total; i++)
        {
            q.Enqueue(new ActionTask
            {
                user = user,
                avatar = avatar,
                callName = callName,
                giftCount = giftCount,
                times = times,
                delay = delay
            });
        }
        if (!_runners.ContainsKey(callName) || _runners[callName] == null)
        {
            _runners[callName] = StartCoroutine(ProcessQueue(callName));
        }
    }

    private IEnumerator ProcessQueue(string callName)
    {
        var q = _queues[callName];
        while (q.Count > 0)
        {
            var task = q.Dequeue();
            float last = _lastExec.TryGetValue(callName, out var t) ? t : -1f;
            float elapsed = last < 0f ? float.MaxValue : (Time.time - last);
            float wait = Mathf.Max(0f, task.delay - elapsed);
            if (wait > 0f) yield return new WaitForSeconds(wait);

            ExecuteAction(task);
            _lastExec[callName] = Time.time;
        }
        _runners[callName] = null;
    }

    /// <summary>
    /// 从 Box 播放视频并等待播放结束（VideoManager 播放完会 Despawn 自己）
    /// </summary>
    public IEnumerator PlayBoxVideoAndWait(string boxPath, int playerType = 2, bool snake = false, Transform parent = null)
    {
        Debug.Log(boxPath);

        GameObject obj = ModVideoPlayerCreater.Instance.OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, boxPath, 2, "Default", false, -10);

        // 等待对象被回收或失活
        yield return new WaitUntil(() => obj == null || !obj.activeInHierarchy);
    }

    /// <summary>
    /// 执行功能
    /// </summary>
    /// <param name="task"></param>
    private void ExecuteAction(ActionTask task)
    {
        switch (task.callName)
        {
            case "乌龟一只":
                MonsterCreater.Instance.OnCreateTortoise(1);
                break;
            case "乌龟十只":
                MonsterCreater.Instance.OnCreateTortoise(10);
                break;
            case "乌龟一百只":
                MonsterCreater.Instance.OnCreateTortoise(100);
                break;
            case "蘑菇一只":
                MonsterCreater.Instance.OnCreateMushroom(1);
                break;
            case "蘑菇十只":
                MonsterCreater.Instance.OnCreateMushroom(10);
                break;
            case "蘑菇一百只":
                MonsterCreater.Instance.OnCreateMushroom(100);
                break;
            case "飞龟一只":
                MonsterCreater.Instance.OnCreateFlyKoopa(1);
                break;
            case "飞龟十只":
                MonsterCreater.Instance.OnCreateFlyKoopa(10);
                break;
            case "飞龟一百只":
                MonsterCreater.Instance.OnCreateFlyKoopa(100);
                break;
            case "飞鱼一只":
                MonsterCreater.Instance.OnCreateFlyFish(1);
                break;
            case "飞鱼十只":
                MonsterCreater.Instance.OnCreateFlyFish(10);
                break;
            case "飞鱼一百只":
                MonsterCreater.Instance.OnCreateFlyFish(100);
                break;
            case "甲壳虫一只":
                MonsterCreater.Instance.OnCreateBeatles(1);
                break;
            case "甲壳虫十只":
                MonsterCreater.Instance.OnCreateBeatles(10);
                break;
            case "甲壳虫一百只":
                MonsterCreater.Instance.OnCreateBeatles(100);
                break;
            case "游戏时间+10s":
                GameStatusController.IsGameFinish = false;
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time += 10;
                break;
            case "游戏时间-10s":
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time -= 10;
                break;
            case "生命+10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife -= (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命+1":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-1":
                Sound.PlaySound("smb_1-up");
                ModData.mLife -= 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "扔香蕉":
                ItemCreater.Instance.OnCreateBanana(1);
                break;
            case "动感DJ":
                ModVideoPlayerCreater.Instance.OnPlayDJ();
                break;
            case "万箭齐发":
                ItemCreater.Instance.OnCreateManyArrow(1);
                break;
            case "抓鸭子":
                ModVideoPlayerCreater.Instance.OnCreateDuckVideoPlayer();
                break;
            case "抓可达鸭":
                ModVideoPlayerCreater.Instance.OnCreatePsyDuckVideoPlayer();
                break;
            case "抓乌龟":
                ModVideoPlayerCreater.Instance.OnCreateKoopaVideoPlayer();
                break;
            case "乌萨奇":
                ModVideoPlayerCreater.Instance.OnPlayWuSaQi();
                break;
            case "灵魂拷问":
                ModVideoPlayerCreater.Instance.OnPlayMenace();
                break;
            case "乌萨奇硬控":
                ModVideoPlayerCreater.Instance.OnPlayWuSaQi(true);
                break;
            case "灵魂拷问硬控":
                ModVideoPlayerCreater.Instance.OnPlayMenace(true);
                break;
            case "上吊":
                ItemCreater.Instance.OnCreateHangSelf();
                break;
            case "一库":
                ItemCreater.Instance.OnCreateMangSeng(1);
                break;
            case "滚石":
                ItemCreater.Instance.OnCreateRollStone(1);
                break;
            case "滚刺":
                ItemCreater.Instance.OnCreateRollArrow(1);
                break;
            case "陨石":
                ItemCreater.Instance.OnCreateMeteorite(1);
                break;
            case "麒麟臂":
                ItemCreater.Instance.OnCreateQiLinBi(1);
                break;
            case "天残脚":
                ItemCreater.Instance.OnCreateTCJiao(1);
                break;
            case "随机天火":
                ItemCreater.Instance.OnCreateSingleUPFire(1);
                break;
            case "全屏天火":
                ItemCreater.Instance.OnCreateUPFire(1);
                break;
            case "随机地火":
                ItemCreater.Instance.OnCreateDownFire(1);
                break;
            case "全屏地火":
                ItemCreater.Instance.OnCreateDownFire(66);
                break;
            case "随机传送":
                GameModController.Instance.OnRandromPlayerPos();
                break;
            case "随机关卡":
                GameModController.Instance.OnRandromPass();
                break;
            case "铁链":
                ItemCreater.Instance.OnCreateChainPlayer(1);
                break;
            case "雷电":
                ItemCreater.Instance.OnCreateLazzer(1);
                break;
            case "砖块+10":
                CreateWallManager.Instance.wallCount += 10;
                break;
            case "石头+10":
                CreateWallManager.Instance.stonesCount += 10;
                break;
            case "美女盲盒":
                ModVideoPlayerCreater.Instance.OnPlayGrilVideo();
                break;
            case "火焰马里奥":
                PlayerModController.Instance.OnRandromPlayer(2);
                break;
            case "上一关":
                if (GameModController.Instance != null) GameModController.Instance.OnEnterNextPass(-1);
                break;
            case "下一关":
                if (GameModController.Instance != null) GameModController.Instance.OnEnterNextPass(1);
                break;
            case "大蘑菇":
                ItemCreater.Instance.OnCreateBigMG(1);
                break;
            case "超人+10秒":
                PlayerModController.Instance.OnSuperMan(10);
                break;
            case "超人-10秒":
                PlayerModController.Instance.OnSuperMan(-10);
                break;
            case "僵尸+10秒":
                PlayerModController.Instance.OnShowZomBie(10);
                break;
            case "僵尸-10秒":
                PlayerModController.Instance.OnShowZomBie(-10);
                break;
            case "广播体操":
                PlayerModController.Instance.OnGuangDance();
                break;
            case "刀马":
                PlayerModController.Instance.OnDMDance();
                break;
            case "凿头":
                PlayerModController.Instance.OnKickHead();
                break;
            case "砍刀形态":
                PlayerModController.Instance.OnRandromPlayer(0);
                break;
            case "拿枪形态":
                PlayerModController.Instance.OnRandromPlayer(1);
                break;
            case "随机形态":
                PlayerModController.Instance.OnRandromPlayer(5);
                break;
            case "大齿轮":
                ItemCreater.Instance.OnCreateBigGear(1);
                break;
            case "撞大运":
                ItemCreater.Instance.OnCreateTrunck(1);
                break;
            case "打板子盲盒":
                ModVideoPlayerCreater.Instance.OnCreateFlog();
                break;
            case "变大":
                if (Config.isLoading) return;
                ModVideoPlayerCreater.Instance.OnPlayBig();
                PlayerModController.Instance.OnChangScale(0.1f);
                break;
            case "变小":
                if (Config.isLoading) return;
                ModVideoPlayerCreater.Instance.OnPlaySmall();
                PlayerModController.Instance.OnChangScale(-0.1f);
                break;
            case "关灯":
                if (Config.isLoading) return;
                Sound.PlaySound("smb_1-up");
                UIMask.Instance.OnCloseLight();
                break;
            case "重新开始":
                if (Config.isLoading) return;
                if (GameModController.Instance != null) GameModController.Instance.OnLoadScene("1-1");
                break;
            case "陷阱数量+1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount< ModData.allTrapCount)
                    ModData.canTrapCount += 1;
                break;
            case "陷阱数量-1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount >0)
                    ModData.canTrapCount -= 1;
                break;
            case "全部陷阱激活":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = ModData.allTrapCount;
                break;
            case "全部陷阱关闭":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = 0;
                break;
            case "无敌星":
                PlayerModController.Instance.OnSetInvincible();
                break;
            case "隐身":
                PlayerModController.Instance.OnSetInvisibilityState();
                break;
            case "火圈":
                Sound.PlaySound("smb_1-up");
                ItemCreater.Instance.OnCreatehuoQuan(1);
                break;
            case "伸缩藤条":
                ItemCreater.Instance.OnCreateRattan(1);
                break;
            case "哭坟":
                ModVideoPlayerCreater.Instance.OnKuFen();
                break;
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeAllConfigs();
    }

    void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(content.transform as RectTransform);
    }

    /// <summary>
    /// 切换配置类型
    /// </summary>
    /// <param name="type"></param>
    public void ChangePrankType(int type)
    {
        isInit = false;
        prankType = (PrankType)type;
        if (type == (int)PrankType.normal)
        {
            RemoveAllItem();
            InitNormalConfig();
        }
        else if (type == (int)PrankType.box)
        {
            RemoveAllItem();
            InitBoxConfig();
        }
        else if (type == (int)PrankType.special)
        {
            RemoveAllItem();
            InitSpecialConfig();
        }
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    public void AddItem()
    {
        if (prankType == PrankType.normal)
        {
            GameObject obj = Instantiate(item, content.transform);
            Dropdown dropdown = obj.transform.GetChild(1).GetComponent<Dropdown>();

            dropdown.ClearOptions();
            dropdown.AddOptions(Calls);

            BarrageNormalSetting config = new BarrageNormalSetting();
            config.CallName = dropdown.options[dropdown.value].text;
            config.Count = 1;
            barrageNormalSetting.Add(config);
        }
        else if (prankType == PrankType.box)
        {
            GameObject obj = Instantiate(box, content.transform);

            BarrageBoxSetting config = new BarrageBoxSetting();
            config.Count = 1;
            barrageBoxSetting.Add(config);
        }
        else if (prankType == PrankType.special)
        {
            GameObject obj = Instantiate(special, content.transform);

            BarrageSpecialBoxSetting config = new BarrageSpecialBoxSetting();
            config.Count = 1;
            barrageSpecialBoxSetting.Add(config);
        }
    }

    /// <summary>
    /// 清空配置
    /// </summary>
    public void RemoveAllItem()
    {
        foreach (Transform obj in content.transform)
        {
            Destroy(obj.gameObject);
        }
    }

    /// <summary>
    /// 初始化所有配置（加载或创建默认配置）
    /// </summary>
    public void InitializeAllConfigs()
    {
        Debug.Log("开始初始化配置...");

        try
        {
            string configDir = Path.Combine(Directory.GetCurrentDirectory(), "Config");
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
                Debug.Log($"创建配置目录: {configDir}");
            }

            //// 2. 初始化普通配置
            //InitializeNormalConfig(configDir);

            //// 3. 初始化盲盒配置
            //InitializeBoxConfig(configDir);

            Debug.Log("所有配置初始化完成");
        }
        catch (Exception ex)
        {
            Debug.LogError($"初始化配置失败: {ex.Message}");
        }
    }

    /// <summary>
    /// 保存配置到本地JSON
    /// </summary>
    public void SaveDataToJson()
    {
        BarrageNormalWrapper wrapper = new BarrageNormalWrapper();
        wrapper.NormalConfigs = barrageNormalSetting;

        string filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "NormalData.json");

        string jsonData1 = JsonUtility.ToJson(wrapper, true);

        File.WriteAllText(filePath1, jsonData1);

        Debug.Log("普通配置数据已保存到: " + filePath1);

        BarrageBoxWrapper barrageBoxWrapper = new BarrageBoxWrapper();
        barrageBoxWrapper.BoxConfigs = barrageBoxSetting;

        string filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "BoxData.json");
        string jsonData2 = JsonUtility.ToJson(barrageBoxWrapper, true);

        File.WriteAllText(filePath2, jsonData2);

        Debug.Log("盲盒配置数据已保存到: " + filePath2);

        BarrageSpecialWrapper barrageSpecialWrapper = new BarrageSpecialWrapper();
        barrageSpecialWrapper.SpecialConfigs = barrageSpecialBoxSetting;

        string filePath3 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "SpecialData.json");
        string jsonData3 = JsonUtility.ToJson(barrageSpecialWrapper, true);

        File.WriteAllText(filePath3, jsonData3);

        Debug.Log("多特效配置数据已保存到: " + filePath3);
    }

    /// <summary>
    /// 读取本地JSON数据
    /// </summary>
    public void LoadDataFromJson()
    {

        string filePath1 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "NormalData.json");

        if (!File.Exists(filePath1))
        {
            Debug.LogWarning("未找到配置文件: " + filePath1);
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath1);

            BarrageNormalWrapper wrapper = JsonUtility.FromJson<BarrageNormalWrapper>(jsonData);
            barrageNormalSetting = wrapper.NormalConfigs;

            Debug.Log($"成功加载 {wrapper.NormalConfigs.Count} 条普通配置数据");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
        }


        string filePath2 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "BoxData.json");
        if (!File.Exists(filePath2))
        {
            Debug.LogWarning("未找到配置文件: " + filePath2);
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath2);

            BarrageBoxWrapper wrapper = JsonUtility.FromJson<BarrageBoxWrapper>(jsonData);
            barrageBoxSetting = wrapper.BoxConfigs;

            Debug.Log($"成功加载 {wrapper.BoxConfigs.Count} 条盲盒配置数据");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
        }

        string filePath3 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "SpecialData.json");
        if (!File.Exists(filePath3))
        {
            Debug.LogWarning("未找到配置文件: " + filePath3);
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath3);

            BarrageSpecialWrapper wrapper = JsonUtility.FromJson<BarrageSpecialWrapper>(jsonData);
            barrageSpecialBoxSetting = wrapper.SpecialConfigs;

            Debug.Log($"成功加载 {wrapper.SpecialConfigs.Count} 条多特效配置数据");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
        }


    }

    /// <summary>
    /// 初始化普通配置item
    /// </summary>
    public void InitNormalConfig()
    {
        for (int i = 0; i < barrageNormalSetting.Count; i++)
        {
            GameObject itemObj = Instantiate(item, content.transform);

            Dropdown dropdown1 = itemObj.transform.GetChild(1).GetComponent<Dropdown>();
            dropdown1.ClearOptions();
            dropdown1.AddOptions(Calls);
            Dropdown dropdown2 = itemObj.transform.GetChild(2).GetComponent<Dropdown>();

            itemObj.transform.GetChild(3).GetComponent<InputField>().text = barrageNormalSetting[i].Message;
            itemObj.transform.GetChild(5).GetComponent<InputField>().text = barrageNormalSetting[i].Tip;
            itemObj.transform.GetChild(7).GetComponent<InputField>().text = barrageNormalSetting[i].Count.ToString();
            itemObj.transform.GetChild(9).GetComponent<InputField>().text = barrageNormalSetting[i].Delay.ToString();

            ChoiceCall(dropdown1, barrageNormalSetting[i].CallName);
            ChoiceCall(dropdown2, barrageNormalSetting[i].Type);

        }
        isInit = true;
    }

    /// <summary>
    /// 初始化盲盒配置box
    /// </summary>
    public void InitBoxConfig()
    {
        RemoveAllItem();
        for (int i = 0; i < barrageBoxSetting.Count; i++)
        {
            GameObject itemObj = Instantiate(box, content.transform);
            GameObject lineObj = itemObj.transform.GetChild(0).gameObject;
            Dropdown dropdown1 = lineObj.transform.GetChild(2).GetComponent<Dropdown>();
            Dropdown dropdown2 = lineObj.transform.GetChild(11).GetComponent<Dropdown>();

            lineObj.transform.GetChild(1).GetComponent<InputField>().text = barrageBoxSetting[i].BoxName;
            lineObj.transform.GetChild(3).GetComponent<InputField>().text = barrageBoxSetting[i].Message;
            lineObj.transform.GetChild(5).GetComponent<InputField>().text = barrageBoxSetting[i].Tip;
            lineObj.transform.GetChild(7).GetComponent<InputField>().text = barrageBoxSetting[i].Count.ToString();
            lineObj.transform.GetChild(9).GetComponent<InputField>().text = barrageBoxSetting[i].Delay.ToString();

            ChoiceCall(dropdown1, barrageBoxSetting[i].Type);
            ChoiceCall(dropdown2, barrageBoxSetting[i].videoName);
        }
        isInit = true;
    }

    /// <summary>
    /// 初始化多特效配置box
    /// </summary>
    public void InitSpecialConfig()
    {
        RemoveAllItem();
        for (int i = 0; i < barrageSpecialBoxSetting.Count; i++)
        {
            GameObject itemObj = Instantiate(special, content.transform);
            GameObject lineObj = itemObj.transform.GetChild(0).gameObject;
            Dropdown dropdown1 = lineObj.transform.GetChild(2).GetComponent<Dropdown>();
            Dropdown dropdown2 = lineObj.transform.GetChild(11).GetComponent<Dropdown>();

            lineObj.transform.GetChild(1).GetComponent<InputField>().text = barrageSpecialBoxSetting[i].BoxName;
            lineObj.transform.GetChild(3).GetComponent<InputField>().text = barrageSpecialBoxSetting[i].Message;
            lineObj.transform.GetChild(5).GetComponent<InputField>().text = barrageSpecialBoxSetting[i].Tip;
            lineObj.transform.GetChild(7).GetComponent<InputField>().text = barrageSpecialBoxSetting[i].Count.ToString();
            lineObj.transform.GetChild(9).GetComponent<InputField>().text = barrageSpecialBoxSetting[i].Delay.ToString();

            ChoiceCall(dropdown1, barrageSpecialBoxSetting[i].Type);
            ChoiceCall(dropdown2, barrageSpecialBoxSetting[i].videoName);
        }
        isInit = true;
    }


    public void ChoiceCall(Dropdown dropdown, string name)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if (dropdown.options[i].text == name)
            {
                dropdown.value = i;
                return;
            }
        }
    }


    /// <summary>
    /// 执行功能
    /// </summary>
    /// <param name="callName"></param>
    public void CallFunction(string user, string avatar, string callName, int giftCount, int times, float delay)
    {
        for (int i = 0; i < giftCount * times; i++)
        {
            switch (callName)
            {
                case "乌龟一只":
                    MonsterCreater.Instance.OnCreateTortoise(1);
                    break;
                case "乌龟十只":
                    MonsterCreater.Instance.OnCreateTortoise(10);
                    break;
                case "乌龟一百只":
                    MonsterCreater.Instance.OnCreateTortoise(100);
                    break;
                case "蘑菇一只":
                    MonsterCreater.Instance.OnCreateMushroom(1);
                    break;
                case "蘑菇十只":
                    MonsterCreater.Instance.OnCreateMushroom(10);
                    break;
                case "蘑菇一百只":
                    MonsterCreater.Instance.OnCreateMushroom(100);
                    break;
                case "飞龟一只":
                    MonsterCreater.Instance.OnCreateFlyKoopa(1);
                    break;
                case "飞龟十只":
                    MonsterCreater.Instance.OnCreateFlyKoopa(10);
                    break;
                case "飞龟一百只":
                    MonsterCreater.Instance.OnCreateFlyKoopa(100);
                    break;
                case "飞鱼一只":
                    MonsterCreater.Instance.OnCreateFlyFish(1);
                    break;
                case "飞鱼十只":
                    MonsterCreater.Instance.OnCreateFlyFish(10);
                    break;
                case "飞鱼一百只":
                    MonsterCreater.Instance.OnCreateFlyFish(100);
                    break;
                case "甲壳虫一只":
                    MonsterCreater.Instance.OnCreateBeatles(1);
                    break;
                case "甲壳虫十只":
                    MonsterCreater.Instance.OnCreateBeatles(10);
                    break;
                case "甲壳虫一百只":
                    MonsterCreater.Instance.OnCreateBeatles(100);
                    break;
                case "游戏时间+10s":
                    GameManager.Instance.time += 10;
                    break;
                case "游戏时间-10s":
                    GameManager.Instance.time -= 10;
                    break;
                case "生命+10%":
                    ModData.mLife += (int)(ModData.mLife * 0.1f);
                    EventManager.Instance.SendMessage(Events.OnChangeLife);
                    break;
                case "生命-10%":
                    ModData.mLife -= (int)(ModData.mLife * 0.1f);
                    EventManager.Instance.SendMessage(Events.OnChangeLife);
                    break;
                case "生命+1":
                    ModData.mLife += 1;
                    EventManager.Instance.SendMessage(Events.OnChangeLife);
                    break;
                case "生命-1":
                    ModData.mLife -= 1;
                    EventManager.Instance.SendMessage(Events.OnChangeLife);
                    break;
                case "扔香蕉":
                    ItemCreater.Instance.OnCreateBanana(1);
                    break;
                case "动感DJ":
                    ModVideoPlayerCreater.Instance.OnPlayDJ();
                    break;
                case "万箭齐发":
                    ItemCreater.Instance.OnCreateManyArrow(1);
                    break;
                case "抓鸭子":
                    ModVideoPlayerCreater.Instance.OnCreateDuckVideoPlayer();
                    break;
                case "抓乌龟":
                    ModVideoPlayerCreater.Instance.OnCreateKoopaVideoPlayer();
                    break;
                case "乌萨奇":
                    ModVideoPlayerCreater.Instance.OnPlayWuSaQi();
                    break;
                case "灵魂拷问":
                    ModVideoPlayerCreater.Instance.OnPlayMenace();
                    break;
                case "乌萨奇硬控":
                    ModVideoPlayerCreater.Instance.OnPlayWuSaQi(true);
                    break;
                case "灵魂拷问硬控":
                    ModVideoPlayerCreater.Instance.OnPlayMenace(true);
                    break;
                case "上吊":
                    ItemCreater.Instance.OnCreateHangSelf();
                    break;
                case "一库":
                    ItemCreater.Instance.OnCreateMangSeng(1);
                    break;
                case "滚石":
                    ItemCreater.Instance.OnCreateRollStone(1);
                    break;
                case "滚刺":
                    ItemCreater.Instance.OnCreateRollArrow(1);
                    break;
                case "陨石":
                    ItemCreater.Instance.OnCreateMeteorite(1);
                    break;
                case "麒麟臂":
                    ItemCreater.Instance.OnCreateQiLinBi(1);
                    break;
                case "天残脚":
                    ItemCreater.Instance.OnCreateTCJiao(1);
                    break;
                case "随机天火":
                    ItemCreater.Instance.OnCreateUPFire(1);
                    break;
                case "全屏天火":
                    ItemCreater.Instance.OnCreateUPFire(66);
                    break;
                case "随机地火":
                    ItemCreater.Instance.OnCreateDownFire(1);
                    break;
                case "全屏地火":
                    ItemCreater.Instance.OnCreateDownFire(66);
                    break;
                case "随机传送":
                    GameModController.Instance.OnRandromPlayerPos();
                    break;
                case "随机关卡":
                    GameModController.Instance.OnRandromPass();
                    break;
                case "铁链":
                    ItemCreater.Instance.OnCreateChainPlayer(1);
                    break;
                case "雷电":
                    ItemCreater.Instance.OnCreateLazzer(1);
                    break;
                case "砖块+10":
                    CreateWallManager.Instance.wallCount += 10;
                    break;
                case "石头+10":
                    CreateWallManager.Instance.stonesCount += 10;
                    break;
                case "美女盲盒":
                    ModVideoPlayerCreater.Instance.OnPlayGrilVideo();
                    break;
            }
        }
    }
}