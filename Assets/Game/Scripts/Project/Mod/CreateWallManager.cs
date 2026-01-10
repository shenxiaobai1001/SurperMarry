using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateWallManager : Singleton<CreateWallManager>
{
    public bool isCreate;
    public GameObject createObj;

    public int wallCount = 0;
    public int stonesCount = 0;
}
