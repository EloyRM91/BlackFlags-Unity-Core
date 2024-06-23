using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class RowShipInventoryScript : RowEconomy
    {
        [Header("This row element's components")]
        [SerializeField] private Image
            _IMG_ResourceIcon;
        [SerializeField] private Text 
            _TEXT_ResourceAmount, 
            _TEXT_ResourceName;
        [SerializeField] private Button SellButton; //Botón de venta que activa abre el panel de slider
        [SerializeField] private ButtonSellAll sellAllButton;

        private void Start()
        {
            SellButton.onClick.AddListener(delegate { ActionSell(); });
            //SellAllButton.onClick.AddListener(delegate { ActionSell(); });
        }

        protected override void SetDisplayInfo()
        {
            Resource thisResource = stock.resource;
            sellAllButton.CurrentResource = thisResource;

            //Nombre recurso
            _TEXT_ResourceName.text = thisResource.resourceName;

            //Color de fondo
            SeBottomColor();

            //Icono del recurso
            _IMG_ResourceIcon.sprite = GetResourceSprite(thisResource);

            //Valores de stock:
            UpdateStockValues();

        }

        public override void UpdateStockValues()
        {
            _TEXT_ResourceAmount.text = stock.amount.ToString();
        }

        private void ActionSell()
        {
            RefreshStock();
            TradeManager.OpenSellPanel(stock);
        }

        private void SellAll()
        {

        }

        private void RefreshStock()
        {
            stock.amount = ShipInventory.Items[stock.resource.Key];
        }
    }

}

