using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneLoader : MonoBehaviour
{

    public Animator animator;
    private int newSceneIndex;
    public ZombieSpawner zombieSpawner;

    private void Awake() {
        if (GameObject.Find("UICanvas")) {
            DontDestroyOnLoad(GameObject.Find("UICanvas"));
        }
    }

    private void Start() {
        if (GameObject.Find("Enemy Spawners")) {
            zombieSpawner = GameObject.Find("Enemy Spawners").GetComponent<ZombieSpawner>();
        }
    }

    public void LoadNextScene(int index) {
        newSceneIndex = index;
        animator.SetTrigger("NewScene");

        if (zombieSpawner) {
            int temp = zombieSpawner.zombiesLeft + zombieSpawner.zombiesAlive;
            zombieSpawner.zombiesLeft = temp;
            zombieSpawner.zombiesAlive = 0;
            PlayerPrefs.SetInt("zombiesAlive", 0);
            PlayerPrefs.SetInt("zombiesLeft", temp);
        }
    }

    void OnFadeComplete() {
        SceneManager.LoadScene(newSceneIndex);
        
        GameObject Player = GameObject.Find("Player");
        if (Player) {
            Player.transform.position = new Vector3(PlayerPrefs.GetFloat("LastDoorLocation"), Player.transform.position.y, Player.transform.position.z);
        }
    }
}
