using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoguManager : MonoBehaviour
{
    public static MoguManager Instance;
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


}
