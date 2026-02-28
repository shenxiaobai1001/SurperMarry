using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [HideInInspector] 
    public List<BarrageValue> readyFunc = new List<BarrageValue>();//等待执行的功能 
    [HideInInspector]
    public List<BarrageValue> executFunc  = new List<BarrageValue>();//正在执行的功能 
    [HideInInspector]
    public Dictionary<int, List<BarrageValue>> groupFunc= new Dictionary<int, List<BarrageValue>>();//正在执行的组排功能

    Dictionary<int, BarrageFuncData> allBarrage;

    bool hasBarrage = false;
    bool checkReadyFunck = false;
    BarrageFuncData executingBarrageData = null;


    void Start()
    {
        GameData.Instance.Init();
        allBarrage = GameData.Instance.barrage_info.GetAllInfo();


    }

    /// <summary> 触发弹幕功能 </summary>
    public void OnAddReadyFunc(string value)
    {
        if (readyFunc == null) readyFunc = new List<BarrageValue>();
        if (executFunc == null) new Dictionary<BarrageValue, BarraegExecutType>();

        var data = GameData.Instance.barrage_info.GetInfo(value);

        BarrageValue barrageValue = new BarrageValue
        {
            name = value,
            barrageFuncData=data,
            BarrageState = BarrageState.Tigger,
            barraegExecutType = BarraegExecutType.ReadyExecut,
        };

        if (data.type==1 && data.group==0)//生成物体
        {
            executFunc.Add(barrageValue);
            return;
        }

        if (OnCheckHighGroupFunc(data))
        {
            readyFunc.Add(barrageValue);
            if (!checkReadyFunck)
            {
                checkReadyFunck = true;
                StartCoroutine(OnCheckReadyFunc());
            }
        }
       else if (OnCheckHighLevelFunc(data))//如果正在执行的功能等级更高
        {
            readyFunc.Add(barrageValue);
            if (!checkReadyFunck)
            {
                checkReadyFunck = true;
                StartCoroutine(OnCheckReadyFunc());
            }
        }
        else
        {
            executFunc.Add(barrageValue);
        }
        hasBarrage = true;
    }

    void OnDisposeBarrageByType(BarrageValue barrageValue)
    {
        if (barrageValue.barrageFuncData.group!=0)
        {
            int group = barrageValue.barrageFuncData.group;

            if (groupFunc.ContainsKey(group))
            {
                groupFunc[group].Add(barrageValue);
            }
            else
            {
                groupFunc.Add(group,new List<BarrageValue> { barrageValue });
            }
        }
    }

    IEnumerator OnExecutFunc()
    {
        while (executFunc.Count > 0)
        {
            //foreach (var kvp in readyFunc)
            //{
            //    if (!OnCheckHighLevelFunc(kvp.barrageFuncData.type, kvp.barrageFuncData.level))
            //    {
            //        temporaryFunc.Add(kvp);
            //    }
            //}
            //if (temporaryFunc != null && temporaryFunc.Count > 0)
            //{
            //    foreach (var kvp in temporaryFunc)
            //    { 
            //        if (readyFunc.Contains(kvp))
            //        {
            //            readyFunc.Remove(kvp);
            //            executFunc.Add(kvp);
            //            BarrageExecutting.OnExecutingBarrage(kvp.name);
            //        }
            //    }
            //}
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator OnCheckReadyFunc()
    {
        List<BarrageValue> temporaryFunc = new List<BarrageValue>();
        while (readyFunc .Count>0)
        {
            foreach (var kvp in readyFunc)
            {
                //if (!OnCheckHighLevelFunc(kvp.barrageFuncData.type))
                //{
                //    temporaryFunc.Add(kvp);
                //}
            }
            if (temporaryFunc != null && temporaryFunc.Count > 0)
            {
                foreach (var kvp in temporaryFunc)
                {
                    if (readyFunc.Contains(kvp))
                    {
                        readyFunc.Remove(kvp);
                      //  executFunc.Add(kvp);
                        BarrageExecutting.OnExecutingBarrage(kvp.name);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public bool OnCheckHighGroupFunc(BarrageFuncData data)
    {
        bool isHigh = false;

        if (executFunc != null && executFunc.Count > 0)
        {
            foreach (var kvp in executFunc)
            {
                if (kvp.barrageFuncData.group == data.group)
                {
                    isHigh = kvp.barrageFuncData.executionlevel > data.executionlevel;
                }
            }
        }
        return isHigh;
    }

    /// <summary>检测当前是否有正在执行的同类行更高级别 </summary>
    public bool OnCheckHighLevelFunc(BarrageFuncData data)
    {
        bool isHigh = false;

        if (executFunc != null && executFunc.Count > 0)
        {
            foreach (var kvp in executFunc)
            {
                switch (kvp.barrageFuncData.type)
                {
                    case 1:
                            isHigh = kvp.barrageFuncData.createlevel > data.createlevel;
                        break;
                    case 2:
                        isHigh = kvp.barrageFuncData.movelevel > data.movelevel;
                        break;
                    case 3:
                        isHigh = kvp.barrageFuncData.controllevel > data.controllevel;
                        break;
                    case 4:
                        isHigh = false;
                        break;
                }
            
            }
        }
        return isHigh;
    }

    public BarrageFuncData OnGetInfoByName(string name)
    {
        BarrageFuncData data = null;

        if (allBarrage != null && allBarrage.Count > 0)
        {
            foreach (var kvp in allBarrage) {

                if (kvp.Value.name == name)
                {
                    data=kvp.Value; break;
                }
            }
        }
        return data;
    }
}

public struct BarrageValue
{
    public string name;
    public BarrageFuncData barrageFuncData;
    public BarrageState BarrageState;
    public BarraegExecutType barraegExecutType;
}
