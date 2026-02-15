using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//Generation
using Generation.Ships;
using Generation.Generators;
using GameMechanics.AI;

//World
using GameMechanics.WorldCities;
using GameMechanics.Ships;
using GameMechanics.save;

//Mods ans settings
using GameSettings.Core;

namespace GameMechanics.Data
{
    /// <summary>
    /// A kingdom class is an object that manage aspects as ship generations or cities info.
    /// </summary>
    public class Kingdom : MonoBehaviour
    {
        #region VARIABLES
        //public static  ushort kingdomsCount = 5;
        //this kingdom parameters
        public ushort tagKey; // El indentificador de este país
        public Material nationalFlag;
        //[SerializeField] public EntityType_KINGDOM thisKingdom;
        public float[] roleFleetsSpawnStatistics; // ????
        [SerializeField] private byte _CountryBaseStrength; // spawn capacity
        public byte CountryBaseStrength { get { return _CountryBaseStrength; } }
        [SerializeField] private List<Settlement> countryPossessions;

        //Sprites & materials
        [SerializeField] private Sprite _spriteSimple, _spriteDetailed;
        public Sprite spriteSimple
        {
            get { return _spriteSimple; }
            set { _spriteSimple = value; }
        }

        public Sprite spriteDetailed
        {
            get { return _spriteDetailed; }
            set { _spriteDetailed = value; }
        }

        public Transform merchantsPooling
        {
            get { return transform.GetChild(2); }
        }

        public Transform atlanticPooling
        {
            get { return transform.GetChild(3); }
        }

        public Transform patrolsPooling
        {
            get { return transform.GetChild(4); }
        }

        //Spawning
        public ShipGenerator shipsGenerator;
        [SerializeField] private Transform _poolContainer_LOC, _poolContainer_EU_M, _poolContainer_PAT;
        //[SerializeField] private GameObject _convoyPrefab;

        //Relations
        public List<Kingdom> atWarWith = new List<Kingdom>();
        public List<Kingdom> atTradeAgreementWith = new List<Kingdom>();

        //Text
        public string KINGDOMNAME, GENTILISM_MALESIN, GENTILISM_MALEPLU, GENTILISM_FEMSIN, GENTILISM_FEMPLU;
        #endregion

        void Awake()
        {
            //Generator of this country, with its own ratio spawns according to database values.
            //TODO: En el futuro esto se tendría que hacer en la pantalla de carga, al menos la parte de la lectura de la bbdd
            shipsGenerator = WorldGenerator.GetShipSpawnData(tagKey, PersistentGameSettings.currentMod != null);
        }
        private void Start()
        {
            if (!PersistentGameSettings.loadingFile)
            {
                //pooling and generation
                CreateArmadaV1();
            }

        }

        //Cities and Towns
        public List<Settlement> GetPortsList()
        {
            return countryPossessions;
        }

        //War and Peace (Not the novel)
        public bool IsAtWar()
        {
            return atWarWith.Count != 0;
        }
        public bool IsAtWarWith(Kingdom k)
        {
            return atWarWith.Contains(k);
        }
        //public bool IsAtWarWith(EntityType_KINGDOM k)
        public bool IsAtWarWith(ushort kingdom)
        {
            return atWarWith.Find(s => s.tagKey == kingdom);
        }

