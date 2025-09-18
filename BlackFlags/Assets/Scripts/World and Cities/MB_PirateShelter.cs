using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics.save;
using GameMechanics.Data;
using GameMechanics.Ships;

namespace GameMechanics.WorldCities
{
    public class MB_PirateShelter : KeyPoint
    {
        [SerializeField] private EntryClass _entryCondition;
        public string tavernName;
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

            switch (_entryCondition.requirement)
            {
                case RequirementEntry.ByFame_Spain: return " tu <color=red>Fama en España</color> debe ser mayor a " + _entryCondition.value + "%";
                case RequirementEntry.ByFame_Portugal: return " tu <color=red>Fama en Portugal</color> debe ser mayor a " + _entryCondition.value + "%";
                case RequirementEntry.ByFame_France: return " tu <color=red>Fama en Francia</color> debe ser mayor a " + _entryCondition.value + "%";
                case RequirementEntry.ByFame_GB: return " tu <color=red>Fama en Gran Bretaña</color> debe ser mayor a " + _entryCondition.value + "%";
                case RequirementEntry.ByFame_Dutch: return " tu <color=red>Fama en Holanda</color> debe ser mayor a " + _entryCondition.value + "%";
                case RequirementEntry.LoyaltyToCodeBiggerThan: return " tu <color=red>Lealtad al Código</color> debe superar el " + _entryCondition.value + "%";
                case RequirementEntry.LoyaltyToCodeLowerThan: return " tu <color=red>Lealtad al Código</color> debe ser inferior al " + _entryCondition.value + "%";
            }

            return r;
        }

        public void SetFromSerializedData(SerializablePirateShelter portData)
        {
            this.cityName = portData.cityName;
            gameObject.name = portData.cityName;
            this.alternativeName = portData.alternativeName;
            this.tavernName = portData.tavernName;
            this.revealed = portData.revealed;
            this.transform.position = SerializationConverter.ToVector3(portData.position);

            //Target Path:
            var entryPoint = this.transform.GetChild(0);
            entryPoint.position = SerializationConverter.ToVector3(portData.entryPoint);

            //Banner's Pivot
            var pivot = this.transform.GetChild(1);
            pivot.position = SerializationConverter.ToVector3(portData.pivotPoint);

            //todo: events point
            
            //todo: modificar el sprite en función del índice
            //todo: (podemos tener más de un tipo de sprite para este tipo de keypoint)
            var index = portData.spriteIndex;
            if(portData.flippedX)
            {
                var s = this.transform.localScale;
                this.transform.localScale = new Vector3(-s.x, s.y, s.z);
            }
        }

        public GameObject GetKeyPointBanner(Transform container)
        {
            string fileName = "Smuggglers post Banner OnScreen - " + (this.cityName.Length > 13 ? "Large" : "Small");
            var prefab = Resources.Load<GameObject>("KeyPointBanners/" + fileName);

            if(prefab == null)
            {
                Debug.LogError("no file");
            }

            var banner = Instantiate(prefab, container);
            banner.name = "Banner Controller - " + this.cityName;
            if(banner.TryGetComponent<UI.WorldMap.BannerController>(out UI.WorldMap.BannerController controller))
            {
                controller.SetNewTarget(transform);
                LinkUIBanner(banner);
                return banner;
            }
            Debug.LogError("no component");
            return null;
        }
    }

    [System.Serializable]
    public enum RequirementEntry {ByFame_Spain, ByFame_GB, ByFame_France, ByFame_Dutch, ByFame_Portugal, LoyaltyToCodeBiggerThan, LoyaltyToCodeLowerThan};

    /// <summary>
    /// This class contains a shelter's entry requirement and a threshold condition value
    /// <example>
    /// <code>
    /// var r = new EntryClass(RequirementEntry.LoyaltyToCodeBiggerThan, 50);
    /// </code>
    /// </example>
    /// </summary>
    [System.Serializable]
    public class EntryClass 
    {
        public RequirementEntry requirement;
        public byte value;

        public EntryClass(RequirementEntry requirement, byte value) {
            this.requirement = requirement;
            this.value = value;
        }

        public EntryClass(EntryClass entry) {
            this.requirement = entry.requirement;
            this.value = entry.value;
        }
    }
}

