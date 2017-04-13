using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EmergingEnemy : MonoBehaviour {

	private Rigidbody2D rb;
	private PolygonCollider2D coll;
    private SpriteRenderer sp;

    public EnemySpawn spawner;

	[HideInInspector] public string spawnDirection;
	public float spawnSpeed;
    public float respawnLag;
	public float targetScale;
	private float spawnTime;
    public bool startFacingRight;
    private int initialDirection = 1;

	private bool autonomous = false;

	public float moveSpeed;
    public float maxSpeed = 5;
    private float lastPos;
	public float targetGravity;
	public bool canFly;
    public float attackDistance;
    private bool inBarrier = false;

    private Animator anim;

	// Use this for initialization
	void Start () {

		rb = GetComponent<Rigidbody2D>();
		coll = GetComponent<PolygonCollider2D>();
        sp = GetComponent<SpriteRenderer>();

		if (spawnDirection == "up" || spawnDirection == "down")
			transform.localScale = new Vector3(targetScale, targetScale, 1);
		else if (spawnDirection == "left" || spawnDirection == "right")
			transform.localScale = new Vector3(targetScale, targetScale, 1);
		else
			Debug.Log ("INVALID SPAWN DIRECTION");

        if (canFly)
            coll.isTrigger = true;


        spawnTime = Time.time;

        if (startFacingRight)
            initialDirection = -1;
        lastPos = transform.position.x;
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {

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
                if (pcPos.x < pos.x)
                {
                    if (transform.localScale.x < 0)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (pos.x < pcPos.x)
                {
                    if (transform.localScale.x > 0)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                if (inBarrier && xVelo > 0)
                    rb.velocity = new Vector2(0, yVelo);
            } else {
                if (rb.velocity.y < .05 && Mathf.Abs(diff.y) < 1)
                {
                    if (pcPos.x < pos.x)
                    {
                        if (transform.localScale.x < 0)
                            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                        rb.AddForce(new Vector2(-moveSpeed / Time.fixedDeltaTime, 0));
                    }
                    else if (pos.x < pcPos.x)
                    {
                        if (transform.localScale.x > 0)
                            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                        rb.AddForce(new Vector2(moveSpeed / Time.fixedDeltaTime, 0));
                    }
                    if (rb.velocity.x > maxSpeed)
                        rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
                    else if (rb.velocity.x < -maxSpeed)
                        rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
                    if (Mathf.Abs(pcPos.x - transform.position.x) <= attackDistance)
                        anim.SetBool("Attack", true);
                    else
                        anim.SetBool("Attack", false);
                }
                if (Mathf.Abs(pcPos.x - transform.position.x) <= attackDistance && Mathf.Abs(diff.y) < 1)
                    anim.SetBool("Attack", true);
                else
                    anim.SetBool("Attack", false);
                if (Mathf.Abs(lastPos - transform.position.x) / Time.fixedDeltaTime >= .4)
                    anim.SetBool("Chasing", true);
                else
                    anim.SetBool("Chasing", false);
                lastPos = transform.position.x;
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
				transform.localScale = new Vector3(targetScale * initialDirection, currScale, 1);
			else if (spawnDirection == "left" || spawnDirection == "right")
				transform.localScale = new Vector3(currScale * initialDirection, targetScale, 1);

		}
		
	}
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Death());
        }
        if (other.gameObject.CompareTag("Stop") && canFly)
        {
            inBarrier = true;
        }
    }

    IEnumerator Death()
    {
        sp.enabled = false;
        yield return new WaitForSeconds(respawnLag);
        spawner.alreadySpawned = false;
        gameObject.SetActive(false);
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Stop") && canFly)
        {
            inBarrier = false;
        }
    }
}
