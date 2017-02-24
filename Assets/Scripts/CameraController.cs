using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    // Here's a camera implementation based on boundry movement
    // Doesn't have to be used
    /*
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
    */


    // create new collision detector
    public CollisionController target;
    //use vector to represent size of focus area
    public Vector2 focusAreaSize;
    // hover camera over player
    public float verticalOffset;
    // different camera smoothing/looking ahead variables
    public float lookAheadDstX;
    public float lookSmoothTimeX;
    public float verticalSmoothTime;
    // create a new focusArea struct
    FocusArea focusArea;

    // more camera smoothing variables/ look ahead vars
    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;


    void Start()
    {
        // init focusArea
        focusArea = new FocusArea(target.boxCollider.bounds, focusAreaSize);
    }
    // update after player moves
    void LateUpdate()
    {
        focusArea.Update(target.boxCollider.bounds);

        //Put camera at focusPosition
        Vector2 focusPosition = focusArea.center + Vector2.up * verticalOffset;
        
        // If camera's x is moving, change dirX
        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
            // If moving in same direction
            if (Mathf.Sign(target.dirX) == Mathf.Sign(focusArea.velocity.x) && target.dirX != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }
            else
            {
                if (!lookAheadStopped)
                {
                    //compensation for opposing directions
                    lookAheadStopped = true;
                    targetLookAheadX = (currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX)) / 4;
                }
            }
        }

        //smooth Y
        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);

        //change look ahead depending on distance and dir
        targetLookAheadX = lookAheadDirX * lookAheadDstX;
        //smooth change
        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition += Vector2.right * currentLookAheadX;
        //make sure camera is in front
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }
    // View collision box
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(focusArea.center, focusAreaSize);
    }
    //define FocusArea
    struct FocusArea
    {
        public Vector2 center;
        float left, right;
        float top, bottom;
        public Vector2 velocity;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            //define borders, and center
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;
            velocity = Vector2.zero;
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
        }

        public void Update(Bounds targetBounds)
        {
            //determines amount camera should move if on edge
            float shiftX = 0;

            //If target moves more left past collision box
            if (targetBounds.min.x < left)
            {
                //move it over by difference between two
                shiftX = targetBounds.min.x - left;
            }

            //if target moves more right collision box
            else if (targetBounds.max.x > right)
            {
                //move it over by difference between two
                shiftX = targetBounds.max.x - right;
            }

            //move whole collision box by difference
            left += shiftX;
            right += shiftX;

            // Now find differences in Y
            float shiftY = 0;

            //If target moves lower past collision box
            if (targetBounds.min.y < bottom)
            {
                //move it over by difference between two
                shiftY = targetBounds.min.y - bottom;
            }

            //if target moves higher above collision box
            else if (targetBounds.max.y > top)
            {
                //move it over by difference between two
                shiftY = targetBounds.max.y - top;
            }

            //move whole collision box by difference
            top += shiftY;
            bottom += shiftY;
            // calculate new center
            center = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);


        }
    }


}
