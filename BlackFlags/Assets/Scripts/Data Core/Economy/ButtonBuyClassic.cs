using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonBuyClassic : ButtonTradeBasic
    {
        [SerializeField] private RowTradingClassic fatherRow;

        public override void SetValue(int v)
        {
            value = v;
            _TEXT_ValueText.text = (v * TradeManager.AmountClassic).ToString();
        }
        protected override void Trade()
        {
            int stock = fatherRow.OfferAmount;

            if (stock != 0)
            {
                byte currentKey = fatherRow.Key;
                int currentAmount = TradeManager.AmountClassic < stock ? TradeManager.AmountClassic : stock;
                int currentPrice = value * currentAmount;
                bool canPurchase = currentPrice <= PersistentGameData._GData_Gold && ShipInventory.HasCapacity(currentKey, currentAmount);

                if (canPurchase && currentAmount != 0)
                {
                    //Change trader stock amount
                    fatherRow.OfferAmount -= currentAmount;
                    
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
                //Throw event
                ThrowBuyActionEvent(fatherRow.RowResource);
            }
        }
    }
}

