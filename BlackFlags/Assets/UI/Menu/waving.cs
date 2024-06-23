using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waving : MonoBehaviour
{
    void Update()
    {
        transform.Translate(Vector3.up * 0.5f * Mathf.Sin(Time.time * 0.8f) * Time.deltaTime);
    }
}
