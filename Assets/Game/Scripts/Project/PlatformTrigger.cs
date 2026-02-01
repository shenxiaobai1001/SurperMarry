using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    public LinePartform cableCar;
    public bool isLeftPlatform = true;



    private void Update()
    {
        if (transform.position.y <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // 确保马里奥有"Player"标签
        {
            if (isLeftPlatform)
                cableCar.OnMarioEnterLeftPlatform();
            else
                cableCar.OnMarioEnterRightPlatform();
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (isLeftPlatform)
                cableCar.OnMarioExitLeftPlatform();
            else
                cableCar.OnMarioExitRightPlatform();
        }
    }

}