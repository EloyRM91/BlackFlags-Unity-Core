using UnityEngine;
//Canvas
using UnityEngine.UI;
//Mechanics
using GameMechanics.Data;
//Datetime
using System;

namespace UI.WorldMap
{
    public class ToggleRationing : Info
    {
        //Toggle parameters
        [SerializeField] private int index;
        private int itemIndex;

        //Components
        private Toggle toggle;
        private Text label;

        private static Color locked = Color.red;
        private static Color unlocked = new Color(0.169f, 0.169f, 0.169f, 1);

        private bool lockedByTiming;
        private DateTime unlockerDate;

        private void Awake()
        {
            //Is this toggle rationing rum or meat?
            itemIndex = index == 10 ? 4 : 2;

            //Components
            toggle = GetComponent<Toggle>();
            label = transform.GetChild(1).GetComponent<Text>();

            //Events
            TimeManager.NewDay += CheckLockDate;
        }
        private void OnEnable()
        {
            CheckLockDate(TimeManager.GetDate());
            if (ShipInventory.Items[itemIndex] + ShipInventory.Surplus[itemIndex] == 0)
            {
                toggle.interactable = false;
            }
            label.color = toggle.interactable ? unlocked : locked;
        }
        protected override void Start()
        {
            toggle.onValueChanged.AddListener(delegate { Action(); });
        }

        private void Action()
        {
            ShipInventory.Rationing(toggle.isOn, index);
            Lock();
        }

        private void Lock()
        {
            toggle.interactable = false;
            label.color = locked;
            lockedByTiming = true;
            unlockerDate = TimeManager.GetFutureDate(3);
        }

        private void Unlock()
        {
            toggle.interactable = true;
            label.color = unlocked;
            lockedByTiming = false;
        }

        private void CheckLockDate(DateTime date)
        {
            if (lockedByTiming)
            {
                var currentDate = TimeManager.GetDate();
                if(currentDate >= unlockerDate)
                {
                    //has the resource currently in inventory?
                    if(ShipInventory.Items[itemIndex] + ShipInventory.Surplus[itemIndex] >= 0)
                    {
                        Unlock();
                    }
                    else
                    {
                        lockedByTiming = false;
                    }
                    
                }
            }
        }

        public override void DisplayInfo()
        {
            string txt = string.Empty;
            if (lockedByTiming)
                txt = $"No se puede modificar otra vez hasta el {TimeManager.GetDateString(unlockerDate)}";
            else if (!toggle.interactable)
                txt = "Falta el recurso a racionar";

            if(txt != string.Empty)
                InfoDispatcher.DisplayInfo(txt);
        }
    }
}



