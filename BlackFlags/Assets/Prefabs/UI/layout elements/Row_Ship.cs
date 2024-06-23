using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Mechanics
using GameMechanics.Ships;

namespace UI.WorldMap
{
    public class Row_Ship : MonoBehaviour
    {
        public static UIConvoyHUD display;
        public bool selected = false;
        public Ship linkedShip;
        private Text _TEXT_Name, _TEXT_Class;

        private void Start()
        {
            GetComponent<Button>().onClick.AddListener(Click);
        }

        public void SetData(Ship ship)
        {
            linkedShip = ship;
            _TEXT_Name = transform.GetChild(0).GetComponent<Text>();
            _TEXT_Class = transform.GetChild(1).GetComponent<Text>();
            _TEXT_Name.text = ship.name_Ship;
            _TEXT_Class.text = ship.GetModelName();
            //StartCoroutine(SetDataSync(ship));
        }

        IEnumerator SetDataSync(Ship ship)
        {
            while (_TEXT_Name == null)
            {
                yield return null;
            }
            while (_TEXT_Class == null)
            {
                yield return null;
            }
            _TEXT_Name.text = ship.name_Ship;
            _TEXT_Class.text = ship.GetModelName();
            StopAllCoroutines();
        }
        private void Click()
        {
            if (!selected)
            {
                //Unselect other rows and select the current one
                display.UnselectAllRows();
                selected = true;
                display.SetShipViewData(linkedShip);
                //Color
                _TEXT_Name.color = Color.blue;
                _TEXT_Class.color = Color.blue;
                //Show ship data view
                display.OpenViewDisplay(0);
                //Ogus pocus ;V
                display.ActivateDisplayPanel();
            }

            //Hago cosas
            // Cosa 1: establezco los datos de esta embarcaicón en el display
            // Cosa 2: desactivo la vista de convoy y activo la vista de embarcación
            // Cosa 3: marci de color azul este objeto, si hay otro seleccionado, pues se desmarca.
        }

        public void Unselect()
        {
            selected = false;
            _TEXT_Name.color = Color.black;
            _TEXT_Class.color = Color.black;
        }
    }
}

