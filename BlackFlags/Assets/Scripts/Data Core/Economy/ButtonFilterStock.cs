using UnityEngine.UI;
using UnityEngine;
using GameMechanics.Data;
namespace UI.WorldMap
{
    public class ButtonFilterStock : ButtonInfo
    {
        [SerializeField] private ResourceType type;
        protected override void Start()
        {
            base.Start();
            GetComponent<Button>().onClick.AddListener(delegate { TradeManager.ShowingResourceofType(type); });
        }

    }
}
