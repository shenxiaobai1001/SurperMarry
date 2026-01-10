using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Special : MonoBehaviour
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

        if (videos.options[videos.value].text != "空")
        {
            string boxPath = $"Box/{videos.options[videos.value].text}";
            yield return barrageConfig.PlayBoxVideoAndWait(boxPath, 2, false, null);
        }
    }

    public void Remove()
    {
        Destroy(gameObject);
        barrageConfig.barrageSpecialBoxSetting.RemoveAt(transform.GetSiblingIndex());
    }

    public void LoadCalls()
    {
        if (barrageConfig != null)
        {
            ClearContainer(selectCalls);
            ClearContainer(calls);

            int boxIndex = transform.GetSiblingIndex();

            if (boxIndex < 0 || boxIndex >= barrageConfig.barrageSpecialBoxSetting.Count)
            {
                Debug.LogError($"索引 {boxIndex} 超出范围");
                return;
            }

            var currentBoxCalls = barrageConfig.barrageSpecialBoxSetting[boxIndex].Calls;

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
            barrageConfig.barrageSpecialBoxSetting[transform.GetSiblingIndex()].Calls.Add(name);
                
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
        Debug.Log(boxIndex);

        if (boxIndex < 0 || boxIndex >= barrageConfig.barrageSpecialBoxSetting.Count)
        {
            Debug.LogError($"Box索引 {boxIndex} 超出范围");
            return;
        }

        var calls = barrageConfig.barrageSpecialBoxSetting[boxIndex].Calls;

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
            BarrageSpecialBoxSetting barrageSpecialBoxSetting = barrageConfig.barrageSpecialBoxSetting[transform.GetSiblingIndex()];

            Transform line = transform.GetChild(0);

            foreach (Transform child in line)
            {
                if (child.gameObject.name == "InputField1") barrageSpecialBoxSetting.BoxName = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "Dropdown1") barrageSpecialBoxSetting.Type = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
                if (child.gameObject.name == "InputField2") barrageSpecialBoxSetting.Message = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField3") barrageSpecialBoxSetting.Tip = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField4")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (int.TryParse(text, out int value))
                    {
                        barrageSpecialBoxSetting.Count = value;
                    }
                    else
                    {
                        barrageSpecialBoxSetting.Count = 1;
                        Debug.Log("解析倍率失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "InputField5")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (float.TryParse(text, out float value))
                    {
                        barrageSpecialBoxSetting.Delay = value;
                    }
                    else
                    {
                        barrageSpecialBoxSetting.Delay = 0;
                        Debug.Log("解析延迟失败，使用默认值.");
                    }
                }
                if (child.gameObject.name == "Dropdown2") barrageSpecialBoxSetting.videoName = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
            }
        }
    }

    /// <summary>
    /// 测试功能：先播放视频，等待视频结束后再触发功能
    /// </summary>
    public void TestCall()
    {
        StartCoroutine(TestCallRoutine());
    }

    private IEnumerator TestCallRoutine()
    {
        // 调用控制器的播放逻辑
        yield return PlayVideoAndWait();

        foreach(var callName in BarrageController.Instance.barrageSpecialBoxSetting[transform.GetSiblingIndex()].Calls)
        {
            int index = UnityEngine.Random.Range(0, BarrageController.Instance.barrageSpecialBoxSetting[transform.GetSiblingIndex()].Calls.Count);
            int times = BarrageController.Instance.barrageSpecialBoxSetting[transform.GetSiblingIndex()].Count;
            float delay = BarrageController.Instance.barrageSpecialBoxSetting[transform.GetSiblingIndex()].Delay;

            BarrageController.Instance.EnqueueAction("测试用户", "", callName, 1, times, delay);
        }
    }
}
        