using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.WorldMap
{
    /// <summary>
    /// an info banner displayer class
    /// </summary>
    public class Info : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDisplayInfo
    {
        protected bool active;
        protected virtual void Start() { }
        public virtual void OnPointerEnter(PointerEventData e)
        {
            active = true;
            StopAllCoroutines();
            StartCoroutine("Timer");
        }
        public virtual void OnPointerExit(PointerEventData e)
        {
            active = false;
            StopAllCoroutines();
            InfoDispatcher.Sleep();
        }
        IEnumerator Timer()
        {
            yield return new WaitForSeconds(0.5f);
            if (active)
            {
                while(true)
                {
                    yield return new WaitForEndOfFrame();
                    active = false;
                    DisplayInfo();
                }

            }
        }
        public virtual void DisplayInfo()
        {
            InfoDispatcher.DisplayInfo("hola :3");
        }
    }
}

