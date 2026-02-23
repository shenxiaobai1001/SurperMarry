using PlayerScripts;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemScripts;
using UnityEngine;
using UnityEngine.Video;

public class DJManager : MonoBehaviour
{
    public MediaPlayer mainPlayer;//播放器
    public GameObject mapObj;
     float fallSpeed = 10;            // 下落速度
     float maxHeight = 10;           // 最大高度限制

    bool isPlayDJ = false;
    private Coroutine beatCoroutine;
    private int currentVideoNumber;
    private Vector3 originalPosition;       // 原始位置

    private float shakeTimer = 0f;         // 上升计时器
    List<float> beatTimes;
    int count = 0;
    void Start()
    {
        mainPlayer.Events.AddListener(OnVideoEvent);         // 订阅播放器本身提供的事件
        // 保存原始位置
        originalPosition =new Vector3(0,0,90);
    }
    private void OnDisable()
    {
        mapObj = null;
        if (beatCoroutine != null)
            StopCoroutine(beatCoroutine);
    }
    private void OnDestroy()
    {
        mapObj = null;
        if (beatCoroutine != null)
            StopCoroutine(beatCoroutine);
    }
    private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType evt, ErrorCode errorCode)
    {
        //当视频加载完毕开始播放视频
        if (evt == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            OnModVideoPlayStart();
        }
    }
    public void OnModVideoPlayStart(bool isPlayDJ,int count)
    {
        this.isPlayDJ = isPlayDJ;
        this.currentVideoNumber = count;
    }

    public void OnModVideoPlayStart()
    {
        if (!isPlayDJ) return;
        mapObj = GameObject.Find("InteractableObjects");
        if (!mapObj) return;

        // 重置位置
        mapObj.transform.localPosition = originalPosition;
        shakeTimer = 0f;

        if (beatCoroutine != null)
            StopCoroutine(beatCoroutine);
        beatTimes = DJTimeLine.beatMapData[currentVideoNumber];
        beatCoroutine = StartCoroutine(BeatShakeCoroutine());
    }
    // 使用固定时间间隔检查，减少误差
    WaitForEndOfFrame wait = new WaitForEndOfFrame();
    IEnumerator BeatShakeCoroutine()
    {
        if (Config.isLoading && PlayerController.Instance._isFinish) yield break;

        // 预处理：转换所有时间戳
        List<float> correctedTimes = beatTimes.Select(t =>
        {
            int sec = (int)t;
            int frames = Mathf.RoundToInt((t - sec) * 100);
            return sec + frames / 60f;
        }).ToList();
        int currentIndex = 0;

        while (currentIndex < correctedTimes.Count)
        {
            if (Config.isLoading && PlayerController.Instance._isFinish) yield break;

            yield return wait;

            double currentTime = mainPlayer.Control.GetCurrentTime();
            ProcessFalling();

            if (currentTime >= correctedTimes[currentIndex])
            {
                //PFunc.Log($"卡点: 原始{beatTimes[currentIndex]:F2}, 转换后{correctedTimes[currentIndex]:F3}, 实际{currentTime:F3}");
                TriggerBeatShake();
                currentIndex++;
            }
        }
    }
    // 触发卡点晃动
    private void TriggerBeatShake()
    {
        // 应用高度变化
        mapObj.transform.localPosition = new Vector3(0, 0, 90);
        // 设置上升计时器
        shakeTimer = 0.1f;
    }

    // 处理下落逻辑
    private void ProcessFalling()
    {
        // 处理上升
        if (shakeTimer > 0)
        {
            // 使用Time.unscaledDeltaTime避免时间缩放影响
            shakeTimer -= Time.deltaTime;

            if (shakeTimer > 0)
            {
                mapObj.transform.localPosition =
                    Vector3.MoveTowards(new Vector3(0, mapObj.transform.localPosition.y,90), 
                    new Vector3(0, maxHeight, 90), fallSpeed * Time.deltaTime);
            }
            else
            {
                shakeTimer = 0f;
            }
        }
        else
        {
            // 应用高度变化
            mapObj.transform.localPosition = new Vector3(0, 0, 90);
        }
    }
}