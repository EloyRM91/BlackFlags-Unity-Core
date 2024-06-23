using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Derp : MonoBehaviour
{
    public Rigidbody shipBody;
    public Transform dirRef;

    private void Start()
    {
        Invoke("Action", 3);
    }

    private void Action()
    {
        shipBody.AddForce(20000 * dirRef.forward);
    }
}
