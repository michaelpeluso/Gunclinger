using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarManager : MonoBehaviour
{
    SpriteRenderer sr;

    void Start()
    {   
        gameObject.SetActive(true);
        sr = GetComponent<SpriteRenderer>();
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
    }
    
    public void UpdateHealthBar(float health, float maxHealth) 
    {
        if (health >= 0 && health <= maxHealth) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 255);
            gameObject.transform.localScale = new Vector3(health / maxHealth, gameObject.transform.localScale.y, gameObject.transform.localScale.z);
            sr.color = Color.Lerp(Color.red, Color.green, health / maxHealth);

            StopAllCoroutines();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
            StartCoroutine(UpdatingHealthBar(health));
        }   
    }
    
    IEnumerator UpdatingHealthBar(float ph) 
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        
        float time = 0;
        if (ph == PlayerPrefs.GetFloat("maxHealth")) {
            yield return null;    
        }
        else {
            while (time < 3) {
                time += Time.deltaTime; 
                yield return null;
            }
        }

        while (time < 5) {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
