using System;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class BarrageExecutting 
{
    public static void OnExecutingBarrage(string callName,int index, BarrageValue barrageFuncData)
    {
        switch (callName)
        {
            case "乌龟一只":
                MonsterCreater.Instance.OnCreateTortoise(1, index);
                break;
            case "乌龟十只":
                MonsterCreater.Instance.OnCreateTortoise(10, index);
                break;
            case "乌龟一百只":
                MonsterCreater.Instance.OnCreateTortoise(100, index);
                break;
            case "蘑菇一只":
                MonsterCreater.Instance.OnCreateMushroom(1, index);
                break;
            case "蘑菇十只":
                MonsterCreater.Instance.OnCreateMushroom(10, index);
                break;
            case "蘑菇一百只":
                MonsterCreater.Instance.OnCreateMushroom(100, index);
                break;
            case "飞龟一只":
                MonsterCreater.Instance.OnCreateFlyKoopa(1, index);
                break;
            case "飞龟十只":
                MonsterCreater.Instance.OnCreateFlyKoopa(10, index);
                break;
            case "飞龟一百只":
                MonsterCreater.Instance.OnCreateFlyKoopa(100, index);
                break;
            case "飞鱼一只":
                MonsterCreater.Instance.OnCreateFlyFish(1, index);
                break;
            case "飞鱼十只":
                MonsterCreater.Instance.OnCreateFlyFish(10, index);
                break;
            case "飞鱼一百只":
                MonsterCreater.Instance.OnCreateFlyFish(100, index);
                break;
            case "甲壳虫一只":
                MonsterCreater.Instance.OnCreateBeatles(1, index);
                break;
            case "甲壳虫十只":
                MonsterCreater.Instance.OnCreateBeatles(10, index);
                break;
            case "甲壳虫一百只":
                MonsterCreater.Instance.OnCreateBeatles(100, index);
                break;
            case "游戏时间+10s":
                GameStatusController.IsGameFinish = false;
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time += 10;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "游戏时间-10s":
                Sound.PlaySound("smb_1-up");
                GameManager.Instance.time -= 10;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "生命+10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "生命-10%":
                Sound.PlaySound("smb_1-up");
                ModData.mLife -= (int)(ModData.mLife * 0.1f);
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "生命+1":
                Sound.PlaySound("smb_1-up");
                ModData.mLife += 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "生命-1":
                Sound.PlaySound("smb_1-up");
                ModData.mLife -= 1;
                EventManager.Instance.SendMessage(Events.OnChangeLife);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "扔香蕉":                     
                ItemCreater.Instance.OnCreateBanana(1, index);
                break;
            case "动感DJ":
                ModVideoPlayerCreater.Instance.OnPlayDJ(index);
                break;
            case "万箭齐发":
                ItemCreater.Instance.OnCreateManyArrow(1, index);
                break;
            case "抓鸭子":
                ModVideoPlayerCreater.Instance.OnCreateDuckVideoPlayer(index);
                break;
            case "抓可达鸭":
                ModVideoPlayerCreater.Instance.OnCreatePsyDuckVideoPlayer(index);
                break;
            case "抓乌龟":
                ModVideoPlayerCreater.Instance.OnCreateKoopaVideoPlayer(index);
                break;
            case "乌萨奇":
                BarrageFuncCreater.Instance.OnCreateWUSAQI(barrageFuncData,index);
                break;
            case "灵魂拷问":
                BarrageFuncCreater.Instance.OnCreateMenace(barrageFuncData, index);
                break;
            case "乌萨奇硬控":
                BarrageFuncCreater.Instance.OnCreateWUSAQI(barrageFuncData, index);
                break;
            case "灵魂拷问硬控":
                BarrageFuncCreater.Instance.OnCreateMenace(barrageFuncData, index);
                break;
            case "上吊":
                BarrageFuncCreater.Instance.OnCreateHangself(barrageFuncData, index);
                break;
            case "一库":
                BarrageFuncCreater.Instance.OnCreateMangSeng(barrageFuncData, index);
                break;
            case "滚石":
                ItemCreater.Instance.OnCreateRollStone(1, index);
                break;
            case "滚刺":
                ItemCreater.Instance.OnCreateRollArrow(1, index);
                break;
            case "陨石":
                ItemCreater.Instance.OnCreateMeteorite(1, index);
                break;
            case "麒麟臂":
                BarrageFuncCreater.Instance.OnCreateQLBi(barrageFuncData, index);
                break;
            case "天残脚":
                BarrageFuncCreater.Instance.OnCreateTCJiao(barrageFuncData, index);
                break;
            case "随机天火":
                ItemCreater.Instance.OnCreateSingleUPFire(1, index);
                break;
            case "全屏天火":
                ItemCreater.Instance.OnCreateUPFire(1, index);
                break;
            case "随机地火":
                ItemCreater.Instance.OnCreateDownFire(1, index);
                break;
            case "全屏地火":
                ItemCreater.Instance.OnCreateDownFire(66, index);
                break;
            case "随机传送":
                GameModController.Instance.OnRandromPlayerPos();
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "随机关卡":
                GameModController.Instance.OnRandromPass(index);
                break;
            case "铁链":
                BarrageFuncCreater.Instance.OnCreateChain(barrageFuncData, index);
                break;
            case "雷电":
                ItemCreater.Instance.OnCreateLazzer(1, index);
                break;
            case "砖块+10":
                CreateWallManager.Instance.wallCount += 10;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "石头+10":
                CreateWallManager.Instance.stonesCount += 10;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "美女盲盒":
                ModVideoPlayerCreater.Instance.OnPlayGrilVideo(index);
                break;
            case "火焰马里奥":
                PlayerModController.Instance.OnRandromPlayer(2);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "上一关":
                if (GameModController.Instance != null) 
                    GameModController.Instance.OnEnterNextPass(-1, index);
                break;
            case "下一关":
                if (GameModController.Instance != null) 
                    GameModController.Instance.OnEnterNextPass(1, index);
                break;
            case "大蘑菇":
                ItemCreater.Instance.OnCreateBigMG(1, index);
                break;
            case "超人+10秒":
                BarrageFuncCreater.Instance.OnCreateSuperMan(barrageFuncData, index,10);
                break;
            case "超人-10秒":
                BarrageFuncCreater.Instance.OnCreateSuperMan(barrageFuncData, index ,-10);
                break;
            case "僵尸+10秒":
                BarrageFuncCreater.Instance.OnCreateZombie(barrageFuncData, index, 10);
                break;
            case "僵尸-10秒":
                BarrageFuncCreater.Instance.OnCreateZombie(barrageFuncData, index, -10);
                break;
            case "广播体操":
                BarrageFuncCreater.Instance.OnCreateDance(barrageFuncData, index);
                break;
            case "刀马":
                BarrageFuncCreater.Instance.OnCreateDaom(barrageFuncData, index);
                break;
            case "凿头":
                PlayerModController.Instance.OnKickHead(index);
                break;
            case "砍刀形态":
                PlayerModController.Instance.OnRandromPlayer(0);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "拿枪形态":
                PlayerModController.Instance.OnRandromPlayer(1);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "随机形态":
                PlayerModController.Instance.OnRandromPlayer(5);
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "大齿轮":
                ItemCreater.Instance.OnCreateBigGear(1, index);
                break;
            case "撞大运":
                ItemCreater.Instance.OnCreateTrunck(1, index);
                break;
            case "打板子盲盒":
                ModVideoPlayerCreater.Instance.OnCreateFlog(barrageFuncData, index);
                break;
            case "变大":
                ModVideoPlayerCreater.Instance.OnPlayBig();
                PlayerModController.Instance.OnChangScale(0.01f, index);
                break;
            case "变小":
                ModVideoPlayerCreater.Instance.OnPlaySmall();
                PlayerModController.Instance.OnChangScale(-0.01f, index);
                break;
            case "关灯":
                Sound.PlaySound("smb_1-up");
                BarrageFuncCreater.Instance.OnCreateMask(barrageFuncData, index);
                break;
            case "重新开始":
                if (GameModController.Instance != null) GameModController.Instance.OnLoadScene("1-1", index);
                break;
            case "陷阱数量+1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount < ModData.allTrapCount)
                    ModData.canTrapCount += 1;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "陷阱数量-1":
                Sound.PlaySound("smb_1-up");
                if (ModData.canTrapCount > 0)
                    ModData.canTrapCount -= 1;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "全部陷阱激活":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = ModData.allTrapCount;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "全部陷阱关闭":
                Sound.PlaySound("smb_1-up");
                ModData.canTrapCount = 0;
                EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
                break;
            case "无敌星":
                PlayerModController.Instance.OnSetInvincible(index);
                break;
            case "隐身":
                PlayerModController.Instance.OnSetInvisibilityState(index);
                break;
            case "火圈":
                Sound.PlaySound("smb_1-up");
                ItemCreater.Instance.OnCreatehuoQuan(1, index);
                break;
            case "伸缩藤条":
                ItemCreater.Instance.OnCreateRattan(1, index);
                break;
            case "哭坟":
                ModVideoPlayerCreater.Instance.OnKuFen(barrageFuncData,index);
                break;
            case "擦皮鞋":
                BarrageFuncCreater.Instance.OnCreateShoe(barrageFuncData, index);
                break;
            case "跳绳":
                BarrageFuncCreater.Instance.OnCreateRopeSkip(barrageFuncData, index,1);
                break;
            case "跳绳盲盒":
                ModVideoPlayerCreater.Instance.OnCreateRopeVideoPlayer(barrageFuncData, index);
                break;
            case "顶乌龟":
                BarrageFuncCreater.Instance.OnCreatePeakKuba(barrageFuncData, index);
                break;
        }
    }

}
