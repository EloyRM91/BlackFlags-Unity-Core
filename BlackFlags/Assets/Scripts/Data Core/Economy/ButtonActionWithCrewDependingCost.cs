using UnityEngine;

namespace GameMechanics.Data
{
    public class ButtonActionWithCrewDependingCost : ButtonActionWithCost
    {
        public int costPerMember;
        public string txt;
        protected override void OnEnable()
        {
            actionCost = costPerMember * (ShipInventory.Crew + 1);
            _TEXT_ValueText.text = $"{txt} - {actionCost}";
            base.OnEnable();
        }

    }
}

