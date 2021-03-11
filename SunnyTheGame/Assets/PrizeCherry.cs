using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrizeCherry : MonoBehaviour
{
    public Animator animator;
    public int Prize = 20;

    public void showPrizeAnimation()
    {
        animator.SetBool("isCatched", true);
    }

    public void destroyCherry()
    {
        Debug.Log("destroyCherry");
        Destroy(gameObject);
    }

}
