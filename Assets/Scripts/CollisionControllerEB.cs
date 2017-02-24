//<<<<<<< HEAD:Assets/Scripts/Controller2D.cs
/*
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Requires box collider
[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {
    public LayerMask collisionMask;
    //Allows you to change the rays projected
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;
    //Variables for calculating spacing for rays
    float horizontalRaySpacing;
    float verticalRaySpacing;
    //Current width of skin
    const float skinWidth = .015f;
    // Create a collider
    public BoxCollider2D colliderControl;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

	void Awake () {
        //init collider from current controller
        colliderControl = this.GetComponent<BoxCollider2D>();
        //determine spacing for ray casting
        calculateRaySpacing();
    }

    public void Start()
    {

    }
    void Update()
    {


    }

    //Movement functions
    public void Move(Vector3 velocity)
    {
        collisions.Reset();
        //update raycast positions before moving
        updateRaycastOrigins();
        //Only check if moving in direction
        if (velocity.x != 0) { 
            //Check Horizontal Collisions
            HorizontalCollisions(ref velocity);
        }
        if (velocity.y != 0)
        {
            //Check for collisions on velocity
            VerticalCollisions(ref velocity);
        }
        //Translate the object according to velocity
        transform.Translate(velocity);
    }

    //Handles vertical collisions
    void VerticalCollisions(ref Vector3 velocity)
    {
        //Direction of velocity
        float directionY = Mathf.Sign(velocity.y);
        //Length of ray (plus skin), since inset in skin
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        //Draw all of the vertical rays in debug mode
        for (int i = 0; i < verticalRayCount; i++)
        {
            //If you're moving down, start with bottom left, else topleft
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //Sets offsets for each ray
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            //Detect a hit from each ray cast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            //Draw ray in debug mode
            Debug.DrawRay(rayOrigin,  Vector2.up * directionY * rayLength, Color.red);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.y = (hit.distance - skinWidth) * directionY;
                //Set to hit distance, to prevent clipping on edges
                rayLength = hit.distance;

                if (directionY == -1)
                {
                    collisions.below = true;
                    collisions.above = false;
                }
                else if (directionY == 1)
                {
                    collisions.above = true;
                    collisions.below = false;
                }
            }
        }

    }


    //Handles horizontal collisions
    void HorizontalCollisions(ref Vector3 velocity)
    {
        //Direction of velocity
        float directionX = Mathf.Sign(velocity.x);
        //Length of ray (plus skin), since inset in skin
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        //Draw all of the vertical rays in debug mode
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //If you're moving down, start with bottom left, else bottom right
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            //Sets offsets for each ray
            rayOrigin += Vector2.up * horizontalRaySpacing * i;
            //Detect a hit from each ray cast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Draw ray in debug mode
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.x = (hit.distance - skinWidth) * directionX;
                //Set to hit distance, to prevent clipping on edges
                rayLength = hit.distance;
                //If you were going in X direction, you collided with X side
                if (directionX == -1)
                {
                    collisions.left = true;
                    collisions.right = false;
                }
                else if (directionX == 1)
                {
                    collisions.right = true;
                    collisions.left = false;
                }
            }
        }

    }

    //Ray update functions
    void calculateRaySpacing()
    {
        //Get bounds, and shrink the skin
        Bounds bounds = colliderControl.bounds;
        bounds.Expand(skinWidth * -2);
        //Make a restriction so that raycounts are between 2 and max
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //Calculate even spacing
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    void updateRaycastOrigins()
    {
        //Get bounds of collider
        Bounds bounds = colliderControl.bounds;
        //shrink the skin by 0.2
        bounds.Expand(skinWidth * -2);

        //Define origins for rays
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
	
    //Define struct for different origins
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }
    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public void Reset()
        {
            above = below = false;
            left = right = false;
        }
    }
}
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Requires box collider
[RequireComponent(typeof(BoxCollider2D))]
public class CollisionControllerEB : MonoBehaviour {

    public LayerMask collisionMask;

    //Allows you to change the rays projected
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    //Variables for calculating spacing for rays
    float horizontalRaySpacing;
    float verticalRaySpacing;

    //keep track of current movement
    public float dirX;
    public float dirY;

    //Current width of skin
    const float skinWidth = .015f;

    // Create a collider
    public BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;

    void Start()
    {

    }

	void Awake () {
        //init collider from current controller
        boxCollider = this.GetComponent<BoxCollider2D>();
        collisions.Init(horizontalRayCount);
        //determine spacing for ray casting
        CalculateRaySpacing();
    }

    public Vector3 UpdateRaytracers(Vector3 velocity) {
        collisions.Reset();
        //update raycast positions before moving
        UpdateRaycastOrigins();
        //Only check if moving in direction
        //Check Horizontal Collisions
        HorizontalCollisions(ref velocity);
        //Check for collisions on velocity
        VerticalCollisions(ref velocity);
        //Translate the object according to velocity
        return velocity;
    }

    //Handles vertical collisions
    void VerticalCollisions(ref Vector3 velocity)
    {
        //Direction of velocity
        float directionY = Mathf.Sign(velocity.y);
        float dirY = directionY;
        //Length of ray (plus skin), since inset in skin
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;
        //Draw all of the vertical rays in debug mode
        for (int i = 0; i < verticalRayCount; i++)
        {
            //If you're moving down, start with bottom left, else topleft
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            //Sets offsets for each ray
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            //Detect a hit from each ray cast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            //Draw ray in debug mode
            Debug.DrawRay(rayOrigin,  Vector2.up * directionY * rayLength, Color.red);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.y = (hit.distance - skinWidth) * directionY;
                //Set to hit distance, to prevent clipping on edges
                rayLength = hit.distance;

                if (directionY == -1)
                {
                    collisions.below[i] = true;
                    collisions.above[i] = false;
                }
                else if (directionY == 1)
                {
                    collisions.above[i] = true;
                    collisions.below[i] = false;
                }
            }
        }

    }


    //Handles horizontal collisions
    void HorizontalCollisions(ref Vector3 velocity)
    {
        //Direction of velocity
        float directionX = Mathf.Sign(velocity.x);
        float dirX = directionX;
        //Length of ray (plus skin), since inset in skin
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;
        //Draw all of the vertical rays in debug mode
        for (int i = 0; i < horizontalRayCount; i++)
        {
            //If you're moving left, start with bottom left, else bottom right
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.topLeft : raycastOrigins.topRight;
            //Sets offsets for each ray
            rayOrigin += Vector2.down * horizontalRaySpacing * i;
            //Detect a hit from each ray cast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Draw ray in debug mode
            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.x = (hit.distance - skinWidth) * directionX;
                //Set to hit distance, to prevent clipping on edges
                rayLength = hit.distance;
                //If you were going in X direction, you collided with X side
                if (directionX == -1)
                {
                    collisions.left[i] = true;
                    collisions.right[i] = false;
                }
                else if (directionX == 1)
                {
                    collisions.right[i] = true;
                    collisions.left[i] = false;
                }
            }
        }

    }

    //Ray update functions
    void CalculateRaySpacing() {
        //Get bounds, and shrink the skin
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);

        //Make a restriction so that raycounts are between 2 and max
        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        //Calculate even spacing
        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
    void UpdateRaycastOrigins()
    {
        //Get bounds of collider
        Bounds bounds = boxCollider.bounds;
        //shrink the skin by 0.2
        bounds.Expand(skinWidth * -2);

        //Define origins for rays
        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }
	
    //Define struct for different origins
    struct RaycastOrigins {

        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }

    public struct CollisionInfo {

        public bool[] above, below,
                      left,  right;

        public void Init(int numberOfRays) {
            above = new bool[numberOfRays];
            below = new bool[numberOfRays];
            left  = new bool[numberOfRays];
            right = new bool[numberOfRays];
        }

        public void Reset() {
            for (int i = 0; i < above.Length; i++) {
                above[i] = false;
                below[i] = false;
                left[i]  = false;
                right[i] = false;
            }
        }

        public bool IsColliding() {
            bool[][] locations = { above, below, left, right };
            for (int i = 0; i < locations.Length; i++)
                for (int j = 0; j < locations[i].Length; j++) {
                    if (locations[i][j])
                        return true;
                }
            return false;
        }

        public bool IsColliding(int location) {
            bool[][] locations = { above, below, left, right };
            for (int i = 0; i < locations[location].Length; i++) {
                if (locations[location][i])
                    return true;
            }
            return false;
        }

        public bool[][] GetAllCollisionLocations() {
            return new bool[][]{ above, below, left, right };
        }

        public bool[] GetCollisionLocation(int location) {
            bool[][] locations = { above, below, left, right };
            return locations[location];
        }
    }
}
//>>>>>>> refs/remotes/origin/master:Assets/Scripts/CollisionController.cs
