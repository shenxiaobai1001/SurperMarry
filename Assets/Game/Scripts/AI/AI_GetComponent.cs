using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Unity的一些扩展方法
static public class AI_GetComponent
{
    /// <summary>获取图片组件</summary>
    public static Button GetButton(this Transform obj)
    {
        return obj.GetComponent<Button>();
    }

    /// <summary>获取图片组件</summary>
    public static Button GetButton(this GameObject obj)
    {
        return obj.GetComponent<Button>();
    }

    /// <summary>获取图片组件</summary>
    public static Button GetButton(this MonoBehaviour obj)
    {
        return obj.GetComponent<Button>();
    }



    /// <summary>获取RectTransform组件</summary>
    public static RectTransform GetRectTransform(this Transform obj)
    {
        return obj.GetComponent<RectTransform>();
    }

    /// <summary>获取RectTransform组件</summary>
    public static RectTransform GetRectTransform(this GameObject obj)
    {
        return obj.GetComponent<RectTransform>();
    }

    /// <summary>获取RectTransform组件</summary>
    public static RectTransform GetRectTransform(this MonoBehaviour obj)
    {
        return obj.GetComponent<RectTransform>();
    }


    /// <summary>获取选项按钮组件</summary>
    public static Toggle GetToggle(this Transform obj)
    {
        return obj.GetComponent<Toggle>();
    }

    /// <summary>获取选项按钮组件</summary>
    public static Toggle GetToggle(this GameObject obj)
    {
        return obj.GetComponent<Toggle>();
    }

    /// <summary>获取选项按钮组件</summary>
    public static Toggle GetToggle(this MonoBehaviour obj)
    {
        return obj.GetComponent<Toggle>();
    }





    /// <summary>获取图片组件</summary>
    public static Image GetImage(this Transform obj)
    {
        return obj.GetComponent<Image>();
    }

    /// <summary>获取图片组件</summary>
    public static Image GetImage(this GameObject obj)
    {
        return obj.GetComponent<Image>();
    }

    /// <summary>获取图片组件</summary>
    public static Image GetImage(this MonoBehaviour obj)
    {
        return obj.GetComponent<Image>();
    }



    

    /// <summary>获取图片组件</summary>
    public static RawImage GetRawImage(this Transform obj)
    {
        return obj.GetComponent<RawImage>();
    }

    /// <summary>获取图片组件</summary>
    public static RawImage GetRawImage(this GameObject obj)
    {
        return obj.GetComponent<RawImage>();
    }

    /// <summary>获取图片组件</summary>
    public static RawImage GetRawImage(this MonoBehaviour obj)
    {
        return obj.GetComponent<RawImage>();
    }





    /// <summary>获取文本组件</summary>
    public static Text GetText(this Transform obj)
    {
        return obj.GetComponent<Text>();
    }

    /// <summary>获取文本组件</summary>
    public static Text GetText(this GameObject obj)
    {
        return obj.GetComponent<Text>();
    }

    /// <summary>获取文本组件</summary>
    public static Text GetText(this MonoBehaviour obj)
    {
        return obj.GetComponent<Text>();
    }





    /// <summary>获取文本组件</summary>
    public static TMP_Text GetTMPText(this Transform obj)
    {
        return obj.GetComponent<TMP_Text>();
    }

    /// <summary>获取文本组件</summary>
    public static TMP_Text GetTMPText(this GameObject obj)
    {
        return obj.GetComponent<TMP_Text>();
    }

    /// <summary>获取文本组件</summary>
    public static TMP_Text GetTMPText(this MonoBehaviour obj)
    {
        return obj.GetComponent<TMP_Text>();
    }





    /// <summary>获取文本组件</summary>
    public static TMP_InputField GetTMPInputField(this Transform obj)
    {
        return obj.GetComponent<TMP_InputField>();
    }

    /// <summary>获取文本组件</summary>
    public static TMP_InputField GetTMPInputField(this GameObject obj)
    {
        return obj.GetComponent<TMP_InputField>();
    }

    /// <summary>获取文本组件</summary>
    public static TMP_InputField GetTMPInputField(this MonoBehaviour obj)
    {
        return obj.GetComponent<TMP_InputField>();
    }





    /// <summary>获取文本组件</summary>
    public static TextMeshProUGUI GetTextMeshProUGUI(this Transform obj)
    {
        return obj.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>获取文本组件</summary>
    public static TextMeshProUGUI GetTextMeshProUGUI(this GameObject obj)
    {
        return obj.GetComponent<TextMeshProUGUI>();
    }

    /// <summary>获取文本组件</summary>
    public static TextMeshProUGUI GetTextMeshProUGUI(this MonoBehaviour obj)
    {
        return obj.GetComponent<TextMeshProUGUI>();
    }
}
