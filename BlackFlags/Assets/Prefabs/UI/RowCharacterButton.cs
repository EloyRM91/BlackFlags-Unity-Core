using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowCharacterButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        //Color Data
        private static Color
            _CLR_nonHighlighted = new Color(0.196f, 0.196f, 0.196f, 1),
            _CLR_highlighted = new Color(1, 0.734f, 0, 1);

        //this row's character
        public Character character;

        //Event: Talk with character
        public delegate void TalkEvent(Character character);
        public static event TalkEvent TalkWithSmuggler;

        private void OnEnable()
        {
            transform.GetChild(0).GetComponent<Text>().color = _CLR_nonHighlighted;
            transform.GetChild(1).GetComponent<Text>().color = _CLR_nonHighlighted;
        }
        void Start()
        {
            GetComponent<Button>().onClick.AddListener(() => TalkWithCharacter());
        }
        public void OnPointerEnter(PointerEventData eData)
        {
            transform.GetChild(0).GetComponent<Text>().color = _CLR_highlighted;
            transform.GetChild(1).GetComponent<Text>().color = _CLR_highlighted;
        }
        public void OnPointerExit(PointerEventData eData)
        {
            transform.GetChild(0).GetComponent<Text>().color = _CLR_nonHighlighted;
            transform.GetChild(1).GetComponent<Text>().color = _CLR_nonHighlighted;
        }
        public void TalkWithCharacter()
        {
            //Close Tavern's UI panels
            UI_SpecialViewsHUD.ClearTavernUI_ST();
            //Fade Off Dialog Box
            DialogManager.DM.OpenDialogBox(character, GetTriggerType());
            //Change Character-Header info
            UI_TavernView.SetCharacterLabelST(character);
            //Is Smuggler? Advise Trade Manager this is the smuggler you're talking with
            if (character is Smuggler) TalkWithSmuggler(character);
        }

        private DialogTrigger GetTriggerType()
        {
            if (character is Smuggler)
            {
                return DialogTrigger.dialogTalkWithSmuggler;
            }
            else if (character is Pirate) return DialogTrigger.dialogTalkWithPirate;
            else return DialogTrigger.dialogNull;
        }
    }
}

