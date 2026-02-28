using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPeakKuba : MonoBehaviour
{
    public GameObject center;
    public Text tx_number;
    public Text tx_succ;
    // Start is called before the first frame update
    void Start()
    {
        if (center) center.SetActive(false);
        EventManager.Instance.AddListener(Events.OnShowKubaCount, OnShowShine);
    }

    // Update is called once per frame
    void Update()
    {
        if (tx_number) tx_number.text = $"剩余数量：{Config.kubaCount}";
        if (tx_succ) tx_succ.text = $"乌龟数量：{Config.hasKubaCount}";
    }

    void OnShowShine(object msg)
    {
        bool show = (bool)msg;
        if (center) center.SetActive(show);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnShowKubaCount, OnShowShine);
    }
}
