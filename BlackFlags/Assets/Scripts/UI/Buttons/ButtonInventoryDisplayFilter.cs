using UnityEngine;
using UnityEngine.UI;
using UI.WorldMap;

namespace GameMechanics.Data
{
    public class ButtonInventoryDisplayFilter : ButtonInfo
    {
        [SerializeField] private ResourceType showingResources;
        protected override void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate { TradeManager.FilterShipInventoryDisplay(showingResources); });
        }

    }
}

