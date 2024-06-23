using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using DG.Tweening;

public class Seq6 : MonoBehaviour
{
    public CannonBehaviour cannon1, cannon2, cannon3;
    public ParticleSystem impact;

    private float speed1 = 0, speed2 = 0;

    void Start()
    {
        Invoke("Sequence", 4f);
    }


    private void Sequence()
    {

        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => { cannon1.Shoot(); cannon2.Shoot(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { impact.Play(); });
        sequence.AppendInterval(0.125f);
        sequence.AppendCallback(() => { cannon3.Shoot(); });


    }

}
