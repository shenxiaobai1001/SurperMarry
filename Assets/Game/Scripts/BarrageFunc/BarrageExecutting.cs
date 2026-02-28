using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageExecutting 
{
    public static void OnExecutingBarrage(string callName)
    {
        switch (callName)
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
                GameStatusController.IsGameFinish = false;
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time += 10;
                break;
            case "游戏时间-10s":
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time -= 10;
                break;
            case "生命+10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife -= (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命+1":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                break;
            case "生命-1":
                Sound.PlaySound("smb_1-up");
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
            case "抓可达鸭":
                ModVideoPlayerCreater.Instance.OnCreatePsyDuckVideoPlayer();
                break;
            case "抓乌龟":
                ModVideoPlayerCreater.Instance.OnCreateKoopaVideoPlayer();
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
                ItemCreater.Instance.OnCreateSingleUPFire(1);
                break;
            case "全屏天火":
                ItemCreater.Instance.OnCreateUPFire(1);
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
            case "美女盲盒":
                ModVideoPlayerCreater.Instance.OnPlayGrilVideo();
                break;
            case "火焰马里奥":
                PlayerModController.Instance.OnRandromPlayer(2);
                break;
            case "上一关":
                if (GameModController.Instance != null) GameModController.Instance.OnEnterNextPass(-1);
                break;
            case "下一关":
                if (GameModController.Instance != null) GameModController.Instance.OnEnterNextPass(1);
                break;
            case "大蘑菇":
                ItemCreater.Instance.OnCreateBigMG(1);
                break;
            case "超人+10秒":
                PlayerModController.Instance.OnSuperMan(10);
                break;
            case "超人-10秒":
                PlayerModController.Instance.OnSuperMan(-10);
                break;
            case "僵尸+10秒":
                PlayerModController.Instance.OnShowZomBie(10);
                break;
            case "僵尸-10秒":
                PlayerModController.Instance.OnShowZomBie(-10);
                break;
            case "广播体操":
                ItemCreater.Instance.OnDance();
                break;
            case "刀马":
                PlayerModController.Instance.OnDMDance();
                break;
            case "凿头":
                PlayerModController.Instance.OnKickHead();
                break;
            case "砍刀形态":
                PlayerModController.Instance.OnRandromPlayer(0);
                break;
            case "拿枪形态":
                PlayerModController.Instance.OnRandromPlayer(1);
                break;
            case "随机形态":
                PlayerModController.Instance.OnRandromPlayer(5);
                break;
            case "大齿轮":
                ItemCreater.Instance.OnCreateBigGear(1);
                break;
            case "撞大运":
                ItemCreater.Instance.OnCreateTrunck(1);
                break;
            case "打板子盲盒":
                ModVideoPlayerCreater.Instance.OnCreateFlog();
                break;
            case "变大":
                if (Config.isLoading) return;
                ModVideoPlayerCreater.Instance.OnPlayBig();
                PlayerModController.Instance.OnChangScale(0.01f);
                break;
            case "变小":
                if (Config.isLoading) return;
                ModVideoPlayerCreater.Instance.OnPlaySmall();
                PlayerModController.Instance.OnChangScale(-0.01f);
                break;
            case "关灯":
                if (Config.isLoading) return;
                Sound.PlaySound("smb_1-up");
                UIMask.Instance.OnCloseLight();
                break;
            case "重新开始":
                if (Config.isLoading) return;
                if (GameModController.Instance != null) GameModController.Instance.OnLoadScene("1-1");
                break;
            case "陷阱数量+1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount < ModData.allTrapCount)
                    ModData.canTrapCount += 1;
                break;
            case "陷阱数量-1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount > 0)
                    ModData.canTrapCount -= 1;
                break;
            case "全部陷阱激活":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = ModData.allTrapCount;
                break;
            case "全部陷阱关闭":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = 0;
                break;
            case "无敌星":
                PlayerModController.Instance.OnSetInvincible();
                break;
            case "隐身":
                PlayerModController.Instance.OnSetInvisibilityState();
                break;
            case "火圈":
                Sound.PlaySound("smb_1-up");
                ItemCreater.Instance.OnCreatehuoQuan(1);
                break;
            case "伸缩藤条":
                ItemCreater.Instance.OnCreateRattan(1);
                break;
            case "哭坟":
                ModVideoPlayerCreater.Instance.OnKuFen();
                break;
            case "擦皮鞋":
                ItemCreater.Instance.OnCreateShoeShine(1);
                break;
            case "跳绳":
                Config.ropeCount += 1;
                ItemCreater.Instance.OnCreateRopeSkip(1);
                break;
            case "跳绳盲盒":
                ModVideoPlayerCreater.Instance.OnCreateRopeVideoPlayer();
                break;
            case "顶乌龟":
                ItemCreater.Instance.OnCreatePeakKuba(1);
                break;
        }
    }

}
