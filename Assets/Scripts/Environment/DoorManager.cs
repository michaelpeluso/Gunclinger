using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private GameObject Player;
    public int sceneIndex;
    public SceneLoader sceneLoader;

    private void OnApplicationStart() {
        PlayerPrefs.SetFloat("LastDoorLocation", 0);
    }

    private void Start() {
        Player = GameObject.Find("Player");
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (other.gameObject == Player) {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {

                if (sceneIndex == 0) {}
                else if (sceneIndex == 1) {
                    PlayerPrefs.SetFloat("LastDoorLocation", transform.position.x);
                }
                else if (sceneIndex == 2) {
                    PlayerPrefs.SetFloat("LastDoorLocation", transform.position.x);
                 }

                sceneLoader.LoadNextScene(sceneIndex);
            }
        }
    }

    private void OnApplicationQuit() {
        PlayerPrefs.DeleteKey("LastDoorLocation");
    }
}
