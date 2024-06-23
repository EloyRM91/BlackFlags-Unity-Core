using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.WorldMap
{
    public class UIClearListener : MonoBehaviour
    {
        void Start()
        {
            UIMap.clearPanels += Clear;
            UISelector._EVENT_UnselectConvoy += Clear;
        }

        private void OnDestroy()
        {
            UIMap.clearPanels -= Clear;
            UISelector._EVENT_UnselectConvoy -= Clear;
        }

        protected virtual void Clear()
        {
            gameObject.SetActive(false);
        }
    }
}

