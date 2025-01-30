using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float speed = 10f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float dashForce = 30f;
    [SerializeField] float dashTime = 0.2f;
    [SerializeField] float dashCD = 1f;

    float direction = 0;
    bool isGrounded = false;
    bool isDashing = false;
    bool canDash = true;  // Corrected: Initialize as true

    InputSystem_Actions controls;

    private void Awake()
    {
        controls = new InputSystem_Actions();
        controls.Player.Dash.performed += ctx => OnDash();  // Bind the dash action
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isDashing)
        {
            Move(direction);
        }
    }

    void OnMove(InputValue value)
    {
        float v = value.Get<float>();
        direction = v;
    }

    void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);  // Corrected from `linearVelocity`
    }

    void OnJump()
    {
        if (isGrounded)
        {
            Jump();
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
    }

    void OnDash()
    {
        if (canDash && direction != 0)
        {
            StartCoroutine(Dash());
        }
    }

    System.Collections.IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;
        float originalGravity = rb.gravityScale;

        rb.gravityScale = 0;  // Disable gravity temporarily during the dash
        rb.linearVelocity = new Vector2(direction * dashForce, 0);  // Corrected from `linearVelocity`

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;  // Restore gravity
        isDashing = false;

        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                if (Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f)
                {
                    isGrounded = true;
                }
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Optional: Handle logic while colliding with the ground continuously
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
