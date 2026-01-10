using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakBrickController : MonoBehaviour
{
    public List<BoxCollider2D> boxCollider2Ds;
    public List<CanBreakBrick> canBreakBrick;
    public BoxCollider2D collider2D;
    [HideInInspector]public bool toPool = false;

    private void OnEnable()
    {
        if (toPool)
        {
            breakCount = 0;
            for (int i = 0; i < canBreakBrick.Count; i++)
            {
                canBreakBrick[i].ResetBrick();
            }
            collider2D.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Meteorite"))
        {
            Sound.PlaySound("smb_breakblock");
            collider2D.enabled = false;
            OnChangeAllBox(true);
        }
    }

    void OnChangeAllBox(bool show)
    {
        for (int i = 0; i < boxCollider2Ds.Count; i++)
        {
            boxCollider2Ds[i].enabled = show;
        }
    }
    int breakCount=0;
    public void OnToPool()
    {
        breakCount++;
        if (breakCount>=4&& toPool)
        {
            SimplePool.Despawn(gameObject);
        }
    }
}
