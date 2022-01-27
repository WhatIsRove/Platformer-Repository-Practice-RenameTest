using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [SerializeField] private float jumpVelocity = 40f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private LayerMask lmGround;
    [Range(0f, 0.3f)][SerializeField] private float movementSmoothening = 0.3f;

    private float jumpPressedRemember = 0f;
    [SerializeField] private float jumpPressedRememberTime = 0.2f;
    [Range(0f, 1f)][SerializeField] private float jumpCutOff = 0.5f;
    [SerializeField] private float mGravityScale = 4f;
    private float wasGroundedRemember = 0f;
    [SerializeField] private float wasGroundedRememberTime = 0.2f;

    private float horizontalMove;
    private bool bFacingRight = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector3 velocity = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); //when it wakes / loads grabs the rigidbody2d of the object the script is attached to.
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal"); // just gets when ever A or D is pressed and returns -1 to 1, unity has preset buttons like "Fire1" or "Horizontal" / "Vertical"

        Vector2 groundedCheckPosition = (Vector2)transform.position + new Vector2(0, -0.1f);
        Vector2 groundedCheckScale = (Vector2)transform.localScale + new Vector2(-0.2f, 0);
        bool bGrounded = Physics2D.OverlapBox(groundedCheckPosition, groundedCheckScale, 0, lmGround);

        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));

        wasGroundedRemember -= Time.deltaTime;
        if(bGrounded)
        {
            wasGroundedRemember = wasGroundedRememberTime;
        }

        jumpPressedRemember -= Time.deltaTime;
        if (Input.GetButtonDown("Jump") && rb.velocity.y <= 0)
        {
            jumpPressedRemember = jumpPressedRememberTime;
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * jumpCutOff);
        }

        if ((wasGroundedRemember > 0) && (jumpPressedRemember > 0))
        {
            wasGroundedRemember = 0;
            jumpPressedRemember = 0;
            rb.velocity = new Vector2(rb.velocity.x, jumpVelocity);
        }

        //flips direction you move to
        if (horizontalMove > 0 && !bFacingRight)
        {
            Flip();
        }

        if (horizontalMove < 0 && bFacingRight)
        {
            Flip();
        }

        //Fast fall
        if (rb.velocity.y < 0.2f) rb.gravityScale = mGravityScale * 2f;
        else rb.gravityScale = mGravityScale;
    }

    private void FixedUpdate()
    {
        //transform.position += new Vector3(horizontalMove, 0, 0) * moveSpeed * Time.fixedDeltaTime; <-- this is a basic move them toward that direction as a Vector3 (x, y, z)
        Vector3 targetVelocity = new Vector2(horizontalMove * moveSpeed * Time.fixedDeltaTime * 10f, rb.velocity.y); //same thing as above but separates it into velocity to look toward

        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothening); //and then slowly smoothening it toward that target velocity from your current

    }

    private void Flip()
    {
        bFacingRight = !bFacingRight;

        transform.Rotate(0f, 180f, 0f);
    }
}
