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
    public UnityEvent OnJump;

    // Start is called before the first frame update
    private void Start()
    {

    }
    private void Awake()
    {
        if (OnPowerJump == null)
            OnPowerJump = new UnityEvent();
        if (OnJump == null)
            OnJump = new UnityEvent();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.CompareTag("Player"))
        {
            var playerAnimator = collision.GetComponent<Animator>();
            var playerBody = collision.GetComponent<Rigidbody2D>();
            Debug.Log("Player entered mushroom area...isJumping: " + playerAnimator.GetBool("isJumping").ToString() + "  -- isJumpEnd: " + playerAnimator.GetBool("isJumpEnd").ToString() + "  -- isPowerPunching: " + playerAnimator.GetBool("isPowerPunching").ToString() + "  -- velocity.y" + playerBody.velocity.y);
            if ((playerAnimator.GetBool("isJumping") || !playerAnimator.GetBool("isJumpEnd")) && playerAnimator.GetBool("isPowerPunching"))
            {
                animator.SetBool("isCatched", true);
                OnPowerJump.Invoke();
            }
            if ((playerAnimator.GetBool("isJumping") || !playerAnimator.GetBool("isJumpEnd")) && !playerAnimator.GetBool("isPowerPunching"))
            {
                animator.SetBool("isCatched", true);
                Debug.Log("Invoke JumpAgain");
                OnJump.Invoke();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            var playerAnimator = collision.GetComponent<Animator>();
            var playerBody = collision.GetComponent<Rigidbody2D>();
            //Debug.Log("Player exited mushroom area...isJumping: " + playerAnimator.GetBool("isJumping").ToString() + "  -- isPowerPunching: " + playerAnimator.GetBool("isPowerPunching").ToString() + "  -- velocity.y" + playerBody.velocity.y);
        }
    }

    public void powerJump()
    {
    }

    public void animationJustStarted()
    {
    }

    public void restartMushroom()
    {
        animator.SetBool("isCatched", false);
    }

    public void destroyMushroom()
    {
        Destroy(gameObject);
    }

}
