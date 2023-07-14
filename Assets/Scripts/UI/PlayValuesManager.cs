using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayValuesManager : MonoBehaviour
{
    int difficulty;
    int startingWave;

    public SceneLoader sceneLoader;

    void Start()
    {
        PlayerPrefs.SetInt("difficulty", 2);
        PlayerPrefs.SetInt("startingWave", 0);
    }

    public void UpdateDif() {
        int val = (int)GameObject.Find("Difficulty Slider").GetComponent<Slider>().value;
        PlayerPrefs.SetInt("difficulty", val);
        GameObject.Find("Difficulty Text").GetComponent<TextMeshProUGUI>().text = val.ToString();
    }
    
    public void UpdateWave() {
        int val = (int)GameObject.Find("Wave Slider").GetComponent<Slider>().value * 5;
        PlayerPrefs.SetInt("startingWave", val);
        GameObject.Find("Wave Text").GetComponent<TextMeshProUGUI>().text = val.ToString();
    }

    public void PlayGame() {
        sceneLoader.LoadNextScene(0);
    }
}