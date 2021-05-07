using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerMovement : ExtendedBehavior
{

	public CharacterController2D controller;
	public PlayerScore scoreController;
	public Animator animator;

	public float runSpeed = 40f;

	public RectTransform m_PanelGameOver;
	public Text m_txtGameOver;
	public Text m_txtHealth;

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

	bool iscombopunching = false;
	int combopunch = 0;
	bool isStillPunching = false;

	bool restartvars = true;


	private Rigidbody2D rb2D;
	private GameObject m_playerpunchcheck;

	void Start() 
	{ 
		rb2D = GetComponent<Rigidbody2D>();
		m_playerpunchcheck = GetChildWithName(rb2D.gameObject, "PunchCheck");
		m_playerpunchcheck.SetActive(false);

        scoreController.GameOverEvent += ScoreController_GameOverEvent;
        scoreController.HealthChangeEvent += ScoreController_HealthChangeEvent;

		//Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"));
	}

    private void ScoreController_HealthChangeEvent(object sender, float e)
    {
		Debug.Log("update score (PlayerMovement): " + e.ToString());
		m_txtHealth.text = "" + e.ToString();
    }

    private void ScoreController_GameOverEvent(object sender, EventArgs e)
    {
		//this.gameObject.SetActive(false);
		rb2D.gameObject.SetActive(false);
		m_PanelGameOver.gameObject.SetActive(true);
    }

    private float GetVerticalSpeed() => rb2D.velocity.y;


	public void RestartGame ()
    {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		m_PanelGameOver.gameObject.SetActive(false);
	}

	// Update is called once per frame
	void Update () {

		setHorizontalMove();

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

		if (Input.GetButtonDown("ComboPunch"))
		{
			Debug.Log("user clicked combo punch!!!: ");
			iscombopunching = true;
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
		controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump, powerdownpunch, powerjump, iscombopunching);
		if (restartvars)
		{
			jump = false; 
			startjumpagain = false;
			powerjump = false;
			restartvars = false;
		}
	}

	private void setHorizontalMove ()
    {
		horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
		if (!isStillPunching || combopunch < 0 || combopunch > 4)
		{
			combopunch = 0;
			isStillPunching = false;
		}
		if (combopunch > 0) horizontalMove = 0;
		animator.SetFloat("Speed", Math.Abs(horizontalMove));
	}
	public void SetPunchActive()
    {
		m_playerpunchcheck.SetActive(true);
	}

	public void onPrepareComboPunch(bool punch)
    {
		Debug.Log("onPrepareComboPunch: isStillPunching: " + isStillPunching + "   -- punch: " + punch);
		if (!isStillPunching && punch) startComboPunch();
	}

	void startComboPunch() {
		iscombopunching = false;
		isStillPunching = true;
		if (combopunch < 0 || combopunch > 4) combopunch = 0;
		++combopunch;
		setHorizontalMove();
		animator.SetBool("isComboPunching", isStillPunching);
		animator.SetBool("isComboPunch" + combopunch.ToString(), true);
        for (int i = 1; i <= 4; i++)
        {
			if (i != combopunch) animator.SetBool("isComboPunch" + i.ToString(), false);
        }
	}
	public void endComboPunch() {
		// disable the player punch check!
		m_playerpunchcheck.SetActive(false);
		int lastpunch = combopunch;
        for (int i = 1; i <= 4; i++)
        {
			animator.SetBool("isComboPunch" + i.ToString(), false);
        }
		//Debug.Log("endComboPunch lastpunch: " + lastpunch.ToString());

		if (iscombopunching)
        {
			startComboPunch();
		}
		else { 
			combopunch = 0;
			isStillPunching = false;
			animator.SetBool("isComboPunching", isStillPunching);
		}

	}
	public void onJumpAgain()
    {
		if (!powerjump) startjumpagain = true;
		//onPrepareJump();
		//Debug.Log("Jump Again? powerjump: " + powerjump);
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
		//Debug.Log("Prep done! Start the jump! powerjump: " + powerjump);
		restartvars = true;
	}
	public void onMidAir()
    {
		jumpMidAir = true;
		animator.SetBool("isJumpStart", false);
		animator.SetBool("isJumping", true);
		animator.SetBool("isJumpMidAir", true);
		//Debug.Log("start going down!");
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
		//Debug.Log("arrived at ground!");
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
		//Debug.Log("Jump ended!");
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
