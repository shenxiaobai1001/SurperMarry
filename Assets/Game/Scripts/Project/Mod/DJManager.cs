using PlayerScripts;
using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

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
        originalPosition = Vector3.zero;
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
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
        currentBeatIndex = 0;
    }

    public void OnModVideoPlayStart()
    {
        if (!isPlayDJ) return;
        if (!mapObj)
        {
            mapObj = GameObject.Find("InteractableObjects");
        }
        // 重置位置
        mapObj.transform.localPosition = originalPosition;
        shakeTimer = 0f;

        if (beatCoroutine != null)
            StopCoroutine(beatCoroutine);
        beatTimes = DJTimeLine.beatMapData[currentVideoNumber];
        beatCoroutine = StartCoroutine(BeatShakeCoroutine());
    }
    int currentBeatIndex = 0;
    // 使用固定时间间隔检查，减少误差
    WaitForEndOfFrame wait = new WaitForEndOfFrame();
    IEnumerator BeatShakeCoroutine()
    {
        if (Config.isLoading && PlayerController.Instance._isFinish)
        {
            yield break;
        }

        // 使用Time.time作为基准时间
        while (currentBeatIndex < beatTimes.Count)
        {    
            // 在帧结束时处理，确保时间更准确
            if (Config.isLoading && PlayerController.Instance._isFinish)
            {
                yield break;
            }
            // 计算从开始到现在的准确时间
            double elapsedTime = mainPlayer.Control.GetCurrentTime();
            ProcessFalling();

            // 检查卡点
            if (currentBeatIndex < beatTimes.Count && elapsedTime >= beatTimes[currentBeatIndex])
            {
                PFunc.Log($"卡点触发: 目标{beatTimes[currentBeatIndex]}, 实际{elapsedTime}");
                TriggerBeatShake();
                currentBeatIndex++;
            }
            yield return wait;
            // yield return null;
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