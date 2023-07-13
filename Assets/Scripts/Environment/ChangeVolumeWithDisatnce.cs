using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeVolumeWithDisatnce : MonoBehaviour
{
    public float maxDistance = 25f;
    public float minVolume = 0f;

    private AudioSource audioSource;
    private GameObject Player;

    void Start()
    {
        Player = GameObject.Find("Player");
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, Player.transform.position);
        float volume = 1f - (distance / maxDistance);
        volume = Mathf.Clamp(volume, minVolume, 2f);
        audioSource.volume = volume;
    }
}
