using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public class ButtonAlternate : MonoBehaviour
    {
        [SerializeField] private GameObject[] _views;
        [SerializeField] private Sprite[] _buttonSprites;
        private bool state;
        void Start()
        {
            //Actions
            GetComponent<Button>().onClick.AddListener(delegate { Click(); });
            //Events
            UIConvoyHUD.npcSelected += SetView3D;
        }
        private void OnDestroy()
        {
            //Events
            UIConvoyHUD.npcSelected -= SetView3D;
        }
        private void Click()
        {
            state = !state;
            _views[0].SetActive(!state);
            _views[1].SetActive(state);
            GetComponent<Image>().sprite = _buttonSprites[state ? 1 : 0];
        }

        private void SetView3D()
        {
            state = true;
            Click();
        }
    }
}


