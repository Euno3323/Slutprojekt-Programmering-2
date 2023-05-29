using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class obstacle : MonoBehaviour
{
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.gameObject.tag == "player") 
		{
			collision.gameObject.GetComponent<playerDie>().Die();
			Debug.Log("player touched the spikes");
		}
	}
}
