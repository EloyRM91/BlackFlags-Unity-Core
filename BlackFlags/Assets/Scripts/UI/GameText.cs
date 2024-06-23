using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using GameSettings.VFX;


namespace UI
{
    [DisallowMultipleComponent]
    public class GameText : Text
    {
        public static GameFontsMode fontMode;

        public static GameFontsMode FontMode
        {
            set { fontMode = value; _EVENT_SetMode(); }
        }

        [SerializeField]private FontType fontType;

        //Data
        private static Color nightColor = new Color(0.905f, 0.878f, 0.772f);

        //private static Font
        //    pirate,
        //    caligraf,
        //    regular;

        //public static Font Pirate { set { pirate = value; } }
        //public static Font Caligraf { set { pirate = value; } }
        //public static Font Regular { set { regular = value; } }

        public static bool canUpdate = false;
        public static Dictionary<FontType, FontStyle[]> _D_FontStyles = new Dictionary<FontType, FontStyle[]>();

        //Events
        public delegate void Change();
        public static event Change _EVENT_SetMode;

        protected override void OnEnable()
        {
            base.OnEnable();
            if (canUpdate)
            {
                SetFontMode();
            }

            if(color == Color.white || color == nightColor)
            {
                SetNightColor();
            }
        }

        protected override void Start()
        {
            base.Start();
            SetFontMode();
            _EVENT_SetMode += SetFontMode;

            if(color == Color.white || color == nightColor)
            {
                GraphicSettings.lightNightChange += SetNightColor;
            }
        }

        protected override void OnDestroy()
        {
            _EVENT_SetMode -= SetFontMode;
            GraphicSettings.lightNightChange -= SetNightColor;
        }

        private void SetFontMode()
        {
            var index = (int)fontMode;
            var newStyle = _D_FontStyles[fontType][index];

            this.font = newStyle.font;
            this.fontSize = newStyle.size;
        }

        private void SetNightColor()
        {
            color = GraphicSettings.NightLight ? nightColor : Color.white;
        }
    }

    public class FontStyle
    {
        public Font font;
        public int size;

        public FontStyle(Font font, int size)
        {
            this.font = font;
            this.size = size;
        }
    }

    public enum GameFontsMode {normal, overSize, dislexic}

    public enum FontType { mainTitle, mainMenuButton, subButton, text22, text27, title, caligraf }
}

