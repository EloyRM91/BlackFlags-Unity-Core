using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Seq1 : MonoBehaviour
{
    public Transform cam, targetPoint;
    public CannonBehaviour cannon1, cannon2;
    public ParticleSystem impact;
    void Start()
    {
        Invoke("Sequence", 4);
    }

    private void Sequence()
    {
        cam.DOLocalMove(targetPoint.position, 1.1f).SetEase(Ease.Linear);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.12f);
        sequence.AppendCallback(() => { cannon1.Shoot(); });
        sequence.AppendInterval(0.25f);
        sequence.AppendCallback(() => { impact.Play(); });
        sequence.AppendInterval(0.27f);
        sequence.AppendCallback(() => { cannon2.Shoot(); });
    }

}
