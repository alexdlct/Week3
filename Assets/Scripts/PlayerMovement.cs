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
    bool canDash = true;  

    bool isFacingRight = true;
    InputSystem_Actions controls;

    Animator anim;

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
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        if(!isDashing){
            Move(direction);
        }
        Debug.Log("Is Grounded: " + isGrounded);
        /*if((isFacingRight && direction < 0) || (isFacingRight && direction > 0)){
            Flip();
        }*/
    }

    void OnMove(InputValue value)
    {
        float v = value.Get<float>();
        direction = v;
        Debug.Log("Direction: " + direction);
    }

    void Move(float dir)
    {
        rb.linearVelocity = new Vector2(dir * speed, rb.linearVelocity.y);
        anim.SetBool("isRunning", dir != 0); //if the object is moving, its running
        Debug.Log("Rididbody velocity: " + rb.linearVelocity);
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
        rb.linearVelocity = new Vector2(direction * dashForce, 0); 
        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;  // Restore gravity
        isDashing = false;

        yield return new WaitForSeconds(dashCD);
        canDash = true;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckIfGrounded(collision);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
      CheckIfGrounded(collision);
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("ground")){
            isGrounded = false;
        }
    }

    void CheckIfGrounded(Collision2D collision){
        if(collision.gameObject.CompareTag("ground")){
            for(int i = 0; i < collision.contactCount; i++){
                if(Vector2.Angle(collision.GetContact(i).normal, Vector2.up) < 45f){
                    isGrounded = true;
                    return;
                }
            }
        }
    }


    private void Flip (){
        isFacingRight = !isFacingRight;
        Vector3 newLocalScale = transform.localScale;
        newLocalScale.x *= -1f;
        transform.localScale = newLocalScale;
    }

    private void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Collectible")){
            Destroy(collision.gameObject);
        }
    }
}
