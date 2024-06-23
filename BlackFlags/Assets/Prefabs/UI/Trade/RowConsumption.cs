using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowConsumption : MonoBehaviour
    {
        [SerializeField] private byte index;
        [SerializeField]
        private Text
            _TEXT_StockValue,
            _TEXT_Consumption_CREW,
            _TEXT_Consumption_SHIP,
            _TEXT_Reserve;
        private static Color
            _COLOR_Ok = new Color(0.196f, 0.196f, 0.196f, 1),
            _COLOR_runOut = Color.red,
            _COLOR_lackOf = new Color(1, 0.58f, 0, 1);
        private void Awake()
        {
            ShipInventory.updateLoad += SetInfo;
        }
        private void OnDestroy()
        {
            ShipInventory.updateLoad -= SetInfo;
        }
        private void OnEnable()
        {
            SetInfo();
        }

        private void SetInfo()
        {
            var consumption = ShipInventory.GetNecessarySupplies7(index);

            _TEXT_StockValue.text = ShipInventory.Items[index].ToString();
            _TEXT_Consumption_CREW.text = (Mathf.Floor(consumption * 10)/10).ToString();
            _TEXT_Consumption_SHIP.text = index == 5 ? 
                (Mathf.Floor((float)PersistentGameData._GData_PlayerShip.GetManteinance() / 4.5f)/10).ToString() : "-";

            //Reserves:
            var totalStock = ShipInventory.Items[index] + ShipInventory.Surplus[index];
            if (totalStock == 0)
            {
                string txt = index == 5 ? "herramientas" : EconomyBehaviour.GetResource(index).resourceName;
                _TEXT_Reserve.text = $"¡No hay {txt}!";
                _TEXT_Reserve.color = _COLOR_runOut;
            }
            else
            {
                var days = (int)Mathf.Floor(7 * totalStock/ (consumption + (index == 5 ? ((float)PersistentGameData._GData_PlayerShip.GetManteinance() / 45) : 0)));
                _TEXT_Reserve.color = days < 5 ? _COLOR_lackOf : _COLOR_Ok;
                _TEXT_Reserve.text = $"{days} días";
            }
                
        }
    }
}

