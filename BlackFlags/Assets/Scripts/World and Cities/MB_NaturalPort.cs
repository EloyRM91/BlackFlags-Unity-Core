using System.Collections;
using System.Collections.Generic;
using GameMechanics.save;
using UnityEngine;

namespace GameMechanics.WorldCities
{
    public class MB_NaturalPort : KeyPoint
    {
        public byte calado;
        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();
        }

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

        public string Calado()
        {
            string c = string.Empty;
            switch (calado)
            {
                case 0: return "embarcaciones menores";
                case 1: return "escoltas ligeros";
                case 2: return "naves de dos puentes";
                case 3: return "cargueros";
                case 4: return "buques de guerra";
            }

            return c;
        }

        protected override void DisplayKeypointPanel()
        {
            UIMap.ui.DisplayInfo(this);
        }

        protected override void DisplayInfo()
        {
            UIMap.ui.DisplayInfo(this);
        }

        public void SetFromSerializedData(SerializableNaturalPort portData)
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

            this.calado = portData.calado;
        }

        public GameObject GetKeyPointBanner(Transform container)
        {
            string fileName = "Natural port Banner OnScreen - " + (this.cityName.Length > 13 ? "Large" : "Small");
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
