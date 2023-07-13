using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawnCamera : MonoBehaviour
{

    public Transform trans;

    private void OnEnable() {
        GameObject.Find("Player").transform.position = trans.position;
    }
}
