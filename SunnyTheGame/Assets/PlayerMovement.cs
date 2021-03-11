using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

	public CharacterController2D controller;
	public Animator animator;

	public float runSpeed = 40f;

	float horizontalMove = 0f;
	bool jump = false;
	bool powerjump = false;
	bool crouch = false;
	bool powerdownpunch = false;
	
	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		animator.SetFloat("Speed", Math.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jump = true;
			animator.SetBool("isJumping", true);
			//Debug.Log("PlayerMovement - update isJumping: " + animator.GetBool("isJumping").ToString());
		}

		if (Input.GetButtonDown("PowerDownPunch") && animator.GetBool("isJumping"))
		{
			powerdownpunch = true;
			animator.SetBool("isPowerPunching", powerdownpunch);
		}
		else if (Input.GetButtonUp("PowerDownPunch") || !animator.GetBool("isJumping"))
		{
			powerdownpunch = false;
			animator.SetBool("isPowerPunching", false);
		}

		if (Input.GetButtonDown("Crouch"))
		{
			crouch = true;
		} 
		else if (Input.GetButtonUp("Crouch"))
		{
			crouch = false;
		}

	}

	void FixedUpdate ()
	{
		//if (horizontalMove != 0f) Debug.Log("horizontalMove2: " + horizontalMove.ToString());
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, powerdownpunch, powerjump);
		jump = false;
		powerjump = false;
		powerdownpunch = false;
	}

	public void onLanding()
    {
		Debug.Log("PlayerMovement - landing isJumping: " + animator.GetBool("isJumping").ToString());
		animator.SetBool("isJumping", false);
		animator.SetBool("isPowerPunching", false);
    }

	public void onUncrouching(bool isCrouching)
    {
		animator.SetBool("isCrouching", isCrouching);
    }

	public void onPowerJump ()
    {
		//Debug.Log("PlayerMovement - Agora devia dar o SUPER SALTO!");
		//animator.SetBool("isJumping", false);
		//animator.SetBool("isPowerPunching", false);
		//powerjump = true;
		//jump = true;
        jump = false;
        powerjump = false;
		powerdownpunch = false;
		animator.SetBool("isPowerPunching", false);
		controller.Move(0.0f, false, true, false, true);
    }
}
