using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class seqExpl : MonoBehaviour
{
    public Transform cam;
    public ParticleSystem ps1, ps2, ps3;
    void Start()
    {
        Invoke("Action", 3f);
    }

    private void Action()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { ps1.Play(); ps2.Play(); });
        sequence.AppendInterval(0.4f);
        sequence.AppendCallback(() => { ps3.Play(); });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(() => { cam.DOShakePosition(0.6f, 0.6f, 10); });
        
    }


}
