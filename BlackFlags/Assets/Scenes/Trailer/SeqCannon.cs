using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SeqCannon : MonoBehaviour
{
    public Transform cannon, cannon2;
    private float d1;
    void Start()
    {
        Invoke("Action", 1);
    }

    //private void FixedUpdate()
    //{
    //    cannon.position = Vector3.MoveTowards(cannon.position, endPos.position, d1);
    //}

    private void Action()
    {
        Sequence sequence = DOTween.Sequence();

        sequence.AppendCallback(() => { cannon.DOLocalMoveX(cannon.localPosition.x + 0.18f, .3f).SetEase(Ease.InQuad); });
        sequence.AppendInterval(0.15f);
        sequence.AppendCallback(() => { cannon2.DOLocalMoveX(cannon2.localPosition.x + 0.18f, .3f).SetEase(Ease.InQuad); });
        

    }
}
