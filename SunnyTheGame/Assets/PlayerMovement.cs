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
	bool startjumpagain = false;
	bool startpowerjump = false;
	bool m_wasInMidAir = false;
	bool restartvars = true;

	private Rigidbody2D rb2D;

	void Start() => rb2D = GetComponent<Rigidbody2D>();

	private float GetVerticalSpeed() => rb2D.velocity.y;

	// Update is called once per frame
	void Update () {

		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		animator.SetFloat("Speed", Math.Abs(horizontalMove));

		if (Input.GetButtonDown("Jump"))
		{
			onPrepareJump();
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
		float vs = GetVerticalSpeed();
		if (!isGoingUp && vs > 0)
		{
			isGoingUp = true;			
		}

		var auxIsJumping = animator.GetBool("isJumping");
		if (auxIsJumping && vs!=0 && vs <= 1.7)
        {
			onMidAir();
		}
		//if (vs != 0) Debug.Log("GetVerticalSpeed: " + vs.ToString());

		// Move our character
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, powerdownpunch, powerjump);
		if (restartvars)
		{
			jump = false; 
			startjumpagain = false;
			powerjump = false;
			restartvars = false;
		}
	}
	public void onJumpAgain()
    {
		if (!powerjump) startjumpagain = true;
		//onPrepareJump();
		Debug.Log("Jump Again? powerjump: " + powerjump);
	}
	public void onPrepareJump()
	{
		jump = false; 
		startjumpagain = false;
		jumpEnded = false;
		animator.SetBool("isJumpStart", true);
		animator.SetBool("isJumping", false);
		animator.SetBool("isPowerPunching", false);
		animator.SetBool("isJumpMidAir", false);
		animator.SetBool("isJumpEnd", false);
		isGoingUp = true;
	}
	public void onStartJumping()
	{
		jump = true;
		powerjump = startpowerjump;
		startpowerjump = false;
		animator.SetBool("isJumpStart", false);
		animator.SetBool("isJumping", true);
		Debug.Log("Prep done! Start the jump! powerjump: " + powerjump);
		restartvars = true;
	}
	public void onMidAir()
    {
		jumpMidAir = true;
		animator.SetBool("isJumpStart", false);
		animator.SetBool("isJumping", true);
		animator.SetBool("isJumpMidAir", true);
		Debug.Log("start going down!");
	}
	public void onLanding()
    {
		//Debug.Log("PlayerMovement - landing isJumping: " + animator.GetBool("isJumping").ToString() + "  --- landing isPowerPunching: " + animator.GetBool("isPowerPunching").ToString() + "  -- powerjump: " + powerjump.ToString());
		//if (!powerjump) animator.SetBool("isJumping", false);
		animator.SetBool("isJumping", false);
		animator.SetBool("isPowerPunching", false);
		animator.SetBool("isJumpStart", false);
		onMidAirEnded();
	}
	public void onMidAirEnded()
    {
		m_wasInMidAir = false;
		jumpMidAir = false;
		isGoingUp = false;
		animator.SetBool("isJumpStart", false);
		animator.SetBool("isJumpMidAir", false);
		Debug.Log("arrived at ground!");
		if (startpowerjump) setLanded();
	}

	public void onLanded()
    {
		setLanded();
	}

	public void setLanded() { 
		jumpEnded = true;
		isGoingUp = false;
		animator.SetBool("isJumpEnd", true);
		if (startpowerjump || startjumpagain) onPrepareJump();
		restartvars = true;
		Debug.Log("Jump ended!");
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
		restartvars = false;
		startpowerjump = true;
		Debug.Log("PlayerMovement - Agora vai dar o SUPER SALTO!");
		//animator.SetBool("isJumpEnd", false);
		//animator.SetBool("isJumpMidAir", false);
		//animator.SetBool("isJumping", false);
		//animator.SetBool("isPowerPunching", false);
		//animator.SetBool("isJumpStart", true);
		//powerdownpunch = false;
		//jump = false;
		//jumpEnded = false;
		//jumpMidAir = false;
		//isGoingUp = false;
		//startjumpagain = false;
		//onPrepareJump();
	}
}
