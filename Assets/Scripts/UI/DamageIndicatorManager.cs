using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicatorManager : MonoBehaviour
{
    public float damageTime;
    public float healthEffectDistance;
    
    void Start()
    {
        gameObject.SetActive(true);
    }

    public void IndicateDamage(GameObject parent, float damage)
    {
        gameObject.transform.SetParent(parent.transform);
        if (parent.GetComponent<CircleCollider2D>()) {
            gameObject.transform.position = new Vector3(parent.GetComponent<CircleCollider2D>().bounds.center.x, parent.GetComponent<CircleCollider2D>().bounds.max.y, parent.transform.position.z);    
        }
        else {
            gameObject.transform.position = new Vector3(parent.GetComponent<BoxCollider2D>().bounds.center.x, parent.GetComponent<BoxCollider2D>().bounds.max.y, parent.transform.position.z);
        }

        GetComponent<TextMeshPro>().text = (damage).ToString();
        GetComponent<TextMeshPro>().color = Color.Lerp(Color.green, Color.red, damage / 100f); //new Color32(255, (byte)(255.0f - (damage / 100.0f * 255.0f)), 0, 255);
        if (damage > 100.0f ) {
            GetComponent<TextMeshPro>().color = new Color32(255, 255, 0, 255);
        }
        //gameObject.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(-5f, 5f));

        StartCoroutine(DamageIndicator(damageTime, healthEffectDistance));
    }

    IEnumerator DamageIndicator(float transition, float healthEffectDistance)
    {
        Vector3 pos = transform.localPosition;
        float time = 0;

        while (time < 5) {
            time += Time.deltaTime / transition;
            transform.localPosition = Vector3.Lerp(pos, new Vector3(pos.x, healthEffectDistance, pos.z), time);
            Color curCol = GetComponent<TextMeshPro>().color;
            GetComponent<TextMeshPro>().color = new Color(curCol.r, curCol.g, curCol.b, curCol.a - Time.deltaTime / transition);

            yield return null;
        }

        Destroy(gameObject, 0.0f);
    }
}
