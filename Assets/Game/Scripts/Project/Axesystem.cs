using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class Axesystem : MonoBehaviour
{
    public ChainBridge bridge;
    bool check = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;

        if (collision.tag.Contains("Player")&& check)
        {
            Sound.PauseOrPlayVolumeMusic(true);
            Config.isLoading = true;
            ModController.Instance.OnModPause();
            PlayerController.Instance.isHit = true;
            check = false;
            bridge.OnMinBrige();
            PlayerController.Instance._playerRb.velocity = Vector2.zero;
            PlayerModController.Instance.animator.SetFloat("Speed_f", 0);
            //PlayerModController.Instance.OnChangeState(false);
            OnEnterNextLevel();
        }
    }

    void OnEnterNextLevel()
    {
        StartCoroutine(NextLevel());
    }
    private static IEnumerator NextLevel()
    {
        yield return new WaitForSeconds(5);
        PlayerController.Instance.OnToFindNpc();
        yield return new WaitForSeconds(3);
        Config.passIndex++;
        if (Config.passIndex >= Config.passName.Length)
        {
            GameModController.Instance.OnLoadScene("StartingScene");
        }
        else
        {
            string name = Config.passName[Config.passIndex];
            GameModController.Instance.OnLoadScene(name);
        }
    }
}
