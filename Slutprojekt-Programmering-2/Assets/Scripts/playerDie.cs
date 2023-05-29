using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerDie : MonoBehaviour
{
    private Vector3 respawnPoint;

	private void Start()
	{
        respawnPoint = transform.position;
	}
	public void Die() 
    {
        transform.position = respawnPoint;
        Debug.Log("Player died");
    }

    public void newRespawnPoint(Vector3 newRespawnPoint) 
    {
        respawnPoint = newRespawnPoint;
    }
}
