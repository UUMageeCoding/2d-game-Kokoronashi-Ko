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

    }

    void FixedUpdate()
    {
        // Apply horizontal movement
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
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


