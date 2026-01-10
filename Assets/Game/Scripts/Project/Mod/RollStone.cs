using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class RollStone : MonoBehaviour
{
    public GameObject[] moveObj;
    public GameObject center;
    public GameObject rayCheck;
    public string sound;

    float movePos;
    float startPos;
    float endPos = 120;
    private Coroutine mainMoveCoroutine;
    public float moveSpeed = 3f; // 水平移动速度
    public float climbSpeed = 2f; // 上移速度
    public LayerMask groundLayer; // 障碍物所在的层（如Ground层）

    [Header("射线检测")]
    public float raycastDistance = 0.51f; // 向前检测的距离
    public Vector2 raycastOffset = new Vector2(0.5f, 0); // 射线起点的偏移

    private bool isClimbing = false;
    private float targetClimbHeight = 0f;
    int currentRockIndex;

    // 添加一个控制变量
    private bool isWaitingForClimb = false;
    private void Start()
    {
       // OnBeginShow();
    }
    public void OnBeginShow( )
    {
        isClimbing = false;
        isWaitingForClimb = false;
        transform.localPosition = new Vector3(0.5f, -0.25f);
        endPos = transform.position.x + 25;
        mainMoveCoroutine= StartCoroutine(OnStartMoveStone());
    }

    IEnumerator OnStartMoveStone()
    {
        while (movePos < endPos)
        {
            movePos = transform.position.x;
            PFunc.Log("OnStartMoveStone", movePos, isClimbing);
        
            // 如果不是在爬升等待状态，才执行石头切换和移动
            if (!isWaitingForClimb)
            {
                moveObj[currentRockIndex].SetActive(false);
                currentRockIndex = (currentRockIndex + 1) % moveObj.Length;
                moveObj[currentRockIndex].SetActive(true);
                GameObject go = moveObj[currentRockIndex];
                go.transform.position = new Vector3(transform.position.x, transform.position.y - 1, 93);

                Sound.PlaySound($"Mod/{sound}");
                // 等待石头动画完成
                go.transform.DOMove(new Vector3(go.transform.position.x, transform.position.y, 93), 0.2f)
                    .SetLoops(2, LoopType.Yoyo)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        go.transform.position = new Vector3(go.transform.position.x, 0, 93);
                    });

        
                transform.position += new Vector3(1, -0.25f);
            }
            yield return new WaitForSeconds(0.3f);
            // 2. 进行射线检测，判断前方是否有障碍物
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
            Debug.DrawRay(transform.position, Vector2.down * raycastDistance, Color.red); // 在Scene视图中绘制射线

            PFunc.Log("进行射线检测", hit.collider, isClimbing);
            if (hit.collider != null )
            {

                if (!isClimbing && !isWaitingForClimb)
                {
                    // 3. 如果检测到障碍物，且不在爬升状态，则开始爬升
                    isClimbing = true;
                    isWaitingForClimb = true; // 设置等待爬升标志

                    // 等待爬升完成
                    yield return StartCoroutine(OnMoveUp());

                    isWaitingForClimb = false; // 爬升完成，清除等待标志
                }
            }
            else
            {
                if (!isClimbing && !isWaitingForClimb &&transform.position.y > 0)
                {
                    // 3. 如果检测到障碍物，且不在爬升状态，则开始爬升
                    isClimbing = true;
                    isWaitingForClimb = true; // 设置等待爬升标志

                    // 等待爬升完成
                    yield return StartCoroutine(OnMoveDown());

                    isWaitingForClimb = false; // 爬升完成，清除等待标志
                }
            }

        }
        ReadyDestory();
    }

    IEnumerator OnMoveUp()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        PFunc.Log("OnMoveUp", hit.collider, isClimbing);

        // 持续向上移动，直到不再检测到障碍物
        while (hit.collider != null)
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
            PFunc.Log("OnMoveUp", hit.collider, isClimbing);
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }
        

        isClimbing = false;
    }
    IEnumerator OnMoveDown()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
        PFunc.Log("OnMoveUp", hit.collider, isClimbing);

        // 持续向下移动，直到检测到障碍物
        while (hit.collider == null)
        {
            hit = Physics2D.Raycast(transform.position, Vector2.down, raycastDistance, groundLayer);
            PFunc.Log("OnMoveUp", hit.collider, isClimbing);
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
            yield return null;
        }

        isClimbing = false;
    }

    void ReadyDestory() {
        if (mainMoveCoroutine != null)
        {
            StopCoroutine(mainMoveCoroutine);
        }

        SimplePool.Despawn(transform.parent.gameObject);
    }
}