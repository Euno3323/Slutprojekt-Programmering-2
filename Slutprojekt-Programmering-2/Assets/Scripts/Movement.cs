using System.Collections;
using UnityEngine;

namespace playerController
{
	public class Movement : MonoBehaviour
	{
		[Header("Object-references")]
		private Rigidbody2D rigidBody; // RigidBody2D
		private new Collider2D collider; // Collider
		private AnimationHandling anim; // Animation-script

		[Space]

		[Header("Bools")]
		[SerializeField]
		public bool isRunning; // Player is running
		[SerializeField]
		public bool isWalled; // Player is against a wall
		[SerializeField]
		public bool isSliding; // Player is sliding
		[SerializeField]
		public bool isGrounded; // Player is on the ground
		[SerializeField]
		private bool hasDashed; // Player has recently dashed
		[SerializeField]
		private bool isDashing; // Player is dashing
		[SerializeField]
		private bool wallJumped; // Player has recently dashed

		[Space]

		[Header("Stats")]
		[SerializeField]
		private float runSpeed; // Running speed
		[SerializeField]
		private float jumpPower; // Jump power
		[SerializeField]
		private float dashSpeed; // dash power
		[SerializeField]
		private float climbSpeed; // Climbing speed

		[Space]

		[Header("Other")]
		[SerializeField]
		private int wallSide; // Direction a wall is compared to the player
		[SerializeField]
		private int side; // Direction player is running

		[Space]

		[Header("Layers")]
		[SerializeField]
		private LayerMask groundLayer; // Layer for ground

		// Run on the first frame
		private void Start()
		{
			// Gets references to components
			rigidBody = GetComponent<Rigidbody2D>();
			collider = GetComponent<Collider2D>();
			anim = GetComponent<AnimationHandling>();
		}

		// Runs every frame
		private void Update()
		{
			// Checks if the player is standing on ground
			GroundHandling();
			// Checks if the player is on a wall
			WallHandling();

			// Get the users input and stores it in x and y, value between -1 and 1 
			float x = Input.GetAxis("Horizontal");
			float y = Input.GetAxis("Vertical");
			// Stores the values in a vector2
			Vector2 dir = new Vector2(x, y);

			// Player runs in the direction of the user input
			Run(dir);

			// User clicks space
			if (Input.GetButtonDown("Jump"))
			{
				// Player is on the ground
				if (isGrounded)
				{
					// Player jumps
					Jump();
				}
				// Player is on a wall and not on the ground
				else if (isWalled && !isGrounded)
				{
					// Player walljumps
					WallJump();
				}
			}

			// Gets exact user input and stores in xRaw and yRaw, value of -1, 0 or 1
			float xRaw = Input.GetAxisRaw("Horizontal");
			float yRaw = Input.GetAxisRaw("Vertical");

			// User clicks lmb, has not dashed recently and is not on a wall
			if (Input.GetButtonDown("Fire1") && !hasDashed && !isWalled)
			{
				// There is user input
				if (xRaw != 0 || yRaw != 0)
				{
					// Player dashes
					Dash(xRaw, yRaw);
				}
			}

			// Player is on a wall, not on the ground and has not walljumped
			if (isWalled && !isGrounded && !wallJumped)
			{
				// Player is sliding
				isSliding = true;
				WallSlide(yRaw);
			}
			else
			{
				// Player is not sliding
				isSliding = false;
			}

			// Player has walljumped and is on the ground
			if (wallJumped && isGrounded)
			{
				// Walljump bool is set to false
				wallJumped = false;
			}

			// Player is moving to the right and is not sliding 
			if (x > 0 && !isSliding)
			{
				// Player looks to the right
				side = 1;
				anim.Flip(side);
			}
			// Player moving to the left and is not sliding
			if (x < 0 && !isSliding)
			{
				// Player looks to the left
				side = -1;
				anim.Flip(side);
			}
			// Player is moving and is not dashing or sliding
			if (x != 0 && !isDashing && !isSliding)
			{
				// isRunning bool for running animation is true
				isRunning = true;
			}
			else
			{
				// isRunning bool for running animation is false
				isRunning = false;
			}
		}

		// Player running
		private void Run(Vector2 dir)
		{
			// Player is dashing
			if (isDashing)
			{
				// Does nothing
				return;
			}
			// Player has recently walljumped
			else if (wallJumped)
			{
				// Player movement slowly increases with the help of linear interpolation
				rigidBody.velocity = Vector2.Lerp(rigidBody.velocity, (new Vector2(dir.x * runSpeed, rigidBody.velocity.y)), 4f * Time.deltaTime);
			}
			else
			{
				// Player moves in the direction of horizontal input of the user, y velocity is unaffected
				rigidBody.velocity = new Vector2(dir.x * runSpeed, rigidBody.velocity.y);
			}
		}

