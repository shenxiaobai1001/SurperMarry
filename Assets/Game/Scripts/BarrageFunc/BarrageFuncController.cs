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

    [HideInInspector] public List<BarrageValue> readyFunc 
        = new List<BarrageValue>();//等待执行的功能 
    [HideInInspector] public Dictionary<BarrageValue, BarraegExecutType> executFunc 
        = new Dictionary<BarrageValue, BarraegExecutType>();//正在执行的功能 名称

    Dictionary<int, BarrageFuncData> allBarrage;

    bool hasBarrage = false;
    bool checkReadyFunck = false;
    string executingFunc = "";
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
        };

        if (data.type==1 && data.group==0)//生成物体
        {
            executFunc.Add(barrageValue,BarraegExecutType.ReadyExecut);
            return;
        }

        if (OnCheckHighLevelFunc(data.type))//如果正在执行的功能等级更高
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
            executFunc.Add(barrageValue, BarraegExecutType.ReadyExecut);
        }
        hasBarrage = true;
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
                if (!OnCheckHighLevelFunc(kvp.barrageFuncData.type))
                {
                    temporaryFunc.Add(kvp);
                }
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


    public bool OnCheckHighLevelFunc(int type)
    {
        bool isHigh = false;

        //if (executFunc != null && executFunc.Count > 0)
        //{
        //    foreach (var kvp in executFunc)
        //    {
        //        if (kvp.barrageFuncData.type >= type)
        //        {
        //            isHigh = true; 
        //            break;
        //        }
        //    }
        //}
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
}
