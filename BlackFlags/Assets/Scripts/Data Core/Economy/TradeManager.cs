using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

namespace GameMechanics.Data
{
    public class TradeManager : EconomyBehaviour
    {
        //Singleton
        private static TradeManager instance;

        //Data
        public static Smuggler currentTrader;
        private int amount = 1;
        private int amountClassicDisplay = 1;
        public static int Amount
        {
            get { return instance.amount; }
            set { instance.amount = value; ChangeAmount(value); }
        }

        public static int AmountClassic
        {
            get { return instance.amountClassicDisplay; }
            set { instance.amountClassicDisplay = value; ChangeAmountClassicDisplay(value); }
        }
        //Graphic Display Elements
        [Header("Trade Panel elements")]
        [SerializeField] private GameObject _tradePanel;
        [SerializeField] private Text 
            _TEXT_LabelName,
            _TEXT_InventoryDescriptor,
            _TEXT_ShipLoad,
            _TEXT_ShipName;

        [Header("Sell Panel elements")]
        [SerializeField] private GameObject _SellFromInventoryPanel;
        [SerializeField] private Text 
            _TEXT_SellingToName,
            _TEXT_MaxAmmount,
            _TEXT_CurrentAmmount,
            _TEXT_AverageCost;
        [SerializeField] private Slider _Slider_Sell;
        [SerializeField] private Image _IMG_SellingPanelImage;
        [SerializeField] private ButtonSellFromSlider _sliderButton;

        //Pooling
        [Header("Pooling: Offer & inventory elements rows")]
        //Trading: pooling and Intantiate
        [SerializeField] private GameObject _PREFAB_tradingResourceRow;
        [SerializeField] private GameObject _PREFAB_shipReourceRow;
        [SerializeField] private Transform content, contentShip;
        [SerializeField] List<GameObject> pool = new List<GameObject>();
        [SerializeField] List<GameObject> poolInventory = new List<GameObject>();

        //Filtering
        private ResourceType currentTypeOfferFilter = ResourceType.Any;
        private List<GameObject> current, showing, currentOnInventory, showingOnInventory;

        //Trading lockers
        [SerializeField] private Sprite[] _IMG_tradeLock;

        //Events
        public delegate void ValueChange(int value);
        public static ValueChange ChangeAmount;
        public static ValueChange ChangeAmountClassicDisplay;

        //Settings
        private ResourceType currentTypeInventoryFilter = ResourceType.Any;

        private void Awake()
        {
            //singleton
            instance = this;
        }
#region EVENTS
        private void Start()
        {
            RowCharacterButton.TalkWithSmuggler += SetCurrentTrader;
            ButtonTrade.buyAction += RegenerateInventory_ResourceOnly;
            ButtonTrade.sellAction += RegenerateInventory_ResourceOnly;
            ButtonTrade.sellAction += RegenerateOfferDisplay_ResourceOnly;
            ShipInventory.updateLoad += UpdateShipLoad;
        }
        private void OnDestroy()
        {
            RowCharacterButton.TalkWithSmuggler -= SetCurrentTrader;
            ButtonTrade.buyAction -= RegenerateInventory_ResourceOnly;
            ButtonTrade.sellAction -= RegenerateInventory_ResourceOnly;
            ButtonTrade.sellAction -= RegenerateOfferDisplay_ResourceOnly;
            ShipInventory.updateLoad -= UpdateShipLoad;
        }
#endregion

#region SETTERS
        private void SetCurrentTrader(Character character)
        {
            currentTrader = (Smuggler)character;
        }
        private void SetCurrentTrader(Smuggler character)
        {
            currentTrader = character;
        }
#endregion

        #region HUD: Main Panel

        public static Sprite GetTradeLocker(byte i)
        {
            return instance._IMG_tradeLock[i];
        }
        public static void SetTradingHUD()
        {
            instance.SetHUD();
        }
        private void SetHUD()
        {
            //Open panel
            _tradePanel.SetActive(true);

            //Set Elements
            _TEXT_LabelName.text = $"Comerciando con {currentTrader.CharacterName}";
            _TEXT_SellingToName.text = $"Vender a {currentTrader.CharacterName}";
            _TEXT_ShipName.text = $"{PersistentGameData._GData_ShipName}:";

            RegenerateOfferDisplay();
            RegenerateInventory_Display();
            FilterShipInventoryDisplay(ResourceType.Any);
        }

