using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    private float platformSpeed;
    private Vector2 platformStartLocation;
    private Vector2 platformEndLocation = Vector2.zero;

    private void Start()
    {
        platformStartLocation = transform.position;
    }

	private void movePlatform() 
    {
        transform.position = Vector2.MoveTowards(platformStartLocation, platformEndLocation, platformSpeed);
    }
}
