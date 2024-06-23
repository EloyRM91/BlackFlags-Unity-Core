using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Seq9 : MonoBehaviour
{
    public Transform cam, ship1, ship2;
    public CannonBehaviour cannon1, cannon2, cannon3;
    public ParticleSystem impact;

    private float speed;
    void Start()
    {
        Invoke("Sequence", 3f);
    }
    private void FixedUpdate()
    {
        ship1.position += ship1.forward * speed * Time.deltaTime;
        ship2.position += ship2.forward * speed * Time.deltaTime;
    }
    private void Sequence()
    {


        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => { cannon1.Shoot(); cannon3.Shoot(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { cannon2.Shoot(); });

        sequence.AppendInterval(0.5f);
        sequence.AppendCallback(() => { ship1.DORotate(new Vector3(0, -70, 0), 7f); });
        sequence.AppendCallback(() => { ship2.DORotate(new Vector3(0, 4, 0), 7f); });
        sequence.AppendCallback(() => { speed = 6; });

        sequence.AppendInterval(0.3f);
        sequence.AppendCallback(() => { impact.Play(); });

    }
}