		// Player jump 
		private void Jump()
		{
			// y-velocity is set to zero
			rigidBody.velocity = new Vector2(rigidBody.velocity.x, 0);
			// Direction and power of the jump is set
			Vector2 force = Vector2.up * jumpPower;
			// Power is applied to the player along the direction of the vector
			rigidBody.AddForce(force, ForceMode2D.Impulse);
		}

		// Player walljump
		private void WallJump() 
		{
			// The wall is on the right
			if (wallSide == 1)
			{
				// Direction and power of the walljump in the opposite direction of the wall
				Vector2 force = (Vector2.up / 1.5f + Vector2.left / 1.5f) * jumpPower;
				rigidBody.AddForce(force, ForceMode2D.Impulse);

				// Player looks in the opposite direction of the wall
				side = wallSide * -1;
				anim.Flip(side);

				StartCoroutine(WallJumpedWait());
			}
			// The wall is on the left
			else if (wallSide == -1)
			{
				Vector2 force = (Vector2.up / 1.5f + Vector2.right / 1.5f) * jumpPower;
				rigidBody.AddForce(force, ForceMode2D.Impulse);

				side = wallSide * -1;
				anim.Flip(side);

				// Couroutine to limit player movement after a walljump
				StartCoroutine(WallJumpedWait());
			}
		}

		// Couroutine to limit player movement after a walljump
		IEnumerator WallJumpedWait()
		{
			wallJumped = true;

			// Waits for 0.5 seconds before player can move normaly
			yield return new WaitForSeconds(.5f);

			wallJumped = false;
		}

		// Player dash
		private void Dash(float x, float y)
		{
			// Sets dash booleans to true
			hasDashed = true;
			isDashing = true;

			// Sets the player velocity to zero
			rigidBody.velocity = Vector2.zero;

			// Creates new direction depending on user input
			Vector2 force = new Vector2(x, y).normalized * dashSpeed;

			// Couroutine for dash
			StartCoroutine(DashWait());

			// While isDashing, a force is applied to the player
			if (isDashing)
			{
				rigidBody.AddForce(force, ForceMode2D.Impulse);
			}
		}

		// IEnumerator to handle dash time, gravity and drag
		IEnumerator DashWait()
		{
			// Saves current gravity
			float gravity = rigidBody.gravityScale;
			// Sets player gravity to zero
			rigidBody.gravityScale = 0;
			// Sets the drag to 8 (Used to make dash feel faster but still as short)
			rigidBody.drag = 8;

			// Waits for 0.15 seconds
			yield return new WaitForSeconds(.15f);

			// Sets drag to zero
			rigidBody.drag = 0;
			// Sets player gravity to original value
			rigidBody.gravityScale = gravity;
			// Player is no longer dashing
			isDashing = false;
		}

		// Player wallslide
		private void WallSlide(float y)
		{
			// Player is dashing
			if (isDashing)
			{
				// Does nothing
				return;
			}
			// Player has walljumped
			else if (wallJumped)
			{
				// Does nothing
				return;
			}
			else
			{
				// Updates the players vertical velocity, however the horizontal velocity is still the same 
				rigidBody.velocity = new Vector2(rigidBody.velocity.x, y * climbSpeed);
			}

			// Player running direction is equal to the direction of the wall
			if (wallSide == side)
			{
				// Flips the players sprite
				anim.Flip(side * -1);
			}
		}

		// Ground detection
		private void GroundHandling()
		{
			// Boxcast detects a collider with groundLayer under the player
			if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, .1f, groundLayer))
			{
				// Player is on the ground
				isGrounded = true;
				// Player can dash again
				hasDashed = false;
			}
			else
			{
				// Player is not on the ground
				isGrounded = false;
			}
		}

		// Wall detection
		private void WallHandling()
		{
			// Boxcast detects a collier on the left or right..
			if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.right, .1f, groundLayer))
			{
				// Player is on a wall
				isWalled = true;
				// Player can dash again
				hasDashed = false;
				// The wall is on the right side of the player
				wallSide = 1;
			}
			else if (Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.left, .1f, groundLayer))
			{
				isWalled = true;
				hasDashed = false;
				// The wall is on the left side of the player
				wallSide = -1;
			}
			else
			{
				// Player is not on a wall
				isWalled = false;
			}
		}
	}
}

