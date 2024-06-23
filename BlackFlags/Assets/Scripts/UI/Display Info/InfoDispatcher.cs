using UnityEngine;
using UnityEngine.UI;

namespace UI.WorldMap
{
    public class InfoDispatcher : MonoBehaviour
    {
        private static Transform infoBanner;
        private static Text _TEXT_Info;
        void Awake()
        {
            //Refs
            infoBanner = transform.GetChild(0);
            _TEXT_Info = infoBanner.GetChild(0).GetComponent<Text>();

            //Inactive banner by default
            Sleep();
        }

        public static void DisplayInfo(string text)
        {
            _TEXT_Info.text = text;
            infoBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(37 + text.Length * 6.2f, 26);
            var pos = Input.mousePosition;
            var y = pos.y + 20;
            var x = pos.x + 20;


            y = y + 10 > Screen.height ? pos.y - 50 : y;
            x = x  + 20 > Screen.width ? pos.x - 20 - text.Length/2 : x;
            infoBanner.position = Vector3.up * y + Vector3.right * (x +  (pos.x < Screen.width/20 ? 70 : 0));
           
            infoBanner.gameObject.SetActive(true);
        }


        public static void Sleep()
        {
            infoBanner.gameObject.SetActive(false);
        }
    }

    public interface IDisplayInfo
    {
        public void DisplayInfo();
    }
}

