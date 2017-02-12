using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require component to exist
[RequireComponent(typeof (Controller2D))]
public class Player : MonoBehaviour {
    //Change time to jump apex to affect floatiness vs heaviness
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    //Time for accelerations
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float moveSpeed = 6;
    //Declares velocity and gravity
    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;


    //Initialize player controller
    Controller2D controller;
	// Use this for initialization
	void Start () {
       
        //Init player controller from current player
        controller = this.GetComponent<Controller2D>();
		
	}
	
	// Update is called once per frame
	void Update () {
        //This is temporarily here for adjustment while playing
        // Math to determine graviy based on height + time
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;

        //Don't apply gravity when on ground
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }
        //Get input vector from left/right buttons
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Jump if on ground TODO: Jump zone (not nessasarily on ground)
        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below)
        {
            velocity.y = jumpVelocity;
        }
        //Dampen the change in x so it's smoother
        //TODO: Bias in change between switching directions
        //TODO: Max speed
        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, 
                                     (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
        //Modify velocity according to gravity
        velocity.y += gravity * Time.deltaTime;
        //Move the player controller
        controller.Move(velocity * Time.deltaTime);
		
	}
}
