using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class ModVideoPlayer : MonoBehaviour
{
    public MediaPlayer mainPlayer;//播放器
    public GameObjFllow objFllow;
    public Canvas mCanvas;
    public GameObject center;
    public GameObject normalPlayer;
    public GameObject specailPlayer;
    UnityAction callback = null;

    void Start()
    {
        mainPlayer.Events.AddListener(OnVideoEvent);         // 订阅播放器本身提供的事件
    }

    public void OnPlayVideo(Vector3 offset, Vector3 scale, Vector3 rotate, string path,int type, string layer = "Default", bool snake = false, int sortingOrder=-5, UnityAction callback = null)
    {
        PFunc.Log("OnPlayVideo", path, layer);
        normalPlayer.SetActive(type==1);
        specailPlayer.SetActive(type == 2);
        objFllow.enabled = offset != Vector3.zero;
        objFllow.offset = offset;
        center.transform.localPosition = offset;
        center.transform.localScale = scale;
        center.transform.localEulerAngles = rotate;
        pathTitle = path;
        this.callback = callback;
       OnInitPlayer();
        if (mCanvas) mCanvas.sortingLayerName = layer;  // Sorting Layer 名称
        if (mCanvas) mCanvas.sortingOrder = sortingOrder;         // Order in Laye
        OnBeginGetVideo();
    }

    void OnInitPlayer()
    {
        mCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        mCanvas.worldCamera = Camera.main;
    }

    private void OnVideoEvent(MediaPlayer mp, MediaPlayerEvent.EventType evt, ErrorCode errorCode)
    {
        //当视频加载完毕开始播放视频
        if (evt == MediaPlayerEvent.EventType.FirstFrameReady)
        {
            OnShow();
        }

        // 当视频暂停
        if (evt == MediaPlayerEvent.EventType.Paused)
        {

        }

        // 当视频播放完
        if (evt == MediaPlayerEvent.EventType.FinishedPlaying)
        {
            OnEnd();
        }
    }
    //播放
    public void OnShow()
    {
        Sound.PauseOrPlayVolumeMusic(true);
        mainPlayer.Play();
        mainPlayer.Control.SetVolume(Sound.VideoVolume);
        EventManager.Instance.SendMessage(Events.OnModVideoPlayStart);
    }

    //跳到结束
    public void OnEnd()
    {
        Sound.PauseOrPlayVolumeMusic(false);
        mainPlayer.CloseMedia();
        if (callback != null) callback.Invoke();
        callback = null;
        SimplePool.Despawn(this.gameObject);
        EventManager.Instance.SendMessage(Events.OnModVideoPlayEnd);
    }

    string pathTitle = "";
    /// <summary>本地视频启用</summary>
    private async void OnBeginGetVideo()
    {
        string videoData = await OnGetPlayBytes(pathTitle);
        if (mainPlayer)
        {
            mainPlayer.gameObject.SetActive(true);
            mainPlayer.OpenMedia(MediaPathType.AbsolutePathOrURL, videoData, false);
        }
    }
    /// <summary> 获取本地视频数据 </summary>
    public async Task<string> OnGetPlayBytes(string path)
    {
        string video = await Loaded.OnLoadVideoAsync(path);
        return video;
    }
}
