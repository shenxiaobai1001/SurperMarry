using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetManager : Singleton<NetManager>
{
    // 消息队列容器
    private Queue<string> _messageQueue = new Queue<string>();

    public void OnDispseMsg(DataInfo dataInfo)
    {
        if (dataInfo == null)
        {
            PFunc.Log("消息空");
            return;
        }

        Scene scene =SceneManager.GetActiveScene();

        if (!Config.passName.Contains(scene.name)) return;

        switch (dataInfo.call)
        {
            case "乌龟一只":
                MonsterCreater.Instance.OnCreateTortoise(1);
                break;
            case "乌龟十只":
                MonsterCreater.Instance.OnCreateTortoise(10);
                break;
            case "乌龟一百只":
                MonsterCreater.Instance.OnCreateTortoise(100);
                break;
            case "蘑菇一只":
                MonsterCreater.Instance.OnCreateMushroom(1);
                break;
            case "蘑菇十只":
                MonsterCreater.Instance.OnCreateMushroom(10);
                break;
            case "蘑菇一百只":
                MonsterCreater.Instance.OnCreateMushroom(100);
                break;
            case "飞龟一只":
                MonsterCreater.Instance.OnCreateFlyKoopa(1);
                break;
            case "飞龟十只":
                MonsterCreater.Instance.OnCreateFlyKoopa(10);
                break;
            case "飞龟一百只":
                MonsterCreater.Instance.OnCreateFlyKoopa(100);
                break;
            case "飞鱼一只":
                MonsterCreater.Instance.OnCreateFlyFish(1);
                break;
            case "飞鱼十只":
                MonsterCreater.Instance.OnCreateFlyFish(10);
                break;
            case "飞鱼一百只":
                MonsterCreater.Instance.OnCreateFlyFish(100);
                break;
            case "甲壳虫一只":
                MonsterCreater.Instance.OnCreateBeatles(1);
                break;
            case "甲壳虫十只":
                MonsterCreater.Instance.OnCreateBeatles(10);
                break;
            case "甲壳虫一百只":
                MonsterCreater.Instance.OnCreateBeatles(100);
                break;
            case "游戏时间+10s":
                GameManager.Instance.time += 10;
                break;
            case "游戏时间-10s":
                GameManager.Instance.time -= 10;
                break;
            case "生命+10%":
                ModData.mLife += (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-10%":
                ModData.mLife -= (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命+1":
                ModData.mLife += 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-1":
                ModData.mLife -= 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "扔香蕉":
                ItemCreater.Instance.OnCreateBanana(1);
                break;
            case "动感DJ":
                ModVideoPlayerCreater.Instance.OnPlayDJ();
                break;
            case "万箭齐发":
                ItemCreater.Instance.OnCreateManyArrow(1);
                break;
            case "抓鸭子":
                ModVideoPlayerCreater.Instance.OnCreateDuckVideoPlayer();
                break;
            case "乌萨奇":
                ModVideoPlayerCreater.Instance.OnPlayWuSaQi();
                break;
            case "灵魂拷问":
                ModVideoPlayerCreater.Instance.OnPlayMenace();
                break;
            case "乌萨奇硬控":
                ModVideoPlayerCreater.Instance.OnPlayWuSaQi(true);
                break;
            case "灵魂拷问硬控":
                ModVideoPlayerCreater.Instance.OnPlayMenace(true);
                break;
            case "上吊":
                ItemCreater.Instance.OnCreateHangSelf();
                break;
            case "一库":
                ItemCreater.Instance.OnCreateMangSeng(1);
                break;
            case "滚石":
                ItemCreater.Instance.OnCreateRollStone(1);
                break;
            case "滚刺":
                ItemCreater.Instance.OnCreateRollArrow(1);
                break;
            case "陨石":
                ItemCreater.Instance.OnCreateMeteorite(1);
                break;
            case "麒麟臂":
                ItemCreater.Instance.OnCreateQiLinBi(1);
                break;
            case "天残脚":
                ItemCreater.Instance.OnCreateTCJiao(1);
                break;
            case "随机天火":
                ItemCreater.Instance.OnCreateUPFire(1);
                break;
            case "全屏天火":
                ItemCreater.Instance.OnCreateUPFire(66);
                break;
            case "随机地火":
                ItemCreater.Instance.OnCreateDownFire(1);
                break;
            case "全屏地火":
                ItemCreater.Instance.OnCreateDownFire(66);
                break;
            case "随机传送":
                GameModController.Instance.OnRandromPlayerPos();
                break;
            case "随机关卡":
                GameModController.Instance.OnRandromPass();
                break;
            case "铁链":
                ItemCreater.Instance.OnCreateChainPlayer(1);
                break;
            case "雷电":
                ItemCreater.Instance.OnCreateLazzer(1);
                break;
            case "砖块+10":
                CreateWallManager.Instance.wallCount += 10;
                break;
            case "石头+10":
                CreateWallManager.Instance.stonesCount += 10;
                break;
        }
    }
}

[Serializable]
public class DataInfo
{
    public string user;       // 用户名字段
    public string userAvatar; // 用户头像URL
    public string call;       // 功能
    public int count;         // 数量
    public int time;          // 功能触发时间
    public string enalbe;
}