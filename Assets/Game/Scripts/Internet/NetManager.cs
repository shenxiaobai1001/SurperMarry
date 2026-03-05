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