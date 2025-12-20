using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//Mechanics
using GameMechanics.Data;
using GameMechanics.Ships;
//Lists filtering
using System.Linq;

//Deserialización
using GameMechanics.save;

namespace GameMechanics.WorldCities
{
    /// <summary>
    /// Cities are a kind of settlement class
    /// Parameters: Population(int), atlantic route's spawn point (Vector3)
    /// </summary>
    public class MB_City : Settlement
    {
        public int population;
        public string tavernName;
        private Vector3 spawnPoint;
        public Vector3 SpawnPoint { get { return spawnPoint; } }
        public List<Convoy> convoysInThisPort = new List<Convoy>();
        public byte imgIndex;

        //CHARACTERS IN CITY
        // [SerializeField] private List<Character> charactersInCity = new List<Character>();

        private void Awake()
        {
            SetExportsFromIndex();
        }

        private void Start()
        {
            //----
            // EUROPEAN CONVOYS
            //---- 

            //Set frequency: the more populated is the city, the more traffic from Europe it receives.
            var ConvoysFrequency = Mathf.Clamp(Mathf.Round(population / 3000), 1, 30);

            //Set entry point for european convoys
            spawnPoint = OceanicRoute.GetOceanicRouteIn(transform.position);

            //Adjust the spawn ratio
            ConvoysFrequency = 52560 / ConvoysFrequency;

            //Call european convoys
            InvokeRepeating("CallAtlanticConvoy", ConvoysFrequency / 2, ConvoysFrequency);

            //----
            // CHARACTERS
            //----

            //Número de contrabandistas en la ciudad
            var Smugglers = population > 6000 ? 2 : 1;
            //Número de armadores en la ciudad
            var SmugglingRatio = population > 6000 ? (population > 20000 ? 3 : 2) : 1;

            //Ratio de inventario de contrabandistas
            var ShipyardMen = population > 2500 ? (population > 20000 ? 2 : 1) : 0; // Número de armadores en la ciudad

            CreateCharacters(Smugglers, ShipyardMen, SmugglingRatio);
        }
        private void CreateCharacters(int nSmug, int nSMen, int ratio)
        {
            //! desde la v0.033 ya no se utilizan tags. En lugar de eso, vamos a usar
            //! un identificador (los mods podrían incluir otros reinos que no sean los establecidos)
            //var kingdom = GameManager.gm.GetKingdombyTag(transform.parent.tag);
            var kingdom = transform.parent.parent.GetComponent<Kingdom>();

            //Create smugglers
            for (int i = 0; i < nSmug; i++)
            {
                //Create offer
                var r = new List<Resource>();
                r.Add(exports[0]);
                if (exports.Count > 1)
                    for (int j = 1; j < exports.Count; j++)
                    {
                        if (UnityEngine.Random.Range(0, 2) == 1)
                            r.Add(exports[j]);
                    }
                var cosas = new int[7] { 0, 2, 3, 4, 5, 16, 17 };
                for (int k = 0; k < 7; k++)
                {
                    var ar = EconomyBehaviour.GetResource((byte)cosas[k]);
                    if (!r.Contains(ar))
                    {
                        if (UnityEngine.Random.Range(0, 2) == 0)
                            r.Add(ar);
                    }
                }
                //Construct smuggler
                //! desde la v0.033 ya no se utiliza el enumerador EntityType_KINGDOM
                //! (los mods podrán incluir otros reinos que no sean los establecidos)
                //var newSmuggler = new Smuggler(kingdom.thisKingdom, r);
                var newSmuggler = new Smuggler(kingdom.tagKey, r);
                newSmuggler.ratio = ratio;
                newSmuggler.SetInventory(true);
                charactersInShelter.Add(newSmuggler);
            }
            //Create shipyardmen
            for (int i = 0; i < nSMen; i++)
            {
                //Construct shipyardman
                var newShipyardMan = new ShipyardMan();
            }
        }
        private void CallAtlanticConvoy()
        {
            //var kingdom = GameManager.gm.GetKingdombyTag(transform.parent.tag);
            var kingdom = transform.transform.parent.GetComponent<Kingdom>();
            var sc = NewDestinationFromThisPort(kingdom, true, 50);
            var routeCities = sc != null ? new Settlement[2] { this, NewDestinationFromThisPort(kingdom, true, 50) } : new Settlement[1] { this };
            kingdom.CallEuropeanConvoy(routeCities, spawnPoint);
        }
        public override void OnMouseDown()
        {
            base.OnMouseDown();
            //if (UIMap.ui.gameObject.activeSelf)
            //    if (!UIMap.panelON && revealed)
            //    {
            //        if (!UIMap.GetGraphicRaycastResult())
            //        {
            //            if (PersistentGameData.playerIsOnPort && PersistentGameData.playerCurrentPort == this)
            //            {
            //                UIMap.ui.DisplayCityViewPanel(this);
            //            }
            //            else
            //            {
            //                UIMap.ui.DisplayInfo(this);
            //            }
            //        }
            //    }
        }

        protected override void DisplayKeypointPanel()
        {
            //Si el jugador hace click y está en puerto, se abre la vista de ciudad
            UIMap.ui.DisplayCityViewPanel(this);
        }

        protected override void DisplayInfo()
        {
            //Si el jugador hace click y no está en el puerto, muestra el panel genérico de información
            UIMap.ui.DisplayInfo(this);
        }

        public void SetFromSerializedData(SerializableCity cityData)
        {
            this.cityName = cityData.cityName;
            gameObject.name = cityData.cityName;
            this.alternativeName = cityData.alternativeName;
            this.population = cityData.population;
            this.tavernName = cityData.tavernName;
            this.revealed = cityData.revealed;
            this.transform.position = SerializationConverter.ToVector3(cityData.position);

            if (cityData.flippedX)
            {
                var s = this.transform.localScale;
                this.transform.localScale = new Vector3(-s.x, s.y, s.z);
            }

            //Target Path:
            var entryPoint = this.transform.GetChild(0);
            entryPoint.position = SerializationConverter.ToVector3(cityData.entryPoint);

            //Banner's Pivot
            var pivot = this.transform.GetChild(1);
            pivot.position = SerializationConverter.ToVector3(cityData.pivotPoint);

            //todo: events point

            //todo: modificar el sprite en función del índice
            //todo: (podemos tener más de un tipo de sprite para este tipo de keypoint)

            var index = cityData.spriteIndex;

            this.exportsIndex = cityData.exports;
        }

        public GameObject GetKeyPointBanner(Transform container)
        {
            string fileName = "City Banner OnScreen - " + (this.cityName.Length > 13 ? "Large" : "Small");
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
