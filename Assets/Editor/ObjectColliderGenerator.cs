using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ObjectColliderGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public List<GameObject> colliderPrefabs;   // 预制体集合
    public int spawnCount = 5;                  // 生成数量
    public Vector3 spawnDirection = Vector3.right; // 生成方向
    public float spacing = 1.0f;               // 物体间距

    [Header("旋转设置")]
    public bool randomRotation = false;         // 是否随机旋转
    public Vector3 minRotation = Vector3.zero; // 最小旋转角度
    public Vector3 maxRotation = new Vector3(0, 360, 0); // 最大旋转角度

    [Header("调试按钮")]
    [Space(10)]
    [Tooltip("点击生成物体")]
    public bool generateObjects = false;
    [Tooltip("点击清除生成的物体")]
    public bool clearObjects = false;

    private List<GameObject> spawnedObjects = new List<GameObject>(); // 存储已生成的物体
    private int generationCount = 0; // 生成次数计数器

    void Update()
    {
        // 只在编辑器模式下工作，且不在播放模式下
        if (!Application.isPlaying)
        {
            if (generateObjects)
            {
                generateObjects = false; // 重置按钮状态
                Generate();
            }

            if (clearObjects)
            {
                clearObjects = false; // 重置按钮状态
                ClearAll();
            }
        }
    }

    void Generate()
    {
        // 先清除已存在的物体
        ClearAll();

        if (colliderPrefabs == null || colliderPrefabs.Count == 0)
        {
            Debug.LogWarning("预制体集合为空！");
            return;
        }

        for (int i = 0; i < spawnCount; i++)
        {
            // 计算位置
            Vector3 spawnPosition = transform.position + spawnDirection.normalized * (spacing * i);

            // 选择预制体
            GameObject prefabToSpawn = GetPrefabToSpawn(i);

            if (prefabToSpawn == null)
            {
                Debug.LogWarning($"预制体为空，跳过生成第 {i} 个物体");
                continue;
            }

            // 生成物体
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = this.transform; // 设为子物体

            // 应用旋转（如果是第一次生成后）
            ApplyRotation(newObj);

            spawnedObjects.Add(newObj);

            // 给物体命名
            newObj.name = $"{prefabToSpawn.name}_{i}";
        }

        generationCount++;
        Debug.Log($"第 {generationCount} 次生成完成，共生成 {spawnedObjects.Count} 个物体");
    }

    GameObject GetPrefabToSpawn(int index)
    {
        // 第一次生成：按顺序选择预制体
        if (generationCount == 0)
        {
            if (index < colliderPrefabs.Count)
            {
                return colliderPrefabs[index];
            }
            else
            {
                // 如果数量超过预制体数量，循环使用
                return colliderPrefabs[index % colliderPrefabs.Count];
            }
        }
        // 后续生成：随机选择预制体
        else
        {
            if (colliderPrefabs.Count > 0)
            {
                return colliderPrefabs[Random.Range(0, colliderPrefabs.Count)];
            }
            return null;
        }
    }

    void ApplyRotation(GameObject obj)
    {
        // 只有在非第一次生成且启用随机旋转时才应用旋转
        if (generationCount > 0 && randomRotation)
        {
            Vector3 randomRotation = new Vector3(
                Random.Range(minRotation.x, maxRotation.x),
                Random.Range(minRotation.y, maxRotation.y),
                Random.Range(minRotation.z, maxRotation.z)
            );
            obj.transform.rotation = Quaternion.Euler(randomRotation);
        }
        else
        {
            // 保持原始旋转
            obj.transform.rotation = Quaternion.identity;
        }
    }

    void ClearAll()
    {
        // 清除已生成的物体
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        spawnedObjects.Clear();

        // 清除所有子物体（防止漏网之鱼）
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        Debug.Log("已清除所有生成的物体");
    }

    // 在场景视图中绘制Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            // 绘制生成位置
            Gizmos.color = Color.green;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition = transform.position + spawnDirection.normalized * (spacing * i);
                Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.5f);
            }

            // 绘制方向箭头
            Gizmos.color = Color.red;
            Vector3 endPoint = transform.position + spawnDirection.normalized * spacing * spawnCount;
            Gizmos.DrawLine(transform.position, endPoint);

            // 绘制箭头头部
            DrawArrow(transform.position, endPoint, 0.3f);
        }
    }

    // 绘制箭头辅助方法
    void DrawArrow(Vector3 start, Vector3 end, float arrowHeadLength = 0.25f)
    {
        Gizmos.DrawLine(start, end);

        Vector3 direction = (end - start).normalized;
        Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + 30, 0) * Vector3.forward;
        Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - 30, 0) * Vector3.forward;

        Gizmos.DrawRay(end, right * arrowHeadLength);
        Gizmos.DrawRay(end, left * arrowHeadLength);
    }

    // 编辑器扩展方法
#if UNITY_EDITOR
    [ContextMenu("强制生成")]
    void ForceGenerate()
    {
        Generate();
    }

    [ContextMenu("强制清除")]
    void ForceClear()
    {
        ClearAll();
    }
#endif
}