using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static Lottery;

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

[Serializable]
public class BarrageLotterySetting
{
    public string Title; // 抽奖标题
    public string Type; // 数据类型名称
    public string Message; // 触发内容
    public string Tip; // 提示
    public int Count; // 倍率
    public float Delay; // 延迟

    public string LotteryCount; // 抽奖个数

    public string avatarPath; // 头像地址

    public List<LotteryItemSetting> LotteryItem = new List<LotteryItemSetting>(); // 抽奖项

}

public enum PrankType
{
    normal,
    box,
    special,
    lottery
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

public class BarrageLottoryWrapper
{
    public List<BarrageLotterySetting> LottoryConfigs;
}

public class BarrageController : MonoBehaviour
{
    public static BarrageController Instance { get; set; }

    // 功能名称
    public List<string> Calls = new List<string> ();

    [Tooltip("当前整蛊配置类型")]
    public PrankType prankType;

    public InputField searchInput;
    public GameObject content;
    public GameObject item;
    public GameObject box;
    public GameObject special;
    public GameObject lottery;
    [Header("视频播放器")]
    public GameObject videoPlayerPrefab;

    public List<BarrageNormalSetting> barrageNormalSetting = new List<BarrageNormalSetting>();
    public List<BarrageBoxSetting> barrageBoxSetting = new List<BarrageBoxSetting>();
    public List<BarrageSpecialBoxSetting> barrageSpecialBoxSetting = new List<BarrageSpecialBoxSetting>();
    public List<BarrageLotterySetting> barrageLotterySettings = new List<BarrageLotterySetting> ();
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

        yield return new WaitUntil(() => obj == null || !obj.activeInHierarchy);
    }

    /// <summary>
    /// 执行功能
    /// </summary>
    /// <param name="task"></param>
    private void ExecuteAction(ActionTask task)
    {
        BarrageFuncController.Instance.OnAddReadyFunc(task.callName);
        return;
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

        // 快捷搜索：实时按关键字过滤当前列表
        if (searchInput != null)
        {
            searchInput.onValueChanged.RemoveListener(OnSearchChanged);
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        }
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
        // 切换类型时清掉搜索，避免残留过滤导致“啥都没有”
        if (searchInput != null)
        {
            searchInput.onValueChanged.RemoveListener(OnSearchChanged);
            searchInput.text = string.Empty;
            searchInput.onValueChanged.AddListener(OnSearchChanged);
        }
        if (type == (int)PrankType.normal)
        {
            RemoveAllItem();
            InitNormalConfig();
        }
        else if(type == (int)PrankType.box)
        {
            RemoveAllItem();
            InitBoxConfig();
        }
        else if (type == (int)PrankType.special)
        {
            RemoveAllItem();
            InitSpecialConfig();
        }
        else if (type == (int)PrankType.lottery)
        {
            RemoveAllItem();
            InitLottoryConfig();
        }

        // 初始化完后立即应用一次过滤
        ApplySearchFilter(searchInput != null ? searchInput.text : string.Empty);
    }

    private void OnSearchChanged(string keyword)
    {
        ApplySearchFilter(keyword);
    }

    private void ApplySearchFilter(string keyword)
    {
        if (content == null) return;

        keyword = (keyword ?? string.Empty).Trim();
        if (string.IsNullOrEmpty(keyword))
        {
            foreach (Transform child in content.transform)
            {
                if (child != null) child.gameObject.SetActive(true);
            }
            return;
        }

        for (int i = 0; i < content.transform.childCount; i++)
        {
            var child = content.transform.GetChild(i);
            if (child == null) continue;

            bool match = IsItemMatch(child, keyword);
            child.gameObject.SetActive(match);
        }
    }

