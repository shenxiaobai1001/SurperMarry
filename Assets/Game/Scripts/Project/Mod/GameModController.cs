using PlayerScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SystemScripts;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        passDistance.Add("1-1",192.5f);
        passDistance.Add("1-2", 155);
        passDistance.Add("1-3", 151.2f);
        passDistance.Add("1-4", 122);
        passDistance.Add("2-1", 132.7f);
        passDistance.Add("2-2", 271.2f);
        passDistance.Add("2-3", 127);
        passDistance.Add("3-1", 169.8f);
        passDistance.Add("3-2", 193.7f);
        passDistance.Add("3-3", 147.7f);
        passDistance.Add("3-4", 132);
        passDistance.Add("4-1", 223);
        passDistance.Add("4-2", 151); 
        passDistance.Add("4-3", 143.2f);
        passDistance.Add("4-4", 163);
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
    public void OnRandromPass()
    {
        Sound.PlaySound("smb_1-up");
        int value =UnityEngine.Random.Range(0, Config.passName.Length);

        string name = Config.passName[value];
        if (mainMoveCoroutine==null)
        {
            ModController.Instance.OnModPause();
            Config.passIndex = value;
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }
    public void OnEnterNextPass(int value)
    {
        Config.passIndex += value;
        if (Config.passIndex < 0)
        {
            Config.passIndex = 0;
            return;
        }
        else if (Config.passIndex >= Config.passName.Length)
        {
            Config.passIndex = Config.passName.Length-1;
            return;
        }
        string name = Config.passName[value];
        if (mainMoveCoroutine == null)
        {
            ModController.Instance.OnModPause();
            Config.passIndex = value;
            mainMoveCoroutine = StartCoroutine(OnLoadScence(name));
        }
    }

    public void OnLoadScene(string name)
    {
        Config.passIndex = Array.IndexOf(Config.passName, name);
        if (mainMoveCoroutine == null)
        {
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
        mainMoveCoroutine = null;
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
   
    }
}
