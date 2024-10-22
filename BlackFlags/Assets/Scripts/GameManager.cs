using System.Collections.Generic;
using System.Linq;
using System.IO; //test
using UnityEngine;

//Mechanics
using GameMechanics.WorldCities;
using GameMechanics.Data;
using GameMechanics.Ships;

//Generation
using Generation.Generators;
using Generation.Ships;
using GameMechanics.AI;

//Save & Load
using GameMechanics.save;

//
using GameSettings.Core;

public class GameManager : MonoBehaviour
{
    //Instance
    public static GameManager gm;

    //Static Data
    [SerializeField] private Kingdom[] Kingdoms;
    [SerializeField] private CurrentsMapData currentsMapData; //Mapa de corrientes, que se obtiene en la escena de carga.

    public CurrentsMapData CurrentsMapData
    {
        get
        {
            if(currentsMapData == null)
            {
                createCurrentsMap();
            }

            return currentsMapData;
        }
    }

    //Routes
    //[SerializeField] private List<Transform> SpawnOceanPoints;
    [SerializeField] private List<KeyPoint> PirateShelters;

    //Generation
    [SerializeField] private Transform _pooling_Pirate;
    [SerializeField] private GameObject _prefab_Pirate;

    //Settings
    public byte pirateActivity;

    //City List
    public List<MB_City> Cities;

    //ForcedPause
    public static bool forcedPause;

    private void Awake()
    {
        //Singleton
        gm = this;
        //LoadDatabase
        WorldGenerator.Initialize();  //TODO: En el futuro esto se tendr�a que hacer en la pantalla de carga
        createCurrentsMap();

        //Set MatController parameters
        MatController.isShowingFlags = true; //En esta escena hay banderas con movimiento independiente al timescale

        //Reset MatController
        MatController.StopTiming();
    }
    private void Start()
    {
        CreateArmada_PIRATE();
        PersistentGameData.CallUpdate();
        PersistentGameData._GData_Reputation = 55;
        ShipInventory.Crew = 5;

        LoadGame("hola");
    }

    //Generation
#region GENERATION
    private void CreateArmada_PIRATE()
    {
        foreach (KeyPoint shelter in PirateShelters)
        {
            for (int i = 0; i < pirateActivity; i++)
            {
                //New pirate convoy
                GameObject newCPirate = GetPirateShip();
                newCPirate.transform.position = shelter.transform.GetChild(0).position;
                var data = newCPirate.GetComponent<ConvoyNPC>();
                data.currentPort = shelter;
                var AI_Pirate = newCPirate.GetComponent<AI_Pirate>();
                AI_Pirate ??= newCPirate.AddComponent<AI_Pirate>();

                //Set Convoy's Data
                var pirateOrigin = Random.Range(0, 2) == 1 ? EntityType_KINGDOM.KINGDOM_Britain : (EntityType_KINGDOM)Random.Range(0, 4);
                AI_Pirate.CreatePirateCharacter(pirateOrigin);

                var pirate = newCPirate.GetComponent<AI_Pirate>().pirateCharacter;
                var ship = Kingdoms[(byte)pirateOrigin].shipsGenerator.GenerateShipData(ShipType_ROLE.Pirate, pirateOrigin);

                ship.name_Ship = WorldGenerator.GiveShipName(pirateOrigin, ShipType_ROLE.Pirate, GenerationMode.Random);
                ship.name_Captain = pirate.CharacterName;
                pirate.shipName = ship.name_Ship;
                data.thisConvoyShips = new Ship[1] { ship };
                data.SetConvoyData();
            }
        }
    }

    //Pooling
    public GameObject GetPirateShip()
    {
        for (int i = 0; i < _pooling_Pirate.childCount; i++)
        {
            GameObject c = _pooling_Pirate.GetChild(i).gameObject;
            if (!c.activeSelf)
            {
                c.gameObject.SetActive(true);
                return c;
            }
        }
        GameObject newCon = Instantiate(_prefab_Pirate, _pooling_Pirate);
        return newCon;
    }
#endregion

