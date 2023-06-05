using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{
	// Collider (collision) enters spikes collider
	private void OnTriggerEnter2D(Collider2D collision)
	{
		// If parentobjects tag is "Player"
		if (collision.gameObject.tag == "Player")
		{
			// Activates Die function for the player
			collision.gameObject.GetComponent<RespawnBehaviour>().Die();
		}
		else 
		{
			// Does nothing
			return;
		}
	}
}
