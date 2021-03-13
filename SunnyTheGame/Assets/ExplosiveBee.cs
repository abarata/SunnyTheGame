using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBee : MonoBehaviour
{
    public CharacterController2D PlayerController;
    public float DamageRadius = 0.9f;
    public float TriggerExplosionDistance = 2.5f;
    public float FlyDistance = 5.0f;
    public float flySpeed = 20f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    public float Damage = 3.0f;
    private Animator animator;
    private float Health = 1.5f;
    private float StartFlyingWhenDistanceToPlayer;
    private bool playerIsInFlyRange = false;
    private bool playerIsInDamageRange = false;
    private bool isExploding = false;
    private Rigidbody2D m_Rigidbody2D;
    private CircleCollider2D m_DamageCollider;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector2 InitialPosition;
    private float horizontalMove = 1f;
    private float verticalMove = 1f;
    private float ymargin = 0.4f;

    private bool isDead
    {
        get { return Health <= 0.0f; }
    }


    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("Events")]
    [Space]
    public FloatEvent OnApplyDamage;

    private void Awake()
    {

        if (OnApplyDamage == null)
            OnApplyDamage = new FloatEvent();

    }
    // Start is called before the first frame update
    void Start()
    {
        m_DamageCollider = GetComponent<CircleCollider2D>();
        m_Rigidbody2D = GetComponent<Rigidbody2D>();
        m_FacingRight = gameObject.GetComponent<SpriteRenderer>().flipX;
        animator = gameObject.GetComponent<Animator>();
        m_DamageCollider.radius = DamageRadius;
        StartFlyingWhenDistanceToPlayer = FlyDistance;
        InitialPosition = m_Rigidbody2D.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        checkIfplayerIsInRange();

        if (!isExploding && playerIsInFlyRange)
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            // check where the bee is
            if (m_Rigidbody2D.position.x <= PlayerController.PlayerPosition.x) horizontalMove = 1f;
            else horizontalMove = -1f;

            if (m_Rigidbody2D.position.y <= PlayerController.PlayerPosition.y - ymargin) verticalMove = 1f;
            if (m_Rigidbody2D.position.y >= PlayerController.PlayerPosition.y + ymargin) verticalMove = -1f;

            float movex = horizontalMove * flySpeed * Time.fixedDeltaTime;
            float movey = verticalMove * flySpeed * Time.fixedDeltaTime;

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(movex * 10f, movey * 5f);

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

            // If the input is moving the player right and the player is facing left...
            if (movex > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (movex < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }else if (!isExploding)
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;
        transform.Rotate(0f, 180f, 0f);
    }

    void checkIfplayerIsInRange()
    {
        float disttofly = m_Rigidbody2D.position.x - PlayerController.PlayerPosition.x;
        float xtoexplode = m_Rigidbody2D.position.x - PlayerController.PlayerPosition.x;
        float ytoexplode = m_Rigidbody2D.position.y - PlayerController.PlayerPosition.y;
        playerIsInFlyRange = (Math.Abs(disttofly) <= StartFlyingWhenDistanceToPlayer);
        if (!isExploding && !isDead) { 
            if (Math.Abs(xtoexplode) <= TriggerExplosionDistance && Math.Abs(ytoexplode) <= TriggerExplosionDistance) {
                Debug.Log(" -- xtoexplode: " + xtoexplode.ToString() + "  -- ytoexplode: " + ytoexplode.ToString() + "  -- TriggerExplosionDistance: " + TriggerExplosionDistance.ToString());
                Explode();
            }
        }

        //Debug.Log("disttofly: " + disttofly.ToString() + " -- StartFlyingWhenDistanceToPlayer: " + StartFlyingWhenDistanceToPlayer.ToString() + " -- xtoexplode: " + xtoexplode.ToString() + "  -- ytoexplode: " + ytoexplode.ToString() + "  -- TriggerExplosionDistance: " + TriggerExplosionDistance.ToString());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInDamageRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInDamageRange = false;
        }
    }
    private void Explode()
    {
        Debug.Log("Start Exploding Bee!");
        isExploding = true;
        animator.SetBool("isDead", true);
    }

    public void destroyExplosiveBee()
    {
        if (playerIsInDamageRange && !isDead) OnApplyDamage.Invoke((Damage * -1));
        Debug.Log("destroyExplosiveBee and/or set damage");
        Destroy(gameObject);
    }

}
