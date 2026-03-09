using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSkipHit : MonoBehaviour
{
    public BarrageRopeSkip ropeSkip;
    public Transform uiNumber;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            PFunc.Log("RopeSkipHit", collision);
            Config.missRopeCount++;
            if (uiNumber) uiNumber.transform.DOScale(1.1f, 0.025f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
            {
                uiNumber.transform.localScale = Vector3.one;
            });
            ropeSkip.triggerPlayer = true;
            EventManager.Instance.SendMessage(Events.OnLazzerHit);
        }
    }
}
