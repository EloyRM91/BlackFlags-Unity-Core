using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Deserialización
using GameMechanics.save;

namespace GameMechanics.WorldCities
{
    public class MB_Town : Settlement
    {
        public int population;
        public byte imgIndex;

        //public override void OnMouseDown()
        //{
        //    base.OnMouseDown();
        //    if (!UIMap.panelON)
        //    {
        //        if (!UIMap.GetGraphicRaycastResult())
        //            UIMap.ui.DisplayInfo(this);
        //    }
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

        public void SetFromSerializedData(SerializableTown town)
        {
            this.cityName = town.cityName;
            gameObject.name = town.cityName;
            this.alternativeName = town.alternativeName;
            this.population = town.population;
            this.revealed = town.revealed;
            this.transform.position = SerializationConverter.ToVector3(town.position);

            if (town.flippedX)
            {
                var s = this.transform.localScale;
                this.transform.localScale = new Vector3(-s.x, s.y, s.z);
            }

            //Target Path:
            var entryPoint = this.transform.GetChild(0);
            entryPoint.position = SerializationConverter.ToVector3(town.entryPoint);

            //Banner's Pivot
            var pivot = this.transform.GetChild(1);
            pivot.position = SerializationConverter.ToVector3(town.pivotPoint);

            //todo: events point

            //todo: modificar el sprite en función del índice
            //todo: (podemos tener más de un tipo de sprite para este tipo de keypoint)

            var index = town.spriteIndex;

            this.exportsIndex = town.exports;
        }

        public GameObject GetKeyPointBanner(Transform container)
        {
            string fileName = "Town Banner OnScreen - " + (this.cityName.Length > 13 ? "Large" : "Small");
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

