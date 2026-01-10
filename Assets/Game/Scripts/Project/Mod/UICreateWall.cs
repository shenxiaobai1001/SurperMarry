using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UICreateWall : MonoBehaviour, IPointerClickHandler
{
    public GameObject NormalWall;
    public GameObject Stone;
    public GameObject uiNormalWall; // UI图标，会跟随鼠标移动
    public GameObject uiStone; // UI图标，会跟随鼠标移动
    public GameObject createParent;
    public GameObject center;
    public Text tx_WallCount;
    public Text ty_StoneCount;

    [Header("创建参数")]
    public float zPosition = 90f; // 生成的物体Z轴位置
    public float createInterval = 0.2f; // 创建间隔时间，避免创建过快
    public LayerMask checkLayer; // 检测碰撞的层级

    // 是否正在创建状态
    private bool isCreating = false;
    // 原始UI位置
    private Vector3 originalUIPosition;
    // 原始父物体
    private Transform originalParent;
    // 原始层级
    private int originalSiblingIndex;
    // 上次创建时间
    private float lastCreateTime = 0f;

    GameObject uiObj;
    bool createWall = false;
    bool createStone=false;
    // 用于对齐的网格大小
    private float gridSize = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        uiNormalWall.Click(OnCreateWall,0);
        uiStone.Click(OnCreateStone,0);
    }

    void OnCreateWall( )
    {
        if (isCreating) return; // 已经在创建状态则忽略
        isCreating = true;
        originalUIPosition = uiNormalWall.transform.position;
        originalParent = uiNormalWall.transform.parent;
        originalSiblingIndex = uiNormalWall.transform.GetSiblingIndex();
        // 进入创建状态
        CreateWallManager.Instance.isCreate = true;
        CreateWallManager.Instance.createObj = NormalWall;

        // 将UI图标移到顶层并开始跟随鼠标
        uiNormalWall.transform.SetAsLastSibling();
        uiNormalWall.GetComponent<Button>().enabled = false;
        uiObj = uiNormalWall;
        createWall = true;
        Debug.Log("开始创建墙壁，按住左键连续创建，右键取消");
    }

    void OnCreateStone()
    {
        if (isCreating) return; // 已经在创建状态则忽略
        isCreating = true;
        originalUIPosition = uiStone.transform.position;
        originalParent = uiStone.transform.parent;
        originalSiblingIndex = uiStone.transform.GetSiblingIndex();
        // 进入创建状态
        CreateWallManager.Instance.isCreate = true;
        CreateWallManager.Instance.createObj = Stone;

        // 将UI图标移到顶层并开始跟随鼠标
        uiStone.transform.SetAsLastSibling();
        uiStone.GetComponent<Button>().enabled = false;
        uiObj = uiStone;
        createStone = true;
        Debug.Log("开始创建墙壁，按住左键连续创建，右键取消");
    }

    void Update()
    {
        if (center) center.SetActive(CreateWallManager.Instance.wallCount > 0 || CreateWallManager.Instance.stonesCount > 0);
        
        if (isCreating)
        {
            // 更新UI图标位置，使其跟随鼠标
            UpdateUIPosition();

            // 检测鼠标左键按住，可以连续创建
            if (Input.GetMouseButton(0))
            {

                TryCreateWall();

            }

            // 鼠标右键点击取消创建
            if (Input.GetMouseButtonDown(1))
            {
                CancelCreation();
            }

            // ESC键也可以取消创建
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CancelCreation();
            }
        }
        OnSetText();
    }

    void UpdateUIPosition()
    {
        if (uiObj == null) return;

        // 获取鼠标位置
        Vector3 mousePos = Input.mousePosition;

        // 直接将UI图标移到鼠标位置
        uiObj.transform.position = mousePos;
    }

    void TryCreateWall()
    {
        if (CreateWallManager.Instance.createObj == null) return;
        if(CreateWallManager.Instance.wallCount<=0&& createWall) return;
        if (CreateWallManager.Instance.stonesCount <= 0 && createStone) return;
        // 获取鼠标在世界坐标中的位置
        Vector3 worldPosition = GetMouseWorldPosition();
        if (IsPositionOccupied(worldPosition)) return;
        // 对齐到网格
        Vector3 alignedPosition = AlignToGrid(worldPosition);
        alignedPosition.z = zPosition;
        GameObject newWall = SimplePool.Spawn(CreateWallManager.Instance.createObj, alignedPosition, Quaternion.identity);
        newWall.transform.SetParent(createParent.transform); 
        if (createWall)
        {
            newWall.GetComponent<BreakBrickController>().toPool = true;
            CreateWallManager.Instance.wallCount--;
        }
        if (createStone)
            CreateWallManager.Instance.stonesCount--;
    }

    bool IsPositionOccupied(Vector3 position)
    {
        // 创建一个稍微小一点的检测范围，避免检测到自身边缘
        float checkRadius = 0.5f;
        // 2D检测
        Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(position.x, position.y), checkRadius, checkLayer);

        return colliders.Length > 0;
    }

    void CancelCreation()
    {
        // 恢复UI
        ResetUI();

        // 退出创建状态
        isCreating = false;
        CreateWallManager.Instance.isCreate = false;
        if (uiObj)
        {
            uiObj.GetComponent<Button>().enabled = true;
            uiObj = null;
        }
       
        createWall = false;
        createStone=false;
    }

    void ResetUI()
    {
        if (uiObj == null) return;

        // 恢复UI的原始位置、父物体和层级
        uiObj.transform.position = originalUIPosition;
        uiObj.transform.SetParent(originalParent);
        uiObj.transform.SetSiblingIndex(originalSiblingIndex);
    }

    Vector3 GetMouseWorldPosition()
    {
        // 获取鼠标在屏幕上的位置
        Vector3 mousePos = Input.mousePosition;

        // 如果是正交相机
        if (Camera.main.orthographic)
        {
            // 将屏幕坐标转换为世界坐标
            mousePos.z = 10f; // 相机到物体的距离
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

            // 由于是2D，我们只需要X和Y坐标
            return new Vector3(worldPos.x, worldPos.y, 0);
        }
        else
        {
            // 对于透视相机，使用射线
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            // 假设地面在Z=zPosition平面
            Plane groundPlane = new Plane(Vector3.forward, new Vector3(0, 0, zPosition));

            if (groundPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            // 如果没有命中，返回默认值
            return Vector3.zero;
        }
    }

    Vector3 AlignToGrid(Vector3 position)
    {
        // 将位置对齐到最近的0.5倍数
        float alignedX = Mathf.Round(position.x / gridSize) * gridSize;
        float alignedY = Mathf.Round(position.y / gridSize) * gridSize;

        return new Vector3(alignedX, alignedY, position.z);
    }

    void OnSetText()
    {
        tx_WallCount.text = $"X{CreateWallManager.Instance.wallCount}";
        ty_StoneCount.text = $"X{CreateWallManager.Instance.stonesCount}";
    }
    // 实现IPointerClickHandler接口
    public void OnPointerClick(PointerEventData eventData)
    {
        // 这个方法可以留空，因为我们已经在Start中设置了事件监听
    }

    // 当脚本被禁用或销毁时
    void OnDisable()
    {
        CancelCreation();
    }

    void OnDestroy()
    {

        // 取消创建
        CancelCreation();
    }

    // 在编辑器中可视化检测范围
#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (isCreating)
        {
            // 获取鼠标在世界坐标中的位置
            Vector3 worldPosition = GetMouseWorldPosition();

            // 对齐到网格
            Vector3 alignedPosition = AlignToGrid(worldPosition);
            alignedPosition.z = zPosition;

            // 绘制检测范围
            Gizmos.color = IsPositionOccupied(alignedPosition) ? Color.red : Color.green;
            Gizmos.DrawWireSphere(alignedPosition, 0.1f);
        }
    }
#endif
}