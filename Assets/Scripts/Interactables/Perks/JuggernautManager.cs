using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JuggernautManager : MonoBehaviour
{
    public float healthBoost;
    private GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
    }

    public void CollectPerk() {
        Player.GetComponent<PlayerControllerPublic>().maxHealth *= healthBoost;
        Player.GetComponent<PlayerControllerPublic>().playerHealth = Player.GetComponent<PlayerControllerPublic>().maxHealth;
    }
}
