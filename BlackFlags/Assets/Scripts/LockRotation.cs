using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{
    private static Quaternion rot;
    private void Awake()
    {
        rot = Quaternion.Euler(-90,0,0);
    }
    private void LateUpdate()
    {
        transform.rotation = rot;
    }
}
