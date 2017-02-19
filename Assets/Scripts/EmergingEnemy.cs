using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmergingEnemy : MonoBehaviour {

	private Rigidbody2D rb;
	private BoxCollider2D coll;

	[HideInInspector] public string spawnDirection;
	public float spawnSpeed;
	public float targetScale;
	private float spawnTime;

	private bool autonomous = false;

	public float moveSpeed;
	public float targetGravity;
	public bool canFly;

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody2D> ();
		coll = GetComponent<BoxCollider2D> ();

		if (spawnDirection == "up" || spawnDirection == "down")
			transform.localScale = new Vector2(targetScale, 0.0f);
		else if (spawnDirection == "left" || spawnDirection == "right")
			transform.localScale = new Vector2(0.0f, targetScale);
		else
			Debug.Log ("INVALID SPAWN DIRECTION");

		spawnTime = Time.time;
		
	}
	
	// Update is called once per frame
	void Update () {

		if (autonomous) {

			GameObject player = GameObject.FindGameObjectWithTag ("Player");
			Vector2 pcPos = player.transform.position;
			Vector2 pos = this.gameObject.transform.position;
			Vector2 diff = pcPos - pos;

			if (canFly) {
				float d = Mathf.Sqrt ((diff.x * diff.x) + (diff.y * diff.y));
				float xVelo = diff.x * (moveSpeed / d);
				float yVelo = diff.y * (moveSpeed / d);
				rb.velocity = new Vector2(xVelo, yVelo);
			} else {
				if (pcPos.x < pos.x)
					rb.AddForce(new Vector2 (-moveSpeed / Time.fixedDeltaTime, 0));
				else if (pos.x < pcPos.x)
					rb.AddForce(new Vector2 (moveSpeed / Time.fixedDeltaTime, 0));
			}

		} else {

			float currScale = (Time.time - spawnTime) * spawnSpeed;

			if (currScale >= targetScale) {
				currScale = targetScale;
				autonomous = true;
				if (!canFly)
					rb.gravityScale = targetGravity;
			}

			if (spawnDirection == "up" || spawnDirection == "down")
				transform.localScale = new Vector2(targetScale, currScale);
			else if (spawnDirection == "left" || spawnDirection == "right")
				transform.localScale = new Vector2(currScale, targetScale);

		}
		
	}
}
