using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public int wave;
    [Range(1, 3)] public int difficulty;
    public int zombiesLeft;
    public int zombiesAlive;
    public float timeBetweenSpawns;

    [Space (15)]
    public GameObject[] spawners;
    public GameObject zombie;
    private GameObject enemyParent;

    [Space (15)] //Perks
    public float scoreMultiplyer;
    public float critMultiplyer;

    private void OnApplicationStart() 
    {
        PlayerPrefs.SetInt("wave", 0);
        PlayerPrefs.SetInt("zombiesAlive", 0);
        PlayerPrefs.SetInt("zombiesLeft", 0);
        BeginWave(0, difficulty);
        scoreMultiplyer = 1f;
    }

    private void OnEnable()
    {
        zombiesLeft = PlayerPrefs.GetInt("zombiesAlive") + PlayerPrefs.GetInt("zombiesAlive");
        //PlayerPrefs.SetInt("zombiesAlive", 0);
        //PlayerPrefs.SetInt("zombiesLeft", zombiesLeft);
        wave = PlayerPrefs.GetInt("wave");
    }

    private void Start() {
        enemyParent = GameObject.Find("Enemies");
    }

    void Update()
    {
        //zombiesAlive = PlayerPrefs.GetInt("zombiesAlive");
        zombiesAlive = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (zombiesLeft == 0 && zombiesAlive == 0) {
            wave++;
            BeginWave(wave, difficulty);
        }
        else if (!IsInvoking("SpawnZombie") && zombiesLeft < 1) {
            InvokeRepeating("SpawnZombie", 0.0f, timeBetweenSpawns);
        }
    }

    void BeginWave(int wv, int dif) {
        timeBetweenSpawns = 3.0f / (float)dif;
        zombiesLeft = wv * dif * 5;
        InvokeRepeating("SpawnZombie", 0.0f, timeBetweenSpawns);
        PlayerPrefs.SetInt("wave", wv);
    }

    public void SpawnZombie() {
        if (zombiesLeft > 0) {
            int spawner = Mathf.RoundToInt(Random.value);
            GameObject newZombie = Instantiate(zombie, spawners[spawner].transform.position, Quaternion.identity);
            if (enemyParent == null) { enemyParent = GameObject.Find("Enemies"); }
            newZombie.transform.parent = enemyParent.transform;
            newZombie.GetComponent<ZombieController>().scoreMultiplyer = scoreMultiplyer;
            
            zombiesLeft--;
            PlayerPrefs.SetInt("zombiesLeft", zombiesLeft);
            PlayerPrefs.SetInt("zombiesAlive", PlayerPrefs.GetInt("zombiesAlive") + 1);
        }
        else {
            CancelInvoke();
            EndWave();
        }
    }

    public void EndWave() {

    }

    private void OnApplicationQuit() {
        PlayerPrefs.DeleteKey("wave");
        PlayerPrefs.DeleteKey("zombiesAlive");
        PlayerPrefs.DeleteKey("zombiesLeft");
    }
}
