using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public class UITracker : MiniMapMaskingResponse
    {
        [SerializeField] private Transform pivot;

        private Transform _WorldTracker;
        //private Animator _anim;
        protected override void Start()
        {
            base.Start();
            _WorldTracker = transform.GetChild(0);
            //_MinimapTracker = transform.GetChild(1);
            //_anim = _WorldTracker.GetComponent<Animator>();
            PlayerMovement.playerSetDestination += SetNewWorldPosition;
            PlayerMovement.ArriveToDestination += Clear;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            PlayerMovement.playerSetDestination -= SetNewWorldPosition;
            PlayerMovement.ArriveToDestination -= Clear;
        }


        private void SetNewWorldPosition(Vector3 newPos)
        {
            _WorldTracker.gameObject.SetActive(true);
            //_MinimapTracker.gameObject.SetActive(true);
            pivot.position = newPos;
            //_anim.SetTrigger("DrawPos");

            _WorldTracker.GetComponent<FixedImageAnimation>().Animate();
        }

        private void Clear()
        {
            _WorldTracker.gameObject.SetActive(false);
            //_MinimapTracker.gameObject.SetActive(false);
        }
    }
}

