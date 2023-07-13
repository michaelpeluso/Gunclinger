using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathIndicatorManager : MonoBehaviour
{
    public float deathTime;
    public float effectDistance;
    public float delay;
    
    void Start()
    {
        gameObject.SetActive(true);
        GetComponent<SpriteRenderer>().enabled = false;
        ;
    }

    public void IndicateDeath(GameObject parent)
    {
        gameObject.transform.SetParent(parent.transform);
        if (parent.GetComponent<CircleCollider2D>()) {
            gameObject.transform.position = new Vector3(parent.GetComponent<CircleCollider2D>().bounds.center.x, parent.GetComponent<CircleCollider2D>().bounds.max.y, parent.transform.position.z);    
        }
        else {
            gameObject.transform.position = new Vector3(parent.GetComponent<BoxCollider2D>().bounds.center.x, parent.GetComponent<BoxCollider2D>().bounds.max.y, parent.transform.position.z);
        }

        StartCoroutine(DeathIndicator(deathTime, effectDistance));
    }

    IEnumerator DeathIndicator(float transition, float effectDistance)
    {
        yield return new WaitForSeconds(delay);
        GetComponent<SpriteRenderer>().enabled = true;

        Vector3 pos = transform.position;
        float time = 0;

        while (time < 1) {
            time += Time.deltaTime / transition;
            transform.position = Vector3.Lerp(pos, new Vector3(pos.x, effectDistance, pos.y), time);
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime);

            yield return null;
        }

        Destroy(gameObject, 0.0f);
    }
    
    public void DestroyMe() {
        Destroy(gameObject);
    }
}
