using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{
    [Header("Object references")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;

    [Header("Walking and Jumping")]
    [SerializeField] private bool isFacingRight = true;
	[SerializeField] private float movementSpeed;
    [SerializeField] private float jumpingPower;
    [SerializeField] private float horizontal;

	[Header("Sliding and WallJumping")]
	[SerializeField] private bool isSliding;
	[SerializeField] private float slideSpeed;
	[SerializeField] private Vector2 wallJumpingPower;

    [Header("Dashing")]
	[SerializeField] private bool canDash = true;
	[SerializeField] private bool isDashing;
	[SerializeField] private float dashingPower;
	[SerializeField] private float dashingTime;
	[SerializeField] private float dashingCooldown;

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		horizontal = Input.GetAxisRaw("Horizontal");

		if (isDashing) 
        {
            return;
        }

		if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
		{
			Flip();
		}

		if (Input.GetButtonDown("Jump") && isGrounded())
        {
            Jump();
        }
        else if (Input.GetButtonDown("Jump") && isSliding) 
        {
            WallJump();
        }

        if (isWalled() && !isGrounded() && horizontal != 0)
        {
            Slide();
            isSliding = true;
        }
        else
        {
            isSliding = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        { 
            StartCoroutine(Dash());
        }
    }

	private void FixedUpdate()
	{
        if (isDashing) 
        {
            return;
        }
		rb.velocity = new Vector2(horizontal * movementSpeed, rb.velocity.y);   
	}

    private void Flip() 
    {
		isFacingRight = !isFacingRight;
		Vector3 localScale = transform.localScale;
		localScale.x *= -1f;
		transform.localScale = localScale;
	}

    private void Jump() 
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

	private bool isGrounded()
    {
        return Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.down, .1f, groundLayer);
    }

    private bool isWalled() 
    {
        if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.left, .1f, wallLayer))
        {
            return true;
        }
        else if (Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, Vector2.right, .1f, wallLayer))
        {
            return true;
        }
        else 
        {
            return false;
        }
    }

    private void Slide() 
    {
        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -slideSpeed, float.MaxValue));
    }

    private void WallJump() 
    {
	    rb.velocity = new Vector2(-transform.localScale.x * wallJumpingPower.x, wallJumpingPower.y);
    }

    private IEnumerator Dash() 
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);

        yield return new WaitForSeconds(dashingTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }
}
