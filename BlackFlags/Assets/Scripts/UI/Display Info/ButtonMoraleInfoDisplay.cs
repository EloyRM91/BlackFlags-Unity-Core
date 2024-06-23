using UnityEngine;
using GameMechanics.Data;

namespace UI.WorldMap
{
    public class ButtonMoraleInfoDisplay : ButtonInfoOnTarget
    {
        [SerializeField] private GameObject infoDisplay;
        void Start()
        {

        }
        public override void DisplayInfo()
        {
            var txt = $"Valor base: {ShipInventory.Morale_Global * 100}%";
            var mod = MoraleModifier.GetMoraleModifier();
            txt += "\n" + (mod != 0 ? $"Por modificadores: {mod * 100}%" : string.Empty);

            txt += $"\n\n        Total: {(ShipInventory.Morale_Global + mod) * 100}%";
            _TEXT_targetText.text = txt;
            infoDisplay.SetActive(true);
        }

        public override void Clear()
        {
            //_TEXT_targetText.text = string.Empty;
            infoDisplay.SetActive(false);
        }
    }
}

