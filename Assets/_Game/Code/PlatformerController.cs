using System.Collections;
using System.Numerics;
using System.Runtime.CompilerServices;
using NUnit.Framework;
using Unity.VisualScripting;
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
    [SerializeField] private float AirDashSpeed = 25f;
    [SerializeField] private float DashingTime = 0.85f;
    
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Animator anim;
    
    [SerializeField] private float maxSpeed = 35f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float friction = 20f;

    [SerializeField] private float skidThresholdSpeed = 5f;
    private bool IsOppositeDirection(float input, float velocity)
{
    return (input > 0 && velocity < -0.1f) || (input < 0 && velocity > 0.1f);
}

    [SerializeField] private float currentSpeed = 0f;
    private Rigidbody2D rb;
    public bool isGrounded;
    public bool isLookingRight;
    private bool CanAirDash;
    private IEnumerator AirDash()
    {
        CanAirDash = false;
        anim.SetBool("AirDashing", true);
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.linearVelocity = new UnityEngine.Vector2(AirDashSpeed * 25, 0); 
        yield return new WaitForSeconds(DashingTime);
        rb.gravityScale = originalGravity;
    }
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
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, jumpForce);
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

        if (Input.GetKey(KeyCode.UpArrow))
        {
            anim.SetBool("IsLooking", true);
        }
        else
        {
            anim.SetBool("IsLooking", false);
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
        if (currentSpeed >= 5f || currentSpeed <= -5f)
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
        if (currentSpeed >= 35f || currentSpeed <= -35f)
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

        if (Input.GetKey(KeyCode.RightShift) && isGrounded == false && CanAirDash == true)
        {
            //StartCoroutine(AirDash());
            Debug.Log("Air dashing");
            rb.AddForce(new UnityEngine.Vector2(25, 0), ForceMode2D.Impulse); //  linearVelocity = new UnityEngine.Vector2(AirDashSpeed * 25, 0); 
            anim.SetBool("AirDashing", true);
            anim.SetBool("CanAirDash", false);
            CanAirDash = false;
        }
        if (isGrounded == true)
        {
            anim.SetBool("AirDashing", false);
            anim.SetBool("CanAirDash", true);
            CanAirDash = true;
            anim.SetBool("IsGrounded", true);
        }
        else
        {
            anim.SetBool("IsGrounded", false);
        }
    
    }

    void FixedUpdate()
    {
       
            rb.linearVelocity = new UnityEngine.Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

            float targetDirection = moveInput;
        
        if (Mathf.Abs(targetDirection) > 0.01f)
        {
            currentSpeed += targetDirection * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, friction * Time.fixedDeltaTime);
        }
            currentSpeed = Mathf.Clamp(currentSpeed, - maxSpeed, maxSpeed);

       
            rb.linearVelocity = new UnityEngine.Vector2(currentSpeed, rb.linearVelocityY);

        if (Mathf.Abs(moveInput) < 0.01f || IsOppositeDirection(moveInput, currentSpeed))
        {
            float dynamicFriction = friction;

            if (IsOppositeDirection(moveInput, currentSpeed) && Mathf.Abs(currentSpeed) > skidThresholdSpeed)
            {
                dynamicFriction *= 3f; 
                anim.SetBool("IsSkidding", true);
            }
            else
            {
                anim.SetBool("IsSkidding", false);
            }

            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, dynamicFriction * Time.fixedDeltaTime);
        }
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


