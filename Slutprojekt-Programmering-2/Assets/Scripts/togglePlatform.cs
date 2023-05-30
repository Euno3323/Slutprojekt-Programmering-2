using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class togglePlatform : MonoBehaviour
{
	[SerializeField] private float toggleDelay;

	private BoxCollider2D platformCollider;

	private void Start()
	{
		platformCollider = GetComponent<BoxCollider2D>();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.name == "Player")
		{
			StartCoroutine(toggle());
		}
	}

	private IEnumerator toggle() 
	{ 
		yield return new WaitForSeconds(toggleDelay);
		platformCollider.enabled = false;

		yield return new WaitForSeconds(toggleDelay);
		platformCollider.enabled = true;
	}
}
