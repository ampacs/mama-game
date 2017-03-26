using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require component to exist
[RequireComponent(typeof(CollisionController))]
public class PlayerController : MonoBehaviour
{
    /*CONTROLS:
    Up/Down or W/S control the camera, to look up/down
    Left/right or A/D = movement TODO: WASD support
    Space to jump
    Either ctrl key to crouch
    */
    //Height distance jumped, recommend to leave at 2f
    public float jumpHeight = 2f;
    //Time it takes for character to reach apex of jump, increase for floatiness, decrease for weight
    public float timeToJumpApex = .4f;
    //This is the base movementspeed (alternatively, max movementspeed), modify this to modify playerchar movement
    public float moveSpeed = 6;
    //This controls the speed/distance of lunge when in air
    public float airSpeedRatio = 1.25f;
    //This controls percent of movespeed when crawling
    public float crawlSpeedPercent = .50f;

    //Increase this to make the player more reactive when moving in an opposite direction, decrease to make them less able to correct movement
    float reactivityPercent = .25f;
    //Slipperiness in Air
    float accelerationTimeAirborne = .2f;
    //Slipperiness on Ground
    float accelerationTimeGrounded = .1f;
    //Gravity controllers so that descending occurs faster than accending (causes heaviness feeling)
    float gravityMultiplierDescending = 1.5f;
    float gravityMultiplierAscending = 1f;

    //Velocity the player will move at when letting go of jump button
    float unPressedVelocity = 2f;

    float gravity;
    float jumpVelocity;
    Vector3 velocity;
    public Vector3 crouchScale = new Vector3(0.79f, 0.3f, 1),
                   normalScale = new Vector3(0.29f, 0.5f, 1);
    float velocityXSmoothing;
    bool clambering = false;
    bool crouchAttempt = false;
    bool currentlyCrouched = false;

    bool centered = false;
    bool pullingUp = false;
    int clamberDir = 0;
    CollisionController controller;

    void Start()
    {
        controller = GetComponent<CollisionController>();

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        print("Gravity: " + gravity + "  Jump Velocity: " + jumpVelocity);
    }

