using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowTradingScript : RowEconomy
    {
#region VARIABLES
        //--------
        //TRADING DATA
        //--------

        //Buying
        private int currentPrice_Buy;
        public int CurrentPrice_Buy
        {
            get { return currentPrice_Buy; }
        }

        //Max
        private int currentPrice_BuyMAX;
        public int CurrentPrice_BuyMAX
        {
            get { return currentPrice_BuyMAX; }
        }

        //Selling
        private int currentPrice_Sell;
        public int CurrentPrice_Sell
        {
            get { return currentPrice_Sell; }
        }

        //Graphic Display
        [Header("this row's HUD Components")]
        [SerializeField]
        private Image
            _IMG_ResourceIcon;
        [SerializeField] private Text
            _TEXT_ResourceName,
            _TEXT_CharacterStock,
            _TEXT_PlayerStock;
        [SerializeField] private ButtonBuy _BTBuy_buy;
        [SerializeField] private ButtonSell _BTSell_sell;
        [SerializeField] private ButtonBuyMax _BTBuy_buyMax;
       

        [Header("Quick Managing Optionss")]
        [SerializeField] private GameObject[] _QuickManaging;
        [SerializeField] private ButtonBuyCustom[] _QuickActionButtons;
        [SerializeField] private Text[] _QuickActionText;
        private ButtonBuyCustom _currentQuickAction;
        byte currentIndex;

#endregion

#region EVENTS
        private void Start()
        {
            ShipInventory.updateLoad += Refresh;
            TradeManager.ChangeAmount += SetPrice;
        }
        private void OnDestroy()
        {
            ShipInventory.updateLoad -= Refresh;
            TradeManager.ChangeAmount -= SetPrice;
        }
#endregion

        protected override void SetDisplayInfo()
        {
            Resource thisResource = stock.resource;

            //Nombre recurso
            _TEXT_ResourceName.text = thisResource.resourceName;

            //Color de fondo
            SeBottomColor();

            //Icono del recurso
            _IMG_ResourceIcon.sprite = GetResourceSprite(thisResource);

            //Valores de stock:
            UpdateStockValues();

            //Actualizar precio en botones de compra y venta
            SetPrice();

            //Botones de gestión rápida
            GetQuickAction();
        }
        
        public override void UpdateStockValues()
        {
            if (stock.amount != 0)
            {
                _TEXT_CharacterStock.text = stock.amount.ToString();
                _TEXT_PlayerStock.text = ShipInventory.Items[stock.resource.Key].ToString();
            }
            else TradeManager.DissapearRow(gameObject);
        }

        private void Refresh() //Actualiza por llamada a evento
        {
            //Precios
            //Establece valor de compra máxima
            _BTBuy_buyMax.SetValue(currentPrice_BuyMAX);

            //Actualiza gestión rápida
            GetQuickAction();

            //Muestra u oculta impedimentos de compra y muestra cantidad a adquirirs
            _BTBuy_buy.UpdateTradingAmount();
        }

        public void SetPrice(int val = 1)
        {
            int baseValue = stock.resource.value;
            float offerModifier = 0;
            float disccountModifier = TradeManager.currentTrader.GetDiscountModifier();

            //----------------
            //Modificadores según oferta
            //----------------
            float sum = 0;
            int a = val <= stock.amount ? val : stock.amount;
            for (int i = 0; i < a; i++)
            {
                offerModifier = GetOfferModifier(stock.resource, stock.amount - i);
                sum += offerModifier;
                if (stock.amount - i == 0) break;
            }

            offerModifier = sum / val;
            //print("trader mod: " + offerModifier);

            //Ajuste final del precio
            currentPrice_Buy = (int)Mathf.Floor(baseValue * (1 + offerModifier) / disccountModifier);

            //Establece valores de compra
            _BTBuy_buy.SetValue(currentPrice_Buy);


            sum = 0;
            a = val <= ShipInventory.Items[stock.resource.Key] ? val : ShipInventory.Items[stock.resource.Key];
            for (int i = 0; i < a; i++)
            {
                offerModifier = GetOfferModifier(stock.resource, stock.amount - i);
                sum += offerModifier;
                if (stock.amount - i == 0) break;
            }

            offerModifier = sum / val;


            //Ajuste final del precio
            currentPrice_Sell = (int)Mathf.Floor((baseValue * (1 + offerModifier) * disccountModifier) / 2f);

            //Establece valores de venta
            _BTBuy_buy.SetValue(currentPrice_Buy);
            _BTSell_sell.SetValue(currentPrice_Sell);

            //Establece valor de compra máxima
            currentPrice_BuyMAX = stock.amount == 1 ? currentPrice_Buy : GetBuyMaxAverageValue(baseValue, disccountModifier);
            _BTBuy_buyMax.SetValue(currentPrice_BuyMAX);
        }

        private int GetBuyMaxAverageValue(int baseValue, float discountModifier)
        {
            float offerModifier = 0;
            float sum = 0;
            for (int i = 0; i < stock.amount; i++)
            {
                offerModifier = GetOfferModifier(stock.resource, stock.amount - i);
                sum += offerModifier;
            }

            offerModifier = sum / stock.amount;

            return (int)Mathf.Floor(baseValue * (1 + offerModifier) * discountModifier);
        }

        private int GetByAmountAverageValue(int baseValue, float discountModifier, int buyingAmount)
        {
            float offerModifier = 0;
            float sum = 0;
            for (int i = 0; i < buyingAmount; i++)
            {
                offerModifier = GetOfferModifier(stock.resource, stock.amount - i);
                sum += offerModifier;
            }

            offerModifier = sum / buyingAmount;

            return (int)Mathf.Floor(baseValue * (1 + offerModifier) * discountModifier);
        }
        private void GetQuickAction()
        {
            _QuickManaging[currentIndex].SetActive(false);
            int amount = 0;
            if (stock.resource.ResourceType == ResourceType.consumible)
            {
                currentIndex = 0;
                amount = (int)ShipInventory.GetNecessarySupplies15(stock.resource.Key);
                _QuickActionText[currentIndex].text = $"Avituallar para 15 días (x{amount})";
            }
            else if (stock.resource.Key == 16)
            {
                currentIndex = 1;
                amount = (int)ShipInventory.GetNecessaryGuns(stock.resource.Key);
                _QuickActionText[currentIndex].text = $"Armar a toda la tripulación (x{amount})";
            }
            else if (stock.resource.ResourceType == ResourceType.weapons)
            {
                currentIndex = 2;
                amount = (int)ShipInventory.GetNecessaryGuns(stock.resource.Key);
                _QuickActionText[currentIndex].text = $"Armar {PersistentGameData._GData_ShipName} (x{amount})";
            }
            else return;

            if (amount <= stock.amount)
            {
                _QuickManaging[currentIndex].SetActive(true);
                _currentQuickAction = _QuickActionButtons[currentIndex];
                _currentQuickAction.Amount = amount;

                int value = GetByAmountAverageValue(stock.resource.value, TradeManager.currentTrader.GetDiscountModifier(), amount);
                _currentQuickAction.SetValue(value * amount);
            }



        }
    }
}

