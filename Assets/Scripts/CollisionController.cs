using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Requires box collider
[RequireComponent(typeof(BoxCollider2D))]
public class CollisionController : MonoBehaviour {

    public LayerMask collisionMask;

    //Allows you to change the rays projected
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    //change jump graces/clamberable conditions, higher number = easier clambers (I recommend you don't change this)
    public float clamberHeightMod = 1.25f;

    //Variables for calculating spacing for rays
    float horizontalRaySpacing;
    float verticalRaySpacing;

    //Current width of skin
    const float skinWidth = .015f;

    //directional values
    public float dirX;
    public float dirY;

    // Create a collider
    public BoxCollider2D boxCollider;
    RaycastOrigins raycastOrigins;
    public CollisionInfo collisions;
    public CollisionInfo clamberCollisions;

    CollisionInfo col;

    Vector3 zeroVector = new Vector3(0, 0, 0);

    float HorzMid;
    float VertMid;

    void Start()
    {

    }


	void Awake () {
        resized();
    }

    public void resized()
    {
        //init collider from current controller
        boxCollider = this.GetComponent<BoxCollider2D>();
        //collisions.Init(horizontalRayCount);
        //determine spacing for ray casting
        CalculateRaySpacing();
    }

    public bool resize(Vector2 size, Vector2 originalSize)
    {
        col.Reset();
        Bounds bounds = boxCollider.bounds;
        //bounds.Expand(skinWidth * -2);
        HorzMid = bounds.size.x / 2;
        VertMid = bounds.size.y;

        Vector3 oldSize = size;
        size.x = -HorzMid + size.x / 2;
        size.y = -VertMid + size.y;
        if (size.x < 0)
        {
            size.x = 0;
        }
        if (size.y < 0)
        {
            size.y = 0;
        }
        Vector3 newSize = (Vector3)size;
        Vector3 negSize = Vector3.left * newSize.x + (Vector3.down * newSize.y);
        
        
        
        col = VerticalCollisions(ref newSize, 0, col);
        if (checkForCollisions(col))
        {
            return true;
        }
        
        //no point in checking underneath since you'll only have problems above you
       // col = VerticalCollisions(ref negSize, 0, col);
        //if (checkForCollisions(col))
        //{
         //   return true;
        //}
        
        
        
        
        
        
        col = HorizontalCollisions(ref newSize, new Vector2(0, 0), col);
        if (checkForCollisions(col))
        {
            return true;
        }
        col = HorizontalCollisions(ref negSize, new Vector2(0, 0), col);
        if (checkForCollisions(col))
        {
            return true;
        }
        transform.localScale = oldSize;
        resized();
        return false;
        

    }

    public Vector3 UpdateRaytracers(Vector3 velocity) {
        collisions.Reset();
        //update raycast positions before moving
        UpdateRaycastOrigins();

        //Get bounds, and shrink the skin
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        //determine up offset to see if you can clamber over
        Vector2 ClamberOffset = new Vector2(0, bounds.size.y * clamberHeightMod);
        clamberCollisions = HorizontalCollisions(ref velocity, ClamberOffset, clamberCollisions);
        
        //Check Horizontal Collisions
        collisions = HorizontalCollisions(ref velocity, zeroVector, collisions);
        //Check for collisions on velocity
        collisions = VerticalCollisions(ref velocity, 0, collisions);
        //Translate the object according to velocity
        return velocity;
    }

    //Handles vertical collisions
    CollisionInfo VerticalCollisions(ref Vector3 velocity, float offset, CollisionInfo col)
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
           // if (i == 0)
             //   print(velocity.y);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.y = (hit.distance - skinWidth) * directionY;
                //Set to hit distance, to prevent clipping on edges
                
                col.below = directionY == -1;
                col.above = directionY == 1;
                /*
                if (directionY == -1)
                {
                    //hit bottom
                    collisions.below[i] = true;
                    collisions.above[i] = false;
                }
                else if (directionY == 1)
                {
                    //hit top
                    collisions.above[i] = true;
                    collisions.below[i] = false;
                }
                */
                rayLength = hit.distance;
            }
        }
        return col;

    }

    void ClamberCollisions(ref Vector3 Velocity)
    {
    }

    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        clamberCollisions.Reset();
        //Determine offset for clambering

        //Get bounds, and shrink the skin
        Bounds bounds = boxCollider.bounds;
        bounds.Expand(skinWidth * -2);
        //determine up offset to see if you can clamber over
        Vector2 ClamberOffset = new Vector2(0, bounds.size.y * clamberHeightMod);

        //If you've moved in x direction, check collisions horizontally
        if (velocity.x != 0)
        {
            //don't pass clamber the real velocity since don't want it to be modified
            Vector3 velocityCopy = velocity;
            collisions = HorizontalCollisions(ref velocity, zeroVector, collisions);
            //check for clambering conditions (Considered a correct clamber if you can move up a certain distance so that there's no left collision but normally is left collision)
            clamberCollisions = HorizontalCollisions(ref velocityCopy, ClamberOffset, clamberCollisions);
        }
        if (velocity.y != 0)
        {
            collisions = VerticalCollisions(ref velocity, 0, collisions);
        }

        transform.Translate(velocity);
    }

    //Handles horizontal collisions
    CollisionInfo HorizontalCollisions(ref Vector3 velocity, Vector2 offset, CollisionInfo col)
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
            Vector2 rayOrigin = (directionX == -1) ? (raycastOrigins.topLeft + offset) : (raycastOrigins.topRight + offset);
            //Sets offsets for each ray
            rayOrigin += Vector2.down * horizontalRaySpacing * i;
            //Detect a hit from each ray cast
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Draw ray in debug mode
            if (offset != (Vector2)zeroVector)
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);
            else
                Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.blue);
            //act according to collision if hit
            if (hit)
            {
                //Bounce off, making sure to count for ignored skin
                velocity.x = (hit.distance - skinWidth) * directionX;
                //Set to hit distance, to prevent clipping on edges
                rayLength = hit.distance;
                //If you were going in X direction, you collided with X side
                col.left = directionX == -1;
                col.right = directionX == 1;
                /*
                if (directionX == -1)
                {
                    //hit left side
                    collisions.left[i] = true;
                    collisions.right[i] = false;
                }
                else if (directionX == 1)
                {
                    //hit right side
                    collisions.right[i] = true;
                    collisions.left[i] = false;
                }
                */
            }
        }
        return col;
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

    bool checkForCollisions(CollisionInfo col)
    {
        return (col.above || col.below || col.right || col.left);
    }
	
    //Define struct for different origins
    struct RaycastOrigins {

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


    public struct CollisionInfoArray {

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
