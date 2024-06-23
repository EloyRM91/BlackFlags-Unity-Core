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
        //this kingdom parameters
        public Material nationalFlag;
        [SerializeField] public EntityType_KINGDOM thisKingdom;
        public float[] roleFleetsSpawnStatistics; // ????
        [SerializeField] private byte _CountryBaseStrength; // spawn capacity
        [SerializeField] private List<Settlement> countryPossessions;

        //Spawning
        public ShipGenerator shipsGenerator;
        [SerializeField] private Transform _poolContainer_LOC, _poolContainer_EU_M, _poolContainer_PAT;
        [SerializeField] private GameObject _convoyPrefab;
        
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
            shipsGenerator = WorldGenerator.GetShipSpawnData((int)thisKingdom, PersistentGameSettings.currentMod != null);
        }
        private void Start()
        {
            //pooling and generation
            CreateArmadaV1();
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
        public bool IsAtWarWith(EntityType_KINGDOM k)
        {
            return atWarWith.Find(s => s.thisKingdom == k);
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
                    var data = newConvoy.GetComponent<ConvoyNPC>();
                    data.currentPort = port;
                    if(newConvoy.GetComponent<AI_LocalMerchant>() == null)
                    {
                        newConvoy.AddComponent<AI_LocalMerchant>();
                    }

                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.LocalMerchant, thisKingdom);
                    ship.name_Ship = WorldGenerator.GiveShipName(thisKingdom, ShipType_ROLE.LocalMerchant, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(thisKingdom);
                    data.thisConvoyShips = new Ship[1] { ship };
                    data.SetConvoyData();
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

                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.Patrol, thisKingdom);
                    ship.name_Ship = WorldGenerator.GiveShipName(thisKingdom, ShipType_ROLE.Patrol, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(thisKingdom);
                    data.thisConvoyShips = new Ship[1] { ship };
                    data.SetConvoyData();
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
                    var ship = shipsGenerator.GenerateShipData(ShipType_ROLE.Merchant, thisKingdom);
                    ship.name_Ship = WorldGenerator.GiveShipName(thisKingdom, ShipType_ROLE.Merchant, GenerationMode.Random);
                    ship.name_Captain = WorldGenerator.GetCharacterName(thisKingdom);
                    data.thisConvoyShips[i] = ship;
                }
                data.SetConvoyData();
            }
            data.SetConvoyData();
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
            GameObject newCon = Instantiate(_convoyPrefab, container);
            return newCon;
        }
    }



}

