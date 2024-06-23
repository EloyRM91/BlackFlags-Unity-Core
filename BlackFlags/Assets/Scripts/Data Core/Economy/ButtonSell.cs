using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonSell : ButtonTrade
    {
//#region EVENTS
//        private void Awake()
//        {
//            TradeManager.ChangeAmount += UpdateTradingAmount;
//        }
//        private void OnDestroy()
//        {
//            TradeManager.ChangeAmount -= UpdateTradingAmount;
//        }
//#endregion

        //Override
        protected override void Trade()
        {
            var index = fatherRow.GetStock().resource.Key;
            var stock = ShipInventory.Items[index];
            if (stock > 0)
            {
                int currentAmount = TradeManager.Amount < stock ? TradeManager.Amount : stock;
                int currentPrice = currentAmount * value;

                //Change trader stock amount
                fatherRow.GetStock().amount += currentAmount;

                //Change value of player's gold (this call an updategold event)
                PersistentGameData._GData_Gold += currentPrice;

                //Alter friendship level
                TradeManager.currentTrader.ModifyFriendship_ByTrading(currentPrice);

                //Change player's ship inventory (this call a onload update event)
                ShipInventory.SetInventoryChange(fatherRow.GetStock().resource.Key, -currentAmount);

                //Update offer-demand prices for this resource
                fatherRow.SetPrice();

                //Update this row's stock values on display
                fatherRow.UpdateStockValues();

                //Throw event
                ThrowSellActionEvent(fatherRow.GetStock().resource);
            }
        }

        //Setter
        public override void SetValue(int v)
        {
            value = v;
            UpdateTradingAmount(TradeManager.Amount);
        }

        //HUD
        public void UpdateTradingAmount(int val)
        {
            var stockAmount = ShipInventory.Items[fatherRow.GetStock().resource.Key];
            int a = val > stockAmount ? stockAmount : val;
            _TEXT_ValueText.text = (value * a).ToString();
        }
    }
}

