using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.DebugUI;

public class GameModController : MonoBehaviour
{
    public static GameModController Instance;
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

    Dictionary<string,float> passDistance=new Dictionary<string, float>();

    public GameStatusController gameStatus;
    public string nowPos = "";

    // Start is called before the first frame update
    void Start()
    {
        OnInitDistance();
    }
    void OnInitDistance()
    {
        if(passDistance==null) passDistance = new Dictionary<string, float>();
        passDistance.Add("1-1", 206);
        passDistance.Add("1-2", 160);
        passDistance.Add("1-3", 202);
        passDistance.Add("1-4", 203);
        passDistance.Add("2-1", 185);
        passDistance.Add("2-2", 325);
        passDistance.Add("2-3", 211);
        passDistance.Add("3-1", 211);
        passDistance.Add("3-2", 266);
        passDistance.Add("3-3", 211);
        passDistance.Add("3-4", 226);
        passDistance.Add("4-1", 371f);
        passDistance.Add("4-2", 235.5f); 
        passDistance.Add("4-3", 268);
        passDistance.Add("4-4", 274.5f);
    }
    public float OnGetLevelEndPos()
    {
        if(passDistance.ContainsKey(nowPos))
        return passDistance[nowPos]-8.5f;
        return 0f;
    }
    public bool  OnCheckBoosLevel()
    {
        return nowPos=="1-4" || nowPos == "2-3" || nowPos == "3-4" || nowPos == "4-4";
    }
    public void OnRandromPlayerPos()
    {
        Sound.PlaySound("smb_1-up");
        float x = passDistance[nowPos];
        float randX= UnityEngine.Random.Range(2,x);
        float randY = UnityEngine.Random.Range(0, 6);
        PlayerController.Instance.transform.position = new Vector3(randX, randY, 90);
    }
    private Coroutine mainMoveCoroutine;
    int barrageIndex = 0;
    public void OnRandromPass(int index)
    {
        Sound.PlaySound("smb_1-up");
        int value =UnityEngine.Random.Range(0, Config.passName.Length);

        string name = Config.passName[value];
        if (mainMoveCoroutine==null)
        {
            barrageIndex = index;
            ModController.Instance.OnModPause();
            Config.passIndex = value;
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }
    public void OnEnterNextPass(int value, int index)
    {
        if (Config.passIndex <=0&& value==-1)
        {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
            return;
        }
        if (Config.passIndex >= Config.passName.Length && value == 1)
        {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, index);
            return;
        }

        if (mainMoveCoroutine == null)
        {
            Sound.PlaySound("smb_1-up");
            PFunc.Log("1OnEnterNextPass", Config.passIndex, value);
            Config.passIndex += value;
            PFunc.Log("2OnEnterNextPass",Config.passIndex);
            if (Config.passIndex < 0)
            {
                Config.passIndex = 0;
            }
            else if (Config.passIndex >= Config.passName.Length)
            {
                Config.passIndex = Config.passName.Length - 1;
            }
            barrageIndex = index;
            string name = Config.passName[Config.passIndex];
            ModController.Instance.OnModPause();
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }

    public void OnLoadScene(string name, int index=0)
    {
        Config.passIndex = Array.IndexOf(Config.passName, name);
        if (mainMoveCoroutine == null)
        {
            barrageIndex = index;
            ModController.Instance.OnModPause();
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }
    public void OnLoadTargetScene(string name)
    {
        if (name == "LoadingScene")
        {
           if(gameStatus) gameStatus.LiveStart.SetActive(true);
        }
        if (mainMoveCoroutine == null)
        {
            ModController.Instance.OnModPause();
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }

    public void OnBeginLoading()
    {
 
    }

    IEnumerator OnLoadScence(string name)
    {
        gameStatus.NpcTalk.SetActive(false);
        Sound.PauseOrPlayVolumeMusic(true);
        yield return Loaded.OnLoadScence(name);
        yield return new WaitForSeconds(1);
        nowPos = name;
        //PlayerController.Instance.transform.position = new Vector3(-2, 0);
        Camera.main.transform.position = new Vector3(1.5f, 5,-10);
        Scene scene = SceneManager.GetActiveScene();
        if (Config.passName.Contains(scene.name))
        {
            Config.isLoading = false;
            Sound.PlayMusic("background");
            if (PlayerModMoveController.Instance != null)
                PlayerModMoveController.Instance.OnSetMinValue(-5.5f, passDistance[nowPos]);
            //PlayerController.Instance.ResetPlayerState();
        }
        if (scene.name == "LoadingScene")
        {
            yield return new WaitForSeconds(1);
            gameStatus.LiveStart.SetActive(false);
            OnLoadScene("1-1");
        }
        mainMoveCoroutine = null;
        if (barrageIndex != 0) {
            EventManager.Instance.SendMessage(Events.OnBarryExecutEnd, barrageIndex);
            barrageIndex = 0;
        }
    }
}
