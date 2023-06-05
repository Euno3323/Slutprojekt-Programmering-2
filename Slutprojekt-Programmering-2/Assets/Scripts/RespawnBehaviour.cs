using UnityEngine;


public class RespawnBehaviour : MonoBehaviour
{
	[SerializeField]
	private Vector2 spawnPosition; // Respawnpoint

	// Runs on the first frame
	private void Start()
	{
		// Sets respawnpoint where the player starts
		spawnPosition = transform.position;
	}

	// Respawns the player
	public void Die() 
    {
		// Teleports the player to the respawnpoint
		transform.position = spawnPosition;
    }
}
