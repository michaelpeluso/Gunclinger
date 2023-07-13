using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForCanvas : MonoBehaviour
{
    private static bool created = false;

    private void Start()
    {
        if (!created)
        {
            DontDestroyOnLoad(gameObject);
            created = true;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit() {
        created = false;
    }
}
