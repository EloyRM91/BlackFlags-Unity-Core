using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public abstract class MiniMapMaskingResponse : MonoBehaviour
    {
        protected virtual void Start()
        {
            UIMinimap.EnableMask += ActivateMask;
        }

        protected virtual void OnDestroy()
        {
            UIMinimap.EnableMask -= ActivateMask;
        }

        protected virtual void ActivateMask(bool masked)
        {
            if (GetComponent<Image>() != null)
                GetComponent<Image>().enabled = masked;

            if (GetComponent<Mask>() != null)
                GetComponent<Mask>().enabled = masked;
        }
    }
}

