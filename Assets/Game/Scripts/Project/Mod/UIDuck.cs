using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDuck : MonoBehaviour
{
    public static UIDuck Instance;
    public GameObject center;
    public GameObject tan;
    public Text tx_null;
    public Text tx_get;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (center) center.SetActive(false);
    }
    public void OnSetCenter(bool show)
    {
        if(center) center.SetActive(show);
    }
    public void OnSetNullCount()
    {
        if (tx_null) tx_null.text= $"{ItemCreater.Instance.allReadyCreateDuck}";
        if (tx_get) tx_get.text = $"{ItemCreater.Instance.allCreateDuck}";
        if (tan) tan.transform.DOScale(1.1f, 0.025f).SetLoops(2, LoopType.Yoyo).OnComplete(() =>
        {
            tan.transform.localScale = Vector3.one;
        });
    }
}
