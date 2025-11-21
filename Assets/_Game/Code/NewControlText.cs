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

public class NewControlTest : MonoBehaviour
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

    // Slope handling variables
    private UnityEngine.Vector2 groundNormal = UnityEngine.Vector2.up;
    [SerializeField] private float slopeRayLength = 0.6f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput == 0 && isGrounded == true)
        {
            anim.SetBool("IsMoving", false);
        }

        // Ground check
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded == true)
        {
            rb.linearVelocity = new UnityEngine.Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetBool("IsJumping", true);
        }

        if (!Input.GetButtonDown("Jump") && isGrounded == false)
        {
            anim.SetBool("IsJumping", false);
        }

        // Animations for keys
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
            friction = 5;
            maxSpeed = 25;
        }
        else
        {
            anim.SetBool("IsCrouching", false);
            friction = 30;
            maxSpeed = 35;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            anim.SetBool("IsMoving", true);
            GetComponent<SpriteRenderer>().flipX = true;
            isLookingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            anim.SetBool("IsMoving", true);
            GetComponent<SpriteRenderer>().flipX = false;
            isLookingRight = true;
        }

        // Speed-based animations
        anim.SetBool("IsJogging", Mathf.Abs(currentSpeed) >= 5f);
        anim.SetBool("IsRunning", Mathf.Abs(currentSpeed) >= 10f);
        anim.SetBool("IsMach", Mathf.Abs(currentSpeed) >= 35f);
        anim.SetBool("IsMoving", currentSpeed != 0);

        // Air dash
        if (Input.GetKey(KeyCode.RightShift) && isGrounded == false && CanAirDash == true)
        {
            rb.AddForce(new UnityEngine.Vector2(25, 0), ForceMode2D.Impulse);
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
        // Detect slope
        RaycastHit2D hit = Physics2D.Raycast(transform.position, UnityEngine.Vector2.down, slopeRayLength, groundLayer);
        if (hit.collider != null)
        {
            groundNormal = hit.normal;
        }
        else
        {
            groundNormal = UnityEngine.Vector2.up;
        }

        // Calculate slope tangent
        UnityEngine.Vector2 slopeTangent = new UnityEngine.Vector2(groundNormal.y, -groundNormal.x);

        // Movement along slope
        UnityEngine.Vector2 moveDirection = isGrounded ? slopeTangent * moveInput : new UnityEngine.Vector2(moveInput, 0f);

        // Apply acceleration/friction
        if (Mathf.Abs(moveInput) > 0.01f)
        {
            currentSpeed += moveInput * acceleration * Time.fixedDeltaTime;
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, friction * Time.fixedDeltaTime);
        }
        currentSpeed = Mathf.Clamp(currentSpeed, -maxSpeed, maxSpeed);

        // Apply velocity along slope
        rb.linearVelocity = new UnityEngine.Vector2(moveDirection.x * moveSpeed, rb.linearVelocity.y);

        // Skid logic
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

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
