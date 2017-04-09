using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MAMAController : MonoBehaviour {

    public float defaultDistance = 15;
    public float defaultSpeed    = 10;

    // distance trigger multipliers
    public float[] distanceThresholds;
    // determine the current max speed
    public float[] speedMults;

    float smoothTime;

    public Vector3 currentVelocity;

    public GameObject target;

    // Use this for initialization
    void Start() {
        // Get the Player GameObject
        target = GameObject.FindWithTag("Player");
        distanceThresholds = new float[] { 0.75f, 1.00f, 2.00f, 2.00f };
        speedMults         = new float[] { 0.50f, 1.00f, 2.00f, 3.00f };
        if (distanceThresholds.Length != speedMults.Length)
            throw new UnityException("distanceThresholds and speedMults do not share the length");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float maxSpeed = defaultSpeed * speedMults[0];
        for (int i = 1; i < distanceThresholds.Length; i++)
            // if the distance between MAMA and the Player is above the set threshold
            if ((this.transform.position - target.transform.position).magnitude > defaultDistance * distanceThresholds[i])
                maxSpeed = defaultSpeed * speedMults[i];
        smoothTime = Time.deltaTime;
        // translate MAMA
        this.transform.position = Vector3.SmoothDamp(this.transform.position, target.transform.position, ref currentVelocity, smoothTime, maxSpeed);
	}

    void OnTriggerEnter2D(Collider2D other) {
        print(other.gameObject.tag);
        if (other.gameObject.CompareTag("Player")) {
            SceneManager.LoadSceneAsync("Tutorial-andre");
        }
    }
}
