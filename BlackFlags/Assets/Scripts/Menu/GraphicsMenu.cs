using UnityEngine;
using UnityEngine.UI;
using GameSettings.VFX;

namespace GameMechanics.Menu
{
    public class GraphicsMenu : MonoBehaviour
    {
        [SerializeField]
        private Toggle
            _Toggle_NightLight,
            _Toggle_PostProcessing;

        void Start()
        {
            _Toggle_NightLight.isOn = GraphicSettings.NightLight;
            _Toggle_PostProcessing.isOn = GraphicSettings.Postprocessing;

            _Toggle_NightLight.onValueChanged.AddListener(delegate { GraphicSettings.NightLight = _Toggle_NightLight.isOn; });
            _Toggle_PostProcessing.onValueChanged.AddListener(delegate { GraphicSettings.Postprocessing = _Toggle_PostProcessing.isOn; });
        }
    }
}


