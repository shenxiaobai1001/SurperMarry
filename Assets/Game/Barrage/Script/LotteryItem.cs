using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LotteryItem : MonoBehaviour
{
    public string itemName;

    public Text itemText;
    public Image choseImage;

    private bool _isFlicker;

    void Awake()
    {
        Init("谢谢参与");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(string text)
    {
        itemName = text;
        itemText = transform.GetChild(0).GetComponent<Text>();
        choseImage = transform.GetChild(1).GetComponent<Image>();

        itemText.text = itemName;
    }

    /// <summary>
    /// 选中
    /// </summary>
    public void SelectedOn()
    {
        LotteryItem[] lotteryItems = FindObjectsByType<LotteryItem>(FindObjectsSortMode.None);
        foreach(var item in lotteryItems)
        {
            item.choseImage.gameObject.SetActive(false);
        }
        choseImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 闪烁
    /// </summary>
    public IEnumerator FlickerOn()
    {
        _isFlicker = true;
        while (_isFlicker)
        {
            yield return new WaitForSeconds(0.1f);
            choseImage.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            choseImage.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// 取消闪烁
    /// </summary>
    public void StopFlicker()
    {
        _isFlicker = false;
        StopCoroutine(FlickerOn());
    }



    // ============ 添加测试按钮 ============

    [ContextMenu("测试 SelectedOn 方法")]
    private void TestSelectedOn()
    {
        Debug.Log($"测试 {gameObject.name} 的 SelectedOn 方法");
        SelectedOn();
    }
}
