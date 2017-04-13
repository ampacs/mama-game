using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDeath : MonoBehaviour {
    public float MinimumHeight = -50f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y < MinimumHeight)
			GameObject.FindGameObjectWithTag ("Checkpoint").GetComponent<Checkpoint> ().RestartLevel (); //this line of code should be called to restart
		
	}
}
