using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class playerMovement : MonoBehaviour
{
    [SerializeField]private float movementSpeed;
	[SerializeField]private float jumpingPower;
    private float horizontal;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) 
        {
            Jump();
        }
    }

	private void FixedUpdate()
	{
		rb.velocity = new Vector2(horizontal * movementSpeed, rb.velocity.y);   
	}

    private void Jump() 
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
    }

    private void isGrounded()
    { 
        Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0f, )
    }
}
