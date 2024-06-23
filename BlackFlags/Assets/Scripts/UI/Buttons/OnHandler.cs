using UnityEngine;
using UnityEngine.EventSystems;

namespace GameMechanics.Utilities
{
    public class OnHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private bool isOnHandler;

        public bool IsOnHandler
        {
            get { return isOnHandler; }
        }

        public void OnPointerEnter(PointerEventData pointerEventData)
        {
            isOnHandler = true;
        }

        public void OnPointerExit(PointerEventData pointerEventData)
        {
            isOnHandler = false;
        }
    }
}


