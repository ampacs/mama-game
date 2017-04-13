using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseTrigger : MonoBehaviour {
    public AudioSource noise;
    private bool NoiseMade;
	// Use this for initialization
	void Start () {
        NoiseMade = false;
	}
    void OnCollisionEnter2D(Collision2D other)
    {
        if (!NoiseMade && other.collider.CompareTag("Player"))
        {
            noise.Play();
            NoiseMade = true;
        }
    }
}
