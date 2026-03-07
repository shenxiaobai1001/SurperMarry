using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class BarrageFuncController : MonoBehaviour
{
    public static BarrageFuncController Instance;
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
    private object _lockReadyFunc = new object(); // 新增：为 readyFunc 添加锁
    private object _lockExecutFunc = new object(); // 专门用于 executFunc 的锁

    [HideInInspector]
    public List<BarrageValue> readyFunc = new List<BarrageValue>();//等待执行的功能 
    [HideInInspector]
    public Dictionary<int, BarrageValue> executFunc = new Dictionary<int, BarrageValue>();//正在执行的功能  
    Dictionary<int, BarrageFuncData> allBarrage;

    bool hasBarrage = false;
    bool checkReadyFunck = false;
    int barrageIndex = 0;

    void Start()
    {
        GameData.Instance.Init();
        allBarrage = GameData.Instance.barrage_info.GetAllInfo();
        EventManager.Instance.AddListener(Events.OnBarryExecutEnd, OnBarryExecutEnd);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnBarryExecutEnd, OnBarryExecutEnd);
    }

    void OnBarryExecutEnd(object msg)
    {
        int index = (int)msg;
        lock (_lockExecutFunc)
        {
            if (executFunc != null && executFunc.Count > 0)
            {
                executFunc.Remove(index);
            }
        }
    }

    /// <summary> 触发弹幕功能 </summary>
    public void OnAddReadyFunc(string value)
    {
        if (readyFunc == null) readyFunc = new List<BarrageValue>();
        if (executFunc == null) executFunc = new Dictionary<int, BarrageValue>();

        var data = GameData.Instance.barrage_info.GetInfo(value);
        PFunc.Log("触发弹幕功能", value, data);

        BarrageValue barrageValue = new BarrageValue
        {
            name = value,
            barrageFuncData = data,
            BarrageState = BarrageState.Tigger,
            barraegExecutType = BarraegExecutType.ReadyExecut,
        };

        if ((OnCheckHighGroupFunc(data) || OnCheckHighLevelFunc(data) || OnCheckQueueLevel(data))
            && data.queuestate == 0)
        {
            OnAddReadFunc();
        }
        else
        {
            lock (_lockExecutFunc)
            {
                barrageIndex++;
                executFunc.Add(barrageIndex, barrageValue);
            }
        }

        if (!hasBarrage)
        {
            hasBarrage = true;
            StartCoroutine(OnExecutFunc());
        }

        void OnAddReadFunc()
        {
            lock (_lockReadyFunc)  // 修改：添加时加锁
            {
                readyFunc.Add(barrageValue);
            }
            if (!checkReadyFunck)
            {
                checkReadyFunck = true;
                StartCoroutine(OnCheckReadyFunc());
            }
        }
    }

    IEnumerator OnExecutFunc()
    {
        while (executFunc.Count > 0)
        {
            List<KeyValuePair<int, BarrageValue>> copy;
            lock (_lockExecutFunc)
            {
                // 创建 executFunc 的副本
                copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
            }
            if (Config.isLoading)
            {
                yield return new WaitUntil(() => !Config.isLoading);
            }
            if (GameStatusController.IsDead)
            {
                yield return new WaitUntil(() => !GameStatusController.IsDead);
            }
            foreach (var kvp in copy)
            {
                if (kvp.Value.barraegExecutType == BarraegExecutType.ReadyExecut)
                {
                    if (Config.isLoading)
                    {
                        yield return new WaitUntil(() => !Config.isLoading);
                    }
                    if (GameStatusController.IsDead)
                    {
                        yield return new WaitUntil(() => !GameStatusController.IsDead);
                    }
                    BarrageExecutting.OnExecutingBarrage(kvp.Value.name, kvp.Key, kvp.Value);
                    lock (_lockExecutFunc)
                    {
                        // 修改值前确保键仍存在
                        if (executFunc.ContainsKey(kvp.Key))
                        {
                            executFunc[kvp.Key].barraegExecutType = BarraegExecutType.Executing;
                        }
                    }
                    yield return null;
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
        hasBarrage = false;
    }
    IEnumerator OnCheckReadyFunc()
    {
        List<BarrageValue> temporaryFunc = new List<BarrageValue>();
        while (true)
        {
            // 修改：在锁内获取 readyFunc 的副本
            List<BarrageValue> copyReadyFunc;
            lock (_lockReadyFunc)
            {
                if (readyFunc.Count == 0)
                {
                    checkReadyFunck = false;
                    yield break;  // 如果没有任务，直接结束协程
                }
                copyReadyFunc = new List<BarrageValue>(readyFunc);
            }

            // 遍历副本而不是原列表
            foreach (var kvp in copyReadyFunc)
            {
                if (!OnCheckHighGroupFunc(kvp.barrageFuncData)//没有同组更高
                    && !OnCheckHighLevelFunc(kvp.barrageFuncData)//没有同级更高
                    && !OnCheckQueueLevel(kvp.barrageFuncData))//没有自我排队
                {
                    lock (_lockExecutFunc)
                    {
                        barrageIndex++;
                        executFunc.Add(barrageIndex, kvp);
                    }
                    temporaryFunc.Add(kvp);
                    if (!hasBarrage)
                    {
                        hasBarrage = true;
                        StartCoroutine(OnExecutFunc());
                    }
                    yield return new WaitForSeconds(0.1f);
                }
                yield return null;
            }

            // 从 readyFunc 中移除已处理的任务
            if (temporaryFunc.Count > 0)
            {
                lock (_lockReadyFunc)
                {
                    foreach (var kvp in temporaryFunc)
                    {
                        if (readyFunc.Contains(kvp))
                        {
                            readyFunc.Remove(kvp);
                        }
                    }
                }
                temporaryFunc.Clear();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary> 检测有没有同级但执行等级更高 </summary>
    public bool OnCheckHighGroupFunc(BarrageFuncData data)
    {
        bool isHigh = false;
        if (data.group != 0)
        {
            List<KeyValuePair<int, BarrageValue>> copy;
            lock (_lockExecutFunc)
            {
                copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
            }
            foreach (var kvp in copy)
            {
                if (kvp.Value.barrageFuncData.group == data.group)
                {
                    isHigh = kvp.Value.barrageFuncData.executionlevel > data.executionlevel;
                    if (isHigh) break;
                }
            }
        }
        return isHigh;
    }

    /// <summary> 检测有没有同级但执行等级更高 </summary>
    public bool OnCheckHighLevelFunc(BarrageFuncData data)
    {
        bool isHigh = false;
        List<KeyValuePair<int, BarrageValue>> copy;
        lock (_lockExecutFunc)
        {
            copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
        }
        foreach (var kvp in copy)
        {
            if (kvp.Value.barrageFuncData.type != data.type) continue;
            switch (kvp.Value.barrageFuncData.type)
            {
                case 1:
                    isHigh = kvp.Value.barrageFuncData.createlevel > data.createlevel;
                    break;
                case 2:
                    isHigh = kvp.Value.barrageFuncData.movelevel > data.movelevel;
                    break;
                case 3:
                    isHigh = kvp.Value.barrageFuncData.controllevel > data.controllevel;
                    break;
                case 4:
                    isHigh = false;
                    break;
            }
            if (isHigh) break;
        }
        return isHigh;
    }
    /// <summary> 检测有没有同功能排队 </summary>
    public bool OnCheckQueueLevel(BarrageFuncData data)
    {
        bool isHigh = false;
        if (data.queue == 1)
        {
            List<KeyValuePair<int, BarrageValue>> copy; 
            lock (_lockExecutFunc)
            {
                copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
            }
            foreach (var kvp in copy)
            {
                isHigh = kvp.Value.barrageFuncData.name == data.name;
                if (isHigh) break;
            }
        }
       // PFunc.Log("OnCheckQueueLevel", data.name, isHigh);
        return isHigh;
    }

    /// <summary> 检测有没有强控以上的功能在触发 </summary>
    public bool OnCheckHasHighControl(BarrageFuncData data)
    {
        bool isHigh = false;
        List<KeyValuePair<int, BarrageValue>> copy;
        lock (_lockExecutFunc)
        {
            copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
        }
        foreach (var kvp in copy)
        {
            isHigh = kvp.Value.name!= data.name && kvp.Value.barrageFuncData.type >= 3;
            if (isHigh) break;
        }
        return isHigh;
    }    
    /// <summary> 检测有没有强控以上的功能在触发 </summary>
    public bool OnCheckHasHighControl()
    {
        bool isHigh = false;
        List<KeyValuePair<int, BarrageValue>> copy;
        lock (_lockExecutFunc)
        {
            copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
        }
        foreach (var kvp in copy)
        {
            isHigh =  kvp.Value.barrageFuncData.type >= 3 && (kvp.Value.BarrageState == BarrageState.Underway
                || kvp.Value.BarrageState == BarrageState.Ready || kvp.Value.BarrageState == BarrageState.Pause);
            PFunc.Log("检查强控", kvp.Value.barrageFuncData.name,kvp.Value.BarrageState);
            if (isHigh) break;
        }
        return isHigh;
    }

    /// <summary> 检测有没有强控且等级高于以上的功能在触发 </summary>
    public bool OnCheckHasHighControlLevel(BarrageFuncData data)
    {
        bool isHigh = false;
        List<KeyValuePair<int, BarrageValue>> copy;
        lock (_lockExecutFunc)
        {
            copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
        }
        foreach (var kvp in copy)
        {
            isHigh = kvp.Value.name != data.name 
                && kvp.Value.barrageFuncData.type >= 3 
                && kvp.Value.barrageFuncData.controllevel > data.controllevel;
            if (isHigh) break;
        }
        return isHigh;
    }
    
    /// <summary> 检测有没有某功能正在执行 </summary>
    public bool OnCheckBarrageFuncByName(string name)
    {
        bool isHigh = false;
        List<KeyValuePair<int, BarrageValue>> copy;
        lock (_lockExecutFunc)
        {
            copy = new List<KeyValuePair<int, BarrageValue>>(executFunc);
        }
        foreach (var kvp in copy)
        {
            isHigh = kvp.Value.name == name
                && (kvp.Value.BarrageState == BarrageState.Underway
                || kvp.Value.BarrageState == BarrageState.Pause);
            if (isHigh) break;
        }
        return isHigh;
    }
}

public class BarrageValue
{
    public string name;
    public BarrageFuncData barrageFuncData;
    public BarrageState BarrageState;
    public BarraegExecutType barraegExecutType;
}
