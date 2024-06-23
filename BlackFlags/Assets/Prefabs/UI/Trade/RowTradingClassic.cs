using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowTradingClassic : RowEconomyBase
    {
        //rowData
        [SerializeField] private byte key;
        private Resource thisRowResource;
        private InventoryItemStacking traderStock;
        private int offerAmount = 1;

        public byte Key
        {
            get { return key; }
        }

        public Resource RowResource
        {
            get { return thisRowResource; }
        }

        public int OfferAmount
        {
            get { return offerAmount; }
            set 
            { 
                offerAmount = value;
                SetStock();
            }
        }

        //Components
        [SerializeField] private Text 
            _TEXT_traderOffer, 
            _TEXT_ShipInventory,
            _TEXT_InventoryAveragePrice;
        [SerializeField] private GameObject _averagePrice;
        [SerializeField] private ButtonBuyClassic _BUTTON_Buy;
        [SerializeField] private ButtonSellClassic _BUTTON_Sell;
        private void Awake()
        {
            thisRowResource = ShipInventory.GetResource(key);
        }
        protected virtual void OnEnable()
        {
            Refresh();
        }

#region EVENTS
        private void Start()
        {
            TradeManager.ChangeAmountClassicDisplay += SetPrice;
        }
        private void OnDestroy()
        {
            TradeManager.ChangeAmountClassicDisplay += SetPrice;
        }
        #endregion

        private void Refresh(byte val = 1)
        {
            //Trader offer amount
            GetStock();

            UpdateStockValues();
            SetPrice(val);
        }

        private void GetStock()
        {
            traderStock = TradeManager.currentTrader.SmugglerInventory.Find(x => x.resource == thisRowResource);
            offerAmount = traderStock == null ? 0 : traderStock.amount;
        }


        private void SetStock()
        {
            if (traderStock == null || offerAmount == 0)
            {
                InventoryItemStacking newStock = new InventoryItemStacking(thisRowResource, offerAmount);
                traderStock = newStock;
                newStock.amount = offerAmount;
                TradeManager.currentTrader.SmugglerInventory.Add(newStock);
                print("OWO");
            }

            traderStock.amount = offerAmount;

            UpdateStockValues();
            SetPrice(TradeManager.AmountClassic);
        }

        public void UpdateStockValues()
        {
            _TEXT_traderOffer.text = offerAmount.ToString();
            var inventory = ShipInventory.Items[key];
            _TEXT_ShipInventory.text = inventory.ToString();
            _averagePrice.SetActive(inventory != 0);

            if (inventory != 0)
            {
                var n = System.Math.Truncate(ShipInventory.AverageCost[Key] * 10)/10;
                _TEXT_InventoryAveragePrice.text = n.ToString();
            }
            
        }

        public void SetPrice(int val = 1)
        {
            int baseValue = thisRowResource.value;
            float offerModifier = 0;
            float disccountModifier = TradeManager.currentTrader.GetDiscountModifier();

            //----------------
            //Modificadores según oferta
            //----------------

            float sum = 0;
            int a = val < offerAmount ? val : offerAmount;
            for (int i = 0; i < a; i++)
            {
                offerModifier = GetOfferModifier(thisRowResource, offerAmount - i);
                sum += offerModifier;

                if (offerAmount - i == 0) break;
            }

            offerModifier = sum / val;

            //Ajuste final del precio
            int currentPrice_Buy = (int)Mathf.Floor(baseValue * (1 + offerModifier) / disccountModifier);

            //Establece valor de compra
            _BUTTON_Buy.SetValue(currentPrice_Buy);

            sum = 0;
            a = val < ShipInventory.Items[key] ? val : ShipInventory.Items[key];
            for (int i = 0; i < a; i++)
            {
                offerModifier = GetOfferModifier(thisRowResource, offerAmount - i);
                sum += offerModifier;
                if (offerAmount - i == 0) break;
            }

            offerModifier = sum / val;

            //Ajuste final del precio
            int currentPrice_Sell = (int)Mathf.Floor((baseValue * (1 + offerModifier) * disccountModifier) / 2f);

            //Establece valor de venta
            _BUTTON_Sell.SetValue(currentPrice_Sell);

        }
    }
}

