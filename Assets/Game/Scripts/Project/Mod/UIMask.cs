using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMask : MonoBehaviour
{
    public static UIMask Instance;
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
    public GameObject Mask;
    float maskTime = 0;


    public void OnCloseLight()
    {
        maskTime += 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (maskTime > 0)
        {
            maskTime-=Time.deltaTime;
        }
        if(Mask) Mask.SetActive(maskTime > 0);
    }
}
