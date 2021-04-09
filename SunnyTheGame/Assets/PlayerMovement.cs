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
	bool jumpEnded = true;
	bool jumpMidAir = false;
	bool isGoingUp = false;
	
	private Rigidbody2D rb2D;

	void Start() => rb2D = GetComponent<Rigidbody2D>();

	private float GetVerticalSpeed() => rb2D.velocity.y;

	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		animator.SetFloat("Speed", Math.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			jumpEnded = false;
			animator.SetBool("isJumpStart", true);
			animator.SetBool("isJumpMidAir", false);
			animator.SetBool("isJumpEnd", false);
			isGoingUp = true;
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
		float vs = GetVerticalSpeed();
		if (!isGoingUp && vs > 0)
		{
			isGoingUp = true;			
		}

		var auxIsJumping = animator.GetBool("isJumping");
		if (auxIsJumping && vs!=0 && vs <= 1.7 && vs >= -1.7)
        {
			onMidAir();
		}

		if (vs != 0) Debug.Log("GetVerticalSpeed: " + vs.ToString());
	}

	public void onStartJumping()
    {
		jump = true;
		animator.SetBool("isJumping", true);
		animator.SetBool("isJumpStart", false);
	}
	public void onLanding()
    {
		//Debug.Log("PlayerMovement - landing isJumping: " + animator.GetBool("isJumping").ToString() + "  --- landing isPowerPunching: " + animator.GetBool("isPowerPunching").ToString() + "  -- powerjump: " + powerjump.ToString());
		if (!powerjump) animator.SetBool("isJumping", false);
		animator.SetBool("isPowerPunching", false);
		onMidAirEnded();
	}

	public void onLanded()
    {
		jumpEnded = true;
		isGoingUp = false;
		animator.SetBool("isJumpEnd", true);
    }

	public void onMidAir()
    {
		jumpMidAir = true;
		animator.SetBool("isJumpMidAir", true);
	}
	public void onMidAirEnded()
    {
		jumpMidAir = false;
		isGoingUp = false;
		animator.SetBool("isJumpMidAir", false);
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
		animator.SetBool("isJumpEnd", false);
		animator.SetBool("isJumpMidAir", false);
		animator.SetBool("isJumping", true);
		animator.SetBool("isJumpStart", false);
		powerdownpunch = false;
		powerjump = true;
		jump = true;
		jumpEnded = false;
		jumpMidAir = false;
		isGoingUp = false;
	}
}
