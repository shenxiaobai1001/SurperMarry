using UnityEngine;

public class RollingRockController : MonoBehaviour
{
    public RollStone rollStone;

    public void OnBeginShow( )
    {
        rollStone.OnBeginShow();
    }

}