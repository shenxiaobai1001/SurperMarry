using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIFlog : MonoBehaviour
{
    public static UIFlog Instance;
    void Awake()
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

    public GameObject Center;
    public List<GameObject> gameObjects;
    public Text tx_number;
    // Start is called before the first frame update
    void Start()
    {
        Center.SetActive(false);
    }
    public void OnStartMove()
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(false);
        }
        Center.SetActive(true);
        FlogPlayer.Instance.OnStartHit();
    }
   
    float changTime = 0;
    float allChangeTime = 1;
    // Update is called once per frame
    void Update()
    {
        if (Config.FlogCount <= 0)
        {
            changTime = 0;
            Center.SetActive(false);
            return;
        }

        changTime += Time.deltaTime;

        if (changTime>= allChangeTime)
        {
            OnShowOneObj(Random.Range(0, gameObjects.Count));
            changTime = 0;
        }

        tx_number.text = $"{Config.FlogCount}";
    }

    void OnShowOneObj(int index)
    {
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(index==i);
        }
    }
}
