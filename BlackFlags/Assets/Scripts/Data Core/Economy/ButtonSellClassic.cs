using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonSellClassic : ButtonTradeBasic
    {
        [SerializeField] private RowTradingClassic fatherRow;

        public override void SetValue(int v)
        {
            value = v;
            _TEXT_ValueText.text = (v * TradeManager.AmountClassic).ToString();
        }

        protected override void Trade()
        {
            int stock = ShipInventory.Items[fatherRow.Key];

            if (stock != 0) //if player can sell
            {
                byte currentKey = fatherRow.Key;
                int currentAmount = TradeManager.AmountClassic < stock ? TradeManager.AmountClassic : stock;
                int currentPrice = value * currentAmount;

                if(currentAmount != 0) //Has this character the current resourt already?
                {
                    //Change trader stock amount
                    fatherRow.OfferAmount += currentAmount;

                    //Change value of player's gold (this call an updategold event)
                    PersistentGameData._GData_Gold += currentPrice;

                    //Alter level of friendship
                    TradeManager.currentTrader.ModifyFriendship_ByTrading(currentPrice);

                    //Change player's ship inventory (this call a onload update event)
                    ShipInventory.SetInventoryChange(currentKey, -currentAmount);

                    //Update offer-demand prices for this resource
                    fatherRow.SetPrice();

                    //Update this row's stock values on display
                    fatherRow.UpdateStockValues();
                }
                //Throw event
                ThrowSellActionEvent(fatherRow.RowResource);
                //TradeManager.RegenerateOfferDisplayST();
            }
        }
    }
}

