using UnityEngine;
using UnityEditor;

[ExecuteInEditMode] // 允许在编辑器模式下执行
public class ObjectGenerator : MonoBehaviour
{
    [Header("生成设置")]
    public GameObject prefabToSpawn;   // 要生成的预制体
    public int spawnCount = 5;         // 生成数量
    public Vector3 spawnDirection = Vector3.right; // 生成方向
    public float spacing = 1.0f;       // 物体间距

    [Header("调试按钮")]
    [Space(10)]
    [Tooltip("点击生成物体")]
    public bool generateObjects = false;
    [Tooltip("点击清除生成的物体")]
    public bool clearObjects = false;

    private GameObject[] spawnedObjects; // 存储已生成的物体

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

        if (prefabToSpawn == null)
        {
            Debug.LogWarning("没有指定要生成的预制体！");
            return;
        }

        spawnedObjects = new GameObject[spawnCount];

        for (int i = 0; i < spawnCount; i++)
        {
            // 计算位置
            Vector3 spawnPosition = transform.position + spawnDirection.normalized * (spacing * i);

            // 生成物体
            GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(prefabToSpawn);
            newObj.transform.position = spawnPosition;
            newObj.transform.parent = this.transform; // 设为子物体

            spawnedObjects[i] = newObj;

            // 给物体命名
            newObj.name = prefabToSpawn.name + "_" + i;
        }

        Debug.Log($"已生成 {spawnCount} 个物体");
    }

    void ClearAll()
    {
        if (spawnedObjects != null)
        {
            foreach (GameObject obj in spawnedObjects)
            {
                if (obj != null)
                {
                    DestroyImmediate(obj);
                }
            }
        }

        // 清除所有子物体（防止漏网之鱼）
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }

        spawnedObjects = null;
        Debug.Log("已清除所有生成的物体");
    }

    // 在场景视图中绘制Gizmos
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < spawnCount; i++)
            {
                Vector3 spawnPosition = transform.position + spawnDirection.normalized * (spacing * i);
                Gizmos.DrawWireCube(spawnPosition, Vector3.one * 0.5f);
            }

            // 绘制方向箭头
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + spawnDirection.normalized * spacing * spawnCount);
            Gizmos.DrawSphere(transform.position + spawnDirection.normalized * spacing * spawnCount, 0.1f);
        }
    }
}