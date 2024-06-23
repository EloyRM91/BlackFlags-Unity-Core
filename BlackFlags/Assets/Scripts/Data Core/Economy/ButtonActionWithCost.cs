using UnityEngine;
using UnityEngine.UI;

namespace GameMechanics.Data
{
    public class ButtonActionWithCost : ButtonTradeBasic
    {
        public int actionCost;

        protected virtual void OnEnable()
        {
            bool canPayActionCost = actionCost <= PersistentGameData._GData_Gold;
            SetButtonColor(canPayActionCost);
            GetComponent<Button>().interactable = canPayActionCost;
        }

        protected override void Trade()
        {
            PersistentGameData._GData_Gold -= actionCost;
        }

    }
}
