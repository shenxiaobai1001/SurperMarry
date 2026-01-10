using PlayerScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjFllow : MonoBehaviour
{
    [Header("跟随设置")]
    public Transform target;              // 要跟随的目标物体
    public Vector3 offset = new Vector3(0f, 0, 0); // 相对偏移量
    public float followSpeed = 5f;       // 跟随速度
    public bool smoothFollow = true;     // 是否平滑跟随
    public bool flowPlayer=true;

    void LateUpdate()
    {
        if (target == null)
        {
            if(flowPlayer)
            {
                if (PlayerController.Instance != null)
                    target = PlayerController.Instance.transform;
            }
            else
            {
                if (Camera.main != null)
                    target = Camera.main.transform;
            }
        }
        if (target == null)
        {
            return;
        }
        // 计算目标位置（考虑目标的旋转）
        Vector3 targetPosition = target.position + offset;
         targetPosition = new Vector3(targetPosition.x, targetPosition.y, 0);
        // 移动跟随物体
        if (smoothFollow)
        {
            // 平滑跟随
            transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
        }
        else
        {
            // 直接跟随
            transform.position = targetPosition;
        }
    }
    private void OnDestroy()
    {
        target = null;
    }
}
