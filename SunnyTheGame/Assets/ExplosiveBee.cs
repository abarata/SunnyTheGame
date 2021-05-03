using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExplosiveBee : ExtendedBehavior
{
    private class HasObstacleInTheWay
    {
        public bool obstacleInTheWay { get; set; } = false;
        public float obstacleVerticalDistance { get; set; } = 0f;
        public float obstacleHorizontalDistance { get; set; } = 0f;
    }

    public CharacterController2D PlayerController;
    public float DamageRadius = 0.9f;
    public float TriggerExplosionDistance = 2.5f;

    [LabelOverride("Fly & Detection Distance")]
    [SerializeField]
    public float FlyDistance = 5.0f;

    public float flySpeed = 20f;
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement

    public float Damage = 3.0f;

    [SerializeField] private Transform m_ObstacleCheck;                           // A position marking where to check if the bee has obstacles in the way.
    private HasObstacleInTheWay obstacleInTheWay;
    const float k_ObstacleCheckRadius = 1.0f; // Radius of the overlap circle to determine if has obstacles in the way

    private Animator animator;
    private float Health = 1.5f;
    private float StartFlyingWhenDistanceToPlayer;
    private bool playerIsInFlyRange = false;
    private bool playerIsInDamageRange = false;
    private bool isExploding = false;
    private Rigidbody2D m_Rigidbody2D;
    private CircleCollider2D m_DamageCollider;
    private CircleCollider2D m_OwnDamageCollider;
    private Vector3 m_Velocity = Vector3.zero;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector2 InitialPosition;
    private float horizontalMove = 1f;
    private float verticalMove = 1f;
    private float ymargin = 2.2f;
    private int getawayfromobstacle = 0;
    private float auxangle = 0f;
    private int numberOfTriesToByPassObstacules = 25;
    private GameObject m_playerpunchcheck;

    private bool isDead
    {
        get { return Health <= 0.0f; }
    }


    [System.Serializable]
    public class FloatEvent : UnityEvent<float> { }

    [Header("Events")]
    [Space]
    public FloatEvent OnApplyDamage;

    private void Init()
    {
        if (obstacleInTheWay == null)
            obstacleInTheWay = new HasObstacleInTheWay();

        if (OnApplyDamage == null)
            OnApplyDamage = new FloatEvent();

    }
    private void Awake()
    {
        Init();
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

        m_playerpunchcheck = GetChildWithName(PlayerController.gameObject, "PunchCheck");
        m_OwnDamageCollider = m_playerpunchcheck.GetComponent<CircleCollider2D>();
    }


    private void detectObstaclesByOverlapping()
    {
        Collider2D[] allOverlappingColliders = new Collider2D[16];
        ContactFilter2D contactFilter = new ContactFilter2D();

        int overlapCount = Physics2D.OverlapCollider(m_OwnDamageCollider, contactFilter.NoFilter(), allOverlappingColliders);
        if (overlapCount > 0)
        {
            foreach (var colaux in allOverlappingColliders)
            {
                if (colaux != null && colaux.gameObject != gameObject)
                {
                    Debug.Log("collided with -> name: " + colaux.name + "    tag: " + colaux.tag);
                    // how much the character should be knocked back
                    var magnitude = 5000;
                    // calculate force vector
                    var force = colaux.transform.position - m_Rigidbody2D.transform.position;
                    // normalize force vector to get direction only and trim magnitude
                    force.Normalize();
                    m_Rigidbody2D.AddForce(force * magnitude);
                }
            }
        }
    }
    private void detectObstaclesByRaycast()
    {
        var checkdirection = transform.TransformDirection(Vector2.right);
        if (!m_FacingRight) checkdirection = transform.TransformDirection(Vector2.left);

        var l_angle = Vector2.Angle((Vector2)transform.position, (Vector2)PlayerController.PlayerPosition);
        l_angle = Mathf.Atan2(transform.position.y, PlayerController.PlayerPosition.x) * Mathf.Rad2Deg;

        Vector3 dir3 = transform.position - PlayerController.transform.position;
        dir3 = PlayerController.transform.InverseTransformDirection(dir3);
        l_angle = Mathf.Atan2(dir3.y, dir3.x) * Mathf.Rad2Deg;

        if (auxangle != l_angle)
        {
            auxangle = l_angle;
            //Debug.Log("l_angle: " + l_angle.ToString());
        }
        var dir = GetDirectionVector2D(l_angle);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, k_ObstacleCheckRadius, PlayerController.m_WhatIsGround);
        // Does the ray intersect any objects excluding the player layer
        if (hit.collider != null)//, layerMask))
        {
            float distance = hit.point.x - transform.position.x;
            float xtoplayer = m_Rigidbody2D.transform.position.x - PlayerController.PlayerPosition.x;
            float ytoplayer = m_Rigidbody2D.transform.position.y - PlayerController.PlayerPosition.y;

            obstacleInTheWay.obstacleInTheWay = true;
            obstacleInTheWay.obstacleHorizontalDistance = (m_Rigidbody2D.position.x <= PlayerController.PlayerPosition.x ? Math.Abs(xtoplayer) : Math.Abs(xtoplayer) * -1);
            obstacleInTheWay.obstacleVerticalDistance = (m_Rigidbody2D.transform.position.y > PlayerController.PlayerPosition.y ? Math.Abs(ytoplayer) : Math.Abs(ytoplayer) * -1);
            //Debug.DrawRay(transform.position, transform.TransformDirection(Vector2.right) * distance, Color.black);
            //Debug.Log("Did Hit: " + hit.collider.name);
            //Debug.DrawLine(m_Rigidbody2D.transform.position, hit.point, Color.red);
            Debug.DrawRay((Vector2)transform.position, dir * k_ObstacleCheckRadius, Color.red);
        }
        else
        {
            Debug.DrawRay((Vector2)transform.position, dir * k_ObstacleCheckRadius, Color.white);
            //Debug.Log("Did not Hit");
        }
    }

    private void detectObstaclesByVelocity()
    {
        float xtoplayer = m_Rigidbody2D.transform.position.x - PlayerController.PlayerPosition.x;
        float ytoplayer = m_Rigidbody2D.transform.position.y - PlayerController.PlayerPosition.y;
        Vector3 dir3 = (PlayerController.PlayerPosition - transform.position).normalized;
        //if (m_Rigidbody2D.position.x < PlayerController.PlayerPosition.x) dir3 = (PlayerController.PlayerPosition - transform.position).normalized;
        //dir3 = PlayerController.transform.InverseTransformDirection(dir3);
        var l_angle = Mathf.Atan2(dir3.y, dir3.x) * Mathf.Rad2Deg;
        var l_distancetoplayer = m_Rigidbody2D.position.x + StartFlyingWhenDistanceToPlayer;
        if (m_Rigidbody2D.position.x > PlayerController.PlayerPosition.x)
        {
            //l_angle += 180f;
            l_distancetoplayer = m_Rigidbody2D.position.x - StartFlyingWhenDistanceToPlayer;
        }
        var dir = GetDirectionVector2D(l_angle);
        var l_raydir = dir * StartFlyingWhenDistanceToPlayer;

        if (auxangle != l_angle)
        {
            auxangle = l_angle;
            //Debug.Log("l_angle: " + l_angle.ToString() + "  -- transform.position.x: " + transform.position.x.ToString() + "  -- StartFlyingWhenDistanceToPlayer: " + StartFlyingWhenDistanceToPlayer.ToString() + "  -- l_distancetoplayer: " + l_distancetoplayer.ToString());
        }
        //if ((l_angle < 0 && m_FacingRight) || (l_angle > 0 && !m_FacingRight)) l_angle *= -1;

        Debug.DrawRay(m_Rigidbody2D.position, l_raydir, Color.black, 0, false);

        if (playerIsInFlyRange && m_Rigidbody2D.velocity == Vector2.zero)
        {
            obstacleInTheWay.obstacleInTheWay = true;
            obstacleInTheWay.obstacleHorizontalDistance = (m_Rigidbody2D.position.x <= PlayerController.PlayerPosition.x ? Math.Abs(xtoplayer) : Math.Abs(xtoplayer) * -1);
            obstacleInTheWay.obstacleVerticalDistance = (m_Rigidbody2D.transform.position.y > PlayerController.PlayerPosition.y ? Math.Abs(ytoplayer) : Math.Abs(ytoplayer) * -1);
            Debug.DrawRay((Vector2)transform.position, dir * dir3.x, Color.red);
        }


    }

    // Update is called once per frame
    void Update()
    {
        Init();
        bool hadObstacleBefore = obstacleInTheWay.obstacleInTheWay;
        obstacleInTheWay.obstacleInTheWay = false;

        detectObstaclesByVelocity();

    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        Init();


        detectObstaclesByOverlapping();


        checkIfplayerIsInRange();

        if (!isExploding && playerIsInFlyRange)
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;

            // check where the bee is
            if (m_Rigidbody2D.position.x <= PlayerController.PlayerPosition.x) horizontalMove = 1f;
            else horizontalMove = -1f;

            if (m_Rigidbody2D.position.y <= PlayerController.PlayerPosition.y - ymargin) verticalMove = 1f;
            if (m_Rigidbody2D.position.y >= PlayerController.PlayerPosition.y + ymargin) verticalMove = -1f;

            if (obstacleInTheWay.obstacleInTheWay || getawayfromobstacle > 0)
            {
                //Debug.Log("getawayfromobstacle: " + getawayfromobstacle.ToString());
                //Debug.Log("obstacleInTheWay.obstacleHorizontalDistance: " + obstacleInTheWay.obstacleHorizontalDistance + "  -- horizontalMove: " + horizontalMove.ToString());

                if (obstacleInTheWay.obstacleHorizontalDistance > 0 && horizontalMove > 0) horizontalMove = horizontalMove * -1f;
                else if (obstacleInTheWay.obstacleHorizontalDistance < 0 && horizontalMove < 0) horizontalMove = horizontalMove * -1f;

                //if (obstacleInTheWay.obstacleVerticalDistance > 0 && verticalMove > 0) verticalMove = verticalMove * -1f;
                //else if (obstacleInTheWay.obstacleVerticalDistance < 0 && verticalMove < 0) verticalMove = verticalMove * -1f;

                getawayfromobstacle += 1;
                if (getawayfromobstacle > numberOfTriesToByPassObstacules) getawayfromobstacle = 0;
            }


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
        }
        else if (!isExploding)
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            getawayfromobstacle = 0;
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
        if (!isExploding && !isDead)
        {
            if (Math.Abs(xtoexplode) <= TriggerExplosionDistance && Math.Abs(ytoexplode) <= TriggerExplosionDistance)
            {
                //Debug.Log(" -- xtoexplode: " + xtoexplode.ToString() + "  -- ytoexplode: " + ytoexplode.ToString() + "  -- TriggerExplosionDistance: " + TriggerExplosionDistance.ToString());
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
        //Debug.Log("Start Exploding Bee!");
        isExploding = true;
        animator.SetBool("isDead", true);
    }

    public void destroyExplosiveBee()
    {
        if (playerIsInDamageRange && !isDead) OnApplyDamage.Invoke((Damage * -1));
        //Debug.Log("destroyExplosiveBee and/or set damage");
        Destroy(gameObject);
    }

}
