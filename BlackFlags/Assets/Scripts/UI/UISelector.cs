using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Tweening
using DG.Tweening;
//Mechanics
using GameMechanics.Ships;

namespace UI.WorldMap
{
    public class UISelector : BannerController
    {
#region VARIABLES
        //References
        public static UISelector instance;
        private Transform pivot;
        private Image _IMG_Selector;
        
        //Tweening
        private Tween tween;
        // Pivot Offset
        private Vector3 localPosRef;
        //Cam Raycast
        private Ray ray;
        private RaycastHit hit;
        //Delegates
        public delegate void PlayerSelectConvoy(Convoy convoy);
        public delegate void PlayerUnselect();
        public static event PlayerSelectConvoy _EVENT_SelectConvoy;
        public static event PlayerUnselect _EVENT_UnselectConvoy;
#endregion
        private void Awake()
        {
            instance = this;
            DOTween.Init();
            DOTween.defaultTimeScaleIndependent = true;
        }
        void Start()
        {
            //References
            pivot = transform.GetChild(0);
            _IMG_Selector = pivot.GetComponent<Image>();
            //Offset
            localPosRef = pivot.localPosition;
            //Events
            UIMap.userclick += OnClickResponse;
        }
        private void OnDestroy()
        {
            //Events
            UIMap.userclick -= OnClickResponse;
        }

        //Overdrives
        protected override void SetUIPosition()
        {
            if (_target != null) base.SetUIPosition();
        }

        /// <summary>
        /// Set a transform as current UI selector's target
        /// </summary>
        /// <param name="newTarget"></param>
        public void SetTarget(Transform newTarget)
        {
            //Color
            switch (newTarget.tag)
            {
                case "Player":
                    _IMG_Selector.color = Color.blue;
                    break;
                case "Pirate":
                    _IMG_Selector.color = Color.black;
                    break;
                default:
                    _IMG_Selector.color = Color.white;
                    break;
            }
            //Target
            _target = newTarget;
            //Animation
            Tween();
            //HUD Info
            _EVENT_SelectConvoy(_target.GetComponent<Convoy>());
        }
        private void Tween()
        {
            tween.Kill();
            pivot.localPosition = localPosRef;
            pivot.gameObject.SetActive(true);
            tween = pivot.DOLocalMove(localPosRef + Vector3.up * 55f, 0.8f).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }

        /// <summary>
        /// Is the transform the current UI-selector target?
        /// </summary>
        /// <param name="something"></param>
        /// <returns></returns>
        public bool IsTarget(Transform something)
        {
            return something == _target;
        }
        private void OnClickResponse()
        {
            if(_target!= null)
            {
                if (!UIMap.GetGraphicRaycastResult())
                {
                    ray = cam.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.transform.CompareTag("Respawn"))
                            Unselect();
                    }
                }
            }
        }
        public void Unselect()
        {
            tween.Kill();
            _target = null;
            pivot.gameObject.SetActive(false);
            _EVENT_UnselectConvoy();
        }
    }
}

