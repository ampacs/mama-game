using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour {

    public string levelNameToLoad = null;

    void OnTriggerEnter2D(Collider2D collision) {
        Debug.Log("Triggerd!");
        SceneManager.LoadSceneAsync(levelNameToLoad);
    }
}
