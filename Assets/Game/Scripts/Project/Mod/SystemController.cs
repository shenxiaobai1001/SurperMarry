using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public void OnGetData()
    {
        float VolumeMusic = 1;
        float VolumeSound = 1;
        if (PlayerPrefs.HasKey("VolumeMusic"))
            VolumeMusic = PlayerPrefs.GetFloat("VolumeMusic");
        if (PlayerPrefs.HasKey("VolumeSound"))
            VolumeSound = PlayerPrefs.GetFloat("VolumeSound");

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
