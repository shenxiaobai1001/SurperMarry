using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using SystemScripts;
using UnityEngine;

public class BarrageFuncCreater : MonoBehaviour
{
    public static BarrageFuncCreater Instance;
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

    public GameObject barrageWusaqi;
    public GameObject barrageMenace;
    public GameObject barrageHangself;
    public GameObject barrageMangseng;
    public GameObject barrageQilinbi;
    public GameObject barrageTcjiao;
    public GameObject barrageChain;
    public GameObject barrageSuperMan;
    public GameObject barrageZombie;
    public GameObject barrageDance;
    public GameObject barrageDmdm;
    public GameObject barrageFlog;
    public GameObject barrageMask;
    public GameObject barrageShoeShine;
    public GameObject barrageRopeSkip;
    public GameObject barragePeakKuba;

    public void OnCreateWUSAQI(BarrageValue barrageFuncData,int index) 
        => InstantiateBarrageFunc(barrageWusaqi, barrageFuncData, index);

    public void OnCreateMenace(BarrageValue barrageFuncData, int index) 
        => InstantiateBarrageFunc(barrageMenace, barrageFuncData, index);

    public void OnCreateHangself(BarrageValue barrageFuncData, int index) 
        => InstantiateBarrageFunc(barrageHangself, barrageFuncData, index, barrageFuncData.name);

    public void OnCreateMangSeng(BarrageValue barrageFuncData, int index)
    => InstantiateBarrageFunc(barrageMangseng, barrageFuncData, index, barrageFuncData.name);

    public void OnCreateQLBi(BarrageValue barrageFuncData, int index)
    => InstantiateBarrageFunc(barrageQilinbi, barrageFuncData, index, barrageFuncData.name);

    public void OnCreateTCJiao(BarrageValue barrageFuncData, int index)
    => InstantiateBarrageFunc(barrageTcjiao, barrageFuncData, index, barrageFuncData.name);
    public void OnCreateDaom(BarrageValue barrageFuncData, int index)
=> InstantiateBarrageFunc(barrageDmdm, barrageFuncData, index);

