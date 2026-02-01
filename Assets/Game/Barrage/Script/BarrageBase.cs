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

    // 针对同一盲盒/多特效配置的串行队列，避免并发导致视频同时播放
    private class BlindBoxRequest
    {
        public BarrageController controller;
        public BarrageBoxSetting box;
        public string user;
        public string avatar;
        public int giftCount;
    }

    private class SpecialBoxRequest
    {
        public BarrageController controller;
        public BarrageSpecialBoxSetting box;
        public string user;
        public string avatar;
        public int giftCount;
    }

    private readonly Dictionary<string, Queue<BlindBoxRequest>> _blindQueues = new Dictionary<string, Queue<BlindBoxRequest>>();
    private readonly HashSet<string> _blindRunning = new HashSet<string>();
    private readonly Dictionary<string, float> _blindLastExecTime = new Dictionary<string, float>();
    private readonly Dictionary<string, Queue<SpecialBoxRequest>> _specialQueues = new Dictionary<string, Queue<SpecialBoxRequest>>();
    private readonly HashSet<string> _specialRunning = new HashSet<string>();

    private static string MakeKey(BarrageBoxSetting box)
    {
        return $"{box.Type}|{box.Message}|{box.BoxName}";
    }
    private static string MakeKey(BarrageSpecialBoxSetting box)
    {
        return $"{box.Type}|{box.Message}|{box.BoxName}";
    }


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
                EnqueueBlindBox(barrageConfigs, config, user, avatar, 1);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "关注")
            {
                EnqueueSpecialBox(barrageConfigs, config, user, avatar, 1);
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
                EnqueueBlindBox(barrageConfigs, config, user, avatar, 1);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "弹幕" && config.Message == content)
            {
                EnqueueSpecialBox(barrageConfigs, config, user, avatar, 1);
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
                EnqueueBlindBox(barrageConfigs, config, user, avatar, giftCount);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "礼物" && config.Message == giftName)
            {
                EnqueueSpecialBox(barrageConfigs, config, user, avatar, giftCount);
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
                EnqueueBlindBox(barrageConfigs, config, user, avatar, 1);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "进入")
            {
                EnqueueSpecialBox(barrageConfigs, config, user, avatar, 1);
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
                EnqueueBlindBox(barrageConfigs, config, user, avatar, 1);
                likeCount[user] -= int.Parse(config.Message);
            }
        }

        // 多特效触发逻辑
        foreach (BarrageSpecialBoxSetting config in barrageConfigs.barrageSpecialBoxSetting)
        {
            if (config.Type == "点赞" && likeCount[user] > int.Parse(config.Message))
            {
                EnqueueSpecialBox(barrageConfigs, config, user, avatar, 1);
            }
        }
    }

    // 盲盒播放视频后再执行功能：如果配置了视频名则先播完再触发
    private IEnumerator PlayBoxVideoThenEnqueue(BarrageController controller, BarrageBoxSetting box, string user, string avatar, int giftCount)
    {
        if (box == null)
            yield break;

        if (box.Calls == null || box.Calls.Count == 0)
        {
            Debug.LogWarning($"盲盒 '{box.BoxName}' 未配置任何功能 Calls");
            yield break;
        }

        int cycles = Mathf.Max(1, giftCount) * Mathf.Max(1, box.Count);
        for (int i = 0; i < cycles; i++)
        {
            if (!string.IsNullOrEmpty(box.videoName) && box.videoName != "空")
            {
                string path = $"Box/{box.videoName}";
                yield return controller.PlayBoxVideoAndWait(path, 2, false, ModVideoPlayerCreater.Instance != null ? ModVideoPlayerCreater.Instance.transform : null);
            }

            int index = UnityEngine.Random.Range(0, box.Calls.Count);
            string callName = box.Calls[index];
            // 每次播放完动画后，只执行一次随机抽中的功能
            controller.EnqueueAction(user, avatar, callName, 1, 1, box.Delay, true);

            // 播放视频与下一轮之间的间隔
            if (box.Delay > 0f && i < cycles - 1)
            {
                yield return new WaitForSeconds(box.Delay);
            }
        }
    }

    // 多特效播放视频后再执行功能：如果配置了视频名则先播完再触发
    private IEnumerator PlaySpecialVideoThenEnqueue(BarrageController controller, BarrageSpecialBoxSetting box, string user, string avatar, int giftCount)
    {
        if (box == null)
            yield break;

        if (box.Calls == null || box.Calls.Count == 0)
        {
            Debug.LogWarning($"盲盒 '{box.BoxName}' 未配置任何功能 Calls");
            yield break;
        }

        // 每轮：播放视频 -> 执行所有功能 -> 等待间隔
        int cycles = Mathf.Max(1, giftCount) * Mathf.Max(1, box.Count);
        for (int i = 0; i < cycles; i++)
        {
            if (!string.IsNullOrEmpty(box.videoName) && box.videoName != "空")
            {
                string path = $"Box/{box.videoName}";
                yield return controller.PlayBoxVideoAndWait(path, 2, false, ModVideoPlayerCreater.Instance != null ? ModVideoPlayerCreater.Instance.transform : null);
            }

            if (box.Calls != null)
            {
                foreach (var callName in box.Calls)
                {
                    controller.EnqueueAction(user, avatar, callName, 1, 1, box.Delay, true);
                }
            }

            if (box.Delay > 0f && i < cycles - 1)
            {
                yield return new WaitForSeconds(box.Delay);
            }
        }
    }

    // 串行化：同一盲盒配置排队处理，避免并发播放视频
    private void EnqueueBlindBox(BarrageController controller, BarrageBoxSetting box, string user, string avatar, int giftCount)
    {
        string key = MakeKey(box);
        if (!_blindQueues.TryGetValue(key, out var q))
        {
            q = new Queue<BlindBoxRequest>();
            _blindQueues[key] = q;
        }
        q.Enqueue(new BlindBoxRequest { controller = controller, box = box, user = user, avatar = avatar, giftCount = giftCount });
        if (!_blindRunning.Contains(key))
        {
            _blindRunning.Add(key);
            StartCoroutine(ProcessBlindBoxQueue(key));
        }
    }

    private IEnumerator ProcessBlindBoxQueue(string key)
    {
        var q = _blindQueues[key];
        while (q.Count > 0)
        {
            var req = q.Dequeue();
            // 在开始前根据上次执行时间应用剩余间隔
            if (req.box != null && req.box.Delay > 0f)
            {
                float last = _blindLastExecTime.TryGetValue(key, out var t) ? t : -1f;
                if (last >= 0f)
                {
                    float elapsed = Time.time - last;
                    float remain = req.box.Delay - elapsed;
                    if (remain > 0f)
                    {
                        Debug.Log($"盲盒队列[{key}] 距上次 {elapsed:F2}s，开始前再等待 {remain:F2}s");
                        yield return new WaitForSeconds(remain);
                    }
                }
            }
            yield return PlayBoxVideoThenEnqueue(req.controller, req.box, req.user, req.avatar, req.giftCount);
            // 记录本次执行时间，用于下一条的前置等待
            _blindLastExecTime[key] = Time.time;
            // 尾部等待可选：当前保留，保障紧接着的下一条也不会立刻开始
            if (q.Count > 0 && req.box != null && req.box.Delay > 0f)
            {
                yield return new WaitForSeconds(req.box.Delay);
            }
        }
        _blindRunning.Remove(key);
    }

    // 串行化：同一多特效配置排队处理
    private void EnqueueSpecialBox(BarrageController controller, BarrageSpecialBoxSetting box, string user, string avatar, int giftCount)
    {
        string key = MakeKey(box);
        if (!_specialQueues.TryGetValue(key, out var q))
        {
            q = new Queue<SpecialBoxRequest>();
            _specialQueues[key] = q;
        }
        q.Enqueue(new SpecialBoxRequest { controller = controller, box = box, user = user, avatar = avatar, giftCount = giftCount });
        if (!_specialRunning.Contains(key))
        {
            _specialRunning.Add(key);
            StartCoroutine(ProcessSpecialBoxQueue(key));
        }
    }

    private IEnumerator ProcessSpecialBoxQueue(string key)
    {
        var q = _specialQueues[key];
        while (q.Count > 0)
        {
            var req = q.Dequeue();
            yield return PlaySpecialVideoThenEnqueue(req.controller, req.box, req.user, req.avatar, req.giftCount);
        }
        _specialRunning.Remove(key);
    }
}
