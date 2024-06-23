using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using DG.Tweening;

public class Seq10 : MonoBehaviour
{
    public CannonBehaviour cannon1, cannon2, cannon3;
    public Transform cam, ship;
    public ParticleSystem impact;

    private float speed = 0;

    void Start()
    {
        Invoke("Sequence", 2f);
    }

    private void FixedUpdate()
    {
        //ship1.position += ship1.forward * speed1 * Time.deltaTime;
        ship.position += ship.forward * speed * Time.deltaTime;

    }

    private void Sequence()
    {
        speed = 7;
        cam.DOLocalMoveZ(cam.position.z -12, 2.5f);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(() => { impact.Play(); cannon1.Shoot(); });
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => {cannon3.Shoot(); });
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => {  cannon2.Shoot(); });

    }

}
