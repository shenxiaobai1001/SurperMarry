using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrageFuncBase : MonoBehaviour
{
    public BarrageValue barrageData ;

    bool needPause = false;

    public virtual void OnStart() { OnBarrageQueueCheck(); }//开始执行
    public virtual void OnPause(){ }//暂停执行
    public virtual void OnContinue(){ OnBarrageQueueCheck(); }//继续执行
    public virtual void OnClose() { }//执行完毕

    public virtual void OnBarrageQueueCheck() { }

}
