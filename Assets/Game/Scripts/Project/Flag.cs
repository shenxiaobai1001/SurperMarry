using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject pole;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(Events.ToGetPoleFlag, OnGetPole);
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
        if (transform.position.y >= 1.5f&& down)
        {
            transform.Translate(5 * Time.deltaTime * Vector3.down);
        }
    }

    bool down = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision == null) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager.Instance.SendMessage(Events.ToGetPoleFlagPlayer, pole);
            down = true;
        }
    }

}
