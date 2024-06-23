using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.WorldCities
{
    public class OnMapClick : MonoBehaviour
    {
        public delegate void ClickOnMap();
        public static event ClickOnMap clickOnMap;

        private void OnMouseDown()
        {
            clickOnMap();
        }
    }
}

