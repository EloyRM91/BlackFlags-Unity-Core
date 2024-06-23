using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonBuyMax : ButtonTrade
    {
        public int amount;

        protected override void Start()
        {
            base.Start();
            ButtonTrade.sellAction += ListenSellCall;
        }
        protected override void Trade()
        {
            var stock = fatherRow.GetStock().amount;

            if (amount != 0)
            {
                int currentPrice = amount * value;
                bool canPurchase = currentPrice <= PersistentGameData._GData_Gold;
                if (canPurchase)
                {
                    //Change trader stock amount
                    fatherRow.GetStock().amount -= amount;

                    //Change value of player's gold (this call an updategold event)
                    PersistentGameData._GData_Gold -= currentPrice;

                    //Change player's ship inventory (this call a onload update event)
                    ShipInventory.SetInventoryChange(fatherRow.GetStock().resource.Key, amount, value);

                    //Update offer-demand prices for this resource
                    fatherRow.SetPrice();

                    //Update this row's stock values on display
                    fatherRow.UpdateStockValues();
                }
                //Enough gold to keep on buying? 
                SetButtonColor(canPurchase);

                //Throw event
                ThrowBuyActionEvent(fatherRow.GetStock().resource);
            }
        }
        public override void SetValue(int v)
        {
            value = v; SetMaxValue();
        }

        private void UpdateText()
        {
            _TEXT_ValueText.text = (value * amount).ToString();
        }

        private void ListenSellCall(Resource r)
        {
            SetMaxValue();
        }

        public void SetMaxValue()
        {
            Resource thisResource = fatherRow.GetStock().resource;

            //Máximo según capacidad de carga
            var shipCapacity = PersistentGameData._GData_PlayerShip.GetCapacity();
            var playerLoad = ShipInventory.shipLoad;
            float remainingCapacity = shipCapacity - playerLoad;
            int amountByCapacity = (int)Mathf.Floor(remainingCapacity / thisResource.weight); //Max value by capacity

            //Máximo según dinero del jugador
            var money = PersistentGameData._GData_Gold;
            int amountByGold = money / value ; //Max value by price

            //Máximo por stock del comerciante
            int amountByStock = fatherRow.GetStock().amount;

            //Selecciona el valor más bajo
            var n = new int[3] { amountByCapacity, amountByGold, amountByStock };
            //print(amountByCapacity + " - " + amountByGold + " - " + amountByStock);
            int minimum = 999;
            for (int i = 0; i < 3; i++)
            {
                if (n[i] < minimum) minimum = n[i];
            }

            amount = Mathf.Clamp(minimum,0,999);
            UpdateText();
        }
    }
}

