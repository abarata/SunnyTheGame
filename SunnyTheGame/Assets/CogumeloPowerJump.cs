using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CogumeloPowerJump : MonoBehaviour
{
    public Animator animator;

    [Header("Events")]
    [Space]

    public UnityEvent OnPowerJump;

    // Start is called before the first frame update
    private void Start()
    {

    }
    private void Awake()
    {
        if (OnPowerJump == null)
            OnPowerJump = new UnityEvent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            var playerAnimator = collision.GetComponent<Animator>();
            var playerBody = collision.GetComponent<Rigidbody2D>();
            Debug.Log("Player entered mushroom area...isJumping: " + playerAnimator.GetBool("isJumping").ToString() + "  -- isPowerPunching: " + playerAnimator.GetBool("isPowerPunching").ToString() + "  -- velocity.y" + playerBody.velocity.y);
            if (playerAnimator.GetBool("isJumping") && playerAnimator.GetBool("isPowerPunching"))
            {
                animator.SetBool("isCatched", true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerAnimator = collision.GetComponent<Animator>();
            var playerBody = collision.GetComponent<Rigidbody2D>();
            Debug.Log("Player exited mushroom area...isJumping: " + playerAnimator.GetBool("isJumping").ToString() + "  -- isPowerPunching: " + playerAnimator.GetBool("isPowerPunching").ToString() + "  -- velocity.y" + playerBody.velocity.y);
            //animator.SetBool("isCatched", false);
        }
    }

    public void powerJump()
    {
        OnPowerJump.Invoke();
    }

    public void destroyMushroom()
    {
        animator.SetBool("isCatched", false);
        //Destroy(gameObject);
    }

}
