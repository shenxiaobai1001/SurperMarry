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
        if (ItemCreater.Instance.lockPlayer && UIChain.Instance != null && UIChain.Instance.gameObject.activeSelf)
        {
            ChainPlayer.Instance.transform.DOShakePosition(0.5f, 0.2f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() =>
        {
            ChainPlayer.Instance.transform.position = new Vector3(Camera.main.transform.position.x, 5, 0);
        });}
        int value = UnityEngine.Random.Range(0, gameObjects.Count);
        for (int i = 0; i < gameObjects.Count; i++)
        {
            gameObjects[i].gameObject.SetActive(value == i);
        }
        EventManager.Instance.SendMessage(Events.OnLazzerHit);
    }
}
