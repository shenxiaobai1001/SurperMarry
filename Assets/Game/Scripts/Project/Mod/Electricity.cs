using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : MonoBehaviour
{
    public List<GameObject> gameObjects;
    private void Start()
    {

    }
    public void OnStartLazzer()
    {
        int value = UnityEngine.Random.Range(0, gameObjects.Count);
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(value == i);
        }
        EventManager.Instance.SendMessage(Events.OnLazzerHit);
    }
}
