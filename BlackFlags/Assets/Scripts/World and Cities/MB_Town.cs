using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.WorldCities
{
    public class MB_Town : Settlement
    {
        public int population;

        //public override void OnMouseDown()
        //{
        //    base.OnMouseDown();
        //    if (!UIMap.panelON)
        //    {
        //        if (!UIMap.GetGraphicRaycastResult())
        //            UIMap.ui.DisplayInfo(this);
        //    }
        //}

        protected override void DisplayKeypointPanel()
        {
            //todavía no hay panel para la vista de villas
            UIMap.ui.DisplayInfo(this);
        }

        protected override void DisplayInfo()
        {
            UIMap.ui.DisplayInfo(this);
        }
    }
}

