using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace GameMechanics.WorldCities
{
    public abstract class MinimapSprite : MonoBehaviour, IsSelectable
    {
        //Selection
        public static bool isSelected;
        protected static Sequence sequence;
        public static MinimapSprite currentSelected;

        public virtual void OnMouseEnter()
        {
            if(!isSelected)
                GetComponent<SpriteRenderer>().color = Color.white;
        }

        public virtual void OnMouseExit() { }

        public virtual void OnMouseDown()
        {
            StartCoroutine("AvoidUIRaycasting");
        }

        public virtual void Select()
        {
            //Tweening settings
            DOTween.defaultTimeScaleIndependent = true;

            if (currentSelected != null)
            {
                if (currentSelected != this)
                    Unselect();
                else return;
            }
            currentSelected = this;
            isSelected = true;
        }

        public virtual void Unselect()
        {
            isSelected = false;
            currentSelected = null;
            sequence.Kill();
        }

        IEnumerator AvoidUIRaycasting()
        {
            UIMap.ui.canGraphicRaycasting = false;
            yield return new WaitForEndOfFrame();
            UIMap.ui.canGraphicRaycasting = true;
        }
    }

    public interface IsSelectable
    {
        void OnMouseEnter();
        void OnMouseExit();
        void OnMouseDown();
    }
}
