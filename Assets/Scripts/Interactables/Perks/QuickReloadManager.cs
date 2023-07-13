using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickReloadManager : MonoBehaviour
{
    public float reloadTimeMultiplyer;
    public GameObject WeaponBox;

    public void CollectPerk() {
        GameObject[] inventory = GameObject.Find("Player").GetComponent<PlayerControllerPublic>().inv;
        foreach (GameObject weapon in GameObject.Find("Player").GetComponent<PlayerControllerPublic>().inv) {
            if (weapon != null) {
                weapon.GetComponent<Weapon_Script>().reloadTime *= reloadTimeMultiplyer;
            }
        }

        foreach (GameObject weapon in WeaponBox.GetComponent<GunBoxManager>().weapons) {
            weapon.GetComponent<Weapon_Script>().reloadTime *= reloadTimeMultiplyer;
        }
    } 
}
