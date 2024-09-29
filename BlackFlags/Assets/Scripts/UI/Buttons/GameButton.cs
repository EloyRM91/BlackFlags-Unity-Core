//Unity
using UnityEngine;
//UI 
using UnityEngine.UI; using UnityEngine.EventSystems;

namespace Utilities.UI
{
	public class GameButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField] private Sprite _highlighted, _notHighlighted;
		private Image _buttonImage;
		protected virtual void Start() { _buttonImage = GetComponent<Image>(); }
		public virtual void OnPointerEnter(PointerEventData eventData)
		{
			if(_highlighted != null)
            {
			_buttonImage.sprite = _highlighted;
            }
		}
		public virtual void OnPointerExit(PointerEventData eventData)
        {
			if(_notHighlighted != null)
            {
			_buttonImage.sprite = _notHighlighted;
            }
		}
	}
}
