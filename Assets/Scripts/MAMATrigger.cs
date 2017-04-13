using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MAMATrigger : MonoBehaviour {
    private GameObject MAMA;

	// Use this for initialization
	void Start () {
        MAMA = GameObject.FindWithTag("MAMA");
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            MAMA.GetComponent<MAMAController>().chasing = true;
        }
    }
}
