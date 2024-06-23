using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities.UI
{
    public class MinimapButton : GameButton
    {
        public static Camera minimapCam;
        [SerializeField] private byte mask;

        //protected override void Start()
        //{
        //    base.Start();
        //    minimapCam = Cameras.minimapCamera;
        //}

        public void SetCameraMask()
        {
            minimapCam.cullingMask = 1 << mask | 1 << 12;
        }
    }
}

