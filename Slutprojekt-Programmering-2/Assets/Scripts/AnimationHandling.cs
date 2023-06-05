using UnityEngine;

namespace playerController 
{
	public class AnimationHandling : MonoBehaviour
	{
		[SerializeField]
		private SpriteRenderer spriteRenderer; // Sprite Renderer
		private Animator animator; // Animator Controller
		private Movement movement; // Movement-script
		private new Rigidbody2D rigidbody; // RigidBody2D

		private string currentState; // Current animation state

		// Animations...
		private string playerIdle = "Idle"; 
		private string playerRun = "Running"; 
		private string playerSlide = "Sliding";
		private string playerJump = "Jumping";
		private string playerFall = "Falling";

		// Runs on the first frame
		void Start()
		{
			// Gets references to components
			spriteRenderer = GetComponent<SpriteRenderer>();
			animator = GetComponent<Animator>();
			movement = GetComponent<Movement>();
			rigidbody = GetComponent<Rigidbody2D>();
		}

		// Runs every frame
		private void Update()
		{
			// Player is sliding and is not on the ground
			if (movement.isSliding && !movement.isGrounded)
			{
				// Sliding animation
				ChangeAnimationState(playerSlide);
			}

			// Player is on the ground and is running
			if (movement.isGrounded && movement.isRunning)
			{
				// Running animation
				ChangeAnimationState(playerRun);
			}

			// Player is on the ground and is not running
			if (movement.isGrounded && !movement.isRunning)
			{
				// Idle animation
				ChangeAnimationState(playerIdle);
			}

			// Player is in the air
			if (!movement.isGrounded && !movement.isWalled)
			{
				// Vertical velocity is positive (player is moving upwards)
				if (rigidbody.velocity.y > 0)
				{
					// Jump animation
					ChangeAnimationState(playerJump);
				}
				// Player is moving downwards
				else
				{
					// Fall animation
					ChangeAnimationState(playerFall);
				}
			}
		}

		// Changes animations state 
		private void ChangeAnimationState(string newState)
		{
			// Animation is already on
			if (currentState == newState)
			{
				// Does nothing
				return;
			}

			// Start playing new animation state
			animator.Play(newState);
			// Sets current animaton state to the new one
			currentState = newState;
		}
		
		// Flips the player sprite
		public void Flip(int side)
		{
			// Player is on a wall
			if (movement.isWalled)
			{
				// Player movement direction is to the the left and player has flipped (player is facing the left)
				if (side == -1 && spriteRenderer.flipX)
				{
					// Does nothing
					return;
				}
				// Player movement direction is the right and player has not flipped
				if (side == 1 && !spriteRenderer.flipX)
				{
					// Does nothing
					return;
				}
			}

			// Flips the player depending on the direction it is already facing
			bool state = (side == 1) ? false : true;
			spriteRenderer.flipX = state;
		}
	}
}


