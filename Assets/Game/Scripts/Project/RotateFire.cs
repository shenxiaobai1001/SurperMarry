using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateFire : MonoBehaviour
{
    public Transform rotateObj;
    public int zValue;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotateObj.Rotate(new Vector3(0,0, zValue) * 90 * Time.deltaTime);
    }
}
