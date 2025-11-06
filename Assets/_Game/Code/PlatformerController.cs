using NUnit.Framework;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.U2D;
using UnityEngine.UIElements;

public class PlatformerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float jumpForce = 12f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Animator anim;
    
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float friction = 20f;

    private float currentSpeed = 0f;
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isLookingRight;

    Animator animator;
    private float moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Set to Dynamic with gravity
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Get horizontal input
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput == 0 && isGrounded == true)
        {
            anim.SetBool("IsMoving", false);
        }


        // Check if grounded
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump input
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("IsJumping", true);
        }

        if (!Input.GetButtonDown("Jump") && isGrounded == false)
        {
            anim.SetBool("IsJumping", false);
        }


        if (Input.GetKey(KeyCode.T))
        {
            anim.SetBool("TPressed", true);
            anim.SetBool("IsSuper", true);
        }
        else
        {
            anim.SetBool("TPressed", false);
        }
        if (Input.GetKey(KeyCode.P))
        {
            anim.SetBool("IsSuper", false);
            anim.SetBool("TPressed", false);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            anim.SetBool("IsCrouching", true);
        }
        else
        {
            anim.SetBool("IsCrouching", false);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetBool("IsMoving", true);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetBool("IsMoving", true);
        }


        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GetComponent<SpriteRenderer>().flipX = true;
            isLookingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GetComponent<SpriteRenderer>().flipX = false;
            isLookingRight = true;
        }


        // Movement Based Code
        if (currentSpeed >= 5f || currentSpeed <= -5f )
        {
            anim.SetBool("IsJogging", true);
        }
        else
        {
            anim.SetBool("IsJogging", false);
        }

        if (currentSpeed >= 10f || currentSpeed <= -10f)
        {
            anim.SetBool("IsRunning", true);
        }
        else
        {
            anim.SetBool("IsRunning", false);
        }
        if (currentSpeed >= 15f || currentSpeed <= -15f)
        {
            anim.SetBool("IsMach", true);
        }
        else
        {
            anim.SetBool("IsMach", false);
        }

        if (currentSpeed == 0)
        {
            anim.SetBool("IsMoving", false);
        }
        else
        {
            anim.SetBool("IsMoving", true);
        }
    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        float targetDirection = moveInput;


        
        if (Mathf.Abs(targetDirection) > 0.01f)
        {
            currentSpeed += targetDirection * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            // Apply friction when no input
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, friction * Time.fixedDeltaTime);
        }

        // Clamp speed to max
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Apply velocity
         rb.linearVelocity = new Vector2(currentSpeed, rb.linearVelocityY);
    }

    // Visualise ground check in editor
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
    



}


