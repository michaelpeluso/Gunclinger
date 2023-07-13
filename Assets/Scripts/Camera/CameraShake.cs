using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{  
    bool isShaking = false;

    float duration;
    float magnitude;

    public GameObject cinemaMachine;
    CinemachineBasicMultiChannelPerlin noise;


	void Start()
	{
        //DontDestroyOnLoad(this);
        CinemachineVirtualCamera vcam = cinemaMachine.GetComponent<CinemachineVirtualCamera>();
        noise = vcam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = noise.m_FrequencyGain = 0f;
	}

    public void Shake(float d, float m) 
    {
        duration = d;
        magnitude = m;

        if (!isShaking) {
            StartCoroutine(ShakeCamera(duration, magnitude));
            isShaking = false;
        }
    }

    private IEnumerator ShakeCamera(float d, float m) {
        while (d > 0f)
        {
            noise.m_AmplitudeGain = m * 2;
            noise.m_FrequencyGain = m;

            d -= Time.deltaTime;
            yield return null;
        }

        noise.m_AmplitudeGain = noise.m_FrequencyGain = 0f;
        isShaking = false;
    }

    /*private void CommunicateWithParallax(Vector3 op) {
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("map")) {
            if (go.GetComponent<ParallaxLayer>()) {
                go.GetComponent<ParallaxLayer>()._camera.localPosition = originalPos;
            }
        }
    }*/
}
