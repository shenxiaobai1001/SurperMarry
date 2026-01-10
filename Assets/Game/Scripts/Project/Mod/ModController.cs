using System.Collections;
using System.Collections.Generic;
using SystemScripts;
using UnityEngine;

public class ModController : MonoBehaviour
{
    public static ModController Instance;
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

    public Transform MonsterParent;
    public Transform itemParent;
    public Transform createParent;
    public GameStatusController statusController;
    public UISystem uISystem;
    public UIPrankContro uPrankContro;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
        BarrageController.Instance.LoadDataFromJson();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uISystem.center.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            uPrankContro.ChangePrank();
        }
    }
    public IEnumerator OnDequeueObjs()
    {
        if (MonsterParent.childCount > 0)
        {
            for (int i = 0; i < MonsterParent.childCount; i++) {
                GameObject GO = MonsterParent.GetChild(i).gameObject;
                if (GO.activeSelf)
                {
                    SimplePool.Despawn(GO);
                }
                yield return null;
            }
        }
        if (itemParent.childCount > 0)
        {
            for (int i = 0; i < itemParent.childCount; i++)
            {
                GameObject GO = itemParent.GetChild(i).gameObject;
                if (GO.activeSelf)
                {
                    SimplePool.Despawn(GO);
                }
                yield return null;
            }
        }
        if (createParent.childCount > 0)
        {
            for (int i = 0; i < createParent.childCount; i++)
            {
                GameObject GO = createParent.GetChild(i).gameObject;
                if (GO.activeSelf)
                {
                    SimplePool.Despawn(GO);
                }
                yield return null;
            }
        }
    }

    public void OnModPause()
    {
        Config.isLoading = true;
        StartCoroutine(OnDequeueObjs());
    }
}
