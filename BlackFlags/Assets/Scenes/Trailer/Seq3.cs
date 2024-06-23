using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using DG.Tweening;

public class Seq3 : MonoBehaviour
{
    public CannonBehaviour cannon;
    public ParticleSystem impact;

    void Start()
    {
        Invoke("Sequence", 4.5f);
    }

    private void Sequence()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { cannon.Shoot(); });
        sequence.AppendInterval(0.12f);
        sequence.AppendCallback(() => { impact.Play(); });
    }

}
