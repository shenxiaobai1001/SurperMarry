using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boom : MonoBehaviour
{
    public ParticleSystem particleSystem;


    private void OnEnable()
    {
        if (particleSystem)
        {
            particleSystem.Play();
        }
        else
        {
            particleSystem=transform.GetChild(0).GetComponent<ParticleSystem>();
            if (particleSystem)
            {
                particleSystem.Play();
            }
        }
    }
}
