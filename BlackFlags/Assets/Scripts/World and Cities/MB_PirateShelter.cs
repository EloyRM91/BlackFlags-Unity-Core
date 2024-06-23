using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameMechanics.Data;
using GameMechanics.Ships;

namespace GameMechanics.WorldCities
{
    public class MB_PirateShelter : KeyPoint
    {
        [SerializeField] private RequirementEntry _requirement;
        [SerializeField] private byte _value;
        //Convoys in this port
        public List<Convoy> convoysInThisPort = new List<Convoy>();
        //Characters in shelter
        [SerializeField] private List<Character> charactersInShelter = new List<Character>();


        //public override void OnMouseDown()
        //{
        //    base.OnMouseDown();
        //    if(UIMap.ui.gameObject.activeSelf)
        //        if (!UIMap.panelON)
        //        {
        //            if (!UIMap.GetGraphicRaycastResult())
        //                UIMap.ui.DisplayInfo(this);
        //        }
        //}

        protected override void DisplayKeypointPanel()
        {
            //todavía no hay panel para la vista de villas
            UIMap.ui.DisplayInfo(this);
        }

        protected override void DisplayInfo()
        {
            UIMap.ui.DisplayInfo(this);
        }

        public void GetIn(Character c)
        {
            charactersInShelter.Add(c);
        }
        public void GetOut(Character c)
        {
            charactersInShelter.Remove(c);
        }
        public string GetRequirementString()
        {
            var r = string.Empty;

            switch (_requirement)
            {
                case RequirementEntry.ByFame_Spain: return " tu <color=red>Fama en España</color> debe ser mayor a " + _value + "%";
                case RequirementEntry.ByFame_Portugal: return " tu <color=red>Fama en Portugal</color> debe ser mayor a " + _value + "%";
                case RequirementEntry.ByFame_France: return " tu <color=red>Fama en Francia</color> debe ser mayor a " + _value + "%";
                case RequirementEntry.ByFame_GB: return " tu <color=red>Fama en Gran Bretaña</color> debe ser mayor a " + _value + "%";
                case RequirementEntry.ByFame_Dutch: return " tu <color=red>Fama en Holanda</color> debe ser mayor a " + _value + "%";
                case RequirementEntry.LoyaltyToCodeBiggerThan: return " tu <color=red>Lealtad al Código</color> debe superar el " + _value + "%";
                case RequirementEntry.LoyaltyToCodeLowerThan: return " tu <color=red>Lealtad al Código</color> debe ser inferior al " + _value + "%";
            }

            return r;
        }
    }

    public enum RequirementEntry {ByFame_Spain, ByFame_GB, ByFame_France, ByFame_Dutch, ByFame_Portugal, LoyaltyToCodeBiggerThan, LoyaltyToCodeLowerThan};
}

