//Core
using System.Collections.Generic;
using UnityEngine;
//Mechanics
using GameMechanics.Data;

namespace GameMechanics.WorldCities
{
    public class MB_SmugglersPost : Settlement
    {
        //CHARACTERS IN SHELTER
        [SerializeField] private List<Character> charactersInShelter = new List<Character>();
        //public override void OnMouseDown()
        //{
        //    base.OnMouseDown();
        //    if (UIMap.ui.gameObject.activeSelf)
        //        if (!UIMap.panelON)
        //        {
        //            if (!UIMap.GetGraphicRaycastResult())
        //                UIMap.ui.DisplayInfo(this);
        //        }

        //}

        private void Start()
        {
            //----
            // CHARACTERS
            //----
            CreateSmugglers(Random.Range(2,4));
        }
        private void CreateSmugglers(int n)
        {
            for (int i = 0; i < n; i++)
            {
                var tradingResources = new List<Resource>();
                for (int j = 0; j < exports.Count; j++)
                {
                    tradingResources.Add(exports[i]);
                }
                tradingResources.Add(EconomyBehaviour.GetResource(16));
                var cosas = new int[7] { 0, 2, 3, 4, 5, 17, 18 };

                for (int k = 0; k < cosas.Length; k++)
                {
                    var ar = EconomyBehaviour.GetResource((byte)cosas[k]);
                    if (!tradingResources.Contains(ar))
                    {
                        if (UnityEngine.Random.Range(0, 3) < 2)
                            tradingResources.Add(ar);
                    }
                }
                var newSmuggler = new Smuggler((EntityType_KINGDOM)Random.Range(0,5), tradingResources);

                foreach (Resource res in newSmuggler.SmugglerOffer)
                {
                    var inventory = newSmuggler.SmugglerInventory;
                    inventory.Add(new InventoryItemStacking(res, UnityEngine.Random.Range(5, 50)));
                }
                charactersInShelter.Add(newSmuggler);
            }
        }

        protected override void DisplayKeypointPanel()
        {
            //todavía no hay panel para la vista de villas
            UIMap.ui.DisplayInfo(this);
        }

        protected override void DisplayInfo()
        {
            UIMap.ui.DisplayInfo(this);
        }
    }
}