    private bool IsItemMatch(Transform itemTransform, string keyword)
    {
        // 匹配关键字段（名称/标题/触发内容/提示）
        StringComparison cmp = StringComparison.OrdinalIgnoreCase;

        bool Contains(string s)
        {
            return !string.IsNullOrEmpty(s) && s.IndexOf(keyword, cmp) >= 0;
        }

        // InputField：标题/名称、触发消息、提示、倍率、延迟
        var fields = itemTransform.GetComponentsInChildren<InputField>(true);
        foreach (var f in fields)
        {
            if (f == null) continue;
            // 标题/名称、触发消息、提示 都在 InputField 内
            if (Contains(f.text)) return true;
        }

        // 如果有 Text（比如按钮 label），也参与搜索
        var texts = itemTransform.GetComponentsInChildren<Text>(true);
        foreach (var t in texts)
        {
            if (t == null) continue;
            if (Contains(t.text)) return true;
        }

        // 下拉框当前选中项也参与搜索（类型、视频名、抽奖个数等）
        var dropdowns = itemTransform.GetComponentsInChildren<Dropdown>(true);
        foreach (var d in dropdowns)
        {
            if (d == null || d.options == null || d.options.Count == 0) continue;
            int idx = Mathf.Clamp(d.value, 0, d.options.Count - 1);
            if (Contains(d.options[idx].text)) return true;
        }

        return false;
    }

    /// <summary>
    /// 添加配置
    /// </summary>
    public void AddItem()
    {
        if(prankType == PrankType.normal)
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
        else if(prankType == PrankType.box)
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
        else if (prankType == PrankType.lottery)
        {
            GameObject obj = Instantiate(lottery, content.transform);

            BarrageLotterySetting config = new BarrageLotterySetting();
            config.Count = 1;
            config.LotteryCount = "8个";

            barrageLotterySettings.Add(config);
        }
    }

    /// <summary>
    /// 清空配置
    /// </summary>
    public void RemoveAllItem()
    {
        foreach(Transform obj in content.transform)
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

        string filePath1 = Path.Combine(Directory.GetCurrentDirectory(),"Config" , "NormalData.json");

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

        BarrageLottoryWrapper barrageLottoryWrapper = new BarrageLottoryWrapper();
        barrageLottoryWrapper.LottoryConfigs = barrageLotterySettings;

        string filePath4 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "LottoryData.json");
        string jsonData4 = JsonUtility.ToJson(barrageLottoryWrapper, true);

        File.WriteAllText(filePath4, jsonData4);

        Debug.Log("抽奖配置数据已保存到: " + filePath4);
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


        string filePath4 = Path.Combine(Directory.GetCurrentDirectory(), "Config", "LottoryData.json");
        if (!File.Exists(filePath4))
        {
            Debug.LogWarning("未找到配置文件: " + filePath4);
            return;
        }

        try
        {
            string jsonData = File.ReadAllText(filePath4);

            BarrageLottoryWrapper wrapper = JsonUtility.FromJson<BarrageLottoryWrapper>(jsonData);
            barrageLotterySettings = wrapper.LottoryConfigs;

            Debug.Log($"成功加载 {wrapper.LottoryConfigs.Count} 条抽奖配置数据");
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

    /// <summary>
    /// 初始化抽奖配置box
    /// </summary>
    public void InitLottoryConfig()
    {
        RemoveAllItem();
        for (int i = 0; i < barrageLotterySettings.Count; i++)
        {
            GameObject itemObj = Instantiate(lottery, content.transform);
            GameObject lineObj = itemObj.transform.GetChild(0).gameObject;
            Dropdown dropdown1 = lineObj.transform.GetChild(2).GetComponent<Dropdown>();
            Dropdown dropdown2 = lineObj.transform.GetChild(11).GetComponent<Dropdown>();

            lineObj.transform.GetChild(1).GetComponent<InputField>().text = barrageLotterySettings[i].Title;
            lineObj.transform.GetChild(3).GetComponent<InputField>().text = barrageLotterySettings[i].Message;
            lineObj.transform.GetChild(5).GetComponent<InputField>().text = barrageLotterySettings[i].Tip;
            lineObj.transform.GetChild(7).GetComponent<InputField>().text = barrageLotterySettings[i].Count.ToString();
            lineObj.transform.GetChild(9).GetComponent<InputField>().text = barrageLotterySettings[i].Delay.ToString();

            ChoiceCall(dropdown1, barrageLotterySettings[i].Type);
            ChoiceCall(dropdown2, barrageLotterySettings[i].LotteryCount);
        }
        isInit = true;
    }


    public void ChoiceCall(Dropdown dropdown, string name)
    {
        for (int i = 0; i < dropdown.options.Count; i++)
        {
            if(dropdown.options[i].text == name)
            {
                dropdown.value = i;
                return;
            }
        }
    }

}