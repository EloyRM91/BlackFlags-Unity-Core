using UnityEngine;
using UnityEngine.UI;
using GameSettings.VFX;

namespace UI
{
    [DisallowMultipleComponent]
    public class GameImage : Image
    {
        private static Color nightColor = new Color(0.97f, 0.955f, 0.915f);

        protected override void OnEnable()
        {
            base.OnEnable();
            SetNightColor();
        }

        protected override void Start()
        {
            base.Start();

            GraphicSettings.lightNightChange += SetNightColor;
        }

        protected override void OnDestroy()
        {
            GraphicSettings.lightNightChange -= SetNightColor;
        }

        private void SetNightColor()
        {
            color = GraphicSettings.NightLight ? nightColor : Color.white;
        }
    }
}