        //Generation
        public void CreateArmadaV1()
        {
            var cityList = countryPossessions.Where(s => s is MB_City).ToList();
            //Local Merchants
            foreach (Settlement port in cityList)
            {
                for (int i = 0; i < _CountryBaseStrength; i++)
                {
                    GameObject newConvoy = GetConvoy(_poolContainer_LOC);
                    newConvoy.transform.position = port.transform.GetChild(0).position;
                    var convoyComponent = newConvoy.GetComponent<ConvoyNPC>();
                    convoyComponent.currentPort = port;
                    var ai = newConvoy.GetComponent<AI_LocalMerchant>();
                    if (ai == null)
                    {
                        ai = newConvoy.AddComponent<AI_LocalMerchant>();
                    }

                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.LocalMerchant, tagKey);
                    ship.name_Ship = WorldGenerator.GiveShipName(tagKey, ShipType_ROLE.LocalMerchant, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(tagKey);
                    convoyComponent.thisConvoyShips = new Ship[1] { ship };
                    convoyComponent.SetConvoyData(this);

                    ai.Awake();
                    ai.SetStateAs_AtPort();
                }
            }
            //Patrols
            if (cityList.Count != 0)
            {
                var patrols = Mathf.Clamp(cityList.Count / 4, 1, 8);
                for (int i = 0; i < patrols; i++)
                {
                    Settlement port = cityList[Random.Range(0, cityList.Count)];
                    GameObject newPatrol = GetConvoy(_poolContainer_PAT);
                    newPatrol.transform.position = port.transform.GetChild(0).position;

                    var data = newPatrol.GetComponent<ConvoyNPC>();
                    data.currentPort = port;
                    if (newPatrol.GetComponent<AI_Patrol>() == null)
                    {
                        newPatrol.AddComponent<AI_Patrol>();
                    }

                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.Patrol, tagKey);
                    ship.name_Ship = WorldGenerator.GiveShipName(tagKey, ShipType_ROLE.Patrol, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(tagKey);
                    data.thisConvoyShips = new Ship[1] { ship };
                    data.SetConvoyData(this);
                }
            }

        }
        public void CallEuropeanConvoy(Settlement[] route, Vector3 spawnOrigin)
        {
            GameObject newConvoy = GetConvoy(_poolContainer_EU_M);
            newConvoy.transform.position = spawnOrigin;
            var data = newConvoy.GetComponent<ConvoyNPC>();
            data.currentPort = route[0];
            if (newConvoy.GetComponent<AI_Merchant>() == null)
            {
                newConvoy.AddComponent<AI_Merchant>();
                data.thisConvoyShips = new Ship[Random.Range(2, 5)];
                for (int i = 0; i < data.thisConvoyShips.Length; i++)
                {
                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.Merchant, tagKey);
                    ship.name_Ship = WorldGenerator.GiveShipName(tagKey, ShipType_ROLE.Merchant, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(tagKey);
                    data.thisConvoyShips[i] = ship;
                }
                data.SetConvoyData(this);
            }
            data.SetConvoyData(this);
            newConvoy.GetComponent<AI_Merchant>().SetRoute(route);
        }
        //Pooling
        private GameObject GetConvoy(Transform container)
        {
            for (int i = 0; i < container.childCount; i++)
            {
                GameObject c = container.GetChild(i).gameObject;
                if (!c.activeSelf)
                {
                    c.gameObject.SetActive(true);
                    return c;
                }
            }
            //GameObject newCon = Instantiate(_convoyPrefab, container);
            GameObject newCon = GameManager.gm.InstantiateMapConvoy(container);
            return newCon;
        }

        public void SetFromSerializedData(SerializableKingdom kingdomData)
        {
            gameObject.name = kingdomData.kingdomName;
            tagKey = kingdomData.tagKey;

            //todo: nationalFlag

            roleFleetsSpawnStatistics = kingdomData.roleFleetSpawnStats;
            _CountryBaseStrength = kingdomData.countryBaseStrength;
            countryPossessions = new List<Settlement>();

            //todo: a ver ahora cómo creo una lista de reinos si los reinos aún se están creando
            // atWarWith = ;
            // atTradeAgreementWith = ;


            KINGDOMNAME = kingdomData.kingdomName;
            GENTILISM_MALESIN = kingdomData.gentilism_MALESIN;
            GENTILISM_MALEPLU = kingdomData.gentilism_MALEPLU;
            GENTILISM_FEMSIN = kingdomData.gentilism_FEMSIN;
            GENTILISM_FEMPLU = kingdomData.gentilism_FEMPLU;
        }

