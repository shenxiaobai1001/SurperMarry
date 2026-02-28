using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class UISystem : MonoBehaviour
{
    public GameObject center;
    public Slider sl_music;
    public Slider sl_sound;
    public TMP_InputField input_life;
    public Dropdown dp_level;
    public GameObject btn_changeLV;
    public Dropdown dp_mode;
    public GameObject btn_changeMD;
    public Dropdown dp_Resolution;
    public GameObject btn_Resolution;
    public GameObject btn_Close;
    public TMP_InputField input_Wall;
    public TMP_InputField input_Stone;
    public GameObject Btn_moren;
    public Toggle tg_dead;

    public Slider sl_statu;
    public Volume postProcessVolume;
    ColorAdjustments colorAdjustments;

    float musicValue = 0;
    float soundValue = 0;
    int lifeCount = 0;
    int gameMode = 0;
    int width=1280;
    int height=960;

    string gameLevel = "1-1";
    // Start is called before the first frame update
    void Start()
    {
        if (postProcessVolume != null && postProcessVolume.profile != null)
        {
            // 尝试从配置文件中获取ColorAdjustments组件
            if (!postProcessVolume.profile.TryGet(out colorAdjustments))
            {
                Debug.Log("在Volume Profile中未找到Color Adjustments效果！");
            }
        }
        sl_music.onValueChanged.AddListener(OnChangeMusic);
        sl_sound.onValueChanged.AddListener(OnChangeSound);
        input_life.onEndEdit.AddListener(OnInputLife);
        dp_level.onValueChanged.AddListener(OnLevelDropChanged);
        btn_changeLV.Click(OnClickChangeLV);
        dp_mode.onValueChanged.AddListener(OnModeDropChanged);
        btn_changeMD.Click(OnClickChangeMD);
        dp_Resolution.onValueChanged.AddListener(OnResolutionDropChanged);
        btn_Resolution.Click(OnClickResolution);
        btn_Close.Click(OnClose);
        input_Wall.onEndEdit.AddListener(OnInputWall);
        input_Stone.onEndEdit.AddListener(OnInputStone);
        tg_dead.onValueChanged.AddListener(OnTgDeadOpen);
        Btn_moren.Click(OnClickMoRen);
        sl_statu.onValueChanged.AddListener(UpdateSaturation);
        center.SetActive(false);
        Invoke("OnInit",1);
    }

    private void OnInit()
    {
        musicValue = (float)Sound.VolumeMusic / (float)1;
        soundValue = (float)Sound.VolumeSound / (float)1;
        sl_music.value = musicValue;
        sl_sound.value = soundValue;

        sl_statu.value = SystemController.Instance.statuValue;
    }
    void OnChangeMusic(float value)
    {
        musicValue = value;

        Sound.OnSetVolume(musicValue, soundValue);
    }

    void OnChangeSound(float value)
    {
        soundValue = value;

        Sound.OnSetVolume(musicValue, soundValue);
    }

    void OnInputLife(string value)
    {
        if (value ==string.Empty) return;
        lifeCount = int.Parse(value);
        ModData.mLife = lifeCount;
    }
    void OnInputWall(string value)
    {
        if (value == string.Empty) return;
        int vaklyuue = int.Parse(value);
        CreateWallManager.Instance.wallCount = vaklyuue;
    }
    void OnInputStone(string value)
    {
        if (value == string.Empty) return;
         int vaklyuue = int.Parse(value);
        CreateWallManager.Instance.stonesCount = vaklyuue;
    }
    private void OnLevelDropChanged(int index)
    {
        int selectedIndex = dp_level.value;

        string selectedText = dp_level.options[selectedIndex].text;

        gameLevel = selectedText;
    }

    private void OnModeDropChanged(int index)
    {
        int selectedIndex = dp_mode.value;
        gameMode = selectedIndex;
    }

    private void OnResolutionDropChanged(int index)
    {
        int selectedIndex = dp_Resolution.value;
        string selectedText = dp_Resolution.options[selectedIndex].text;
        PFunc.Log(selectedText);
        string[] reso = selectedText.Split('x');
        width = int.Parse(reso[0]);
        height = int.Parse(reso[1]);
    }
    private void OnTgDeadOpen(bool value)
    {
        ModData.ShowDeadPanel=value;
        EventManager.Instance.SendMessage(Events.OnShowDeadPanel);
    }

    void OnClickResolution()
    {
        Sound.PlaySound("smb_coin");
        PFunc.Log("OnClickResolution", width, height);
        Screen.SetResolution( width,height, FullScreenMode.Windowed);
    }

    void OnClickChangeLV()
    {
        Sound.PlaySound("smb_coin");
        GameModController.Instance.OnLoadScene(gameLevel);
    }

    void OnClickChangeMD()
    {
        Sound.PlaySound("smb_coin");
        ModData.mTrap = gameMode ==1;
    }

    void OnClickMoRen()
    {
        musicValue = 1;
        soundValue = 1;
        sl_music.value = musicValue;
        sl_sound.value = soundValue;
        Sound.OnSetVolume(musicValue, soundValue);
        width = 1280;
        height = 960;
        ModData.mTrap = false;
        Screen.SetResolution(width, height, FullScreenMode.Windowed);
        Sound.PlaySound("smb_coin");
    }

    // 更新后处理效果的实际参数
    private void UpdateSaturation(float value)
    {
        if (colorAdjustments != null)
        {
            // 将0-1的Slider值映射到-100到100的范围
            // 当value=0.5时，saturationValue=0
            // 当value=1时，saturationValue=100
            // 当value=0时，saturationValue=-100
            float saturationValue = (value - 0.5f) * 200f;

            saturationValue = Mathf.Clamp(saturationValue, -100f, 100f);

            colorAdjustments.saturation.Override(saturationValue);
            PlayerPrefs.SetFloat("Saturation", value);
        }
    }
    void OnClose()
    {
        center.SetActive(false);
    }
}
