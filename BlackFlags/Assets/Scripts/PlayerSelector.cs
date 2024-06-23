using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.WorldMap
{
    public class PlayerSelector : ShipSelector
    {
        public void SelectPlayer()
        {
            UISelector.instance.SetTarget(transform);
        }
        protected override void OnMouseDown()
        {
            base.OnMouseDown();
        }
    }
}


