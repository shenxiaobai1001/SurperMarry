using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperManEffect : MonoBehaviour
{
    public  SpriteRenderer spriteRenderer;
    private void OnEnable()
    {
        spriteRenderer.color = Color.white;
        spriteRenderer.DOFade(0, 0.5f).onComplete += () => { SimplePool.Despawn(gameObject); };
    }
}
