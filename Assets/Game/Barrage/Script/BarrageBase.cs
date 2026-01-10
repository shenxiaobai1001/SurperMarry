using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BarrageData
{
    public string Type;
    public string name;
    public string message;
    public string userAvatar;
    public int num; // 礼物数量
    public int count; // 点赞数量
}

public class BarrageBase : MonoBehaviour
{
    public Dictionary<string, int> likeCount = new Dictionary<string, int>();


    public void HandleAttention(string json)
    {
        BarrageData info = JsonUtility.FromJson<BarrageData>(json);
        string user = info.name;
        string avatar = info.userAvatar;

        BarrageController barrageConfigs = FindAnyObjectByType<BarrageController>();
        if (barrageConfigs == null)
        {
            Debug.LogWarning("未找到 BarrageController 配置，忽略关注事件");
            return;
        }
        // 普通触发逻辑
        foreach (BarrageNormalSetting config in barrageConfigs.barrageNormalSetting)
        {
            if (config.Type == "关注")
            {
                barrageConfigs.EnqueueAction(user, avatar, config.CallName, 1, config.Count, config.Delay);
            }
        }

        // 盲盒触发逻辑
        foreach (BarrageBoxSetting config in barrageConfigs.barrageBoxSetting)
        {
            if (config.Type == "关注")
            {
                StartCoroutine(PlayBoxVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "关注")
            {
                StartCoroutine(PlaySpecialVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }
    }

    public void HandleBarrage(string json)
    {
        BarrageData info = JsonUtility.FromJson<BarrageData>(json);
        string user = info.name;
        string avatar = info.userAvatar;
        string content = info.message;

        BarrageController barrageConfigs = FindAnyObjectByType<BarrageController>();
        if (barrageConfigs == null)
        {
            Debug.LogWarning("未找到 BarrageController 配置，忽略弹幕事件");
            return;
        }
        // 普通触发逻辑
        foreach (BarrageNormalSetting config in barrageConfigs.barrageNormalSetting)
        {
            if (config.Type == "弹幕" && config.Message == content)
            {
                barrageConfigs.EnqueueAction(user, avatar, config.CallName, 1, config.Count, config.Delay);
            }
        }

        // 盲盒触发逻辑
        foreach (BarrageBoxSetting config in barrageConfigs.barrageBoxSetting)
        {
            if (config.Type == "弹幕" && config.Message == content)
            {
                StartCoroutine(PlayBoxVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "弹幕" && config.Message == content)
            {
                StartCoroutine(PlaySpecialVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }
    }

    public void HandleGift(string json)
    {
        BarrageData info = JsonUtility.FromJson<BarrageData>(json);
        string giftName = info.message;
        string user = info.name;
        string avatar = info.userAvatar;
        int giftCount = info.num;

        BarrageController barrageConfigs = FindAnyObjectByType<BarrageController>();
        if (barrageConfigs == null)
        {
            Debug.LogWarning("未找到 BarrageController 配置，忽略礼物事件");
            return;
        }
        // 普通触发逻辑
        foreach (BarrageNormalSetting config in barrageConfigs.barrageNormalSetting)
        {
            if (config.Type == "礼物" && config.Message == giftName)
            {
                barrageConfigs.EnqueueAction(user, avatar, config.CallName, giftCount, config.Count, config.Delay);
            }
        }

        // 盲盒触发逻辑
        foreach (BarrageBoxSetting config in barrageConfigs.barrageBoxSetting)
        {
            if (config.Type == "礼物" && config.Message == giftName)
            {
                StartCoroutine(PlayBoxVideoThenEnqueue(barrageConfigs, config, user, avatar, giftCount));
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "礼物" && config.Message == giftName)
            {
                StartCoroutine(PlaySpecialVideoThenEnqueue(barrageConfigs, config, user, avatar, giftCount));
            }
        }
    }

    public void HandleJoin(string json)
    {
        BarrageData info = JsonUtility.FromJson<BarrageData>(json);
        string user = info.name;
        string avatar = info.userAvatar;

        BarrageController barrageConfigs = FindAnyObjectByType<BarrageController>();
        if (barrageConfigs == null)
        {
            Debug.LogWarning("未找到 BarrageController 配置，忽略进入事件");
            return;
        }
        // 普通触发逻辑
        foreach (BarrageNormalSetting config in barrageConfigs.barrageNormalSetting)
        {
            if (config.Type == "进入")
            {
                barrageConfigs.EnqueueAction(user, avatar, config.CallName, 1, config.Count, config.Delay);
            }
        }

        // 盲盒触发逻辑
        foreach (BarrageBoxSetting config in barrageConfigs.barrageBoxSetting)
        {
            if (config.Type == "进入")
            {
                StartCoroutine(PlayBoxVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "进入")
            {
                StartCoroutine(PlaySpecialVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }
    }

    public void handleLike(string json)
    {
        BarrageData info = JsonUtility.FromJson<BarrageData>(json);
        string user = info.name;
        string avatar = info.userAvatar;
        int count = info.count;

        if (!likeCount.ContainsKey(user))
        {
            likeCount.Add(user, count);
        }
        else
        {
            likeCount[user] += count;
        }

        BarrageController barrageConfigs = FindAnyObjectByType<BarrageController>();
        if (barrageConfigs == null)
        {
            Debug.LogWarning("未找到 BarrageController 配置，忽略点赞事件");
            return;
        }
        // 普通触发逻辑
        foreach (BarrageNormalSetting config in barrageConfigs.barrageNormalSetting)
        {
            if (config.Type == "点赞" && likeCount[user] > int.Parse(config.Message))
            {
                barrageConfigs.EnqueueAction(user, avatar, config.CallName, 1, config.Count, config.Delay);
                likeCount[user] -= int.Parse(config.Message);
            }
        }

        // 盲盒触发逻辑
        foreach (BarrageBoxSetting config in barrageConfigs.barrageBoxSetting)
        {
            if (config.Type == "点赞" && likeCount[user] > int.Parse(config.Message))
            {
                StartCoroutine(PlayBoxVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
                likeCount[user] -= int.Parse(config.Message);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "点赞" && likeCount[user] > int.Parse(config.Message))
            {
                StartCoroutine(PlaySpecialVideoThenEnqueue(barrageConfigs, config, user, avatar, 1));
            }
        }
    }

    // 盲盒播放视频后再执行功能：如果配置了视频名则先播完再触发
    private IEnumerator PlayBoxVideoThenEnqueue(BarrageController controller, BarrageBoxSetting box, string user, string avatar, int giftCount)
    {
        if (box == null)
            yield break;

        if (!string.IsNullOrEmpty(box.videoName) && box.videoName != "空")
        {
            string path = $"Box/{box.videoName}";
            yield return controller.PlayBoxVideoAndWait(path, 2, false, null);
        }

        if (box.Calls == null || box.Calls.Count == 0)
        {
            Debug.LogWarning($"盲盒 '{box.BoxName}' 未配置任何功能 Calls");
            yield break;
        }

        int index = UnityEngine.Random.Range(0, box.Calls.Count);
        string callName = box.Calls[index];
        controller.EnqueueAction(user, avatar, callName, Mathf.Max(1, giftCount), box.Count, box.Delay, true);
    }

    // 多特效播放视频后再执行功能：如果配置了视频名则先播完再触发
    private IEnumerator PlaySpecialVideoThenEnqueue(BarrageController controller, BarrageSpecialBoxSetting box, string user, string avatar, int giftCount)
    {
        if (box == null)
            yield break;

        if (!string.IsNullOrEmpty(box.videoName) && box.videoName != "空")
        {
            string path = $"Box/{box.videoName}";
            yield return controller.PlayBoxVideoAndWait(path, 2, false, null);
        }   

        if (box.Calls == null || box.Calls.Count == 0)
        {
            Debug.LogWarning($"盲盒 '{box.BoxName}' 未配置任何功能 Calls");
            yield break;
        }

        foreach (var callName in box.Calls)
        {
            controller.CallFunction(user, avatar, callName, Mathf.Max(1, giftCount), box.Count, box.Delay);
        }
    }
}
