using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public int totalPoints;
    private GameObject totalPointsIndicator;
    
    public GameObject pointsIndicator;
    public float showTime;
    public float beginDistance;
    public float effectDistance;
    
    void Start()
    {
        totalPointsIndicator = transform.GetChild(0).gameObject;
        totalPoints = 0;
    }

    public void IndicatePoints(float points)
    {
        GameObject indicator = Instantiate(pointsIndicator, transform);
        indicator.transform.SetParent(transform);

        Vector3 totalPointsPos = gameObject.transform.GetChild(0).position;
        indicator.transform.position = new Vector3(totalPointsPos.x, totalPointsPos.y, totalPointsPos.z);
        
        indicator.GetComponent<TextMeshProUGUI>().text = (points).ToString("+#;-#;+0");
        StartCoroutine(PointsIndicator(showTime, effectDistance, indicator));

        totalPoints = int.Parse(totalPointsIndicator.GetComponent<TextMeshProUGUI>().text) + (int)points; 
        totalPointsIndicator.GetComponent<TextMeshProUGUI>().text = (totalPoints).ToString();
    }

    IEnumerator PointsIndicator(float transition, float effectDistance, GameObject ind)
    {
        Vector3 pos = ind.transform.localPosition;
        pos.y += beginDistance;
        float time = 0;

        while (time < 1) {
            time += Time.deltaTime / transition;
            ind.transform.localPosition = Vector3.Lerp(pos, new Vector3(pos.x, effectDistance, pos.z), time);
            Color curCol = ind.GetComponent<TextMeshProUGUI>().color;
            ind.GetComponent<TextMeshProUGUI>().color = new Color(curCol.r, curCol.g, curCol.b, curCol.a - Time.deltaTime / transition);

            yield return null;
        }

        Destroy(ind, 0.0f);
    }
}
