using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonBuyCustom : ButtonTrade
    {
        private int amount;
        private bool canTrade;

        public int Amount
        {
            set { amount = value; }
        }
        public override void SetValue(int v) 
        {
            value = v; _TEXT_ValueText.text = (v).ToString();
            bool hasFunds = value <= PersistentGameData._GData_Gold;
            bool hasCapacity = ShipInventory.HasCapacity(fatherRow.GetStock().resource.Key, amount);

            canTrade = hasFunds && hasCapacity;
            SetButtonColor(canTrade);
        }
        protected override void Trade()
        {
            if(amount != 0 && canTrade)
            {
                //Change trader stock amount
                fatherRow.GetStock().amount -= amount;

                //Change value of player's gold (this call an updategold event)
                PersistentGameData._GData_Gold -= value;

                //Alter level of friendship
                TradeManager.currentTrader.ModifyFriendship_ByTrading(value);

                //Change player's ship inventory (this call a onload update event)
                ShipInventory.SetInventoryChange(fatherRow.GetStock().resource.Key, amount, value/amount);

                //Update this row's stock values on display
                fatherRow.UpdateStockValues();
            }

            //Throw event
            ThrowBuyActionEvent(fatherRow.GetStock().resource);
        }
    }
}

