using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovePartform : MonoBehaviour
{
    public Vector3 targetPos;
    public float moveTime;
    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(targetPos, moveTime).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo); // ÎÞÏÞÑ­»·
    }

}
