using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBreak : MonoBehaviour
{
    public List<Rigidbody2D> rigidbody2Ds;
    public GameObject aniObj;
    public GameObject chipObj;
    public Animator animator;
    Vector3 leftForce = new Vector3(-5, 0);
    Vector3 rightForce = new Vector3(5, 0);

    public IEnumerator OnAddAllForceIE()
    {
        if (chipObj) chipObj.SetActive(true);
        if (rigidbody2Ds == null|| rigidbody2Ds.Count<=0) yield break;
        for (int i = 0; i < rigidbody2Ds.Count; i++) {
            int index = i;
            bool left = index % 2 == 0;
            Vector3 force = left ? leftForce : rightForce;
            rigidbody2Ds[i].AddForce(force,ForceMode2D.Impulse);
            yield return null;
        }
    }
    public IEnumerator OnHuifuIE()
    {
        if (chipObj) chipObj.SetActive(false);
        if (aniObj) aniObj.SetActive(true);
        yield return null;
        if (animator) animator.SetTrigger("huifu");
        yield return new WaitForSeconds(1);
        if (aniObj) aniObj.SetActive(false);
        gameObject.SetActive(false);
    }
    private void OnDestroy()
    {
        rigidbody2Ds.Clear();
        rigidbody2Ds = null;
    }
}
