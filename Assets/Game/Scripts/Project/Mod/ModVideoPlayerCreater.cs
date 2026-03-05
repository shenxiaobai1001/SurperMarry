using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using UnityEngine.Video;

public class ModVideoPlayerCreater : MonoBehaviour
{
    public static ModVideoPlayerCreater Instance;
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

    public GameObject ModVideoPlayer;
    public Transform videoParent;
    public bool IsPlaying = false; 
    public GameObject InteractableObjects;

    // 当前播放的视频编号和协程引用

    private int currentVideoNumber;
    private int grilVideoNumber;
    private Coroutine beatCoroutine;
    private void Start()
    {
        EventManager.Instance.AddListener(Events.OnModVideoPlayEnd, OnVideoPlayEnd);
        EventManager.Instance.AddListener(Events.OnModVideoPlayStart, OnModVideoPlayStart);
        DJTimeLine.InitializeBeatMapData();
    }

    public void OnPlayDJ(int Callname)
    {
       int number = Random.Range(1, 12);
        GameObject obj = OnCreateModVideoPlayer(
            Vector3.zero, new Vector3(1.5f,1.5f,1), Vector3.zero, $"DJ/{number}", 2,"Default", false, -10
            , () => { EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, Callname); });
        obj.GetComponent<DJManager>().OnModVideoPlayStart(true, number);
    }

    public void OnPlayGrilVideo(int Callname)
    {
        int number = Random.Range(1, 78);
        grilVideoNumber = number;
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, $"GirlMysteryBox/{grilVideoNumber}", 2,"Default",false,-10, 
            () => { EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, Callname); });
    }

    // 修改现有的视频播放结束方法
    void OnModVideoPlayStart(object msg)
    {
        
    }
    // 修改现有的视频播放结束方法
    void OnVideoPlayEnd(object msg)
    {
        IsPlaying = false;
        // 停止卡点协程
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
            beatCoroutine = null;
        }
    }

    string nullDUCK = "GreenScreen/Duck/Null";
    string getDUCK = "GreenScreen/Duck/Get";
    Queue<int> onCreate = new Queue<int>();
    Queue<int> onCreatePsy = new Queue<int>();
    Queue<int> onCreateKoopa = new Queue<int>();
    Queue<int> onCreateRote = new Queue<int>();
    public void OnCreateDuckVideoPlayer(int callIndex)
    {
        int index = Random.Range(-30, 82);
        bool getduck = index >= 5;
        string title = getduck ? getDUCK : nullDUCK;
        int duckPath = OnGetValue(getduck, index);
        string path = $"{title}/{duckPath}";
        int CALL = callIndex;
        duckPath = getduck ? duckPath : 0;
        onCreate.Enqueue(duckPath);
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, path, 2, "Video", false, -5
         , () => { OnBeginCreateDuck(CALL); });
    }

    public void OnCreatePsyDuckVideoPlayer(int callIndex)
    {
        int index = Random.Range(-30, 82);
        bool getduck = index >= 5;
        // bool getduck = true;
        string title = getduck ? getDUCK : nullDUCK;
        int duckPath = OnGetValue(getduck, index);
        string path = $"{title}/{duckPath}";
        int CALL = callIndex;
        duckPath = getduck ? duckPath : 0;
        onCreatePsy.Enqueue(duckPath);
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, path, 2, "Video", false, -5
    , () => { OnBeginCreatePsyDuck(CALL); });
    }
    string nullTurtles= "GreenScreen/Turtles/Null";
    string getTurtles = "GreenScreen/Turtles/Get";
    public void OnCreateKoopaVideoPlayer(int callIndex)
    {
        int index = Random.Range(-30, 82);
        bool getduck = index >= 5;
        string title = getduck ? getTurtles : nullTurtles;
        int duckPath = OnGetValue(getduck, index);
     
        string path = $"{title}/{duckPath}";
        duckPath = getduck ? duckPath : 0;
        onCreateKoopa.Enqueue(duckPath);
        int CALL = callIndex;
        OnCreateModVideoPlayer(Vector3.zero, new Vector3(0.75f, 0.75f, 1),  Vector3.zero, path, 2, "Video", false, -5
        , () => { OnBeginCreateKoopa(CALL); });
    }
    string nullRope = "GreenScreen/Rope/Null";
    string getRope = "GreenScreen/Rope/Get";
    public void OnCreateRopeVideoPlayer(BarrageValue barrageFuncData, int callIndex)
    {
        int index = Random.Range(0, 12);
        bool getduck = index >= 2;
        string title = getduck ? getRope : nullRope;
        int duckPath = 0;
        switch (index)
        {
            case 0:
                duckPath = 1;
                break;
                case 1:
                duckPath = 2;
                break;
            case 2:
                duckPath = 5;
                break;
            case 3:
                duckPath = 10;
                break;
            case 4:
                duckPath = 20;
                break;
            case 5:
                duckPath = 30;
                break;
            case 6:
                duckPath = 40;
                break;
            case 7:
                duckPath = 49;
                break;
            case 8:
                duckPath = 60;
                break;
            case 9:
                duckPath = 80;
                break;
            case 10:
                duckPath = 88;
                break;
            case 11:
                duckPath = 100;
                break;
        }
        string path = $"{title}/{duckPath}";
        OnCreateModVideoPlayer(Vector3.zero, new Vector3(0.75f, 0.75f, 1), Vector3.zero, path, 2,
            "Video", false, -5, () => { OnBeginCreateRote(barrageFuncData,callIndex, duckPath); });
    }
    void OnBeginCreateRote(BarrageValue barrageFuncData, int index,int count)
    {
        BarrageFuncCreater.Instance.OnCreateRopeSkip(barrageFuncData, index, count);
    }
    public int OnGetValue(bool getduck,int index)
    {
        int duckPath = 0;
        if (getduck)
        {
            if (index >= 5 && index < 21)
            {
                duckPath = 1;
            }
            else if (index >= 21 && index < 25)
            {
                duckPath = 1;
            }
            else if (index >= 25 && index < 29)
            {
                duckPath = 2;
            }
            else if (index >= 29 && index < 33)
            {
                duckPath = 11;
            }
            else if (index >= 33 && index < 37)
            {
                duckPath = 11;
            }
            else if (index >= 37 && index < 41)
            {
                duckPath = 15;
            }
            else if (index >= 41 && index < 45)
            {
                duckPath = 20;
            }
            else if (index >= 45 && index < 49)
            {
                duckPath = 40;
            }
            else if (index >= 49 && index < 53)
            {
                duckPath = 50;
            }
            else if (index >= 53 && index < 57)
            {
                duckPath = 80;
            }
            else if (index >= 57 && index < 61)
            {
                duckPath = 100;
            }
            else if (index >= 61 && index < 64)
            {
                duckPath = 200;
            }
            else if (index >= 65 && index < 67)
            {
                duckPath = 300;
            }
            else if (index >= 67 && index < 70)
            {
                duckPath = 500;
            }
            else if (index >= 70 && index < 73)
            {
                duckPath = 1000;
            }
            else if (index >= 73 && index < 75)
            {
                duckPath = 3000;
            }
            else if (index >= 75 && index < 78)
            {
                duckPath = 5000;
            }
            else if (index >= 78 && index < 82)
            {
                duckPath = 10000;
            }
        }
        else
            duckPath = Random.Range(1, 24);

        return duckPath;
    }
    void OnBeginCreateDuck(int index )
    {
        if (onCreate == null || onCreate.Count <= 0) return;
        int getduck = onCreate.Dequeue();
        if (getduck > 0)
            ItemCreater.Instance.OnCreateDuck(getduck, index);
    }
    void OnBeginCreatePsyDuck(int index)
    {
        if (onCreatePsy == null || onCreatePsy.Count <= 0) return;
        int getduck = onCreatePsy.Dequeue();
        if (getduck > 0)
            ItemCreater.Instance.OnCreatePsyDuck(getduck, index);
    }

    void OnBeginCreateKoopa(int index)
    {
        if (onCreateKoopa == null || onCreateKoopa.Count <= 0) return;
        int getduck = onCreateKoopa.Dequeue();
        if (getduck > 0)
            MonsterCreater.Instance.OnCreateTortoise(getduck, index);
    }

    public GameObject OnCreateModVideoPlayer(Vector3 offset, Vector3 scale, Vector3 rotateA,string path, int type, string layer = "Video", bool snake = false,int sortingOrder=-5, UnityAction callback = null)
    {
        GameObject vplayerObj = SimplePool.Spawn(ModVideoPlayer, transform.position, Quaternion.identity);
        ModVideoPlayer vplayer = vplayerObj.GetComponent<ModVideoPlayer>();
        vplayer.OnPlayVideo(offset, scale, rotateA, path, type, layer, snake, sortingOrder, callback);
        vplayerObj.transform.SetParent(videoParent);
        IsPlaying = true;
        return vplayerObj;
    }

    public void OnPlayTrunck()
    {
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, "GreenScreen/trunck", 2);
    }
    public void OnPlayBig()
    {
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, "GreenScreen/big", 2);
    }
    public void OnPlaySmall()
    {
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, "GreenScreen/small", 2);
    }
    public void OnCreateFlog(BarrageValue barrageFuncData, int callIndex)
    {
        int index = Random.Range(0, 9);
        string title = "GreenScreen/Flog";
        int duckPath = 0;

        switch (index)
        {
            case 0:
                duckPath = 8;
                break;
            case 1:
                duckPath = 18;
                break;
            case 2:
                duckPath = 20;
                break;
            case 3:
                duckPath = 30;
                break;
            case 4:
                duckPath = 66;
                break;
            case 5:
                duckPath = 73;
                break;
            case 6:
                duckPath = 88;
                break;
            case 7:
                duckPath = 128;
                break;
            case 8:
                duckPath = 188;
                break;
        }
        string path = $"{title}/{duckPath}";
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, path, 2, "Video", false, -5
         , () => { OnShowFlog(barrageFuncData, callIndex); });

        Config.FlogCount += duckPath;
    }
    void OnShowFlog(BarrageValue barrageFuncData, int index)
    {
        BarrageFuncCreater.Instance.OnCreateFlog(barrageFuncData, index);
    }

    public void OnKuFen(BarrageValue barrageFuncData, int index)
    {
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, "GreenScreen/kufen", 2, "Video", false, -5,
          () =>{ OnCloseKufen(barrageFuncData,index); }  );
        ItemCreater.Instance.OnCreateZhiQian(1, 0);
        PlayerModController.Instance.OnSetPlayerContro(false, false, true);
    }

    void OnCloseKufen(BarrageValue barrageFuncData, int index)
    {
        if (!BarrageFuncController.Instance.OnCheckHasHighControl(barrageFuncData.barrageFuncData))
        {
            PlayerModController.Instance.OnSetPlayerContro(true, true, true);
        }

        EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnModVideoPlayEnd, OnVideoPlayEnd);
    }
}
