using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManyArrow : MonoBehaviour
{
    public List<GameObject> listSprites = new List<GameObject>();
    public float inver = 1;
    int targetCount = 16;
    int count = 0;

    private void OnEnable()
    {
        count = 0;
        StartCoroutine(OnRandeArrowUp());
    }

    IEnumerator OnRandeArrowUp()
    {
        // 记录总共执行的轮数
        int totalRounds = 2;

        for (int round = 0; round < totalRounds; round++)
        {
            // 创建当前轮次的可用列表
            List<GameObject> availableSprites = new List<GameObject>(listSprites);

            // 随机执行当前轮次的所有精灵
            while (availableSprites.Count > 0 && count < listSprites.Count)
            {
                // 从可用列表中随机选择一个
                int randomIndex = Random.Range(0, availableSprites.Count);
                GameObject selectedSprite = availableSprites[randomIndex];

                Sound.PlaySound("Mod/arrow");
                // 执行动画
                ExecuteArrowAnimation(selectedSprite, round);

                // 从可用列表中移除已执行的精灵
                availableSprites.RemoveAt(randomIndex);

                count++;

                // 等待间隔时间
                yield return new WaitForSeconds(inver);
            }

            // 如果已达到目标次数，提前退出
            if (count >= targetCount)
            {
                break;
            }

            // 一轮结束后等待一下（如果需要的话）
            yield return new WaitForSeconds(inver * 0.5f);
        }

        // 等待所有动画完成后再回收
        yield return new WaitForSeconds(1f);
        SimplePool.Despawn(gameObject);
    }

    private void ExecuteArrowAnimation(GameObject arrow, int round)
    {
        Transform trans = arrow.transform;

        // 保存初始位置
        Vector3 originalPos = trans.localPosition;

        // 执行动画
        trans.DOLocalMove(new Vector3(trans.localPosition.x, 11), 0.5f)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                // 动画完成后重置到原始位置
                trans.localPosition = originalPos;
            });
    }
}