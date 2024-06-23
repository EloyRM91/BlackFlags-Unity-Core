using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonSellAll : ButtonTradeBasic
    {
        private Resource currentResource;
        private int inventoryAmount;
        public Resource CurrentResource
        {
            //set{ currentResource = value; inventoryAmount = ShipInventory.Items[value.Key]; }
            set { currentResource = value; }
        }
        protected override void Trade()
        {
            GetPriceValue();
            //Price
            int currentPrice = inventoryAmount * value;
            print($"vendiendo {inventoryAmount} por {value}");
            //Change trader stock amount
            InventoryItemStacking s = TradeManager.currentTrader.SmugglerInventory.Find(stack => stack.resource == currentResource);
            if (s == null)
            {
                s = new InventoryItemStacking(currentResource, inventoryAmount);
                TradeManager.currentTrader.SmugglerInventory.Add(s);
            }
            else
            {
                s.amount += inventoryAmount;
            }

            //Change value of player's gold (this call an updategold event)
            PersistentGameData._GData_Gold += currentPrice;

            //Alter friendship level
            TradeManager.currentTrader.ModifyFriendship_ByTrading(currentPrice);

            //Change player's ship inventory (this call a onload update event)
            //stockAmount -= tradingAmount;
            ShipInventory.SetInventoryChange(currentResource.Key, -inventoryAmount);

            //Update rows (this throw an sellAction event)
            ThrowSellActionEvent(currentResource);
        }


        private void GetPriceValue()
        {
            int baseValue = currentResource.value;
            float offerModifier = 0;
            float disccountModifier = TradeManager.currentTrader.GetDiscountModifier();

            inventoryAmount = ShipInventory.Items[currentResource.Key];
            //Modificadores según oferta

            float sum = 0;
            for (int i = 0; i < inventoryAmount; i++)
            {
                InventoryItemStacking s = TradeManager.currentTrader.SmugglerInventory.Find(stack => stack.resource == currentResource);
                int traderOfferAmount = s == null ? 0 : s.amount;
                offerModifier = RowEconomy.GetOfferModifier(currentResource, traderOfferAmount - i);
                sum += offerModifier;
            }

            offerModifier = sum / inventoryAmount;


            value = (int)Mathf.Floor((baseValue / 1.7f) * (1 - offerModifier) * disccountModifier);
        }

    }
}

