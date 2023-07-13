using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaUpManager : MonoBehaviour
{
    public float regenMultiplyer;

    public void CollectPerk() {
        GameObject.Find("Player").GetComponent<PlayerControllerPublic>().energyRegenStep *= regenMultiplyer;
    }
}
