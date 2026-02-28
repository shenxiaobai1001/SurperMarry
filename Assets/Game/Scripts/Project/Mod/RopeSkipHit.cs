using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeSkipHit : MonoBehaviour
{
    public RopeSkip ropeSkip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("Player"))
        {
            PFunc.Log("Åöµ½Íæ¼Ò");
            Config.missRopeCount++;
            ropeSkip.triggerPlayer = true;
            EventManager.Instance.SendMessage(Events.OnLazzerHit);
        }
    }
}
