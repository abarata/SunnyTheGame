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
		}

		if (Input.GetButtonDown("PowerDownPunch"))
		{
			powerdownpunch = true;
		}
		else if (Input.GetButtonUp("PowerDownPunch"))
		{
			powerdownpunch = false;
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
		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, powerdownpunch, powerjump);
		jump = false;
		powerjump = false;
	}

	public void onLanding()
    {
		//Debug.Log("PlayerMovement - landing isJumping: " + animator.GetBool("isJumping").ToString() + "  --- landing isPowerPunching: " + animator.GetBool("isPowerPunching").ToString() + "  -- powerjump: " + powerjump.ToString());
		if (!powerjump) animator.SetBool("isJumping", false);
		animator.SetBool("isPowerPunching", false);
    }

	public void onCrouching(bool isCrouching)
    {
		animator.SetBool("isCrouching", isCrouching);
    }
	public void onPowerPunching(bool isPowerPunching)
    {
		animator.SetBool("isPowerPunching", isPowerPunching);
    }

	public void onPowerJump ()
    {
		//Debug.Log("PlayerMovement - Agora vai dar o SUPER SALTO!");
		animator.SetBool("isJumping", true);
		powerdownpunch = false;
		powerjump = true;
		jump = true;
	}
}
