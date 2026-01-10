using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TCJiao : MonoBehaviour
{
    public Transform boomPos;
    public GameObject boom;
    public Transform targetSprite;

    Vector3 startPos;
    float allTime = 1.5f;
    float time = 0;
    float boomTime = 0;
    void Awake()
    {
        startPos = new Vector3(18, 20);
    }

    public void OnStarMove()
    {
        boomTime = 0;
        time = 0;
        targetSprite.localPosition = startPos;
        targetSprite.DOLocalMove(new Vector3(7.3f, 9.4f), 0.2f).OnComplete(() => { OnBeginCreateBoom(); });
    }

    void OnBeginCreateBoom()
    {
        PlayerModMoveController.Instance.TriggerModMove(MoveType.MaxLeft, new Vector3(-1, -0.5f), 25, allTime, true);
        CameraShaker.Instance.StartShake(allTime);
        StartCoroutine(OnCreateBoom());
    }

    IEnumerator OnCreateBoom()
    {
        while (time <= allTime)
        {
            time += Time.deltaTime;
            boomTime += Time.deltaTime;
            if (boomTime > 0.15f)
            {      
                GameObject bb = SimplePool.Spawn(boom, boomPos.transform.position, Quaternion.identity);
                bb.transform.parent = ItemCreater.Instance.modItemPatent;
                bb.transform.localEulerAngles = boomPos.transform.localEulerAngles;
                bb.SetActive(true);
                boomTime = 0;
            }
            yield return null;
        }
        PFunc.Log("OnCreateBoom", time);
        if (IsInvoking("OnBeginCreateBoom"))
        {
            CancelInvoke("OnBeginCreateBoom");
        }
        if (IsInvoking("OnCreateBoom"))
        {
            CancelInvoke("OnCreateBoom");
        }
        SimplePool.Despawn(this.gameObject);
    }
    private void OnDestroy()
    {
        if (IsInvoking("OnBeginCreateBoom"))
        {
            CancelInvoke("OnBeginCreateBoom");
        }
        if (IsInvoking("OnCreateBoom"))
        {
            CancelInvoke("OnCreateBoom");
        }
    }
}
