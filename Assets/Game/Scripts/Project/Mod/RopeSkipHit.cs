using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSkipHit : MonoBehaviour
{
    public BarrageRopeSkip ropeSkip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PFunc.Log("RopeSkipHit", collision);
        if (collision.gameObject.tag.Contains("Player"))
        {
            PFunc.Log("RopeSkipHit", collision);
            Config.missRopeCount++;
            ropeSkip.triggerPlayer = true;
            EventManager.Instance.SendMessage(Events.OnLazzerHit);
        }
    }
}
