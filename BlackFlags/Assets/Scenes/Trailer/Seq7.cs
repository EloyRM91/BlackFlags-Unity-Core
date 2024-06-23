using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Seq7 : MonoBehaviour
{
    public Transform cam, ship, targetPos;
    public CannonBehaviour cannon1, cannon2, cannon3;
    public ParticleSystem impact, impact2;

    private float speed;
    void Start()
    {
        Invoke("Sequence", 1.3f);
    }
    private void FixedUpdate()
    {
        ship.position += ship.forward * speed * Time.deltaTime;
    }
    private void Sequence()
    {
        speed = 9;
        ship.DORotate(new Vector3(0,5,0),4.5f);
        cam.DOLocalMove(targetPos.position, 2.5f);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => { cannon1.Shoot();  });
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => { cannon2.Shoot(); impact.Play(); });
        sequence.AppendInterval(0.26f);
        sequence.AppendCallback(() => { cannon3.Shoot(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { impact2.Play(); });

    }

}
