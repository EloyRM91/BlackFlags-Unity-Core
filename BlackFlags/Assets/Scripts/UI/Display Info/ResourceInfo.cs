using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics.Data;

namespace UI.WorldMap
{
    public class ResourceInfo : Info
    {
        //Variables
        [SerializeField] private byte index;
        private Resource thisResource;

        public override void DisplayInfo()
        {
            InfoDispatcher.DisplayInfo(thisResource.resourceName);
        }
        public void SetResource(byte i)
        {
            thisResource = EconomyBehaviour.GetResource(i);
        }
        public void SetResource(Resource r)
        {
            thisResource = r;
        }
    }
}

