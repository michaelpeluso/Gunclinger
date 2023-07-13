using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerFinder : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //DontDestroyOnLoad(this);
        Transform Player = GameObject.Find("Player").transform;
        GetComponent<CinemachineVirtualCamera>().Follow = Player;
        GetComponent<CinemachineVirtualCamera>().LookAt = Player;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
