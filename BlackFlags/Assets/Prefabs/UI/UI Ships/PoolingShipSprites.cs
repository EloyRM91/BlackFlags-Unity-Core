using UnityEngine;
using UnityEngine.UI;
using GameMechanics.Data;

namespace UI.WorldMap
{
    public class PoolingShipSprites : MiniMapMaskingResponse
    {
        private static Transform container;
        private static GameObject prefab;

        void Awake()
        {
            container = transform;
            prefab = Resources.Load<GameObject>("Pooling/OnMapConvoy");
        }

        public static UIShipSprites GetSprite()
        {
            for (int i = 0; i < container.childCount; i++)
            {
                var banner = container.GetChild(i);
                if (!banner.gameObject.activeSelf)
                {
                    var sp = banner.GetComponent<UIShipSprites>();
                    if (!sp.HasTarget())
                    {
                        return banner.GetComponent<UIShipSprites>();
                    }    
                }
            }

            GameObject newSprite = Instantiate(prefab, container);
            newSprite.SetActive(false);
            var nsp = newSprite.GetComponent<UIShipSprites>();
            return nsp;
        }

        public static UIShipSprites GetPlayerSprite()
        {
            return container.GetChild(0).GetComponent<UIShipSprites>();
        }

    }
}


