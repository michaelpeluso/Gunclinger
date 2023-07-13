using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthEnergyManager : MonoBehaviour
{
    
    public RectTransform healthBar;
    public RectTransform energyBar;
    private float health;
    private float energy;

    private float maxHealthSize;
    private float maxEnergySize;

    private RectTransform newRect;

    private GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");

        maxHealthSize = healthBar.rect.width;
        maxEnergySize = energyBar.rect.max.x;
    }

    void Update()
    {
        health = Player.GetComponent<PlayerControllerPublic>().playerHealth;
        energy = Player.GetComponent<PlayerControllerPublic>().playerEnergy;

        healthBar.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, healthBar.rect.min.x, maxHealthSize * (health / Player.GetComponent<PlayerControllerPublic>().maxHealth));
    }
}
