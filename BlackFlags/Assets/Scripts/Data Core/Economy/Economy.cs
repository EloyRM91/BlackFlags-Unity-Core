using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameMechanics.Data
{
    //Load Ship State
    public enum FreeboardLoad { ligero, fletado, sobrecargado }
    //Resources data
    public abstract class EconomyBehaviour : MonoBehaviour
    {
        //Resources
        protected static readonly Dictionary<int, Resource> D_WorldResources = new Dictionary<int, Resource>()
        {
            //BIENES FUNGIBLES
            {0, new Resource("Cereales", 0.06f, 12, ResourceType.consumible, 0) },
            {1, new Resource("Fruta", 0.07f, 25, ResourceType.consumible, 1) },
            {2, new Resource("Carne", 0.1f, 90, ResourceType.consumible, 2) },
            {3, new Resource("Cerveza", 0.14f, 25, ResourceType.consumible, 3) },
            {4, new Resource("Ron", 0.12f, 100, ResourceType.consumible, 4) },
            {5, new Resource("Suministros navales", 0.2f, 24, ResourceType.consumible, 5)},

            //BIENES COMERCIALES
            {6, new Resource("Cacao", 0.1f, 180, ResourceType.tradingGoods, 6) },
            {7, new Resource("Café", 0.1f, 160,ResourceType.tradingGoods, 7) },
            {8, new Resource("Azúcar", 0.1f, 180, ResourceType.tradingGoods, 8)},
            {9, new Resource ("Algodón", 0.07f, 170, ResourceType.tradingGoods, 9) },
            {10, new Resource("Tabaco",0.07f, 220, ResourceType.tradingGoods, 10) },
            {11, new Resource("Tinte", 0.1f, 230, ResourceType.tradingGoods, 11) },
            {12, new Resource("Sal", 0.12f, 190, ResourceType.tradingGoods, 12) },
            {13, new Resource("Especias", 0.4f, 420, ResourceType.tradingGoods, 13) },

            //BIENES DE LUJO
            {14, new Resource("Perlas",0.12f, 2300, ResourceType.luxury, 14) },
            {15, new Resource("Plata", 0.4f, 6400, ResourceType.luxury, 15) },

            //ARMAMENTO
            {16, new Resource("Armas de mano", 0.03f, 160, ResourceType.weapons, 16) },
            {17, new Resource("Falconete", 0.1f, 320, ResourceType.weapons, 17) },
            {18, new Resource("Cañón de 8 libras", 0.3f, 900, ResourceType.weapons, 18) },
            {19, new Resource("Cañón de 12 libras", 0.6f, 1300, ResourceType.weapons, 19) },
            {20, new Resource("Cañón de 24 libras", 0.9f, 2500, ResourceType.weapons, 20) },
        };

        //Armory
        protected static readonly Dictionary<byte, string> D_Armory = new Dictionary<byte, string>()
        {
            //NOMBRES DE ARMAS
            {0, "armas de mano" },
            {1, "cañones de 8 libras" },
            {2, "cañones de 12 libras" },
            {3, "cañones de 24 libras" },
        };

        //Initial resources
        protected static readonly Dictionary<LevelOfDifficulty, int[]> D_InitialResources = new Dictionary<LevelOfDifficulty, int[]>()
        {
            {LevelOfDifficulty.easy, new int[21] {8,6,0,5,2,10,0,0,0,0,0,0,0,0,0,0,7,2,4,0,0} },
            {LevelOfDifficulty.normal, new int[21] {6,2,0,6,0,8,0,0,0,0,0,0,0,0,0,0,6,2,2,0,0} },
            {LevelOfDifficulty.hard, new int[21] {5,0,0,5,0,7,0,0,0,0,0,0,0,0,0,0,5,4,2,0,0} }
        };
        public static Resource GetResource(byte index)
        {
            return D_WorldResources[index];
        }
        public static Sprite GetResourceSprite(Resource resource)
        {
            //var key = GetResourceKey(resource);
            var key = resource.Key;
            return UIMap.GetResourceSprite(key);
        }
        //public static int GetResourceKey(Resource resource)
        //{
        //    List<int> keyList = new List<int>(D_WorldResources.Keys);
        //    return keyList.Find(x => D_WorldResources[x] == resource);
        //}
        public static string GetWeaponName(byte index)
        {
            return D_Armory[index];
        }
        public static Resource RandomResource(ResourceType t)
        {
            switch (t)
            {
                case ResourceType.consumible: return D_WorldResources[Random.Range(0, 6)];
                case ResourceType.tradingGoods: return D_WorldResources[Random.Range(6,14)];
                case ResourceType.luxury: return D_WorldResources[Random.Range(14, 16)];
                case ResourceType.weapons: return D_WorldResources[Random.Range(16, 21)];
                default: return null;
            }
        }
        public static Resource RandomWeapon() { return D_WorldResources[Random.Range(16, 21)]; }
    }

    [System.Serializable]
    public class Resource
    {
        public string resourceName;
        public float weight;
        public int value;
        private byte key; 
        public byte Key
        {
            get { return key; }
        }
        private ResourceType resourceType;

        public ResourceType ResourceType
        {
            get { return resourceType; }
        }

        public void SetPrice_Absolute(int newPrice) { value = newPrice; }

        public void SetPrice_Relative(float percent) { value = (int)(value * percent); }

        public Resource(string n, float w, int v, ResourceType t, byte k)
        {
            resourceName = n;
            weight = w;
            value = v;
            key = k;
            resourceType = t;
        }
    }
    [System.Serializable]
    public class InventoryItemStacking
    {
        public Resource resource;
        public int amount;
        public InventoryItemStacking(Resource r, int n) { resource = r; amount = n; }
    }
    public enum ResourceType { consumible = 0, tradingGoods = 1 , luxury = 2, weapons = 3, Any}

    public enum LevelOfDifficulty { easy = 0, normal = 1, hard = 2}
}

