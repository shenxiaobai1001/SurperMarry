using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainBridge : MonoBehaviour
{
    public Transform bridgeTrans;
    int maxBrigeCount = 0;
    int bridegIndex = 0;
    Coroutine coroutine;
    // Start is called before the first frame update
    void Start()
    {
        maxBrigeCount = bridgeTrans.childCount;
    }

    public void OnMinBrige()
    {
        bridegIndex = maxBrigeCount - 1;
        if (coroutine==null)
        {
            coroutine = StartCoroutine(OnMinBrigeIE());
        }
    }
    IEnumerator OnMinBrigeIE()
    {
        while (bridegIndex > 0)
        {
            bridgeTrans.GetChild(bridegIndex).gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            bridegIndex--;
        }
        coroutine = null;
    }
}
