using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowTradingClasicArmory : RowTradingClassic
    {
        [SerializeField] private Text _TEXT_armorCapacity;
        [SerializeField] private int armoryIndex;
        protected override void OnEnable()
        {
            base.OnEnable();
            CheckArmorCapacity();
        }

        private void CheckArmorCapacity()
        {
            var capacity = PersistentGameData._GData_PlayerShip.GetPowerCapacity()[armoryIndex];
            _TEXT_armorCapacity.text = capacity == 0 ? "-" : capacity.ToString();
        }
    }
}

