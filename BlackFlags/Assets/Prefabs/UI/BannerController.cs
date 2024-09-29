using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.WorldMap
{
    public class BannerController : MonoBehaviour
    {
        [SerializeField] protected Transform _target;
        public static Camera cam;

        public Transform Target {
            get {
                return _target;
            }
        }

        protected virtual void Update()
        {
            SetUIPosition();
        }

        protected virtual void SetUIPosition()
        {
            transform.position = cam.WorldToScreenPoint(_target.position);
        }

        public bool HasTarget()
        {
            return _target != null;
        }

        public void ForcePosition()
        {
            SetUIPosition();
        }
    }
}
