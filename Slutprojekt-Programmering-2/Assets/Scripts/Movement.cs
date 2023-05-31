using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class Movement : MonoBehaviour
{
	private Rigidbody2D rigidBody;
	private Collider2D collider;

	[SerializeField]
	private bool isWalled;
	[SerializeField]
	private bool isGrounded;
	[SerializeField]
	private bool hasDashed;
	[SerializeField]
	private bool isDashing;
	[SerializeField]
	private bool wallJumped;
	[SerializeField]
	private float runSpeed;
	[SerializeField]
	private float jumpPower;
	[SerializeField]
	private float dashSpeed;
	[SerializeField]
	private float climbSpeed;
	[SerializeField]
	private float wallSide;

	[SerializeField]
	private LayerMask groundLayer;
	[SerializeField]
	private LayerMask wallLayer;

    private void Start() 
    {
		rigidBody = GetComponent<Rigidbody2D>();
		collider = GetComponent<Collider2D>();
    }

	private void Update()
	{
		GroundHandling();

		WallHandling();

		float x = Input.GetAxis("Horizontal");
		float y = Input.GetAxis("Vertical");
		Vector2 dir = new Vector2(x, y);

		Run(dir);

		if (Input.GetButtonDown("Jump")) 
		{
			if (isGrounded)
			{
				Jump();
			}
			else if (isWalled) 
			{
				WallJump();
			}
		}

		float xRaw = Input.GetAxisRaw("Horizontal");
		float yRaw = Input.GetAxisRaw("Vertical");

		if (Input.GetButtonDown("Fire1") && !hasDashed) 
		{
			if (xRaw != 0 || yRaw != 0)
			{
				Dash(xRaw, yRaw);
			}
		}

		if (isWalled && !isGrounded) 
		{
			WallSlide(yRaw);
		}

		if (wallJumped && isGrounded) 
		{
			wallJumped = false;
		}
	}

	private void Run(Vector2 dir)
	{
		if (isDashing)
		{
			return;
		}
		else if (wallJumped)
		{
			rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, (new Vector2(dir.x * runSpeed, rigidBody.velocity.y)), 2f * Time.deltaTime);
		}
		else 
		{
			rigidBody.velocity = new Vector2(dir.x * runSpeed, rigidBody.velocity.y);
		}
	}

	private void Jump() 
	{
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
			rigidBody.velocity += Vector2.up * jumpPower;
	}

	private void WallJump() 
	{
		if (wallSide == 1)
		{
			wallJumped = true;
			rigidBody.velocity += (Vector2.up / 1.5f + Vector2.left / 1.5f) * jumpPower;
		}
		else if (wallSide == -1)
		{
			wallJumped = true;
			rigidBody.velocity += (Vector2.up / 1.5f + Vector2.right / 1.5f) * jumpPower;
		}
	}

	private void Dash(float x, float y) 
	{
		hasDashed = true;
		isDashing = true;

		rigidBody.velocity = Vector2.zero;

		Vector2 dir = new Vector2(x, y);

		StartCoroutine(DashWait());

		if (isDashing) 
		{
			rigidBody.velocity += dir.normalized * dashSpeed;
		}
	}

	IEnumerator DashWait() 
	{
		float gravity = rigidBody.gravityScale;
		rigidBody.gravityScale = 0;
		rigidBody.drag = 8;

		yield return new WaitForSeconds(.15f);

		rigidBody.drag = 0;
		rigidBody.gravityScale = gravity;
		isDashing = false;
	}

	private void WallSlide(float y) 
	{
		if (isDashing)
		{
			return;
		}
		else if (wallJumped) 
		{
			return;
		}
		else
		{
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, y * climbSpeed);
		}
	}

	IEnumerator wallJumpedWait() 
	{
		yield return new WaitForSeconds(.5f);

		wallJumped = false;
	}

	private void GroundHandling()
	{
		if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, .1f, groundLayer))
		{
			isGrounded = true;
			hasDashed = false;
		}
		else
		{
			isGrounded = false;
		}
	}

	private void WallHandling()
	{
		if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.right, .1f, wallLayer))
		{
			isWalled = true;
			hasDashed = false;
			wallSide = 1;
		}
		else if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.left, .1f, wallLayer))
		{
			isWalled = true;
			hasDashed = false;
			wallSide = -1;
		}
		else
		{
			isWalled = false;
		}
	}
}
