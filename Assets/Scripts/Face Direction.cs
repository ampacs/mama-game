using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceDirection : MonoBehaviour {

    private bool facingRight = true;
    float lastPos = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        float moved = transform.position.x - lastPos;
        if (moved > 0)
            facingRight = true;
        else
            facingRight = false;
        lastPos = transform.position.x;
	}
}
