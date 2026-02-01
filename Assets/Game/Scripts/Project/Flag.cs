using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject pole;
    public GameObject endPos;
    bool isToEnd = false;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(Events.ToGetPoleFlag, OnGetPole);
        if(endPos) endPos.SetActive(false);
    }
    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(Events.ToGetPoleFlag, OnGetPole);
    }
    void OnGetPole(object msg)
    {
        down = true;
    }

    private void Update()
    {
        if (transform.position.y >= 1&& down)
        {
            transform.Translate(5 * Time.deltaTime * Vector3.down);
        }
        else if ( transform.position.y <= 1 &&!isToEnd)
        {
            isToEnd = true;
            if (endPos) endPos.SetActive(true);
        }
    }

    bool down = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag("ModCollider"))
        {
            EventManager.Instance.SendMessage(Events.ToGetPoleFlagPlayer, pole);
            down = true;
        }
    }

}
