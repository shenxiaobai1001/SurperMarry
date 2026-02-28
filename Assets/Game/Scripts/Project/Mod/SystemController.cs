using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class SystemController : MonoBehaviour
{
    public static SystemController Instance;

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

        OnGetData(); 
    }
    [HideInInspector]
    public int maxAirWallHp;
    [HideInInspector]
    public int airWallHp;
    public bool airWallContin = false;

    public int scheduleDeviation;

    public float statuValue = 0.5f;
    public Volume postProcessVolume;
    ColorAdjustments colorAdjustments;
    public void OnGetData()
    {
        float VolumeMusic = 1;
        float VolumeSound = 1;
        if (PlayerPrefs.HasKey("VolumeMusic"))
            VolumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        if (PlayerPrefs.HasKey("VolumeSound"))
            VolumeSound = PlayerPrefs.GetFloat("VolumeSound");
        if (PlayerPrefs.HasKey("Saturation"))
        {
            if (postProcessVolume != null && postProcessVolume.profile != null)
            {
                // 尝试从配置文件中获取ColorAdjustments组件
                if (!postProcessVolume.profile.TryGet(out colorAdjustments))
                {
                    Debug.Log("在Volume Profile中未找到Color Adjustments效果！");
                }
            }
            statuValue = PlayerPrefs.GetFloat("Saturation");
            if (colorAdjustments != null)
            {
                // 将0-1的Slider值映射到-100到100的范围
                // 当value=0.5时，saturationValue=0
                // 当value=1时，saturationValue=100
                // 当value=0时，saturationValue=-100
                float saturationValue = (statuValue - 0.5f) * 200f;

                saturationValue = Mathf.Clamp(saturationValue, -100f, 100f);

                colorAdjustments.saturation.Override(saturationValue);
            }
        }
           

        PFunc.Log("OnGetData", VolumeMusic, VolumeSound);
        Sound.OnSetVolume(VolumeMusic, VolumeSound);
    }

    public void OnSetAirwallHp(int max, int now)
    {
        maxAirWallHp = max;
        airWallHp = now;
        airWallContin = airWallHp > 0;
        PlayerPrefs.SetInt("maxAirWallHp", maxAirWallHp);
    }
    public void OnSetWallHp(int now)
    {
        if (airWallHp <= 0) return;
        airWallHp -= now;
        airWallContin = airWallHp > 0;

    }
}
