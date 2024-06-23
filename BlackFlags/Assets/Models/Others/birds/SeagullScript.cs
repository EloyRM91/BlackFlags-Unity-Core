using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class SeagullScript : MonoBehaviour
{
    [SerializeField] private int distance, time;
    private Sequence sequence;
    void OnEnable()
    {
        Move();
    }

    private void OnDisable()
    {
        sequence.Kill();
    }
    private void Move()
    {
        sequence.Kill();
        sequence = DOTween.Sequence().SetUpdate(true);
        sequence.AppendInterval(Random.Range(4, 10));
        sequence.Append(transform.DOMove(transform.position - distance * transform.forward, time));
        sequence.SetLoops(-1);
        //sequence.Play();
    }
}
