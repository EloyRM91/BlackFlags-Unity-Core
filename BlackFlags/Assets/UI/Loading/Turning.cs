using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turning : MonoBehaviour
{

    private void Update()
    {
        transform.Rotate(-Vector3.forward * 180 * Time.deltaTime);
    }

}
