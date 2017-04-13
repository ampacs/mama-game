using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Checkpoint : MonoBehaviour {

	private bool hasPassed; //true if player reached checkpoint

	//name of the scenes to restart at before and after the checkpoint was reached
	public string preCheckScene;
	public string postCheckScene;

	private GameObject player;

	public bool isLevelEnd;

	void Start () {

		player = GameObject.FindGameObjectWithTag ("Player");
		hasPassed = false;
		
	}

	void Update () {

		Debug.Log (hasPassed);

	}

	void OnTriggerEnter2D (Collider2D other) {

		if (other.gameObject == player) {

			hasPassed = true;

			if (isLevelEnd) {

				//automatically takes player to the next scene
				RestartLevel ();

			}

		}

	}

	//call this function when the player dies
	public void RestartLevel() {

		if (hasPassed) {

			SceneManager.LoadScene (postCheckScene);

		} else {

			SceneManager.LoadScene (preCheckScene);

		}

	}

}
