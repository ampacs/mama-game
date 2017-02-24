using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour {

    public static SoundManager instance = null;

    public AudioSource fxSource,
                       musicSource;

    void Awake() {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //if (SceneManager.GetActiveScene().name.Equals("Tutorial")) {
        //    Destroy(gameObject);
        //}
    }

    public void PlaySingle(AudioClip clip) {
        fxSource.clip = clip;
        fxSource.Play();
    }

	// Use this for initialization
	void Start () {
        // audioSource = GetComponent<AudioSource>();
	}

    void Update() {

    }
}
