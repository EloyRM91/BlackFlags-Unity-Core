using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class ButtonBuy : ButtonTrade
    {
        [SerializeField] private Image purchaseLockState;

        //Override
        protected override void Trade()
        {
            var stock = fatherRow.GetStock().amount;

            if(stock != 0)
            {
                byte currentKey = fatherRow.GetStock().resource.Key;
                int currentAmount = TradeManager.Amount < stock ? TradeManager.Amount : stock;
                int currentPrice = currentAmount * value;
                bool canPurchase = currentPrice <= PersistentGameData._GData_Gold && ShipInventory.HasCapacity(currentKey, currentAmount);
                if (canPurchase)
                {
                    //Change trader stock amount
                    fatherRow.GetStock().amount -= currentAmount;

                    //Change value of player's gold (this call an updategold event)
                    PersistentGameData._GData_Gold -= currentPrice;
                    
                    //Alter level of friendship
                    TradeManager.currentTrader.ModifyFriendship_ByTrading(currentPrice);

                    //Change player's ship inventory (this call a onload update event)
                    ShipInventory.SetInventoryChange(currentKey, currentAmount, value);

                    //Update offer-demand prices for this resource
                    fatherRow.SetPrice();

                    //Update this row's stock values on display
                    fatherRow.UpdateStockValues();
                }

                //Enough gold to keep on buying? 
                //SetButtonColor(canPurchase);

                //Throw event
                ThrowBuyActionEvent(fatherRow.GetStock().resource);
            }
        }

        //Setter
        public override void SetValue(int v)
        {
            value = v;
            UpdateTradingAmount();
        }

        //HUD
        public void UpdateTradingAmount()
        {
            var val = TradeManager.Amount;
            var stockAmount = fatherRow.GetStock().amount;
            int a = val > stockAmount ? stockAmount : val;
            _TEXT_ValueText.text = (value * a).ToString();

            purchaseLockState.enabled = false;

            bool lockedByFunds = a * fatherRow.CurrentPrice_Buy > PersistentGameData._GData_Gold;
            bool lockedByCapacity = !(ShipInventory.HasCapacity(fatherRow.GetStock().resource.Key, a));

            if (lockedByFunds)
            {
                purchaseLockState.enabled = true;
                purchaseLockState.sprite = TradeManager.GetTradeLocker(0);
            }
            else if (lockedByCapacity)
            {
                purchaseLockState.enabled = true;
                purchaseLockState.sprite = TradeManager.GetTradeLocker(1);
            }

            SetButtonColor(!lockedByFunds && !lockedByCapacity);
        }
    }
}

