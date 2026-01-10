using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flag : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
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
            down = true;
        }
    }

}