        public void RegenerateOfferDisplay()
        {
            //Clean scroll view
            foreach (GameObject row in pool)
            {
                row.SetActive(false);
            }
            current = new List<GameObject>();

            //Show Smuggler offer
            currentTrader.SmugglerInventory = currentTrader.SmugglerInventory.FindAll(stock => stock.amount != 0);

            Debug.Log("en lista: " + currentTrader.SmugglerInventory.Count);
            foreach (InventoryItemStacking stock in currentTrader.SmugglerInventory)
            {
                Debug.Log(stock.resource.resourceName + " - " + stock.amount);
                if (stock.amount > 0)
                {
                    //Get pool objetc
                    var r = GetRow(content, _PREFAB_tradingResourceRow, current).GetComponent<RowTradingScript>();
                    //Set stock data and info display
                    r.SetResourceInfo(stock);
                }
            }
            showing = new List<GameObject>(current);
        }

        private void RegenerateOfferDisplay_ResourceOnly(Resource resource)
        {
            var stock = currentTrader.SmugglerInventory.Find(st => st.resource == resource);
            GameObject row = current.Find(s => s.GetComponent<RowTradingScript>().GetStock().resource == resource);

            if (stock != null)
            {
                RowTradingScript r;
                if (row == null)
                {
                    print("no encuentro el recurso :@");
                    //Get pool objetc
                    row = GetRow(content, _PREFAB_tradingResourceRow, current);
                }
                row.SetActive(true);
                r = row.GetComponent<RowTradingScript>();

                //Set stock data and info display
                r.SetResourceInfo(stock);
                showing = new List<GameObject>(current);
            }
            else //if don't have stock, remove the row
            {
                if (row != null)
                    DissapearRow(row.GetComponent<RowTradingScript>());
            }
            ShowingResourceofType(currentTypeOfferFilter);
        }

        private void RegenerateInventory_Display()
        {
            //Show Ship Inventory
            foreach (GameObject row in poolInventory)
            {
                row.SetActive(false);
            }
            var inventory = ShipInventory.Items;
            currentOnInventory = new List<GameObject>();
            for (byte i = 0; i < inventory.Length; i++)
            {
                if (inventory[i] != 0)
                {
                    //Get pool objetc
                    var r = GetRow(contentShip, _PREFAB_shipReourceRow, currentOnInventory).GetComponent<RowShipInventoryScript>();

                    //Set stock data and info display
                    r.SetResourceInfo(i, inventory[i]);
                }
            }
            showingOnInventory = new List<GameObject>(currentOnInventory);
        }

        private void RegenerateInventory_ResourceOnly(Resource resource)
        {
            //bool activeBefore = true;
            //var advancedPanel = _tradePanel.transform.GetChild(0).gameObject;
            //if (!advancedPanel.activeSelf)
            //{
            //    advancedPanel.SetActive(true);
            //    activeBefore = false;
            //}
            int amount = ShipInventory.Items[resource.Key];
            GameObject row = currentOnInventory.Find(s => s.GetComponent<RowShipInventoryScript>().GetStock().resource == resource);

            RowShipInventoryScript rs;

            if(amount != 0)
            {
                if (row == null)
                {
                    row = GetRow(contentShip, _PREFAB_shipReourceRow, currentOnInventory);
                }
                rs = row.GetComponent<RowShipInventoryScript>();
                rs.SetResourceInfo(resource.Key, amount);
                ResourceType type = rs.GetStockType() != ResourceType.weapons ? ResourceType.Any : ResourceType.weapons;
                FilterShipInventoryDisplay(currentTypeInventoryFilter);
            }
            else
            {
                if (row != null)
                {
                    currentOnInventory.Remove(row);
                    row.SetActive(false);
                }
            }
            //advancedPanel.SetActive(true);
        }

        private GameObject GetRow(Transform parentTarget, GameObject prefab, List<GameObject> currentList)
        {
            //Try get object from list
            List<GameObject> currentPool = parentTarget == content ? pool : poolInventory;
            foreach (GameObject row in currentPool)
            {
                if (!row.activeSelf)
                {
                    row.SetActive(true);
                    currentList.Add(row);
                    return row;
                }
            }
            //Expand pool
            GameObject r = Instantiate(_PREFAB_tradingResourceRow, parentTarget);
            currentPool.Add(r);
            return r;
        }

        public static void CloseTradePanel()
        {
            instance._tradePanel.SetActive(false);
        }

        public static void DissapearRow(RowTradingScript row)
        {
            instance.current.Remove(row.gameObject);
            row.gameObject.SetActive(false);
        }
        public static void DissapearRow(GameObject row)
        {
            instance.current.Remove(row);
            row.SetActive(false);
        }

