using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour {

	//if true, only one enemy will spawn here
	public bool spawnOnce;

	//the direction that the enemy will grow in
	//e.g. if the spawn is on the ground, it should grow up
	//or if it's on the ceiling, it should grow down
	public string spawnDirection;
	public GameObject enemy;

	[HideInInspector] public bool alreadySpawned;
    private bool doneSpawning;
	public float spawnSpeed;
    public float respawnLag;
    public Vector2 spawnPoint;

	public float enemySpeed;
    public float enemyMaxSpeed;
	public float enemySize;
	public float enemyGravity;
	public bool enemyCanfly;
    public bool startFacingRight;

	// Use this for initialization
	void Start () {
        spawnPoint = new Vector2(transform.position.x + spawnPoint.x, transform.position.y + spawnPoint.y);
        doneSpawning = false;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerStay2D (Collider2D other) {

		if (other.tag == "Player" ) {

			if (!alreadySpawned && !doneSpawning) {
				
				GameObject spawnedEnemy = Instantiate (enemy, spawnPoint, Quaternion.identity);
				EmergingEnemy enemyScript = spawnedEnemy.GetComponent<EmergingEnemy> ();
                enemyScript.spawner = GetComponent<EnemySpawn>();
                enemyScript.startFacingRight = startFacingRight;
                enemyScript.spawnDirection = spawnDirection;
				enemyScript.moveSpeed = enemySpeed;
                enemyScript.maxSpeed = enemyMaxSpeed;
				//enemyScript.targetScale = enemySize;
				enemyScript.targetGravity = enemyGravity;
				enemyScript.canFly = enemyCanfly;
                enemyScript.spawnSpeed = spawnSpeed;
                enemyScript.respawnLag = respawnLag;
				alreadySpawned = true;
                if (spawnOnce)
                    doneSpawning = true;

			}

		}

	}

}
