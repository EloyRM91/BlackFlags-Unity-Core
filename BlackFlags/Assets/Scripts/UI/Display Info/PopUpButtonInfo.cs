using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public class PopUpButtonInfo : ButtonInfo
    {
        protected float offsetPos;
        protected Tween currentTween;
        protected override void Start()
        {
            base.Start();
            offsetPos = transform.localPosition.y;
        }
        public override void OnPointerEnter(PointerEventData e)
        {
            base.OnPointerEnter(e);
            if(GetComponent<Button>().interactable)
            {
                currentTween.Kill();
                currentTween = transform.DOLocalMoveY(offsetPos + 10, 0.2f);
            }
        }
        public override void OnPointerExit(PointerEventData e)
        {
            base.OnPointerExit(e);
            if (GetComponent<Button>().interactable)
            {
                currentTween.Kill();
                currentTween = transform.DOLocalMoveY(offsetPos, 0.3f);
            }
        }

        public override void DisplayInfo()
        {
            InfoDispatcher.DisplayInfo(GetComponent<Button>().interactable ? txt : txt + " - No disponible");
        }
    }
}