        //Filtering: Smuggler offer display
        public static void ShowingResourceofType(ResourceType type)
        {
            instance.currentTypeOfferFilter = type;
            if (type == ResourceType.Any)
            {
                instance.showing = new List<GameObject>(instance.current);
            }
            else
            {
                instance.showing = new List<GameObject>(instance.current).Where(x => x.GetComponent<RowTradingScript>().GetStockType() == type).ToList();
            }
            instance.UpdateView(instance.current, instance.showing);
        }

        private void UpdateView(List<GameObject> globalList, List<GameObject> filteredList)
        {
            foreach (GameObject row in globalList)
            {
                row.SetActive(false);
            }
            foreach (GameObject row in filteredList)
            {
                row.SetActive(true);
            }
        }

        //Filtering: Ship Inventory Display
        public static void FilterShipInventoryDisplay(ResourceType type)
        {
            instance.currentTypeInventoryFilter = type;
            string txt = string.Empty;
            if (type == ResourceType.Any)
            {
                instance.showingOnInventory = new List<GameObject>(instance.currentOnInventory).Where(x => x.GetComponent<RowShipInventoryScript>().GetStockType() != ResourceType.weapons).ToList();

                int dm = 9999;
                int lack = 0;
                int lackOf = 0;
                for (byte i = 0; i < 5; i++)
                {
                    var totalStock = ShipInventory.Items[i] + ShipInventory.Surplus[i];

                    if (totalStock == 0)
                    {
                        lack++;
                        lackOf = i;
                    }
                    var consumption = ShipInventory.GetNecessarySupplies7(i);
                    var days = (int)Mathf.Floor(7 * totalStock / (consumption));

                    dm = days < dm ? days : dm;
                }

                if (lack > 1)
                    txt = $"Faltan suministros básicos a bordo";
                else if (lack == 1)
                    txt = $"Es preciso abastecerse de {D_WorldResources[lackOf].resourceName}.";
                else
                    txt = $"Hay suministros a bordo para mantener a la tripulación {0} días.";

                var n = ShipInventory.Items[5];
                txt += 
                    n > 25 ? " Hay herramientas de sobra para reparos y carenado" :
                    n > 16 ? " Las herramientas y materiales a bordo son suficientes." :
                    n > 9 ? " La reserva de herramientas podría ser mayor." :
                    " Los materiales y herramientas son insuficientes";

            }
            else if (type == ResourceType.weapons)
            {
                instance.showingOnInventory = new List<GameObject>(instance.currentOnInventory).Where(x => x.GetComponent<RowShipInventoryScript>().GetStockType() == type).ToList();
                txt = UI.WorldMap.UIConvoyHUD.CheckArmory();
            }
            instance.UpdateView(instance.currentOnInventory, instance.showingOnInventory);
            instance._TEXT_InventoryDescriptor.text = txt;
        }

        private void UpdateShipLoad()
        {
            _TEXT_ShipLoad.text = $"{System.Math.Truncate(ShipInventory.shipLoad * 10)/10} / {PersistentGameData._GData_PlayerShip.GetCapacity()} ton.\n {ShipInventory.Crew} tripulantes";
        }
        #endregion

        #region HUD: Selling Emergent Panel
        public static void OpenSellPanel(InventoryItemStacking stock) //Panel con slider
        {
            instance.DisplaySellPanelData(stock);
        }

        private void DisplaySellPanelData(InventoryItemStacking stock) //Panel con slider
        {
            var thisResource = stock.resource;
            _SellFromInventoryPanel.SetActive(true);
            _IMG_SellingPanelImage.sprite = GetResourceSprite(thisResource);
            _Slider_Sell.maxValue = stock.amount;
            _TEXT_MaxAmmount.text = stock.amount.ToString();
            _Slider_Sell.value = 1;
            _TEXT_CurrentAmmount.text = _Slider_Sell.value.ToString();
            _TEXT_AverageCost.text = $"Coste unitario: {ShipInventory.AverageCost[stock.resource.Key]} ";

            _sliderButton.CurrentResource = stock.resource;
            _sliderButton.Amount = (int)_Slider_Sell.value;
            
        }

        public void OnSliderChange()
        {
            _sliderButton.Amount = (int)_Slider_Sell.value;
            _TEXT_CurrentAmmount.text = _Slider_Sell.value.ToString();
        }

        public static void UpdateSliderST(int max)
        {
            instance.UpdateSlider(max);
        }

        private void UpdateSlider(int max)
        {
            if (max == 0)
            {
                _SellFromInventoryPanel.SetActive(false);
            }
            else
            {
                _TEXT_MaxAmmount.text = max.ToString();
                _Slider_Sell.maxValue = max;
                _Slider_Sell.value = _Slider_Sell.value <= max ? _Slider_Sell.value : max;
                _sliderButton.Amount = (int)_Slider_Sell.value;
            }
        }

        #endregion

    }

}


