using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIShoeShine : MonoBehaviour
{
    public GameObject center;
    public Text tx_number;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(Events.OnShowShine, OnShowShine);
    }

    // Update is called once per frame
    void Update()
    {
        if(tx_number) tx_number.text = $"สฃำเดฮสý{Config.shineCount}";
    }

    void OnShowShine(object msg)
    {
        bool show = (bool)msg;
        if (center) center.SetActive(show);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.OnShowShine, OnShowShine);
    }
}
