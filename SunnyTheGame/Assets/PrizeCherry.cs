using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PrizeCherry : MonoBehaviour
{
    public Animator animator;
    public float AreaRadius = 0.25f;
    public int Prize = 20;
    private bool prizewon;

    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    [Header("Events")]
    [Space]
    public IntEvent OnPrizeWin;

    private void Awake()
    {
        if (OnPrizeWin == null)
            OnPrizeWin = new IntEvent();
    }
    private void Start()
    {
        prizewon = false;
        gameObject.GetComponent<CircleCollider2D>().radius = AreaRadius;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!prizewon && collision.CompareTag("Player"))
        {
            prizewon = true;
            animator.SetBool("isCatched", true);
            OnPrizeWin.Invoke(Prize);
        }
    }

    public void destroyCherry()
    {
        Debug.Log("destroyCherry");
        Destroy(gameObject);
    }

}
