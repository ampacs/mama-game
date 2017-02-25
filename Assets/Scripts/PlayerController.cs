using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Require component to exist
[RequireComponent(typeof(CollisionController))]
public class PlayerController : MonoBehaviour
{
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
        crouching = (Input.GetKey(KeyCode.LeftControl)) ? true : false;
        if (!crouching && previousState && !CanStandUp())
        {
            crouching = true;
        }

        if (input.x != 0)
            direction.x = input.x;
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
        if (clambering)
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
            float targetVelocityX = input.x * maximumMovementSpeed;
            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing,
                                         (collisionController.collisions.IsColliding(1)) ? accelerationTimeGrounded : accelerationTimeAirborne);
            //Modify velocity according to gravity
            velocity.y += gravity * Time.deltaTime * (velocity.y < 0 ? gravityMultiplierDescending : gravityMultiplierAscending);
        }
        //Move the player controller
        velocityAdjusted = collisionController.UpdateRaytracers(velocity * Time.deltaTime);
        transform.Translate(velocityAdjusted);

    }

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
        Vector2 direction = Vector2.up;
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, transform.localScale.y * size.y * .5f, 0), direction, normalScale.y - crouchScale.y);
        Debug.DrawRay(transform.position + new Vector3(0, transform.localScale.y * size.y * .5f, 0), direction * (normalScale.y - crouchScale.y), Color.cyan);
        return hit ? false : true;
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
}