using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MAMAController : MonoBehaviour {

    public float defaultDistance = 15;
    public float defaultSpeed    = 10;
    public bool chasing = true;

    private AudioSource music;

    // distance trigger multipliers
    public float[] distanceThresholds;
    // determine the current max speed
    public float[] speedMults;
    // MAMA music volume at each corresponding distance
    public float[] soundVolume;

    float smoothTime;

    public Vector3 currentVelocity;

    public GameObject target;

    // Use this for initialization
    void Start() {
        // Get the Player GameObject
        target = GameObject.FindWithTag("Player");
        music = GameObject.FindWithTag("MAMA Music").GetComponent<AudioSource>();
        distanceThresholds = new float[] { 0.75f, 1.00f, 2.00f, 3.00f };
        speedMults         = new float[] { 0.50f, 1.00f, 2.00f, 3.00f };
        soundVolume        = new float[] { 0.90f, 0.60f, 0.30f, 0.00f };
        if (distanceThresholds.Length != speedMults.Length)
            throw new UnityException("distanceThresholds and speedMults do not share the length");
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (chasing)
        {
            float maxSpeed = defaultSpeed * speedMults[0];
            for (int i = 0; i < distanceThresholds.Length; i++)
                // if the distance between MAMA and the Player is above the set threshold
                if ((this.transform.position - target.transform.position).magnitude > defaultDistance * distanceThresholds[i])
                {
                    maxSpeed = defaultSpeed * speedMults[i];
                    music.volume = soundVolume[i];

                }
            smoothTime = Time.deltaTime;
            // translate MAMA
            this.transform.position = Vector3.SmoothDamp(this.transform.position, target.transform.position, ref currentVelocity, smoothTime, maxSpeed);
            if((this.transform.position.x - target.transform.position.x) > -1.5)
                GameObject.FindGameObjectWithTag("Checkpoint").GetComponent<Checkpoint>().RestartLevel();
        }
	}
}