    void Update()
    {
        //change player boundries (crouch attempt) if you let go when crouched or press down and not crouched
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
        {
            crouchAttempt = true;
        }
        if (currentlyCrouched && (Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)))
        {
            crouchAttempt = true;
        }
        if (controller.collisions.above || controller.collisions.below)
        {
            velocity.y = 0;
        }

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space) && controller.collisions.below && !currentlyCrouched)
        {
            velocity.y = jumpVelocity;
        }
        //Stop velocity when you let go of button
        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > unPressedVelocity)
        {
            velocity.y = unPressedVelocity;
        }
        //if there's room ahead to clamber (not being collided) but you're hugging a wall (colliding), you can clamber
        if ((!controller.clamberCollisions.left && controller.collisions.left || 
            !controller.clamberCollisions.right && controller.collisions.right) && !clambering && !currentlyCrouched)
        {
            if (controller.collisions.left)
                clamberDir = -1;
            else
                clamberDir = 1;

            clambering = true;
            pullingUp = false;
            centered = false;
        }
        //If you aren't touching anything, you're not clambering
        if (clambering && !controller.collisions.left && !controller.collisions.right)
        {
            clambering = false;
            centered = false;
            pullingUp = false;
        }
        //if you move opposite direction to the wall you're clambering, stop clambering
        if (input.x * clamberDir < 0)
        {
            clambering = false;
            centered = false;
            
        }
       
        if (clambering)
        {
            //push against wall to make raycasts register
            velocity.x = 2*clamberDir;
            //if your clambercollider hits something, you've moved down far enough to be centered
            if (controller.clamberCollisions.right && controller.collisions.right || controller.collisions.left && controller.clamberCollisions.left)
            {
                centered = true;
                velocity.y = 0;
            }
            //add some gravity while not centered to push you down (realistic grabbing --> weight)
            if (!centered)
                velocity.y += gravity * Time.deltaTime;
            //otherwise record a pullup call
            else if (input.x != 0 )
            {
                pullingUp = true;
               
            }
            // if pulling up, move the player
            if (pullingUp)
            {
                velocity.y = moveSpeed * Time.deltaTime * 20;
            }
            //actually move the character
            controller.Move(velocity * Time.deltaTime);
        }
        //not clambering motion

        else
        {
            if (crouchAttempt)
            {
                
                if (!currentlyCrouched && controller.collisions.below)
                {
                    //transform to make sure you dont phase through the floor
                    transform.position -= (Vector3.up * (normalScale.y / 2 - crouchScale.y / 2));
                    if (!controller.resize(crouchScale, normalScale))
                    {
                        currentlyCrouched = true;
                        
                    }
                    else
                    {
                        transform.position += (Vector3.up * (normalScale.y / 2 - crouchScale.y / 2));
                    }

                   
                    crouchAttempt = false;
                }
                else if (currentlyCrouched)
                {
                    transform.position += (Vector3.up * (normalScale.y/2 - crouchScale.y/2));
                    if (!controller.resize(normalScale, crouchScale))
                    {
                        currentlyCrouched = false;
                        crouchAttempt = false;

                    }
                    else
                    {
                        transform.position -= (Vector3.up * (normalScale.y / 2 - crouchScale.y / 2));
                        crouchAttempt = true;
                    }

                }
                else
                {
                    crouchAttempt = false;
                }
            }
            // use airspeedRatio to handle leaping in air
            float targetVelocityX = input.x * ((controller.collisions.below) ? moveSpeed : airSpeedRatio*moveSpeed) * ((currentlyCrouched) ? crawlSpeedPercent : 1);
            //handle biasing during movement velocity
            if (targetVelocityX * velocity.x < 0)
            {
                //add a reactivity to acceleration
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                    (controller.collisions.below) ? accelerationTimeGrounded - accelerationTimeGrounded * reactivityPercent
                    : accelerationTimeAirborne - accelerationTimeAirborne * reactivityPercent);

            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);
            }
            //add heavier feeling on gravity falling down
            velocity.y += gravity * Time.deltaTime * (velocity.y < 0 ? gravityMultiplierDescending : gravityMultiplierAscending); ;

            controller.Move(velocity * Time.deltaTime);
        }
        
    }


}

    /*
    //Change time to jump apex to affect floatiness vs heaviness
    public float jumpHeight = 4;
    public float timeToJumpApex = .4f;
    //Time for accelerations
    public float acceleration = 1;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float gravityMultiplierAscending = 1f;
    public float gravityMultiplierDescending = 1.5f;

    public float maximumMovementSpeed = 6;
    public float unPressedVelocity = 2f;
    public Vector3 crouchScale,
                   normalScale;

    bool jumpGrace;
    public float jumpCollisionGrace = .2f;
    float lastTimeCollided;
    //Declares velocity and gravity
    bool clambering,
         crouching;
    bool previousState;
    int currentClamberingPosition;
    int directionOfClamberableLedge;
    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    Vector2 direction;
    Vector3 velocity,
            velocityAdjusted,
            size;
    Vector3[] clamberingPositions;



    //Initialize player controller
    CollisionController collisionController;
    // Use this for initialization
    void Start()
    {
        //Init player controller from current player
        direction = new Vector3(1, 0, 0);
        clambering = false;
        crouching = false;
        previousState = false;  // standing, crouching
        currentClamberingPosition = 0;
        directionOfClamberableLedge = 0;
        collisionController = this.GetComponent<CollisionController>();
        size = GetComponent<BoxCollider2D>().size;
        clamberingPositions = new Vector3[2];
    }

    // Update is called once per frame
    void FixedUpdate()
    {


        //transform.rotation = Quaternion.identity;
        //This is temporarily here for adjustment while playing
        // Math to determine graviy based on height + time
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        previousState = crouching;

        if (collisionController.collisions.IsColliding(1))
        {
            lastTimeCollided = Time.time;
        }
        //Don't apply gravity when on ground
        if (collisionController.collisions.IsColliding(0) || collisionController.collisions.IsColliding(1))
        {
            velocity.y = 0;
        }
        //Get input vector from left/right buttons
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        // Jump if on ground TODO: Jump zone (not nessasarily on ground)

        jumpGrace = ((Time.time - lastTimeCollided) <= jumpCollisionGrace) && !(collisionController.collisions.IsColliding(1)) && (velocity.y == 0);
        if (Input.GetKey(KeyCode.Space) && (collisionController.collisions.IsColliding(1) || jumpGrace) && CanStandUp())
            velocity.y = jumpVelocity;

        //cut off velocity a bit if you stop pressing jump
        if (Input.GetKeyUp(KeyCode.Space) && velocity.y > -unPressedVelocity)
        {
            velocity.y = unPressedVelocity;
        }
<<<<<<< HEAD

        /*
        crouching = (Input.GetKey(KeyCode.DownArrow)) ? true : false;
=======
        crouching = (Input.GetKey(KeyCode.LeftControl)) ? true : false;
>>>>>>> parent of d5e60c2... Player Controller Updates
        if (!crouching && previousState && !CanStandUp())
        {
            crouching = true;
        }
        

        if (input.x != 0)
            direction.x = input.x;
        /*
        if (!clambering)
        {
            directionOfClamberableLedge = CheckClamberableLedge();
        }
        else if (!HasClimbed(directionOfClamberableLedge))
        {
            GetClamberingPositions(directionOfClamberableLedge);
        }
        if (directionOfClamberableLedge != 0 && direction.x == directionOfClamberableLedge)
            clambering = true;
        if (input.y == -1 || direction.x != directionOfClamberableLedge || HasClambered())
        {
            clambering = false;
            currentClamberingPosition = 0;
            directionOfClamberableLedge = 0;
        }
        //Dampen the change in x so it's smoother
        //TODO: Bias in change between switching directions
        //TODO: Max speed
        
        if (false)//(clambering)
        {
            transform.localScale = normalScale;
            velocity = Vector3.zero;
            if (input.y == 1 || input.x == directionOfClamberableLedge)
            {
                if (HasClimbed(directionOfClamberableLedge) && currentClamberingPosition < clamberingPositions.Length - 1)
                {
                    currentClamberingPosition++;
                    GetClamberingPositions(directionOfClamberableLedge, 0);
                }
                Vector3 targetVelocity = clamberingPositions[currentClamberingPosition] - transform.position;
                velocity = Vector3.Lerp(velocity, targetVelocity, 1f);
            }
        }
        
        else
        {
            /*
            if (crouching)
            {
                transform.localScale = crouchScale;
                input.x *= .5f;
            }
            else if (previousState)
            {
                transform.localScale = normalScale;
                transform.position += new Vector3(0, .5f, 0);
            }
<<<<<<< HEAD
            

            //give a different air speed for longer jumps
            float targetVelocityX = (collisionController.collisions.IsColliding(1)) 
                ? input.x * maximumMovementSpeed : input.x * maximumAirMovementSpeed;
            //If target and current are in opposite directions, (pos * neg = neg)
            if (targetVelocityX * velocity.x < 0)
            {
                //add a reactivity to acceleration
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                    (collisionController.collisions.IsColliding(1)) ? accelerationTimeGrounded + accelerationTimeGrounded * reactivityPercent
                    : accelerationTimeAirborne + accelerationTimeAirborne * reactivityPercent);

            }
            else
            {
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                                             (collisionController.collisions.IsColliding(1)) ? accelerationTimeGrounded : accelerationTimeAirborne);
            }
=======
            float targetVelocityX = input.x * maximumMovementSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                                         (collisionController.collisions.IsColliding(1)) ? accelerationTimeGrounded : accelerationTimeAirborne);
>>>>>>> parent of d5e60c2... Player Controller Updates
            //Modify velocity according to gravity
            velocity.y += gravity * Time.deltaTime * (velocity.y < 0 ? gravityMultiplierDescending : gravityMultiplierAscending);
        }
        //Move the player controller
        velocityAdjusted = collisionController.UpdateRaytracers(velocity * Time.deltaTime);
        transform.Translate(velocityAdjusted);
        */

