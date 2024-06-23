using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.WorldCities
{
    public class MB_NaturalPort : KeyPoint
    {
        public byte calado;
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();
        }

        //public override void OnMouseDown()
        //{
        //    base.OnMouseDown();
        //    if (UIMap.ui.gameObject.activeSelf)
        //        if (!UIMap.panelON)
        //        {
        //            if (!UIMap.GetGraphicRaycastResult())
        //                UIMap.ui.DisplayInfo(this);
        //        }
        //}

        public string Calado()
        {
            string c = string.Empty;
            switch (calado)
            {
                case 0: return "embarcaciones menores";
                case 1: return "escoltas ligeros";
                case 2: return "naves de dos puentes";
                case 3: return "cargueros";
                case 4: return "buques de guerra";
            }

            return c;
        }

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
