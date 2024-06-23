using UnityEngine.UI;
using UnityEngine;

namespace GameMechanics.Data
{

    public abstract class ButtonTradeBasic : MonoBehaviour
    {
        protected int value;
        [SerializeField] protected Text _TEXT_ValueText;

        //Events
        public delegate void TradingAction(Resource resource);
        public static event TradingAction buyAction;
        public static event TradingAction sellAction;

        protected virtual void Start()
        {
            GetComponent<Button>().onClick.AddListener(delegate { Trade(); });
        }

        protected abstract void Trade();
        public virtual void SetValue(int v) { value = v; _TEXT_ValueText.text = v.ToString(); }

        protected void ThrowBuyActionEvent(Resource r)
        {
            buyAction(r);
        }

        protected void ThrowSellActionEvent(Resource r)
        {
            sellAction(r);
        }

        //static info
        protected static Color
            canTrade = new Color(0.196f, 0.196f, 0.196f, 1),
            cannotTrade = Color.red;

        protected void SetButtonColor(bool can)
        {
            _TEXT_ValueText.color = can ? canTrade : cannotTrade;
        }
    }
        
    public abstract class ButtonTrade : ButtonTradeBasic
    {
        [SerializeField] protected RowTradingScript fatherRow;
    }
}



