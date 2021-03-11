using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterController2D : ExtendedBehavior
{
	[SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
	[SerializeField] private float m_PowerJumpForce = 900f;                          // Amount of force added when the player poewr jumps.
	[SerializeField] private float m_PunchDownForce = 800f;                          // Amount of force added when the player Power Punches.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	public Vector3 PlayerPosition;

	private List<string> tagsWhereDontCrouch = new List<string>() { "Platform", "PowerJump" };
	private List<string> tagsWhereIsNotGround = new List<string>() { "PowerJump" };

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool m_wasCrouching = false;

	[System.Serializable]
	public class GameObjectEvent : UnityEvent<GameObject> {}
	
	public GameObjectEvent OnPrizeEvent;
	
    private string gameobjectname = "";

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();

		if (OnPrizeEvent == null)
			OnPrizeEvent = new GameObjectEvent();

	}
    private void Update()
    {
		bool wasGrounded = m_Grounded;
		m_Grounded = false;
		PlayerPosition = m_Rigidbody2D.position;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				if (gameobjectname != colliders[i].gameObject.name) {
					gameobjectname = colliders[i].gameObject.name;
					//Debug.Log("1- wasGrounded: " + wasGrounded.ToString()  + "  -- gameobject.name: " + colliders[i].gameObject.name + "  -- gameobject.tag: " + colliders[i].gameObject.tag);
				}
				m_Grounded = true;
				if (!wasGrounded)
				{
					//Debug.Log("1- wasGrounded: " + wasGrounded.ToString()  + "  -- gameobject.name: " + colliders[i].gameObject.name + "  -- gameobject.tag: " + colliders[i].gameObject.tag);
					Debug.Log("PlayerMovement - colliding  -- wasGrounded: " + wasGrounded.ToString() + "  -- m_Rigidbody2D.velocity.y: " + m_Rigidbody2D.velocity.y.ToString() + "  -- gameobject.name: " + colliders[i].gameObject.name + "  -- gameobject.tag: " + colliders[i].gameObject.tag);
					if (colliders[i].gameObject.CompareTag("PRIZE"))
                    {
						OnPrizeEvent.Invoke(colliders[i].gameObject);
                    }

					if (!tagsWhereIsNotGround.Contains(colliders[i].gameObject.tag))
					{
					}

					//if (m_Rigidbody2D.velocity.y <= 0)
					//{
					//}

					OnLandEvent.Invoke();

				}

			}
		}
	}


	public void Move(float move, bool crouch, bool jump, bool powerdownpunch, bool powerjump)
	{
		// If crouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			var whatcolider = Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround);
			if (whatcolider && !tagsWhereDontCrouch.Contains(whatcolider.tag))
			{
				Debug.Log("whatcolider.tag: " + whatcolider.tag);
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || m_AirControl)
		{

			// If crouching
			if (crouch)
			{
				if (!m_wasCrouching)
				{
					m_wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= m_CrouchSpeed;

				// Disable one of the colliders when crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = false;
			}
			else
			{
				// Enable the collider when not crouching
				if (m_CrouchDisableCollider != null)
					m_CrouchDisableCollider.enabled = true;

				if (m_wasCrouching)
				{
					m_wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			if (!m_Grounded && powerdownpunch)
			{
				// freeze the player before going down fast!
//				var saved_velocity = m_Velocity;
				Debug.Log("CharController2D - agora pára por 1 segundo!");
				m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation; //Freeze character Y axis
				this.Wait(0.4f, () =>
				{
					m_Rigidbody2D.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
					m_Rigidbody2D.AddForce(new Vector2(0f, (-1* m_PowerJumpForce)));
					targetVelocity = new Vector2(0.0f, (-1 * (m_PunchDownForce * 40f * Time.fixedDeltaTime)));
					//m_Velocity = saved_velocity;
					//m_Rigidbody2D.velocity = new Vector2(0.0f, -10f);
				});
			}
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && m_FacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (m_Grounded && jump)
		{
			// Add a vertical force to the player.
			m_Grounded = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, (powerjump ? m_PowerJumpForce : m_JumpForce)));
		}

	}


	private void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		transform.Rotate(0f, 180f, 0f);
	}
}