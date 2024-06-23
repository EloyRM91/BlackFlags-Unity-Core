using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToggleFontMode : Toggle
    {
        [SerializeField] private GameFontsMode fontMode;

        protected override void Start()
        {
            onValueChanged.AddListener(delegate { if(isOn) OnSelect(); });
        }

        private void OnSelect()
        {
            GameText.FontMode = fontMode;
        }
    }
}


