//Core
using System.Collections.Generic;
using UnityEngine;
using GameSettings.Core;
//Mechanics
using GameMechanics.Data;
using GameMechanics.save;

namespace GameMechanics.WorldCities
{
    public class MB_SmugglersPost : Settlement
    {
        //CHARACTERS IN SHELTER
        // [SerializeField] private List<Character> charactersInShelter = new List<Character>();
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
            if (PersistentGameSettings.loadingFile == false)
            {
                //*Genera datos de la nueva partida
                //----
                // CHARACTERS
                //----
                CreateNewSmugglers(Random.Range(2, 4));
            }

        }
        private void CreateNewSmugglers(int n)
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
                var KingdomsContainer = GameObject.FindWithTag("Kingdoms").transform;
                var newSmuggler = new Smuggler((ushort)Random.Range(0, KingdomsContainer.childCount), tradingResources);

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
            UIMap.ui.DisplayInfo(this);
        }

        protected override void DisplayInfo()
        {
            UIMap.ui.DisplayInfo(this);
        }

        public void SetFromSerializedData(SerializableSmugglersPost portData)
        {
            this.cityName = portData.cityName;
            gameObject.name = portData.cityName;
            this.alternativeName = portData.alternativeName;
            this.revealed = portData.revealed;
            this.transform.position = portData.ToVector3(portData.position);

            if (portData.flippedX)
            {
                var s = this.transform.localScale;
                this.transform.localScale = new Vector3(-s.x, s.y, s.z);
            }

            //Target Path:
            var entryPoint = this.transform.GetChild(0);
            entryPoint.position = portData.ToVector3(portData.entryPoint);

            //Banner's Pivot
            var pivot = this.transform.GetChild(1);
            pivot.position = portData.ToVector3(portData.pivotPoint);

            //todo: events point

            //todo: modificar el sprite en función del índice
            //todo: (podemos tener más de un tipo de sprite para este tipo de keypoint)
            var index = portData.spriteIndex;

            this.exportsIndex = portData.exports;

            //* Personajes en este enclave:
            SerializableBaseCharacter[] chs = portData.serializedCharacters;
            if (chs == null) return;

            for (int i = 0; i < chs.Length; i++)
            {
                SerializableBaseCharacter schar = chs[i];

                if (schar is SerializableSmuggler)
                {
                    var newSmuggler = (schar as SerializableSmuggler).Deserialize();
                    charactersInShelter.Add(newSmuggler);
                }
            }

        }

        public GameObject GetKeyPointBanner(Transform container)
        {
            string fileName = "Smuggglers post Banner OnScreen - " + (this.cityName.Length > 13 ? "Large" : "Small");
            var prefab = Resources.Load<GameObject>("KeyPointBanners/" + fileName);

            if (prefab == null)
            {
                Debug.LogError("no file");
            }

            var banner = Instantiate(prefab, container);
            banner.SetActive(revealed);
            banner.name = "Banner Controller - " + this.cityName;
            if (banner.TryGetComponent<UI.WorldMap.BannerController>(out UI.WorldMap.BannerController controller))
            {
                controller.SetNewTarget(transform.GetChild(1));
                controller.SetNewText(cityName);
                LinkUIBanner(banner);
                return banner;
            }
            Debug.LogError("no component");
            return null;
        }
    }
}
