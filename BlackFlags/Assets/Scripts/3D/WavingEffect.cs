using UnityEngine;
using DG.Tweening;

namespace GameMechanics.Mechanics
{
    public class WavingEffect : MonoBehaviour
    {
        private Tween tween;
        private Vector3 offset;

        //Parameters
        public float amplitude = 11, timeFrequency = 2;
        private void Awake()
        {
            offset = transform.position;
        }
        private void OnEnable()
        {
            transform.position = offset;
            SetMovement();
        }
        private void OnDisable()
        {
            tween.Kill();
        }

        public void SetMovement()
        {
            tween.Kill();
            tween = transform.DOMove(offset + Vector3.up * amplitude, timeFrequency).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }
    }
}

