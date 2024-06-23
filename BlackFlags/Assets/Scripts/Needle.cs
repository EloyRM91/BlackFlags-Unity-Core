using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Needle : MonoBehaviour
{
    void Start()
    {
        PlayerMovement.playerLookRotation += MoveNeedle;
    }

    private void OnDestroy()
    {
        PlayerMovement.playerLookRotation -= MoveNeedle;
    }

    private void MoveNeedle(Quaternion rot)
    {
        transform.rotation = rot;
    }
}
