using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeqCity : MonoBehaviour
{
    public Transform cam, camTarget, ship;
    public Transform seagull1, seagull2, seagull3;

    private float birbspeed, shipspeed;
    void Start()
    {
        Invoke("Seq", 4);
    }

    private void FixedUpdate()
    {
        seagull1.position += -seagull1.forward * birbspeed * Time.deltaTime;
        seagull2.position += -seagull1.forward * birbspeed * Time.deltaTime;
        seagull3.position += -seagull1.forward * birbspeed * Time.deltaTime;
        ship.position += ship.forward * shipspeed * Time.deltaTime;
    }

    private void Seq()
    {
        seagull1.gameObject.SetActive(true);
        seagull2.gameObject.SetActive(true);
        seagull3.gameObject.SetActive(true);

        shipspeed = 21;
        birbspeed = 300;

        cam.DORotate(new Vector3(5.1f, 104f, 0), 2f).SetEase(Ease.InOutQuad);
        cam.DOLocalMove(camTarget.position, 2).SetEase(Ease.InOutQuad);
    }
}
