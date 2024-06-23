//Core
using UnityEngine; using System.Collections.Generic;
//UI
using UnityEngine.UI;
//Mechanics
using GameMechanics.Ships;
//Game Data
using Generation.Generators;
using GameMechanics.Data;

namespace GameMechanics.Menu
{
    public class ShipSelector : MonoBehaviour
    {
        #region VARIABLES

        //-----
        //COMPONENTS REF
        //-----

        //Text Components
        [SerializeField] private Text _TEXT_description, _TEXT_shipClass, _TEXT_ShipName, _TEXT_Resume;
        //Ship name's input field
        [SerializeField] private InputField _inputField;
        //Models
        [SerializeField] private GameObject[] _ships;
        //Icons
        [SerializeField] private Sprite[] _shipIcons;
        //Image Components
        [SerializeField] private Image _selectedShipIcon;

        //-----
        //PARAMETERS
        //-----

        //current options index
        private int index = 0;
        //Selected ship
        private static GameObject currentShip;

        //----
        //DATA
        //----

        //Description text
        private string[] _descriptions = new string[3]
        {
        "Usado comúnmente por marinas europeas y por piratas, el balandro destacó por ser un barco ágil y rápido. Su diseño alcanzó su apogeo entre los siglos XVIII y XIX.",
        "Diseñados para un fácil manejo y poco mantenimiento, los faluchos fueron faeneros que posteriormente fueron armados como pequeños barcos corsarios.",
        "De mayor carga y eslora que el falucho, la tartana fue un barco destinado al tráfico costero empleado como patrulla por los europeos, desde Portugal hasta Grecia."
        };
        //Names
        private string[] _names = new string[3] { "Balandro", "Falucho", "Tartana" };

        //----
        //SHIPS STATS & UI REFS
        //----
        //Text
        [Header("Ship Stats")]
        //Fillers
        [SerializeField] private Image[] _IMGS_Fillers;
        [SerializeField] private Text 
            _Stat_SmoothWind,
            _Stat_Upwind, 
            _Stat_Maneuverability, 
            _Stat_Load, 
            _Stat_Maintenance, 
            _Stat_Hull, 
            _Stat_Range, 
            _Stat_FirePower;

        #endregion

        void Start()
        {
            _inputField.onValueChanged.AddListener(delegate { SetShipName(); });
            Select(0);
            GetRandomName();
        }
        public void Next()
        {
            if (index < 2)
            {
                index++;
                Select(index);
            }
        }
        public void Back()
        {
            if (index > 0)
            {
                index--;
                Select(index);
            }
        }
        private void Select(int i)
        {
            _TEXT_description.text = _descriptions[i];
            _TEXT_shipClass.text = _names[i];

            switch (i)
            {
                case 0: PersistentGameData._GData_PlayerShip = new ShipSubCategory_CoastalSloop(); break;
                case 1: PersistentGameData._GData_PlayerShip = new ShipSubCategory_TwoMastlesFelucca(); break;
                case 2: PersistentGameData._GData_PlayerShip = new ShipSubCategory_TwoMastlesTartain(); break;
            }
            PersistentGameData._GData_PlayerShip.name_Ship = _TEXT_ShipName.text;

            if (currentShip != null)
                currentShip.SetActive(false);
            currentShip = _ships[i];
            _selectedShipIcon.sprite = _shipIcons[i];
            currentShip.SetActive(true);

            //Get selected ship stats
            SetStats(PersistentGameData._GData_PlayerShip);
        }

        private void GetRandomName()
        {
            _inputField.text = WorldGenerator.GiveShipName(PersistentGameData._GDataPlayerNation, ShipType_ROLE.Pirate, GenerationMode.Random);
        }


        private void SetShipName()
        {
            PersistentGameData._GData_ShipName = _inputField.text;
            _TEXT_Resume.text = _inputField.text;
        }

        //Stats
        private void SetStats(Ship selectedShip)
        {
            //Velocidad de sotavento
            _Stat_SmoothWind.text = $"{selectedShip.GetMinSmoothSpeed()} - {selectedShip.GetMaxSmoothSpeed()} nudos";
            _IMGS_Fillers[0].fillAmount = selectedShip.GetMaxSmoothSpeed() / 15;

            //Velocidad de bordada
            _Stat_Upwind.text = $"{selectedShip.GetMinUpwindSpeed()} - {selectedShip.GetMaxUpwindSpeed()}";
            _IMGS_Fillers[1].fillAmount = selectedShip.GetMaxUpwindSpeed() / 15;

            //Maniobrabilidad
            _Stat_Maneuverability.text = selectedShip.GetManeuverability() + " %";
            _IMGS_Fillers[2].fillAmount = selectedShip.GetManeuverability() / 100;

            //Tonelaje
            _Stat_Load.text = selectedShip.GetCapacity() + "t";
            _IMGS_Fillers[3].fillAmount = Mathf.Clamp(selectedShip.GetCapacity() / 350, 0.02f, 1);

            //Mantenimiento
            _Stat_Maintenance.text = selectedShip.GetManteinance().ToString();
            _IMGS_Fillers[4].fillAmount = Mathf.Clamp(selectedShip.GetManteinance() / 170, 0.02f,1);

            //Casco
            _Stat_Hull.text = selectedShip.GetShellPoints().ToString();
            _IMGS_Fillers[5].fillAmount = (float)selectedShip.GetShellPoints() / 60;

            //Rango de Visión
            _Stat_Range.text = selectedShip.GetVisibility().ToString();
            _IMGS_Fillers[6].fillAmount = (float)selectedShip.GetVisibility() / 10;

            //Ataque
            _Stat_FirePower.text = selectedShip.GetFirePower().ToString();
            _IMGS_Fillers[7].fillAmount = (float)selectedShip.GetFirePower() / 110;
        }
    }
}

