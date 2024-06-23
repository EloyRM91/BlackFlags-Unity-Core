using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    private void FixedUpdate()
    {
        transform.position += transform.forward * 250 * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject impact = transform.GetChild(0).gameObject;

        impact.SetActive(true);
        impact.transform.SetParent(null);

        Destroy(gameObject);
    }
}
