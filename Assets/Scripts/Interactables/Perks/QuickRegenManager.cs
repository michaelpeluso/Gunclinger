using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickRegenManager : MonoBehaviour
{
    public float regenTimeMultiplyer;
    
    public void CollectPerk() {
        GameObject.Find("Player").GetComponent<PlayerControllerPublic>().healthRegenDelay *= regenTimeMultiplyer;
    }
}
