using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

	//if true, only one enemy will spawn here
	//if false, an enemy will spawn every time the player enters the target zone
	public bool spawnOnce;

	//the direction that the enemy will grow in
	//e.g. if the spawn is on the ground, it should grow up
	//or if it's on the ceiling, it should grow down
	public string spawnDirection;
	public GameObject enemy;

	private bool alreadySpawned;
	public float spawnTime;
    public Vector2 spawnPoint;

	public float enemySpeed;
    public float enemyMaxSpeed;
	public float enemySize;
	public float enemyGravity;
	public bool enemyCanfly;

	// Use this for initialization
	void Start () {
        spawnPoint = new Vector2(transform.position.x + spawnPoint.x, transform.position.y + spawnPoint.y);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter2D (Collider2D other) {

		if (other.tag == "Player") {

			if (!spawnOnce || !alreadySpawned) {
				
				GameObject spawnedEnemy = Instantiate (enemy, spawnPoint, Quaternion.identity);
				EmergingEnemy enemyScript = spawnedEnemy.GetComponent<EmergingEnemy> ();
				enemyScript.spawnDirection = spawnDirection;
				enemyScript.moveSpeed = enemySpeed;
                enemyScript.maxSpeed = enemyMaxSpeed;
				enemyScript.targetScale = enemySize;
				enemyScript.targetGravity = enemyGravity;
				enemyScript.canFly = enemyCanfly;
				alreadySpawned = true;

			}

		}

	}

}
