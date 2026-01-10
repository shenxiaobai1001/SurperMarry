using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

//Unity的一些扩展方法
static public class AI_Click
{
    /// <summary>添加按钮点击</summary>
    public static void Click(this GameObject obj, UnityAction callback, int snake = 1, int interval = 100,
        bool canBeEmpty = false, bool autoPlay = true, string sound_url = "Common/Click")
    {
        if (obj == null)
        {
            if (!canBeEmpty) PFunc.Log("按钮点击异常:", "空对象");
            return;
        }

        long t1 = 0;
        var button = obj.GetButton();
        if (button == null)
        {
            button = obj.AddComponent<Button>();
        }
        if(button == null)
            return;
        if (button.targetGraphic==null)
        {
            Image tar;
            if (button.transform.childCount!=0)
            {
                tar = obj.transform.GetChild(0).GetImage();
            }
            else
            {
                tar = button.GetComponent<Image>();
            }
            
            button.targetGraphic = tar;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(delegate
        {
            long t2 = DateTime.Now.ToFileTime();
            if (t2 - t1 < 1e4 * interval) return;
            t1 = t2;
            //if (autoPlay) Sound.PlaySound(sound_url);
            callback();
            switch (snake)
            {
                case 1:
                    obj.GetRectTransform().DOShakePosition(0.5f, 10, 50);
                    break;
                case 2:
                    Sequence loopTween = DOTween.Sequence();
                    loopTween.Append(obj.transform.DOScale(1.4f, 0.3f));
                    loopTween.AppendInterval(0.8f);
                    loopTween.Append(obj.transform.DOScale(1f, 0));
                    loopTween.onComplete += () => { loopTween.Kill(); };
                    break;
            }
        });

        Image image = obj.GetImage();
        if (image) image.raycastTarget = true;
    }
    /// <summary>添加选项按钮点击</summary>
    public static void ToggleClick(this GameObject obj, UnityAction callback, bool canBeEmpty = false, bool clernUp = false, bool okAndNot = false, string sound_url = "Common/click")
    {
        var toggle = obj.GetToggle();

        if (toggle == null) return;

        ToggleClick(toggle, callback, canBeEmpty, clernUp, okAndNot, sound_url);
    }

    /// <summary>添加选项按钮点击</summary>
    public static void ToggleClick(this Toggle obj, UnityAction callback, bool canBeEmpty = false, bool clernUp = false, bool okAndNot = false, string sound_url = "Common/click")
    {
        if (obj == null)
        {
           // if (!canBeEmpty) vv.Warn("选项按钮点击异常:", "空对象");
            return;
        }

        void cb_click(bool ok)
        {
            if (!ok && !okAndNot) return;
         //   if (sound_url != "") Sound.PlaySound(sound_url);
            callback();
        }

        if (clernUp)
        {
            obj.onValueChanged.RemoveListener(delegate (bool ok)
            {
                cb_click(ok);
            });
        }

        obj.onValueChanged.AddListener(delegate (bool ok)
        {
            cb_click(ok);
        });
    }

    /// <summary>添加按钮触摸</summary>
    public static void Touch(this GameObject obj, UnityAction cbDown, UnityAction cbUp)
    {
        EventTrigger trigger = obj.AddComponent<EventTrigger>();

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) =>
        {
            cbDown();
        });

        trigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) =>
        {
            cbUp();
        });

        trigger.triggers.Add(pointerUpEntry);
    }

    private static Dictionary<GameObject, Coroutine> pressCoroutines = new Dictionary<GameObject, Coroutine>();

    public static IEnumerator WhilePressed(UnityAction<bool> callback, bool isPressed, float intervalFirst, float interval)
    {
        bool isFirstPress = true;
        callback.Invoke(isFirstPress);
        yield return new WaitForSeconds(intervalFirst);
        isFirstPress = false;
        while (isPressed)
        {
            callback.Invoke(isFirstPress);
            yield return new WaitForSeconds(interval);
        }
    }

    /// <summary>添加长按按钮点击</summary>
    public static void LongPress(this GameObject obj, UnityAction<bool> callback, float intervalFirst = 0.5f, float interval = 0.15f, bool canBeEmpty = false, string sound_url = "Common/click")
    {
        if (obj == null)
        {
            if (!canBeEmpty) PFunc.Log("按钮点击异常:", "空对象");
            return;
        }

        Button button = obj.GetComponent<Button>();
        if (button == null)
        {
            button = obj.AddComponent<Button>();
        }

        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = obj.AddComponent<EventTrigger>();
        }

        bool isPressed = false;

        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) =>
        {
            isPressed = true;
         //   if (sound_url != "") Sound.PlaySound(sound_url);
            Coroutine coroutine;
            if (pressCoroutines.TryGetValue(obj, out coroutine))
            {
                button.StopCoroutine(coroutine);
            }

            pressCoroutines[obj] = button.StartCoroutine(WhilePressed(callback, isPressed, intervalFirst, interval));
        });

        trigger.triggers.Add(pointerDownEntry);

        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) =>
        {
            isPressed = false;
            if (pressCoroutines.TryGetValue(obj, out Coroutine coroutine))
            {
                button.StopCoroutine(coroutine);
                pressCoroutines.Remove(obj);
            }
        });
        trigger.triggers.Add(pointerUpEntry);

        Image image = obj.GetComponent<Image>();
        if (image) image.raycastTarget = true;
    }

    /// <summary>输入框监听</summary>
    public static void InputBox(this TMP_InputField inputField, UnityAction onValueChanged, UnityAction onEndEdit, bool canBeEmpty = false)
    {
        if (inputField == null)
        {
         //   if (!canBeEmpty) vv.Warn("选项按钮点击异常:", "空对象");
            return;
        }

        inputField.onValueChanged.AddListener(delegate
        {
            if (onValueChanged != null)
            {
                onValueChanged();
            }
        });

        inputField.onEndEdit.AddListener(delegate
        {
            if (onEndEdit != null)
            {
                onEndEdit();
            }
        });
    }
}
