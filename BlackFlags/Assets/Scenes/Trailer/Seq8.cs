using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Seq8 : MonoBehaviour
{
    public Transform cam, ship;
    public CannonBehaviour cannon1, cannon2, cannon3, cannon4, cannon5, cannon6;
    private float speed;
    void Start()
    {
        Invoke("Sequence", 3f);
    }
    private void FixedUpdate()
    {
        ship.position += ship.forward * speed * Time.deltaTime;
    }

    private void Sequence()
    {
        speed = 9;
        ship.DORotate(new Vector3(0, 135, 0), 9f);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { cannon1.Shoot(); cannon2.Shoot(); cannon3.Shoot(); });
        sequence.AppendInterval(0.65f);
        sequence.AppendCallback(() => { cannon4.Shoot(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { cannon5.Shoot(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { cannon6.Shoot(); });

    }
}
