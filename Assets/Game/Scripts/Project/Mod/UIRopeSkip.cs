using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRopeSkip : MonoBehaviour
{
    public GameObject center;
    public Text tx_number;
    public Text tx_miss;
    public Text tx_succ;
    // Start is called before the first frame update
    void Start()
    {
        if (center) center.SetActive(false);
        EventManager.Instance.AddListener(Events.OnShowRope, OnShowShine);
    }

    // Update is called once per frame
    void Update()
    {
        if (tx_number) tx_number.text = $"剩余次数{Config.ropeCount}";
        if (tx_succ) tx_succ.text = $"成功次数{Config.succRopeCount}";
        if (tx_miss) tx_miss.text = $"失败次数{Config.missRopeCount}";
    }

    void OnShowShine(object msg)
    {
        bool show = (bool)msg;
        if (center) center.SetActive(show);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnShowRope, OnShowShine);
    }
}
