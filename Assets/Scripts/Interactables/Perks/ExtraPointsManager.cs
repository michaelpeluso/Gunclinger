using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtraPointsManager : MonoBehaviour
{
    public float scoreMultiplyer;
    private float multiplyer;

    void OnApplcationStart() {
        multiplyer = 1f;
    }

    public void CollectPerk() {
        multiplyer = scoreMultiplyer;

        GameObject[] zombies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject zombie in zombies) {
            zombie.GetComponent<ZombieController>().scoreMultiplyer = multiplyer;
        }

        GameObject.Find("Enemy Spawners").GetComponent<ZombieSpawner>().scoreMultiplyer = multiplyer;
    }
}