/*

    public int CheckClamberableLedge()
    {
        Vector2 direction = Vector2.right;
        RaycastHit2D[] hitRight = { Physics2D.Raycast(transform.position + new Vector3(0, size.y / 1.99f, 0),      direction, size.x/3, collisionController.collisionMask),
                                    Physics2D.Raycast(transform.position + new Vector3(0, size.y / 1.50f, 0),      direction, size.x/3, collisionController.collisionMask)},
                       hitLeft = { Physics2D.Raycast(transform.position + new Vector3(0, size.y / 1.99f, 0), -1 * direction, size.x/3, collisionController.collisionMask),
                                    Physics2D.Raycast(transform.position + new Vector3(0, size.y / 1.50f, 0), -1 * direction, size.x/3, collisionController.collisionMask)};

        Debug.DrawRay(transform.position + new Vector3(0, size.y / 1.99f, 0), -1 * direction * size.x / 3, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, size.y / 1.50f, 0), -1 * direction * size.x / 3, Color.green);
        Debug.DrawRay(transform.position + new Vector3(0, size.y / 1.99f, 0), direction * size.x / 3, Color.yellow);
        Debug.DrawRay(transform.position + new Vector3(0, size.y / 1.50f, 0), direction * size.x / 3, Color.yellow);

        return (hitRight[0] && !hitRight[1] ? 1 : 0) - (hitLeft[0] && !hitLeft[1] ? 1 : 0);  // -1 -> Left; 1 -> Right; 0 -> None/Both
    }

    public bool HasClimbed(int direction)
    {
        Vector2 ledgeDirection = (direction == 1) ? Vector2.right : Vector2.left;
        RaycastHit2D[] hits = { Physics2D.Raycast(transform.position - new Vector3(0, size.y / 1.99f, 0), ledgeDirection, size.x/2),
                                Physics2D.Raycast(transform.position                                    , ledgeDirection, size.x/2)};

        Debug.DrawRay(transform.position - new Vector3(0, size.y / 1.99f, 0), ledgeDirection * size.x / 2, Color.green);
        Debug.DrawRay(transform.position, ledgeDirection * size.x / 2, Color.green);

        Debug.Log("*  " + (hits[0] == true));
        Debug.Log("** " + (hits[1] == true));
        return !hits[0] && !hits[1] ? true : false;
    }

    public bool HasClambered()
    {
        Vector2 direction = Vector2.down;
        RaycastHit2D[] hits = { Physics2D.Raycast(transform.position - new Vector3(transform.localScale.x * size.x / 1.99f, 0, 0), direction, size.y),
                                Physics2D.Raycast(transform.position + new Vector3(transform.localScale.x * size.x / 1.99f, 0, 0), direction, size.y)};

        Debug.DrawRay(transform.position - new Vector3(transform.localScale.x * size.x / 1.99f, 0, 0), direction * size.y, Color.green);
        Debug.DrawRay(transform.position + new Vector3(transform.localScale.x * size.x / 1.99f, 0, 0), direction * size.y, Color.green);

        return hits[0] && hits[1] ? true : false;
    }


    
    public bool CanStandUp()
    {
        /*
        Vector2 direction = Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, transform.localScale.y * size.y * .5f, 0), direction, normalScale.y - crouchScale.y);
        Debug.DrawRay(transform.position + new Vector3(0, transform.localScale.y * size.y * .5f, 0), direction * (normalScale.y - crouchScale.y), Color.cyan);
        return hit ? false : true;
        
        return true;
    }

    void GetClamberingPositions(int direction, float yOffset = 1f)
    {
        clamberingPositions[0] = transform.position + new Vector3(0, yOffset, 0);
        clamberingPositions[1] = clamberingPositions[0] + new Vector3(2 * size.x * direction, 0, 0);
    }

    public Vector3 GetPlayerDirection()
    {
        return direction;
    }

    public Vector3 GetPlayerVelocity()
    {
        return velocity;
    }

    public Vector3 GetPlayerTimeAdjustedVelocity()
    {
        return velocityAdjusted;
    }

    */
