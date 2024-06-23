using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Sound
{
    public class TrackRowScript : MonoBehaviour
    {
        private static TrackRowScript currentPlaying = null;
        public static int activeTracks = 0;

        private static Color
            selectedColor = new Color(1, 0.733f, 0, 1),
            notSelectedColor = new Color(1, 1, 1, 0),
            disabledColor = new Color(0.781f, 0.781f, 0.781f, 0.8f);

        public static Color DisabledColor
        {
            get { return disabledColor; }
        }

        //Events
        public delegate void Selection(int value);
        public static event Selection SelectTrack;
        public delegate void SelectionVoid();
        public static event SelectionVoid SkipCurrent;

        void Start()
        {
            //Toggle
            transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener(delegate { ToggleSelect(); });

            //Button
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { ButtonSelect(); });

        }

        private void ToggleSelect()
        {
            var thisToggle = transform.GetChild(0).GetComponent<Toggle>();
            transform.GetChild(1).GetComponent<Button>().interactable = thisToggle.isOn;
            transform.GetChild(1).GetChild(0).GetComponent<Text>().color = thisToggle.isOn ? new Color(0.15f, 0.15f, 0.15f, 1) : disabledColor;

            activeTracks = thisToggle.isOn ? activeTracks + 1 : activeTracks - 1;
            if (!thisToggle.isOn)
            {
                transform.GetChild(1).GetComponent<Image>().color = notSelectedColor;
                if (currentPlaying == this)
                {
                    SkipCurrent(); // si se reproduce este pista actualmente debe saltar a otra;
                }
                if (activeTracks == 0) currentPlaying = null;
            }
            else
            {
                if(activeTracks == 1)
                {
                    ButtonSelect();
                }
            }

        }

        private void ButtonSelect()
        {
            if (currentPlaying != this)
            {
                if (currentPlaying != null)
                {
                    currentPlaying.transform.GetChild(1).GetComponent<Image>().color = notSelectedColor;
                }

                currentPlaying = this;

                currentPlaying.transform.GetChild(1).GetComponent<Image>().color = selectedColor;

                SelectTrack(transform.GetSiblingIndex());
            }
        }

        public static void SetCurrentPlay(byte index, Transform container)
        {
            if(container.childCount > 0)
            {
                if (currentPlaying != null)
                {
                    currentPlaying.transform.GetChild(1).GetComponent<Image>().color = notSelectedColor;
                }
                currentPlaying = container.GetChild(index).GetComponent<TrackRowScript>();
                currentPlaying.transform.GetChild(1).GetComponent<Image>().color = selectedColor;
            }
        }

        public static void SetCurrentPlay(byte index)
        {
            var container = GameObject.FindWithTag("TrackContainer").transform;

            if (currentPlaying != null)
            {
                currentPlaying.transform.GetChild(1).GetComponent<Image>().color = notSelectedColor;
            }
            currentPlaying = container.GetChild(index).GetComponent<TrackRowScript>();
            currentPlaying.transform.GetChild(1).GetComponent<Image>().color = selectedColor;
        }

        public static bool isActiveTrack(Transform container, byte index)
        {
            return container.GetChild(index).GetChild(0).GetComponent<Toggle>().isOn;
        }
    }
}

