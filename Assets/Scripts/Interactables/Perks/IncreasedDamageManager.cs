using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncreasedDamageManager : MonoBehaviour
{
    public float damageBoost;
    private GameObject[] bullets;
    public bool collected;

    void OnApplcationStart() {
        collected = false;
    }

    private void FixedUpdate() {
        if (collected) {
            bullets = GameObject.FindGameObjectsWithTag("bullet");
            foreach (GameObject bullet in bullets) {
                bullet.GetComponent<BulletHandler>().damageMultiplyer = damageBoost;
            }
        }
    }

    public void CollectPerk() {
        collected = true;
    }
}
