using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Box : MonoBehaviour
{
    public Transform selectCalls;
    public Transform calls;
    public GameObject selectCallObj;
    public GameObject callObj;

    public Dropdown videos;

    private BarrageController barrageConfig;

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

        if(videos.options[videos.value].text != "空")
        {
            string boxPath = $"Box/{videos.options[videos.value].text}";
            yield return barrageConfig.PlayBoxVideoAndWait(boxPath, 2, false, ModVideoPlayerCreater.Instance != null ? ModVideoPlayerCreater.Instance.transform : null);
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
        barrageConfig.barrageBoxSetting.RemoveAt(transform.GetSiblingIndex());
    }

    public void LoadCalls()
    {
        if (barrageConfig != null)
        {
            ClearContainer(selectCalls);
            ClearContainer(calls);

            int boxIndex = transform.GetSiblingIndex();

            if (boxIndex < 0 || boxIndex >= barrageConfig.barrageBoxSetting.Count)
            {
                Debug.LogError($"索引 {boxIndex} 超出范围");
                return;
            }

            var currentBoxCalls = barrageConfig.barrageBoxSetting[boxIndex].Calls;

            Dictionary<string, int> callCountDict = new Dictionary<string, int>();

            // 统计每个名称的出现次数
            foreach (string name in currentBoxCalls)
            {
                if (callCountDict.ContainsKey(name))
                {
                    callCountDict[name]++;
                }
                else
                {
                    callCountDict[name] = 1;
                }
            }

            foreach (var kvp in callCountDict)
            {
                string name = kvp.Key;
                int count = kvp.Value;

                GameObject obj = Instantiate(callObj, calls);
                Text text = obj.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
                if (text != null) text.text = name;

                InputField inputField = obj.transform.GetChild(1).GetComponent<InputField>();
                if (inputField != null)
                {
                    inputField.text = count.ToString();

                    inputField.onValueChanged.AddListener((value) =>
                    {
                        if (int.TryParse(value, out int intValue))
                        {
                            ChangeCountInCall(name, intValue);
                        }
                        else
                        {
                            // 处理无效输入
                            ChangeCountInCall(name, 0);
                        }
                    });
                }
            }

            // 加载未选择的功能
            foreach (string name in barrageConfig.Calls)
            {
                if (!callCountDict.ContainsKey(name)) // 只显示未选择的
                {
                    GameObject obj = Instantiate(selectCallObj, selectCalls);
                    Text text = obj.transform.GetChild(0).gameObject.GetComponent<Text>();
                    if (text != null) text.text = name;

                    Button button = obj.GetComponent<Button>();
                    if (button != null)
                    {
                        button.onClick.AddListener(() => JoinCall(obj));
                    }
                }
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
    public void JoinCall(GameObject call)
    {
        GameObject obj = Instantiate(callObj, calls);
        if(obj != null)
        {
            string name = call.transform.GetChild(0).GetComponent<Text>().text;
            barrageConfig.barrageBoxSetting[transform.GetSiblingIndex()].Calls.Add(name);

            obj.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = name;
            InputField inputField = obj.transform.GetChild(1).GetComponent<InputField>();
            if (inputField != null)
            {
                inputField.onValueChanged.AddListener((value) =>
                {
                    if (int.TryParse(value, out int intValue))
                    {
                        ChangeCountInCall(name, intValue);
                    }
                    else
                    {
                        // 处理无效输入
                        ChangeCountInCall(name, 0);
                    }
                });
            }
        }
        Destroy(call);
    }

    /// <summary>
    /// 修改功能数量
    /// </summary>
    public void ChangeCountInCall(string callName, int value)
    {
        int boxIndex = transform.GetSiblingIndex();

        if (boxIndex < 0 || boxIndex >= barrageConfig.barrageBoxSetting.Count)
        {
            Debug.LogError($"Box索引 {boxIndex} 超出范围");
            return;
        }

        var calls = barrageConfig.barrageBoxSetting[boxIndex].Calls;

        for (int i = calls.Count - 1; i >= 0; i--)
        {
            if (calls[i] == callName)
            {
                calls.RemoveAt(i);
            }
        }

        // 2. 添加指定数量
        for (int i = 0; i < value; i++)
        {
            calls.Add(callName);
        }

        Debug.Log($"功能 '{callName}' 设置为 {value} 个");
    }

    /// <summary>
    /// 修改配置
    /// </summary>
    public void ChangeConfig()
    {
        if (barrageConfig.isInit)
        {
            BarrageBoxSetting barrageBoxSetting = barrageConfig.barrageBoxSetting[transform.GetSiblingIndex()];

            Transform line = transform.GetChild(0);

            foreach (Transform child in line)
            {
                if (child.gameObject.name == "InputField1") barrageBoxSetting.BoxName = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "Dropdown1") barrageBoxSetting.Type = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
                if (child.gameObject.name == "InputField2") barrageBoxSetting.Message = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField3") barrageBoxSetting.Tip = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField4")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (int.TryParse(text, out int value))
                    {
                        barrageBoxSetting.Count = value;
                    }
                    else
                    {
                        barrageBoxSetting.Count = 1;
                        Debug.Log("解析倍率失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "InputField5")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (float.TryParse(text, out float value))
                    {
                        barrageBoxSetting.Delay = value;
                    }
                    else
                    {
                        barrageBoxSetting.Delay = 0;
                        Debug.Log("解析延迟失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "Dropdown2") barrageBoxSetting.videoName = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
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
        if (barrageConfig == null || boxIndex < 0 || boxIndex >= barrageConfig.barrageBoxSetting.Count)
        {
            Debug.LogWarning("BarrageController/盲盒索引无效，无法模拟弹幕触发");
            return;
        }

        string msg = barrageConfig.barrageBoxSetting[boxIndex].Message;
        var data = new BarrageData
        {
            Type = barrageConfig.barrageBoxSetting[boxIndex].Type,
            name = "测试用户",
            message = msg,
            userAvatar = "",
            num = 1,
            count = 1
        };
        string json = JsonUtility.ToJson(data);

        switch(data.Type)
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
        }
    }
}
