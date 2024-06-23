using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using DG.Tweening;

public class Seq2 : MonoBehaviour
{
    public Transform cam, targetPoint;
    public CannonBehaviour cannon1, cannon2;
    public ParticleSystem impact, impact2;

    public Image panel;
    void Start()
    {
        Invoke("Sequence", 4);

        panel.DOFade(1, 0.6f);
    }

    private void Sequence()
    {
        cam.DOLocalMove(targetPoint.position, 2f).SetEase(Ease.Linear);
        cam.DORotateQuaternion(Quaternion.Euler(cam.rotation.eulerAngles.x, 170, cam.rotation.eulerAngles.z), 4f);
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(0.2f);
        sequence.AppendCallback(() => { cannon1.Shoot(); });
        sequence.AppendInterval(0.1f);
        sequence.AppendCallback(() => { impact.Play(); });
        sequence.AppendInterval(0.15f);
        sequence.AppendCallback(() => { impact2.Play(); });
        sequence.AppendInterval(0.3f);
        sequence.AppendCallback(() => { cannon2.Shoot(); });
    }

}
