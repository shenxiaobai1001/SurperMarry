using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Normal : MonoBehaviour
{
    private BarrageController barrageConfig;

    void Awake()
    {
        barrageConfig = FindAnyObjectByType<BarrageController>();
    }


    void Update()
    {
        
    }

    public void Remove()
    {
        Destroy(gameObject);
        barrageConfig.barrageNormalSetting.RemoveAt(transform.GetSiblingIndex());
    }

    /// <summary>
    /// 修改配置
    /// </summary>
    public void ChangeConfig()
    {
        if(barrageConfig.isInit)
        {
            BarrageNormalSetting barrageNormalSetting = barrageConfig.barrageNormalSetting[transform.GetSiblingIndex()];

            foreach (Transform child in transform.transform)
            {
                if (child.gameObject.name == "Dropdown1") barrageNormalSetting.CallName = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
                if (child.gameObject.name == "Dropdown2") barrageNormalSetting.Type = child.gameObject.GetComponent<Dropdown>().options[child.gameObject.GetComponent<Dropdown>().value].text;
                if (child.gameObject.name == "InputField1") barrageNormalSetting.Message = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField2") barrageNormalSetting.Tip = child.gameObject.GetComponent<InputField>().text;
                if (child.gameObject.name == "InputField3")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if (int.TryParse(text, out int value))
                    {
                        barrageNormalSetting.Count = value;
                    }
                    else
                    {
                        barrageNormalSetting.Count = 1;
                        Debug.Log("解析倍率失败，使用默认值.");
                    }

                }
                if (child.gameObject.name == "InputField4")
                {
                    string text = child.gameObject.GetComponent<InputField>().text;
                    if(float.TryParse(text, out float value))
                    {
                        barrageNormalSetting.Delay = value;
                    }
                    else
                    {
                        barrageNormalSetting.Delay = 0;
                        Debug.Log("解析延迟失败，使用默认值.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 测试功能
    /// </summary>
    public void TestCall()
    {
        string callName = BarrageController.Instance.barrageNormalSetting[transform.GetSiblingIndex()].CallName;
        int times = BarrageController.Instance.barrageNormalSetting[transform.GetSiblingIndex()].Count;
        float delay = BarrageController.Instance.barrageNormalSetting[transform.GetSiblingIndex()].Delay;

        BarrageController.Instance.EnqueueAction("测试用户", "", callName, 1, times, delay);
    }
}
