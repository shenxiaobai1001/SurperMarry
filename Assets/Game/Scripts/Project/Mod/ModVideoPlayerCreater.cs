using DG.Tweening;
using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    bool isPlayDJ = false;

    public void OnPlayDJ()
    {
        if (Config.isLoading) return;
       int number = Random.Range(1, 13);
        currentVideoNumber = number;
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, $"DJ/{currentVideoNumber}", 1);
        isPlayDJ = true;
    }
    public void OnPlayGrilVideo()
    {
        if (Config.isLoading) return;
        int number = Random.Range(1, 78);
        grilVideoNumber = number;
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, $"GirlMysteryBox/{grilVideoNumber}", 2,"Default",false,-10);

    }
    private IEnumerator BeatShakeCoroutine()
    {
        // 获取当前视频的卡点数据
        if (!DJTimeLine.beatMapData.ContainsKey(currentVideoNumber))
        {
            Debug.LogWarning($"No beat map data found for video {currentVideoNumber}");
            yield break;
        }

        List<float> beatTimes = DJTimeLine.beatMapData[currentVideoNumber];
        int currentBeatIndex = 0;
        float startTime = 0f;
        float accumulatedTime = 0f; // 累计时间，用于物体运动计时

        // 控制物体的相关变量
        float currentY = 0f; // 当前Y轴位置
        float targetY = 0f; // 目标Y轴位置
        float riseStartTime = 0f; // 上升开始时间
        float riseDuration = 0.05f; // 上升持续时间
        float fallSpeed = 10f; // 下落速度
        bool isRising = false; // 是否正在上升
        bool isFalling = false; // 是否正在下落

        if (InteractableObjects == null)
        {
            InteractableObjects = GameObject.Find("InteractableObjects");
        }

        // 保存物体的初始Y轴位置
        if (InteractableObjects != null)
        {
            currentY = InteractableObjects.transform.position.y;
        }

        if (Config.isLoading)
        {
            yield break;
        }

        while (currentBeatIndex < beatTimes.Count)
        {
            if (Config.isLoading)
            {
                yield break;
            }

            startTime += Time.deltaTime;
            accumulatedTime += Time.deltaTime;

            // 更新物体位置
            if (isRising)
            {
                // 上升过程
                float elapsed = accumulatedTime - riseStartTime;
                float t = Mathf.Clamp01(elapsed / riseDuration);

                // 使用线性插值上升
                float newY = Mathf.Lerp(currentY, targetY, t);

                // 更新物体的Y轴位置
                Vector3 pos = InteractableObjects.transform.position;
                pos.y = newY;
                InteractableObjects.transform.position = pos;

                // 上升结束，开始下落
                if (t >= 1f)
                {
                    isRising = false;
                    isFalling = true;
                    currentY = targetY; // 更新当前位置
                }
            }
            else if (isFalling)
            {
                // 下落过程
                currentY -= fallSpeed * Time.deltaTime;

                // 更新物体的Y轴位置
                Vector3 pos = InteractableObjects.transform.position;
                pos.y = currentY;
                InteractableObjects.transform.position = pos;

                // 如果落到初始位置以下，停止下落
                if (currentY <= 0f)
                {
                    currentY = 0f;
                    isFalling = false;

                    // 确保物体在初始高度
                    Vector3 posFinal = InteractableObjects.transform.position;
                    posFinal.y = 0f;
                    InteractableObjects.transform.position = posFinal;
                }
            }

            // 检查是否到达下一个卡点时间（考虑一定的误差范围）
            if (currentBeatIndex < beatTimes.Count &&
                startTime >= beatTimes[currentBeatIndex] )
            {
                PFunc.Log("触发物体上升", startTime);

                // 触发物体上升
                if (InteractableObjects != null)
                {
                    // 如果正在下落，立即中断下落开始新的上升
                    if (isFalling)
                    {
                        isFalling = false;
                    }
                    // 如果正在上升，重置上升
                    else if (isRising)
                    {
                        // 从当前位置继续上升，确保总共上升1个单位
                        targetY = currentY + 1f;
                    }
                    else
                    {
                        // 从当前位置开始上升
                        targetY = currentY + 1f;
                    }

                    // 记录上升开始时间
                    riseStartTime = accumulatedTime;
                    isRising = true;

                    // 立即更新一次位置，避免延迟
                    float t = Mathf.Clamp01((accumulatedTime - riseStartTime) / riseDuration);
                    float newY = Mathf.Lerp(currentY, targetY, t);

                    Vector3 pos = InteractableObjects.transform.position;
                    pos.y = newY;
                    InteractableObjects.transform.position = pos;
                }
                else
                {
                    Debug.LogWarning("InteractableObjects is null!");
                }

                currentBeatIndex++;
            }

            yield return null;
        }

        Debug.Log($"Beat coroutine finished for video {currentVideoNumber}");
    }

    public void SnakeObj()
    {
        if (InteractableObjects == null)
        {
            InteractableObjects = GameObject.Find("InteractableObjects");
        }

        if (InteractableObjects != null)
        {
            Transform trans = InteractableObjects.transform;
            Vector3 originalPos = trans.localPosition;

            //InteractableObjects.transform.DOKill();

            // 更快速的往返位移
            float duration = 0.08f; // 总时间更短
            float offsetY = 0.15f;  // 位移幅度

            // 先向上移动
            InteractableObjects.transform.DOLocalMoveY(originalPos.y + offsetY, duration / 2)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    // 然后快速返回
                    InteractableObjects.transform.DOLocalMoveY(originalPos.y, duration / 2)
                        .SetEase(Ease.InQuad);
                });
        }
    }
    // 修改现有的视频播放结束方法
    void OnModVideoPlayStart(object msg)
    {
        if (isPlayDJ)
        {
            // 开始卡点协程
            if (beatCoroutine != null)
                StopCoroutine(beatCoroutine);
            beatCoroutine = StartCoroutine(BeatShakeCoroutine());
        }
    }
    // 修改现有的视频播放结束方法
    void OnVideoPlayEnd(object msg)
    {
        IsPlaying = false;
        isPlayDJ = false;
        // 停止卡点协程
        if (beatCoroutine != null)
        {
            StopCoroutine(beatCoroutine);
            beatCoroutine = null;
        }
        bool hasPlaying = false;
        for (int i = 0; i < videoParent.childCount; i++)
        {
            if (videoParent.GetChild(i).gameObject.activeSelf)
            {
                hasPlaying = true;
                break; 
            }
        }
        if (!hasPlaying)
        {
            PlayerModController.Instance.OnChanleModAni();
            PlayerController.Instance.isHit = false;
        }
    }
    public void OnPlayWuSaQi(bool isHit=false)
    {
        if (ItemCreater.Instance.isHang)
            PlayerModController.Instance.OnCancelHangSelf();
        PlayerController.Instance.isHit= isHit;
        int number = Random.Range(1, 13);
        float scaleValue = Random.Range(0.5f, 2);
        int rotateValue= Random.Range(0, 360);
        Vector3 scale = new Vector3(scaleValue, scaleValue,1);
        Vector3 rotate = new Vector3(0, 0, rotateValue);
        OnCreateModVideoPlayer(Vector3.zero, scale, rotate, "GreenScreen/wusaqi", 2);
        Invoke("OnTriggerDao", 0.9f);
    }
    public void OnPlayMenace(bool isHit = false)
    {
        PlayerController.Instance.isHit = isHit;
        if (ItemCreater.Instance.isHang)
            PlayerModController.Instance.OnCancelHangSelf();

        int number = Random.Range(1, 39);
        OnCreateModVideoPlayer(new Vector3(-0.5f,0.4f,90), Vector3.one, Vector3.zero, $"Question/{number}", 2);
        PlayerModController.Instance.OnTiggerManace();
    }
    void OnTriggerDao()
    {
        PlayerModController.Instance.OnTiggerDao();
    }
    string nullDUCK = "GreenScreen/Duck/Null";
    string getDUCK = "GreenScreen/Duck/Get";
    Queue<int> onCreate = new Queue<int>();
    Queue<int> onCreateKoopa = new Queue<int>();
    public void OnCreateDuckVideoPlayer()
    {
        int index = Random.Range(0, 150);
        bool getduck = index >= 17;
        string title = getduck ? getDUCK : nullDUCK;
        int duckPath = 0;
        if (getduck)
        {
            if (index >= 17 && index < 21)
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
            else if (index >= 73 && index < 75)
            {
                duckPath = 500;
            }
            else if (index == 77)
            {
                duckPath = 1000;
            }
            else if (index == 79)
            {
                duckPath = 3000;
            }
            else if (index == 80)
            {
                duckPath = 5000;
            }
            else if (index == 81)
            {
                duckPath = 10000;
            }
            else
            {
                duckPath = 1;
            }
        }
        else
            duckPath = Random.Range(1, 24);
        string path = $"{title}/{duckPath}";
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, path, 2);
        duckPath = getduck ? duckPath : 0;
        onCreate.Enqueue(duckPath);
        Invoke("OnBeginCreateDuck",2);
    }
    public void OnCreateKoopaVideoPlayer()
    {
        int index = Random.Range(0, 150);
        bool getduck = index >= 17;
        string title = getduck ? getDUCK : nullDUCK;
        int duckPath = 0;
        if (getduck)
        {
            if (index >= 17 && index < 21)
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
            else if (index >= 73 && index < 75)
            {
                duckPath = 500;
            }
            else if (index == 77)
            {
                duckPath = 1000;
            }
            else if (index == 79)
            {
                duckPath = 3000;
            }
            else if (index == 80)
            {
                duckPath = 5000;
            }
            else if (index == 81)
            {
                duckPath = 10000;
            }
            else
            {
                duckPath = 1;
            }
        }
        else
            duckPath = Random.Range(1, 24);
        string path = $"{title}/{duckPath}";
        OnCreateModVideoPlayer(Vector3.zero, Vector3.one, Vector3.zero, path, 2);
        duckPath = getduck ? duckPath : 0;
        onCreateKoopa.Enqueue(duckPath);
        Invoke("OnBeginCreateKoopa", 2);
    }
    void OnBeginCreateDuck( )
    {
        if (onCreate == null || onCreate.Count <= 0) return;
        int getduck = onCreate.Dequeue();
        if (getduck > 0)
            ItemCreater.Instance.OnCreateDuck(getduck);
    }
    void OnBeginCreateKoopa()
    {
        if (onCreateKoopa == null || onCreateKoopa.Count <= 0) return;
        int getduck = onCreateKoopa.Dequeue();
        if (getduck > 0)
            MonsterCreater.Instance.OnCreateTortoise(getduck);
    }
    public GameObject OnCreateModVideoPlayer(Vector3 offset, Vector3 scale, Vector3 rotateA,string path, int type, string layer = "Video", bool snake = false,int sortingOrder=-5)
    {
        GameObject vplayerObj = SimplePool.Spawn(ModVideoPlayer, transform.position, Quaternion.identity);
        ModVideoPlayer vplayer = vplayerObj.GetComponent<ModVideoPlayer>();
        vplayer.OnPlayVideo(offset, scale, rotateA, path, type, layer, snake, sortingOrder);
        vplayerObj.transform.SetParent(videoParent);
        IsPlaying = true;
        return vplayerObj;
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnModVideoPlayEnd, OnVideoPlayEnd);
    }
}
