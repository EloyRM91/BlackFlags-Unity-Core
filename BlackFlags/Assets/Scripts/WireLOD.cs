using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class WireLOD : MonoBehaviour
{
    private static float _distance;
    private static Transform cam;
    private bool onSleep = false;

    private void Start()
    {
        //cam = Camera.main.transform;
        cam = GameObject.FindWithTag("BattleCam").transform;
        SetLines(true);
    }
    void Update()
    {
        if (Vector3.Distance(transform.position, cam.position) > 2600)
        {
            if (!onSleep)
            {
                onSleep = true;
                SetLines(false);
            }

        }
        else if (onSleep)
        {
            onSleep = false;
            SetLines(true);
        }
    }

    private void SetLines(bool val)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(val);
        }
    }
}
