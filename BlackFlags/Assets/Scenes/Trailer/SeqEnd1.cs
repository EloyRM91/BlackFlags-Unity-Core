using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeqEnd1 : MonoBehaviour
{
    public Transform cam, ship1, ship2;
    private float speed;
    void Start()
    {
        Invoke("Action",2);
    }

    private void FixedUpdate()
    {
        ship1.position += ship1.forward * speed * Time.deltaTime;
    }

    private void Action()
    {
        speed = 10;
        cam.DORotate(new Vector3(cam.rotation.eulerAngles.x, 178, cam.rotation.eulerAngles.z), 3.5f).SetEase(Ease.InOutQuad);
        ship2.DORotate(new Vector3(20, ship2.rotation.eulerAngles.y, 37), 6);
        ship2.DOLocalMoveY(ship2.position.y - 7, 6);
    }

}
