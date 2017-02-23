using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float smoothTime = 0.05f,
                 cameraOffsetPositionVelocityMultiplier = 0.3333f;
    public Vector3 cameraVelocityMultiplier = new Vector3(20f, 30f, 10f);
    Vector3 playerPosition,
            currentVelocity,
            velocity;
    GameObject player;

	// Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        currentVelocity = Vector3.zero;
        velocity = Vector3.zero;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 cameraVelocityTarget = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z) -
                                       transform.position;
        Vector3 cameraPositionOffset = player.GetComponent<PlayerController>().GetPlayerDirection();
        cameraPositionOffset.x += player.GetComponent<PlayerController>().GetPlayerVelocity().x * cameraOffsetPositionVelocityMultiplier;

        currentVelocity = Vector3.SmoothDamp(currentVelocity, cameraVelocityTarget + cameraPositionOffset, ref velocity, smoothTime);
        currentVelocity.Scale(cameraVelocityMultiplier);
        currentVelocity *= Time.deltaTime;
        transform.Translate(currentVelocity);
	}
}