        public void GetArmadaFromSerializedData(SerializableKingdom kingdomData, Transform containers)
        {
            //Mercantes 
            {
                SerializableConvoy[] merchants = kingdomData.countryMerchants;
                Debug.Log(merchants.Length);

                foreach (SerializableConvoy merchant in merchants)
                {

                    GameObject newConvoy = GetConvoy(containers);
                    Vector3 position = merchant.ToVector3(merchant.position);
                    newConvoy.transform.position = position;

                    Quaternion rotation = merchant.ToQuaternion(merchant.rotation);
                    newConvoy.transform.rotation = rotation;

                    var data = newConvoy.GetComponent<ConvoyNPC>();


                    SerializableShip[] ships = merchant.convoyShips;

                    foreach (SerializableShip ship in ships)
                    {
                        Ship s = ship.GetShipFromSerializedData();
                        Debug.Log(s.name_Ship);
                    }
                }
            }

        }

        public void GetArmadaFromSerializedData(
            SerializableConvoy[] merchants,
            SerializableConvoy[] patrols,
            SerializableConvoy[] euConvoys)
        {
            Transform container = null;
            //Mercantes 
            {
                container = merchantsPooling;
                foreach (SerializableConvoy merchant in merchants)
                {

                    GameObject newConvoy = GetConvoy(container);
                    Vector3 position = merchant.ToVector3(merchant.position);
                    newConvoy.transform.position = position;

                    Quaternion rotation = merchant.ToQuaternion(merchant.rotation);
                    newConvoy.transform.rotation = rotation;

                    var convoyComponent = newConvoy.GetComponent<ConvoyNPC>();

                    //todo: establecemos el puerto objetivo
                    //todo: obtenemos el id y con él la referencia al Keypoint
                    // convoyComponent.currentPort = 

                    var ai = newConvoy.GetComponent<AI_LocalMerchant>();
                    if (ai == null)
                    {
                        ai = newConvoy.AddComponent<AI_LocalMerchant>();
                    }

                    SerializableShip[] ships = merchant.convoyShips;
                    convoyComponent.thisConvoyShips = new Ship[ships.Length];
                    for (int i = 0; i < ships.Length; i++)
                    {
                        SerializableShip ship = ships[i];
                        Ship s = ship.GetShipFromSerializedData();
                        Debug.Log(s.name_Ship);
                        Debug.Log(s.name_Captain);
                        convoyComponent.thisConvoyShips[i] = s;
                        convoyComponent.SetConvoyData(this);
                    }

                    //?pruebas: establecer comportamiento ia en puerto
                    ai.Awake();
                    ai.SetStateAs_AtPort();
                }
            }

            //Patrullas
            {
                container = patrolsPooling;
            }
        }

        public void SetFromSerializedData(SerializableKingdom kingdomData, Settlement[] belongings)
        {
            SetFromSerializedData(kingdomData);
            countryPossessions = belongings.ToList();
        }

        /// <summary>
        /// Establece los datos de relaciones con otros reinos a partir de arrays de claves de identificación
        /// </summary>
        /// <param name="atWarWith"></param>
        /// <param name="atTradeAgrrementWith"></param>
        public void SetTreatmentsFromId(ushort[] atWarWith, ushort[] atTradeAgrrementWith)
        {
            for (int i = 0; i < atWarWith.Length; i++)
            {
                var targetId = atWarWith[i];
                Kingdom k = GameManager.gm.GetKingdombyTag(targetId);
                if (k)
                {
                    this.atWarWith.Add(k);
                }
            }
            for (int i = 0; i < atTradeAgrrementWith.Length; i++)
            {
                var targetId = atWarWith[i];
                Kingdom k = GameManager.gm.GetKingdombyTag(targetId);

                if (k)
                {
                    this.atTradeAgreementWith.Add(k);
                }
            }
        }

        // public void OnCountryTerritoryChanges(List<Settlement> belongings)
        // {
        //     this.countryPossessions = belongings;
        // }

        public void OnCountryTerritoryChanges()
        {
            countryPossessions = new List<Settlement>();

            var cities = transform.GetChild(0);
            for (int i = 0; i < cities.childCount; i++)
            {
                var c = cities.GetChild(i);
                countryPossessions.Add(c.GetComponent<MB_City>());
            }

            var villages = transform.GetChild(1);
            for (int i = 0; i < villages.childCount; i++)
            {
                var v = villages.GetChild(i);
                countryPossessions.Add(v.GetComponent<MB_Town>());
            }
        }
    }
}

