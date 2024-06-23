using System.Collections;
using UnityEngine; using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class ButtonSellFromSlider : ButtonTrade
    {
        private int currentIndex;
        [SerializeField] private Text _TEXT_TotalSellingValue;
        public int CurrentIndex
        {
            get { return currentIndex; }
        }
        private Resource currentResource;
        private int tradingAmount;

        public Resource CurrentResource
        {
            set 
            { 
                currentResource = value; 
                currentIndex = value.Key; 
                //GetPriceValue(); 
            }
        }
        public int Amount
        {
            set { tradingAmount = value; StartCoroutine("IEGetPriceValue"); }
        }
        protected override void Trade()
        {
            if (tradingAmount > 0 && tradingAmount <= ShipInventory.Items[currentIndex])
            {
                //Price
                int currentPrice = tradingAmount * value;

                //Change trader stock amount
                InventoryItemStacking s = TradeManager.currentTrader.SmugglerInventory.Find(stack => stack.resource == currentResource);
                if (s == null)
                {
                    s = new InventoryItemStacking(currentResource, tradingAmount);
                    TradeManager.currentTrader.SmugglerInventory.Add(s);
                } 
                else
                {
                    s.amount += tradingAmount;
                }
               
                //Change value of player's gold (this call an updategold event)
                PersistentGameData._GData_Gold += currentPrice;

                //Alter friendship level
                TradeManager.currentTrader.ModifyFriendship_ByTrading(currentPrice);

                //Change player's ship inventory (this call a onload update event)
                //stockAmount -= tradingAmount;
                ShipInventory.SetInventoryChange(currentResource.Key, -tradingAmount);

                //Update rows (this throw an sellAction event)
                ThrowSellActionEvent(currentResource);

                var max = ShipInventory.Items[currentIndex];
                TradeManager.UpdateSliderST(max);
            }
        }

        public override void SetValue(int v) 
        {
            //print($"value: {v * amount}, amount = {amount} // Base value: {currentResource.value}");
            value = v; 
            _TEXT_ValueText.text = $"Vender(x{tradingAmount})";
            _TEXT_TotalSellingValue.text = (v * tradingAmount).ToString();
        }

        IEnumerator IEGetPriceValue()
        {
            while (currentResource == null)
            {
                yield return null;
            }
            GetPriceValue();
        }

        private void GetPriceValue()
        {
            int baseValue = currentResource.value;
            float offerModifier = 0;
            float disccountModifier = TradeManager.currentTrader.GetDiscountModifier();
            //Modificadores según oferta

            float sum = 0;
            for (int i = 0; i < tradingAmount; i++)
            {
                InventoryItemStacking s = TradeManager.currentTrader.SmugglerInventory.Find(stack => stack.resource == currentResource);
                int traderOfferAmount = s == null ? 0 : s.amount;
                offerModifier = RowEconomy.GetOfferModifier(currentResource, traderOfferAmount - i);
                sum += offerModifier;
            }

            offerModifier = sum / tradingAmount;
            //print("inventory mod: " + offerModifier);


            var currentAveragePrice = (int)Mathf.Floor((baseValue / 1.7f) * (1 - offerModifier) * disccountModifier);
            SetValue(currentAveragePrice);
        }
    }
}

