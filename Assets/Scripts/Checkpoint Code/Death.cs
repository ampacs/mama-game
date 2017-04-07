using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (transform.position.y < -20.0f)
			GameObject.FindGameObjectWithTag ("Checkpoint").GetComponent<Checkpoint> ().RestartLevel (); //this line of code should be called to restart
		
	}
}
