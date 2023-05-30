using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerRespawn : MonoBehaviour
{
	[SerializeField] private Vector2 respawnPoint;

	private void Start()
	{
		respawnPoint = transform.position;
	}
	public void Die()
	{
		transform.position = respawnPoint;
		Debug.Log("Player died");
	}

	public void newRespawnPoint(Vector2 newRespawnPoint)
	{
		respawnPoint = newRespawnPoint;
	}
}