    //Routes setters and Info
#region DATA ACCESS
    public Kingdom GetKingdombyTag(string tag)
    {
        switch (tag)
        {
            case "KingdomSpain": return Kingdoms[0];
            case "KingdomPortugal": return Kingdoms[1];
            case "KingdomFrance": return Kingdoms[2];
            case "KingdomDutch": return Kingdoms[3];
            case "KingdomBritain": return Kingdoms[4];
        }
        return null;
    }
    public KeyPoint GetShelter(KeyPoint currentShelter)
    {
        var ports = new List<KeyPoint>(PirateShelters);
        ports.Remove(currentShelter);
        return ports[Random.Range(0, ports.Count)];
    }
    public KeyPoint GetCity(KeyPoint currentCity)
    {
        var list = new List<KeyPoint>(Kingdoms[Random.Range(0, 5)].GetPortsList().Where(p => p is MB_City).ToList());
        list.Remove(currentCity);
        if (list.Count == 0) return GetCity(currentCity);
        return list[Random.Range(0, list.Count)];
    }
#endregion

    //Currents

    //Game's currents map
    public void createCurrentsMap()
    {
        string path = (
            PersistentGameSettings.currentMod == null ? 
            Application.streamingAssetsPath : PersistentGameSettings.currentMod.ModStreaming
        ) + "/";

#if UNITY_EDITOR
        path = "";
#endif
        path += "CurrentsMapData.json";

       //Testeo de ruta: 
// StreamWriter outputFile = new StreamWriter(Path.Combine(Application.streamingAssetsPath, "HOLAAAAAA.txt"));
// outputFile.WriteLine("path: " + path);
// outputFile.WriteLine("State:");
        // outputFile.Close();   
        // try //!BUILD:  este try no funciona en la build
        // {
            currentsMapData = Serialization.SerializationUtils.LoadCurrentsMap(path);
            // outputFile.WriteLine("success");
        // }
        // catch (System.Exception e)
        // {
        //     if(PersistentGameSettings.currentMod != null)
        //     {
        //         outputFile.WriteLine(e);
        //         Debug.LogError(e);

        //         try {
        //             //Si falla la ruta del mod, usa la ruta por defecto.
        //             path = Application.streamingAssetsPath + "/CurrentsMapData.json";
        //             currentsMapData = Serialization.SerializationUtils.LoadCurrentsMap(path);
        //             Debug.LogWarning("GameManagerError: Error al cargar el mapa de corrientes del mod. Se ha cargado el mapa estándar");
        //         }
        //         catch(System.Exception ee)
        //         {
        //             Debug.LogError("GameManagerError: Error al cargar el mapa de corrientes");
        //             Debug.LogError(ee);
        //         }
        //     }
        // }
        // outputFile.Close();    
    }

    public float getCurrentEffect(Transform target)
    {
        //var rotIndex = Mathf.Floor(targetRotation / 8);
        float x = target.transform.position.x - currentsMapData.boundingBox.minX;
        float z = target.transform.position.z - currentsMapData.boundingBox.minZ;

        int i = (int)Mathf.Floor(x / currentsMapData.tileSizeX);
        int j = (int)Mathf.Floor(z / currentsMapData.tileSizeZ);
        //print(i + "  " + j);

       
        //print(currentsMapData.arrows[j, i].direction);
        //var strength = currentsMapData.arrows[j, i].dragStrength;

        if(currentsMapData.arrows[j, i].direction == 0) 
        {
            // print("no hay corriente");
            return 0;
        }

        var currentAngle = 45 * (currentsMapData.arrows[j, i].direction - 1);
        //print(currentsMapData.arrows[j, i].direction + "  --  " + currentsMapData.arrows[j, i].dragStrength);
        //print(currentAngle);
        //print(target.transform.rotation.eulerAngles.y);

        //Diferencia de ángulo entre el barco y la corriente
        var dif = Mathf.Abs(Mathf.DeltaAngle(currentAngle, target.transform.rotation.eulerAngles.y));

        //Cálculo de la fuerza de corriente en base a la diferencia de �ngulo con el barco
        // print(dif);
        // print(Mathf.Clamp(1 - dif / 30, -1, 1) * currentsMapData.arrows[j, i].dragStrength);
        return Mathf.Clamp(1 - dif / 30, -1, 1) * currentsMapData.arrows[j, i].dragStrength;
    }

#region SAVE & LOAD

    public void SaveGame(string fileName)
    {
        var savedGameBinaryFormat = new SavedGameBinaryFormat(new savedFile());
        savedGameBinaryFormat.SaveGame(fileName);
    }

    public void LoadGame(string fileName)
    {
        var loaderBinaryFormat = new LoaderBinaryFormat();
        savedFile savedGameData = loaderBinaryFormat.LoadGame(fileName);

        if(savedGameData != null)
        {
            print(savedGameData.playerGold);
            print(savedGameData.playerName);
            print(savedGameData.playerShipName);
        }
        else
        {
            Debug.LogError("Dummy Error");
        }
    }
#endregion
}
