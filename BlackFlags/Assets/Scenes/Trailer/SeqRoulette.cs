using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeqRoulette : MonoBehaviour
{
    public Transform camPivot;
    void Start()
    {
        StartCoroutine("Seq");
    }

    IEnumerator Seq()
    {
        yield return new WaitForSeconds(2);
        camPivot.DORotate(new Vector3(camPivot.rotation.eulerAngles.x, -165, camPivot.rotation.eulerAngles.z), 12);
        yield return new WaitForSeconds(.2f);

        for (int i = 1; i < transform.childCount; i++)
        {
            transform.GetChild(i-1).gameObject.SetActive(false);
            transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(.2f);
        }
    }
}
