using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Unity的一些扩展方法
static public class AI_GetGameObject
{
    static int choose_index = 0;
    static Transform choose = null;

    /// <summary>获取节点回调</summary>
    static void CbFind(Transform t, string data, int index)
    {
        if (index == 1)
        {
            choose_index = 999;
            choose = null;
        }

        var name = data.ToString();

        for (int i = 0; i < t.childCount; i++)
        {
            var t1 = t.GetChild(i);
            if (t1.childCount > 0)
            {
                CbFind(t1, data, index + 1);
            };

            if(t1.name == name)
            {
                if (choose == null || index < choose_index)
                {
                    choose = t1;
                    choose_index = index;
                }
            }
        }
    }

    /// <summary>获取节点</summary>
    static GameObject Find(GameObject obj, string data)
    {
        CbFind(obj.transform, data, 1);

       // if (choose!=null && choose_index > 5) Debug.Log("节点层级过深, 存在安全风险和效率问题: "+choose_index+ "层");

        return choose == null ? null : choose.gameObject;
    }

    /// <summary>根据名称获取子节点(模糊查找，要确保名称的唯一性)</summary>
    static public GameObject Get(this GameObject obj, object data)
    {
        var str = data.ToString();

        if (str.Contains("/"))//全路径查找
        {
            return obj.transform.Find(str)?.gameObject;
        }

        return Find(obj, str);
    }

    /// <summary>根据名称获取子节点(模糊查找，要确保名称的唯一性)</summary>
    static public GameObject Get(this Transform obj, object data)
    {
        return Get(obj.gameObject, data);
    }

    /// <summary>根据索引获取子节点</summary>
    static public GameObject GetChild(this GameObject go, int index)
    {
        Transform node = go.transform.GetChild(index);
        return node?.gameObject;
    }

    /// <summary>根据名称获取子节点</summary>
    static public GameObject GetChild(this Transform go, string name)
    {
        Transform node = go.transform.Find(name);
        return node?.gameObject;
    }

    /// <summary>根据名称获取子节点</summary>
    static public GameObject GetChild(this GameObject go, string name)
    {
        Transform node = go.transform.Find(name);
        return node?.gameObject;
    }

    /// <summary>获取对象的坐标</summary>
    public static Vector3 GetPos(this GameObject obj)
    {
        return obj.transform.localPosition;
    }

    /// <summary>获取对象的坐标</summary>
    public static Vector3 GetPos(this Transform obj)
    {
        return obj.localPosition;
    }

    /// <summary>获取对象的坐标</summary>
    public static float GetPosX(this GameObject obj)
    {
        return obj.transform.localPosition.x;
    }

    /// <summary>获取对象的坐标</summary>
    public static float GetPosY(this GameObject obj)
    {
        return obj.transform.localPosition.y;
    }

    /// <summary>获取对象的坐标</summary>
    public static float GetPosX(this Transform obj)
    {
        return obj.localPosition.x;
    }

    /// <summary>获取对象的坐标</summary>
    public static float GetPosY(this Transform obj)
    {
        return obj.localPosition.y;
    }

    /// <summary>获取对象属性组件</summary>
    public static Rect GetRect(this GameObject obj)
    {
        return obj.GetComponent<RectTransform>().rect;
    }

    /// <summary>获取对象属性组件</summary>
    public static Rect GetRect(this Transform obj)
    {
        return obj.GetComponent<RectTransform>().rect;
    }

    /// <summary>获取尺寸</summary>
    public static Vector2 GetSize(this GameObject obj)
    {
        var t = obj.GetComponent<RectTransform>();
        if (t == null) return new Vector2(0, 0);

        return t.sizeDelta;
    }

    /// <summary>获取尺寸</summary>
    public static Vector2 GetSize(this Transform obj)
    {
        var t = obj.GetComponent<RectTransform>();
        if (t == null) return new Vector2(0, 0);

        return t.sizeDelta;
    }

    /// <summary>获取对象的坐标[屏幕中心为原点,不能有缩放节点]</summary>
    public static Vector2 GetCenterPos(this GameObject obj)
    {
        float x = 0;
        float y = 0;

        var t = obj.transform;
        x += t.localPosition.x;
        y += t.localPosition.y;

        while (t.parent && !t.parent.GetComponent<Canvas>())
        {
            t = t.parent;
            x += t.localPosition.x;
            y += t.localPosition.y;
        }

        return new Vector2(x, y);
    }

    /// <summary>获取对象的坐标[屏幕中心为原点,不能有缩放节点]</summary>
    public static Vector2 GetCenterPos(this Transform obj)
    {
        return GetCenterPos(obj.gameObject);
    }

    /// <summary>获取所有子对象</summary>
    public static List<GameObject> GetAllChild(this GameObject obj)
    {
        List<GameObject> objs = new List<GameObject>();

        if (obj == null)
        {
            return objs;
        }

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            objs.Add(obj.transform.GetChild(i).gameObject);
        }

        return objs;
    }

    /// <summary>获取所有子对象</summary>
    public static List<GameObject> GetAllChild(this Transform obj)
    {
        return GetAllChild(obj.gameObject);
    }
}
