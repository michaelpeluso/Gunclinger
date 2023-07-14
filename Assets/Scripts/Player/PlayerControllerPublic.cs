using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;

public class PlayerControllerPublic : MonoBehaviour
{
    [HideInInspector] public GameObject[] inv = new GameObject[3];
    [HideInInspector] public bool spriteIsFlipped;
    [HideInInspector] public int currEqupt;
    [HideInInspector] public List<string> perks;

    [HideInInspector] public float playerHealth;
    [HideInInspector] public float playerEnergy;
    [HideInInspector] public float maxHealth;
    [HideInInspector] public float maxEnergy;
    [HideInInspector] public float healthRegenDelay;
    [HideInInspector] public float energyRegenDelay;
    [HideInInspector] public float energyRegenStep;
    [HideInInspector] public GameObject damageIndicator;
    [HideInInspector] public GameObject healthBar;

    private void Start() 
    {
        CheckDoubles();
        UpdateInv();
        if (playerHealth != maxHealth) { playerHealth = maxHealth; }
    }

    public void TakeDamage(float damage) 
    {
        playerHealth -= damage;
        if (playerHealth < 0) {
            playerHealth = 0;
        }

        healthBar.GetComponent<HealthBarManager>().UpdateHealthBar(playerHealth, maxHealth);
        
        GameObject di = Instantiate(damageIndicator, gameObject.transform);
        di.GetComponent<DamageIndicatorManager>().IndicateDamage(gameObject, damage);
    }

    public void Regen(float regen) {
        playerHealth = regen;
        healthBar.GetComponent<HealthBarManager>().UpdateHealthBar(playerHealth, maxHealth);
    }

    private void CheckDoubles() 
    {
        int numMusicPlayers = FindObjectsOfType<PlayerControllerPublic>().Length;
        if (numMusicPlayers != 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void UpdateInv() 
    {
        foreach (Transform gun in GameObject.Find("Weapons").transform) { 
            if (gun.GetComponent<Weapon_Script>().weaponType == Weapon_Script.WeaponType.primary) {
                inv[2] = gun.gameObject;
            } 
            if (gun.GetComponent<Weapon_Script>().weaponType == Weapon_Script.WeaponType.secondary) {
                inv[1] = gun.gameObject;
            }
        }
    }

}
