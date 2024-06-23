using UnityEngine;
using UnityEngine.UI;


namespace GameMechanics.Data
{

    public abstract class RowEconomyBase : EconomyBehaviour
    {
        #region ECONOMY

        public static float GetOfferModifier(Resource resource, int ammount)
        {
            int baseValue = resource.value;

            float sensivityFactor;
            int peakFactor;
            switch (resource.ResourceType)
            {
                case ResourceType.consumible:
                    sensivityFactor = 0.013f;
                    peakFactor = 25;
                    break;
                case ResourceType.tradingGoods:
                    sensivityFactor = 0.039f;
                    peakFactor = 45;
                    break;
                case ResourceType.luxury:
                    sensivityFactor = 0.012f;
                    peakFactor = 9;
                    break;
                case ResourceType.weapons:
                    sensivityFactor = 0.025f;
                    peakFactor = 18;
                    break;
                default:
                    sensivityFactor = 0.025f;
                    peakFactor = 10;
                    break;
            }

            return Mathf.Clamp((peakFactor - ammount) * sensivityFactor, -.62f, .62f);
        }

        #endregion
    }
    public abstract class RowEconomy : RowEconomyBase
    {
        [Header("Inherited components (by RowEconomy class)")]
        [SerializeField] private Image _IMG_Bottom;

        //Data
        protected InventoryItemStacking stock;

        private static Color[] colorsByResource = new Color[4]
        {
            new Color(0.596f, 0.301f, 0, 0.309f),
            new Color(1, 0.9f, 0, 0.309f),
            new Color(1, 0, 0.89f, 0.309f),
            new Color(1, 0, 0, 0.32f)
        };

        //Get & Set
        public ResourceType GetStockType()
        {
            return stock.resource.ResourceType;
        }

        public InventoryItemStacking GetStock()
        {
            return stock;
        }

        public virtual void SetResourceInfo(InventoryItemStacking resource)
        {
            stock = resource;
            SetDisplayInfo();
        }
        public virtual void SetResourceInfo(int key, int amount)
        {
            stock = new InventoryItemStacking(D_WorldResources[key], amount);
            SetDisplayInfo();
        }

        public virtual void SeBottomColor()
        {
            Resource thisResource = stock.resource;
            var n = (byte)thisResource.ResourceType;

            _IMG_Bottom.color = colorsByResource[n];
        }

        protected abstract void SetDisplayInfo();
        public abstract void UpdateStockValues();
    }
}

