using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour
{
    private Rigidbody2D myRigidBody;
    public Collider2D m_PlatformDisableCollider;
    public CharacterController2D CharacterController;
    private bool isUnderneath;

    // Start is called before the first frame update
    void Start()
    {
        isUnderneath = false;

        // Disable the colliders 
        if (m_PlatformDisableCollider != null)
            m_PlatformDisableCollider.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Debug.Log("OnTriggerEnter" + other.gameObject.name);
        if (other.gameObject.CompareTag("Player")) {
            if (m_PlatformDisableCollider != null)
                m_PlatformDisableCollider.enabled = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        //Debug.Log("OnTriggerExit" + other.gameObject.name);
        if (other.gameObject.CompareTag("Player")) {
            if (m_PlatformDisableCollider != null)
                m_PlatformDisableCollider.enabled = true;
        }
        
    }

    private Vector3 auxpos;
    private void FixedUpdate()
    {

    }
}
