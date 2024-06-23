using UnityEngine; using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI.WorldMap
{
    public class ButtonInfoOnTarget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDisplayInfo
    {
        [SerializeField] protected Text _TEXT_targetText;
        [TextArea][SerializeField] private string message;
        void Start()
        {

        }
        public virtual void OnPointerEnter(PointerEventData e)
        {
            DisplayInfo();
        }
        public virtual void OnPointerExit(PointerEventData e)
        {
            Clear();
        }
        public virtual void DisplayInfo()
        {
            _TEXT_targetText.text = message;
        }

        public virtual void Clear()
        {
            _TEXT_targetText.text = string.Empty;
        }

    }
}

