using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Lottery : MonoBehaviour
{
    public GameObject[] lotteryObjs;
    public Transform selectCalls;
    public Transform selectBoxs;
    public Transform selectSpecials;
    public Transform selectAvatar;
    public Transform calls;


    public GameObject selectCallObj;
    public GameObject callObj;

    public Dropdown videos;

    private BarrageController barrageConfig;


    [Serializable]
    public class LotteryItemSetting
    {
        public string lotteryName;
        public SettingType type;
        public string callName;
        public int count;
    }

    public enum SettingType{
        None,
        Box,
        Special
    }

    private void TriggerConfigByType(BarrageBase barrageBase, string type, string message, int triggerCount)
    {
        if (barrageBase == null) return;

        var data = new BarrageData
        {
            Type = type,
            name = "抽奖用户",
            message = message,
            userAvatar = "",
            num = Mathf.Max(1, triggerCount),
            count = Mathf.Max(1, triggerCount)
        };

        string json = JsonUtility.ToJson(data);
        switch (type)
        {
            case "礼物":
                barrageBase.HandleGift(json);
                break;
            case "弹幕":
                barrageBase.HandleBarrage(json);
                break;
            case "关注":
                barrageBase.HandleAttention(json);
                break;
            case "进入":
                barrageBase.HandleJoin(json);
                break;
            case "点赞":
                barrageBase.HandleLike(json);
                break;
            default:
                // 兜底：按弹幕触发（需要 Message 匹配）
                barrageBase.HandleBarrage(json);
                break;
        }
    }

    private void Awake()
    {
        barrageConfig = FindAnyObjectByType<BarrageController>();
    }

    void Start()
    {

    }

    void Update()
    {
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    // 改为由 BarrageController 统一播放
    IEnumerator PlayVideoAndWait()
    {
        if (barrageConfig == null)
        {
            barrageConfig = FindAnyObjectByType<BarrageController>();
            if (barrageConfig == null)
            {
                Debug.LogError("未找到 BarrageController，无法播放视频");
                yield break;
            }
        }

        if (videos.options[videos.value].text != "空")
        {
            string boxPath = $"Box/{videos.options[videos.value].text}";
            yield return barrageConfig.PlayBoxVideoAndWait(boxPath, 2, false, ModVideoPlayerCreater.Instance != null ? ModVideoPlayerCreater.Instance.transform : null);
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
        if (barrageConfig != null && barrageConfig.barrageLotterySettings != null)
        {
            int idx = transform.GetSiblingIndex();
            if (idx >= 0 && idx < barrageConfig.barrageLotterySettings.Count)
            {
                barrageConfig.barrageLotterySettings.RemoveAt(idx);
            }
        }
    }

    public void LoadCalls()
    {
        if (barrageConfig != null)
        {
            ClearContainer(selectCalls);
            ClearContainer(calls);

            int boxIndex = transform.GetSiblingIndex();

            if (boxIndex < 0 || boxIndex >= barrageConfig.barrageLotterySettings.Count)
            {
                Debug.LogError($"索引 {boxIndex} 超出范围");
                return;
            }

            // 已选择的列表
            List<string> inCalls = new List<string>();
            foreach(var item in barrageConfig.barrageLotterySettings[boxIndex].LotteryItem)
            {
                inCalls.Add(item.callName);
            }

            for(int i = 0; i < barrageConfig.barrageLotterySettings[boxIndex].LotteryItem.Count; i++)
            {
                GameObject obj = Instantiate(callObj, calls);

                InputField text1 = obj.transform.GetChild(0).gameObject.GetComponent<InputField>();
                if (text1 != null) text1.text = barrageConfig.barrageLotterySettings[boxIndex].LotteryItem[i].callName;

                // 修改奖项名称
                InputField inputField1 = obj.transform.GetChild(0).GetComponent<InputField>();
                if (inputField1 != null)
                {
                    inputField1.onValueChanged.AddListener((value) =>
                    {
                        barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].lotteryName = value;
                    });
                }

                InputField inputField2 = obj.transform.GetChild(1).GetComponent<InputField>();
                if (inputField2 != null)
                {
                    inputField2.text = barrageConfig.barrageLotterySettings[boxIndex].LotteryItem[i].count.ToString();

                    inputField2.onValueChanged.AddListener((value) =>
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].count = intValue;
                        }
                        else
                        {
                            // 处理无效输入
                            barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].count = 1;
                        }
                    });
                }

                Button btn_close = obj.transform.GetChild(2).GetComponent<Button>();
                if (btn_close != null)
                {
                    btn_close.onClick.AddListener(() => RemoveCall(btn_close));
                }
            }


            // 加载未选择的功能
            foreach (string name in barrageConfig.Calls)
            {
                // 检查是否已存在
                bool alreadyExists = false;
                foreach (string inName in inCalls)
                {
                    if (name == inName)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // 如果已存在，跳过
                if (alreadyExists) continue;

                // 创建新的选项
                GameObject obj = Instantiate(selectCallObj, selectCalls);
                Text text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                if (text != null) text.text = name;

                Button button = obj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => JoinCall(obj, SettingType.None));
                }
            }

            // 加载未选择的盲盒配置
            foreach (var item in barrageConfig.barrageBoxSetting)
            {
                // 检查是否已存在
                bool alreadyExists = false;
                foreach (string inName in inCalls)
                {
                    if (item.BoxName == inName)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // 如果已存在，跳过
                if (alreadyExists) continue;

                GameObject obj = Instantiate(selectCallObj, selectBoxs);
                Text text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                if (text != null) text.text = item.BoxName;

                Button button = obj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => JoinCall(obj, SettingType.Box));
                }
                
            }

            // 加载未选择的多特效配置
            foreach (var item in barrageConfig.barrageSpecialBoxSetting)
            {
                // 检查是否已存在
                bool alreadyExists = false;
                foreach (string inName in inCalls)
                {
                    if (item.BoxName == inName)
                    {
                        alreadyExists = true;
                        break;
                    }
                }

                // 如果已存在，跳过
                if (alreadyExists) continue;

                GameObject obj = Instantiate(selectCallObj, selectSpecials);
                Text text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                if (text != null) text.text = item.BoxName;

                Button button = obj.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.AddListener(() => JoinCall(obj, SettingType.Special));
                }
                
            }

            // 加载本地图片
            try
            {
                string configDir = Path.Combine(Directory.GetCurrentDirectory(), "Config");
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                var pngFiles = Directory.GetFiles(configDir, "*.png", SearchOption.TopDirectoryOnly);
                List<string> options = new List<string> { "空" };
                foreach (var file in pngFiles)
                {
                    Debug.Log(Path.GetFileName(file));
                    GameObject obj = Instantiate(selectCallObj, selectAvatar);
                    Text text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                    if (text != null) text.text = Path.GetFileName(file);

                    Button button = obj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() =>
                        {
                            barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].avatarPath = file;
                        });
                    }
                }

            }
            catch (Exception e)
            {
                Debug.LogWarning($"加载 Config png 失败: {e.Message}");
            }
            
        }
    }


    private void ClearContainer(Transform container)
    {
        if (container == null) return;

        for (int i = container.childCount - 1; i >= 0; i--)
        {
            Transform child = container.GetChild(i);
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
    }


    /// <summary>
    /// 选入
    /// </summary>
    public void JoinCall(GameObject call, SettingType type)
    {
        string input = barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryCount;
        string numberOnly = new string(input.Where(char.IsDigit).ToArray());
        int itemCount = int.Parse(numberOnly);

        if(itemCount > barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem.Count)
        {
            GameObject obj = Instantiate(callObj, calls);
            if (obj != null)
            {
                string name = call.transform.GetChild(0).GetComponent<Text>().text;
                LotteryItemSetting lotteryItemSetting = new LotteryItemSetting();
                lotteryItemSetting.lotteryName = name;
                lotteryItemSetting.type = type;
                lotteryItemSetting.callName = name;
                lotteryItemSetting.count = 1;
                barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem.Add(lotteryItemSetting);


                obj.transform.GetChild(0).GetComponent<InputField>().text = name;

                // 修改奖项名称
                InputField inputField1 = obj.transform.GetChild(0).GetComponent<InputField>();
                if (inputField1 != null)
                {
                    inputField1.onValueChanged.AddListener((value) =>
                    {
                        barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].lotteryName = value;

                        Debug.Log(value);
                    });
                }

                // 修改概率
                InputField inputField2 = obj.transform.GetChild(1).GetComponent<InputField>();
                if (inputField2 != null)
                {
                    inputField2.onValueChanged.AddListener((value) =>
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].count = intValue;
                        }
                        else
                        {
                            // 处理无效输入
                            barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()].LotteryItem[inputField1.transform.parent.GetSiblingIndex()].count = 1;
                        }
                    });
                }

                Button btn_close = obj.transform.GetChild(2).GetComponent<Button>();
                if (btn_close != null)
                {
                    btn_close.onClick.AddListener(() => RemoveCall(btn_close));
                }
            }
            Destroy(call);
        }
        else
        {
            Debug.Log("超出设置范围.");
        }
    }

    /// <summary>
    /// 移除
    /// </summary>
    public void RemoveCall(Button button)
    {
        int siblingIndex = transform.GetSiblingIndex();
        int itemIndex = button.transform.parent.GetSiblingIndex();

        var lotteryItem = barrageConfig.barrageLotterySettings[siblingIndex].LotteryItem[itemIndex];
        SettingType settingType = lotteryItem.type;
        string callName = lotteryItem.callName; // 保存功能名称

        Destroy(button.transform.parent.gameObject);
        barrageConfig.barrageLotterySettings[siblingIndex].LotteryItem.RemoveAt(itemIndex);

        CreateSelectObjectForType(settingType, callName); // 传递功能名称
    }

    private void CreateSelectObjectForType(SettingType settingType, string callName)
    {
        // 获取对应的父对象
        Transform parent = settingType switch
        {
            SettingType.None => selectCalls,
            SettingType.Box => selectBoxs,
            SettingType.Special => selectSpecials,
            _ => null
        };

        if (parent == null)
        {
            Debug.LogError($"未找到SettingType {settingType}对应的父对象");
            return;
        }

        CreateSelectCallObject(parent, callName); // 传递功能名称
    }

    private void CreateSelectCallObject(Transform parent, string callName)
    {
        GameObject newObj = Instantiate(selectCallObj, parent);

        // 设置文本 - 使用传递进来的功能名称
        if (newObj.transform.GetChild(0).TryGetComponent<Text>(out Text text))
        {
            text.text = callName; // 使用callName而不是name
        }

        // 设置按钮点击事件
        if (newObj.TryGetComponent<Button>(out Button button))
        {
            button.onClick.AddListener(() => JoinCall(newObj, SettingType.None));
        }
    }



    /// <summary>
    /// 修改配置
    /// </summary>
    public void ChangeConfig()
    {
        if (barrageConfig.isInit)
        {
            BarrageLotterySetting barrageLotterySetting = barrageConfig.barrageLotterySettings[transform.GetSiblingIndex()];

            Transform line = transform.GetChild(0);

            foreach (Transform child in line)
            {
                if (child.gameObject.name == "InputField1") barrageLotterySetting.Title = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "Dropdown1") barrageLotterySetting.Type = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
                if (child.gameObject.name == "InputField2") barrageLotterySetting.Message = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField3") barrageLotterySetting.Tip = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField4")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (int.TryParse(text, out int value))
                    {
                        barrageLotterySetting.Count = value;
                    }
                    else
                    {
                        barrageLotterySetting.Count = 1;
                        Debug.Log("解析倍率失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "InputField5")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (float.TryParse(text, out float value))
                    {
                        barrageLotterySetting.Delay = value;
                    }
                    else
                    {
                        barrageLotterySetting.Delay = 0;
                        Debug.Log("解析延迟失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "Dropdown2") barrageLotterySetting.LotteryCount = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
            }
        }
    }

    /// <summary>
    /// 测试功能
    /// </summary>
    public void TestCall()
    {
        BarrageBase barrageBase = FindAnyObjectByType<BarrageBase>();
        if (barrageBase == null)
        {
            Debug.LogWarning("未找到 BarrageBase，无法模拟弹幕触发");
            return;
        }

        // 盲盒弹幕触发只依赖 Message 匹配，因此这里用当前配置的 Message 做一次模拟
        int boxIndex = transform.GetSiblingIndex();
        if (barrageConfig == null) barrageConfig = FindAnyObjectByType<BarrageController>();
        if (barrageConfig == null || boxIndex < 0 || boxIndex >= barrageConfig.barrageLotterySettings.Count)
        {
            Debug.LogWarning("BarrageController/盲盒索引无效，无法模拟弹幕触发");
            return;
        }

        // 走抽奖专用队列（BarrageBase.HandleLottery -> EnqueueLottery -> ProcessLotteryQueue），
        // 从而按“倍率Count”和“间隔Delay”执行连抽。
        var setting = barrageConfig.barrageLotterySettings[boxIndex];
        string msg = setting.Message;
        var data = new BarrageData
        {
            Type = setting.Type,
            name = "测试用户",
            message = msg,
            userAvatar = "",
            num = 1,
            count = 1
        };
        string json = JsonUtility.ToJson(data);
        barrageBase.HandleLottery(json);
    }

    // 由 BarrageBase 触发抽奖时，调用该方法真正弹出 UI 并执行逻辑
    // 返回本次创建的转盘 UI 根物体（用于外部串行等待），创建失败返回 null
    public GameObject StartLotteryUI()
    {
        // 缓存索引/配置引用，避免回调里再访问 transform（Lottery 可能被 Destroy）
        int idx = transform.GetSiblingIndex();
        if (barrageConfig == null) barrageConfig = FindAnyObjectByType<BarrageController>();
        if (barrageConfig == null || barrageConfig.barrageLotterySettings == null || idx < 0 || idx >= barrageConfig.barrageLotterySettings.Count)
        {
            Debug.LogWarning("Lottery: BarrageController/抽奖索引无效，无法启动抽奖UI");
            return null;
        }

        var lotterySetting = barrageConfig.barrageLotterySettings[idx];

        if(lotterySetting.LotteryItem == null || lotterySetting.LotteryItem.Count == 0)
        {
            Debug.LogWarning($"未配置执行方法：{idx}");
            return null;
        }

        string input = lotterySetting.LotteryCount;
        string numberOnly = new string(input.Where(char.IsDigit).ToArray());
        int itemCount = int.Parse(numberOnly);

        GameObject obj = new GameObject();
        switch (itemCount)
        {
            case 4:
                obj = Instantiate(lotteryObjs[0]);
                break;
            case 8:
                obj = Instantiate(lotteryObjs[1]);
                break;
            case 12:
                obj = Instantiate(lotteryObjs[2]);
                break;
        }

        if (obj == null)
        {
            Debug.LogWarning("Lottery: 未能创建抽奖UI预制体");
            return null;
        }

        // 记录 root：LotteryController 会 Destroy(root)，外部可用它判断何时结束
        GameObject uiRoot = obj;

    obj = obj.transform.GetChild(0).gameObject;

        // 设置标题
        Text title = obj.transform.GetChild(0).GetComponent<Text>();
    title.text = lotterySetting.Title;

        GameObject Items = obj.transform.GetChild(2).gameObject;
        int index = 0;
        foreach (Transform Item in Items.transform)
        {
            LotteryItem lotteryItem = Item.GetComponent<LotteryItem>();
            lotteryItem.Init(lotterySetting.LotteryItem[index].callName);
            index++;
            if (index == lotterySetting.LotteryItem.Count) break;
        }

        LotteryController lotteryController = Items.GetComponent<LotteryController>();
        // 将每个奖项的 count 作为概率权重传给 LotteryController
        if (lotteryController != null)
        {
            lotteryController.weights.Clear();
            for (int i = 0; i < lotterySetting.LotteryItem.Count; i++)
            {
                int w = 1;
                try
                {
                    w = lotterySetting.LotteryItem[i].count;
                }
                catch { }
                lotteryController.weights.Add(Mathf.Max(0, w));
            }

            // 对齐到 UI 实际的 item 数量，避免 mismatch
            while (lotteryController.weights.Count < lotteryController.lotteryItems.Count)
            {
                lotteryController.weights.Add(1);
            }
            if (lotteryController.weights.Count > lotteryController.lotteryItems.Count)
            {
                lotteryController.weights.RemoveRange(lotteryController.lotteryItems.Count, lotteryController.weights.Count - lotteryController.lotteryItems.Count);
            }
        }
        // 订阅抽奖结果：itemName 就是 callName
        lotteryController.OnResult = (selectedCallName) =>
        {
            // 回调触发时 Lottery 组件可能已被 Destroy（比如配置面板被关闭/重建）
            if (barrageConfig == null || barrageConfig.barrageLotterySettings == null || idx < 0 || idx >= barrageConfig.barrageLotterySettings.Count)
            {
                Debug.LogWarning("Lottery: 配置索引越界，无法执行抽中功能");
                return;
            }

            var runtimeSetting = barrageConfig.barrageLotterySettings[idx];
            if (runtimeSetting == null || runtimeSetting.LotteryItem == null)
            {
                Debug.LogWarning("Lottery: 抽奖配置为空，无法执行抽中功能");
                return;
            }

            var setting = runtimeSetting.LotteryItem.Find(x => x.callName == selectedCallName);
            if (setting == null)
            {
                Debug.LogWarning($"Lottery: 未找到抽中项配置: {selectedCallName}");
                return;
            }

            // 抽奖的 Count 作为执行次数（至少 1 次）
            int execCount = Mathf.Max(1, setting.count);

            switch (setting.type)
            {
                case SettingType.None:
                    // 直接执行普通功能
                    BarrageController.Instance.EnqueueAction("抽奖用户", "", setting.callName, 1, execCount, 0f);
                    break;

                case SettingType.Box:
                    {
                        // 抽中“盲盒配置”时：语义应等同于触发该盲盒配置（走其倍率/Delay/视频/串行队列），而不是只抽一次 Calls。
                        // 这里通过构造礼物事件 JSON，交给 BarrageBase.HandleGift 走同一条匹配与执行链路。
                        var barrageBase = FindAnyObjectByType<BarrageBase>();
                        if (barrageBase == null)
                        {
                            Debug.LogWarning("Lottery: 未找到 BarrageBase，无法触发盲盒");
                            break;
                        }

                        var boxConfig = barrageConfig.barrageBoxSetting.Find(b => b.BoxName == setting.callName);
                        if (boxConfig == null)
                        {
                            Debug.LogWarning($"Lottery: 未找到盲盒配置: {setting.callName}");
                            break;
                        }

                        // execCount 表示“触发次数”（用于重复触发同一盲盒配置）；盲盒自身的倍率 box.Count 会在 BarrageBase 内部生效。
                        TriggerConfigByType(barrageBase, boxConfig.Type, boxConfig.Message, execCount);
                        break;
                    }

                case SettingType.Special:
                    {
                        // 抽中“多特效配置”同理：应等同于触发该配置（走倍率/Delay/视频/串行队列）
                        var barrageBase = FindAnyObjectByType<BarrageBase>();
                        if (barrageBase == null)
                        {
                            Debug.LogWarning("Lottery: 未找到 BarrageBase，无法触发多特效");
                            break;
                        }

                        var specialConfig = barrageConfig.barrageSpecialBoxSetting.Find(b => b.BoxName == setting.callName);
                        if (specialConfig == null)
                        {
                            Debug.LogWarning($"Lottery: 未找到多特效配置: {setting.callName}");
                            break;
                        }

                        TriggerConfigByType(barrageBase, specialConfig.Type, specialConfig.Message, execCount);
                        break;
                    }
            }
        };
        lotteryController.StartCoroutine(lotteryController.LotteryStart(lotterySetting.avatarPath));

        return uiRoot;
    }


}