    public void OnCreateSuperMan(BarrageValue barrageFuncData, int index,float time)
    {

        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("³¬ÈË+10Ãë"))
        {
            Sound.PlaySound("smb_1-up");
            Config.superManTime += time;
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            if(barrageFuncData.name== "³¬ÈË+10Ãë")
               InstantiateBarrageFunc(barrageSuperMan, barrageFuncData, index);
        }
    }
    public void OnCreateZombie(BarrageValue barrageFuncData, int index, float time)
    {
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("½©Ê¬+10Ãë")
            || BarrageFuncController.Instance.OnCheckBarrageFuncByName("½©Ê¬-10Ãë"))
        {
            Sound.PlaySound("smb_1-up");
            Config.allZombieTime += time;
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            if (barrageFuncData.name == "½©Ê¬+10Ãë")
                InstantiateBarrageFunc(barrageZombie, barrageFuncData, index);
        }
    }
    public void OnCreateChain(BarrageValue barrageFuncData, int index)
    {
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("ÌúÁ´"))
        {
            Sound.PlaySound("Mod/lock");
            Config.chainCount += 20;
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            InstantiateBarrageFunc(barrageChain, barrageFuncData, index, barrageFuncData.name);
        }
    }

    public void OnCreateDance(BarrageValue barrageFuncData, int index)
    => InstantiateBarrageFunc(barrageDance, barrageFuncData, index);

    public void OnCreateFlog(BarrageValue barrageFuncData, int index)
    {
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("´ò°å×ÓÃ¤ºÐ"))
        {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            InstantiateBarrageFunc(barrageFlog, barrageFuncData, index, barrageFuncData.name);
        }
    }

    public void OnCreateMask(BarrageValue barrageFuncData, int index)
    {
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("¹ØµÆ"))
        {
            Config.maskTime += 1;
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            Config.maskTime += 1;
            InstantiateBarrageFunc(barrageMask, barrageFuncData, index);
        }
    }

    public void OnCreateShoe(BarrageValue barrageFuncData, int index)
    {
        Sound.PlaySound("Mod/capixie");
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("²ÁÆ¤Ð¬"))
        {
            Sound.PlaySound("smb_1-up");
            Config.shineCount += 5;
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            InstantiateBarrageFunc(barrageShoeShine, barrageFuncData, index, barrageFuncData.name);
        }
    }
    public void OnCreateRopeSkip(BarrageValue barrageFuncData, int index,int count)
    {
        Config.ropeCount += count;
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("ÌøÉþ")
            || BarrageFuncController.Instance.OnCheckBarrageFuncByName("ÌøÉþÃ¤ºÐ"))
        {
         
            Sound.PlaySound("smb_1-up");
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            InstantiateBarrageFunc(barrageRopeSkip, barrageFuncData, index, barrageFuncData.name);
        }
    }
    public void OnCreatePeakKuba(BarrageValue barrageFuncData, int index)
    {
        Config.kubaCount += 5;
        if (BarrageFuncController.Instance.OnCheckBarrageFuncByName("¶¥ÎÚ¹ê"))
        {
            Sound.PlaySound("smb_1-up");
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
        }
        else
        {
            InstantiateBarrageFunc(barragePeakKuba, barrageFuncData, index, barrageFuncData.name);
        }
    }

    public GameObject InstantiateBarrageFunc(GameObject prefab, BarrageValue barrageFuncData, int index, string callName="")
    {
        Vector3 createPos = OnCreatePos(callName);
        PFunc.Log(createPos, callName);
        GameObject obj = SimplePool.Spawn(prefab, createPos, Quaternion.identity);
        obj.transform.SetParent(transform);
        obj.SetActive(true);

        OnCreateEnd(barrageFuncData.name, obj, barrageFuncData,index);
        return obj;
    }

    public Vector3 OnCreatePos(string callName)
    {
        Vector3 createPos = Vector3.zero;
        if (string.IsNullOrEmpty(callName)) return createPos;
        Vector3 vectorPlayer = PlayerController.Instance.transform.position;
        switch (callName)
        {
            case "ÉÏµõ":
                float value = GameStatusController.IsHidden ? 37 : 0;
                createPos = new Vector3(Camera.main.transform.position.x, value);
                break;
            case "Ò»¿â":
                createPos = new Vector3(Camera.main.transform.position.x, vectorPlayer.y + 15, 0);
                break;
            case "ÌúÁ´":
                createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                break;
            case "´ò°å×ÓÃ¤ºÐ":
                createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                break;
            case "²ÁÆ¤Ð¬":
                createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                break;
            case "ÌøÉþ":
            case "ÌøÉþÃ¤ºÐ":
                createPos = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, 0);
                break;
            case "¶¥ÎÚ¹ê":
                createPos = vectorPlayer;
                break;
        }
        return createPos;
    }

    public void OnCreateEnd(string callName, GameObject obj,BarrageValue barrageFuncData, int index)
    {
        switch (callName)
        {
            case "ÎÚÈøÆæ":
            case "ÎÚÈøÆæÓ²¿Ø":
                obj.GetComponent<BarrageWuSaqi>().OnStart(barrageFuncData, index);
                break;
            case "Áé»ê¿½ÎÊ":
            case "Áé»ê¿½ÎÊÓ²¿Ø":
                obj.GetComponent<BarrageMenace>().OnStart(barrageFuncData, index);
                break;
            case "ÉÏµõ":
                Sound.PlaySound("Mod/hangself");
                obj.GetComponent<BarrageHangSelf>().OnStart(barrageFuncData, index);
                break;
            case "Ò»¿â":
                Sound.PlaySound("Mod/mangseng");
                obj.GetComponent<BarrageMangseng>().OnStart(barrageFuncData, index);
                break;
            case "÷è÷ë±Û":
                Sound.PlaySound("Mod/QLBi");
                ItemCreater.Instance.qlCount++;
                obj.GetComponent<QLBI>().OnStart(barrageFuncData, index);
                break;
            case "Ìì²Ð½Å":
                Sound.PlaySound("Mod/TCJiao");
                ItemCreater.Instance.tcCount++;
                obj.GetComponent<TCJiao>().OnStart(barrageFuncData, index);
                break;
            case "ÌúÁ´":
                Sound.PlaySound("Mod/lock");
                Config.chainCount += 20;
                obj.GetComponent<BarrageChainPlayer>().OnStart(barrageFuncData, index);
                break;
            case "³¬ÈË+10Ãë":
                Sound.PlaySound("smb_1-up");
                Config.superManTime += 10;
                obj.GetComponent<BarrageSuperMan>().OnStart(barrageFuncData, index);
                break;
            case "½©Ê¬+10Ãë":
                Sound.PlaySound("smb_1-up");
                Config.allZombieTime += 10;
                obj.GetComponent<BarrageZombie>().OnStart(barrageFuncData, index);
                break;
            case "¹ã²¥Ìå²Ù":
                Sound.PlayMusic("Mod/guangbo");
                obj.GetComponent<BarrageDance>().OnStart(barrageFuncData, index);
                break;
            case "µ¶Âí":
                Sound.PlayMusic("Mod/dmdm");
                obj.GetComponent<BarrageDaom>().OnStart(barrageFuncData, index);
                break;
            case "´ò°å×ÓÃ¤ºÐ":
                obj.GetComponent<BarrageFlogPlayer>().OnStart(barrageFuncData, index);
                break;
            case "¹ØµÆ":
                obj.GetComponent<UIMask>().OnStart(barrageFuncData, index);
                break;
            case "²ÁÆ¤Ð¬":
                Config.shineCount += 20;
                obj.GetComponent<BarrageShoeShine>().OnStart(barrageFuncData, index);
                break;
            case "ÌøÉþ":
            case "ÌøÉþÃ¤ºÐ":
                obj.GetComponent<BarrageRopeSkip>().OnStart(barrageFuncData, index);
                break;
            case "¶¥ÎÚ¹ê":
                obj.GetComponent<BarragePeakKuba>().OnStart(barrageFuncData, index);
                break;
        }
    }
}
