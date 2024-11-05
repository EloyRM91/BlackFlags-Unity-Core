using UnityEngine;
using System.Collections.Generic;

//Mechanics
using GameMechanics.Ships;
using GameMechanics.Data;
using GameMechanics.WorldCities;

//Data
using System.IO;
using GameSettings.Core;

//Serialization:
using System;
using System.Runtime.Serialization.Formatters.Binary;
using GameMechanics.save;

namespace GameMechanics.Data
{
    public class PersistentGameData : Persistent
    {
        #region VARIABLES
        //Singleton pattern
        private static PersistentGameData instance;

        //
        //private static ShipInventory shipInventory;

        //----
        //PLAYER - CUSTOM 
        //----
        //Player Values
        public static string _GData_PlayerName;
        public static EntityType_KINGDOM _GDataPlayerNation;
        public static Sprite _GData_PlayerFlag;
        public static Sprite _GData_PlayerAvatar;

        //Player Ship
        public static string _GData_ShipName;
        public static Ship _GData_PlayerShip;


        //Game Main Values

        private static int _gold;
        public static int _GData_Gold
        {
            get { return _gold; }
            set { _gold = value; if (updateGold != null) updateGold(value); }
        }

        private static int _reputation;
        public static int _GData_Reputation
        {
            get { return _reputation; }
            set { _reputation = value; UIMap.ui.UpdateReputation(value); }
        }


        //DIFFICULTY AND MODIFIERS
        private static GameDifficulty _difficulty;
        public static GameDifficulty _GData_Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; SetDifficultyModifiers(value); }
        }

        private static float
        _BYDIFFICULTY_friendShipModifier,
        _BYDIFFICULTY_TradeModifier,
        _BYDIFFICULTY_MoraleModifier;

        public static float _GData_FriendshipModier { get { return _BYDIFFICULTY_friendShipModifier; } }
        public static float _GData_TradeModifier { get { return _BYDIFFICULTY_TradeModifier; } }
        public static float _GData_MoraleModifier { get { return _BYDIFFICULTY_MoraleModifier; } }

        private static bool
            _GAME_MoreEvents,
            _GAME_AgressiveKingdoms,
            _GAME_AlwaysAttack;

        public static bool _GDATA_MoreEvents { get { return _GAME_MoreEvents; } set { _GAME_MoreEvents = value; } }
        public static bool _GDATA_AgressiveKingdoms { get { return _GAME_AgressiveKingdoms; } set { _GAME_AgressiveKingdoms = value; } }
        public static bool _GDATA_AlwaysAttack { get { return _GAME_AlwaysAttack; } set { _GAME_AlwaysAttack = value; } }

        //PLAYER - IN-WORLD DATA
        public static KeyPoint playerCurrentPort;
        public static bool playerIsOnPort;

        //SCENE
        public static bool currentSceneIsTutorial;

        //EVENTS
        public delegate void UpdateIntValue(int val);
        public static event UpdateIntValue updateGold;
        #endregion

        protected override void Awake()
        {
            //Make persistent
            base.Awake();
            //Seat
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
            //Initialize a sample texture
            _GData_PlayerFlag = Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 4, 4), Vector2.zero, 1);
            //Initialize a sample player name
            _GData_PlayerName = "Guybrush Threepwood";
            //Initialize a sample ship name
            _GData_ShipName = "Sereníssima";
        }

#region EVENTS

        private void Start()
        {
            //Events
            PlayerMovement._EVENT_ArriveToPort += PlayerArrive2Port;
            PlayerMovement._EVENT_ExitFromPort += PlayerExitFromPort;
        }
        
        private void OnDestroy()
        {
            //Events
            PlayerMovement._EVENT_ArriveToPort -= PlayerArrive2Port;
            PlayerMovement._EVENT_ExitFromPort -= PlayerExitFromPort;

            if (File.Exists("TempCheckMod.txt") && PersistentGameSettings.currentMod != null)
                File.Delete("TempCheckMod.txt");
        }

        public static void CallUpdate()
        {
            updateGold(_gold);
        }
#endregion

#region DIFFICULTY SETTINGS

        private static void SetDifficultyModifiers(GameDifficulty level)
        {
            switch (level)
            {
                case GameDifficulty.easy:
                    _gold = 1100;
                    _BYDIFFICULTY_friendShipModifier = 0.05f;
                    _BYDIFFICULTY_TradeModifier = 0.08f;
                    _BYDIFFICULTY_MoraleModifier = 0.1f;
                    break;
                case GameDifficulty.normal:
                    _gold = 750;
                    _BYDIFFICULTY_friendShipModifier = 0f;
                    _BYDIFFICULTY_TradeModifier = 0.02f;
                    _BYDIFFICULTY_MoraleModifier = -0.01f;
                    break;
                case GameDifficulty.hard:
                    _gold = 450;
                    _BYDIFFICULTY_friendShipModifier = -0.02f;
                    _BYDIFFICULTY_TradeModifier = -0.02f;
                    _BYDIFFICULTY_MoraleModifier = -0.05f;
                    break;
                case GameDifficulty.nightmare:
                    _gold = 300;
                    _BYDIFFICULTY_friendShipModifier = -0.05f;
                    _BYDIFFICULTY_TradeModifier = -0.03f;
                    _BYDIFFICULTY_MoraleModifier = -0.06f;
                    break;
            }
        }
#endregion

#region WORLDPOINTS & NAVIGATION

        private void PlayerArrive2Port(KeyPoint k)
        {
            playerIsOnPort = true;
            playerCurrentPort = k;
        }
        private void PlayerExitFromPort()
        {
            playerIsOnPort = false;
            //playerCurrentPort = null;
        }
#endregion

#region CAMPAIGNS & TUTORIAL
        //public static void SetInventory(ShipInventory inventory) { shipInventory = inventory; }
        public static void SetAsTutorial(bool val)
        {
            currentSceneIsTutorial = val;
        }
        #endregion

#region SET DATA
        public static void getDataFromSavedFile(SavedFile savedFile)
        {
            _GData_PlayerName = savedFile.playerName;
            _GData_ShipName = savedFile.playerShipName;
            _GData_PlayerShip = GetShipDataFromSerializedInfo(savedFile.playerShipData);
            _GData_Gold = savedFile.playerGold;

            _GData_PlayerFlag = instance.GetSpriteFromBytes(savedFile.playerFlag, 600, 400);
            _GData_PlayerAvatar = instance.GetSpriteFromBytes(savedFile.playerAvatar, 300, 300);

            _GData_Difficulty = savedFile.settings_gameDifficulty;
            _GDATA_MoreEvents = savedFile.settings_MoreEvents;
            _GDATA_AgressiveKingdoms = savedFile.settings_AggresiveKingdoms;
            _GDATA_AlwaysAttack = savedFile.settings_AgressivePatrols;

            //Creamos una instancia contenedora de los datos que no se destruya al cargar la escena y permita seguir cargando datos.
            var fileContainer = instance.gameObject.AddComponent<PersistentSavedFileContainer>();
            fileContainer.savedFile = savedFile;
        }

        private Sprite GetSpriteFromBytes(byte[] bytes, ushort witdh, ushort height)
        {
            var flagTex = new Texture2D(witdh, height);
            flagTex.LoadRawTextureData(bytes);
            flagTex.Apply();
            return Sprite.Create(flagTex, new Rect(0, 0, witdh, height), new Vector2(0, 0), 1);
        }

        public static Ship GetShipDataFromSerializedInfo(SerializableShip shipData)
        {
            return shipData.GetShipFromSerializedData();
        }

        public static Ship GetShipDataFromSerializedInfo(SavedFile savedFile)
        {
            var shipData = savedFile.playerShipData;
            return GetShipDataFromSerializedInfo(shipData);
        }
        #endregion
    }
}

namespace GameMechanics.save
{
    #region SERIALIZATION STRUCTURES

    [Serializable]
    public abstract class SerializationConverter
    {
        public float[] ConvertV3(Vector3 vector)
        {
            return new float[3] { vector.x, vector.y, vector.z };
        }

        public float[] ConvertV2(Vector2 vector)
        {
            return new float[2] { vector.x, vector.y };
        }

        public Vector3 ToVector3(float[] values)
        {
            return new Vector3(values[0], values[1], values[2]);
        }

        public Vector2 ToVector2(float[] values)
        {
            return new Vector3(values[0], values[1]);
        }

        public float[] ConvertQuaternion(Quaternion rot)
        {
            return new float[] { rot.x, rot.y, rot.z, rot.w };
        }
    }

    [Serializable]
    public abstract class SerializableKeyPoint : SerializationConverter
    {
        public string cityName;
        public bool revealed;
    }

    [Serializable]
    public abstract class SerializableSettlement : SerializableKeyPoint
    {
        public SerializableResource[] exports;
    }

    [Serializable]
    public class SerializableCity : SerializableSettlement
    {
        public int population;
    }

    [Serializable]
    public class SerializableTown : SerializableSettlement
    {
        public int population;
    }

    [Serializable]
    public class SerializableSumgglersPost : SerializableSettlement
    {

    }

    [Serializable]
    public class SerializablePirateShelter : SerializableSettlement
    {

    }

    [Serializable]
    public class SerializableResource : SerializationConverter
    {
        public byte key;

        public SerializableResource(Resource r)
        {
            key = r.Key;
        }
    }

    [Serializable]
    public class SerializableInventoryStacking : SerializationConverter
    {
        public SerializableResource resource;
        public int amount;

        public SerializableInventoryStacking(Resource r, int n)
        {
            resource = new SerializableResource(r);
            amount = n;
        }

        public SerializableInventoryStacking(SerializableResource r, int n)
        {
            resource = r;
            amount = n;
        }

        public SerializableInventoryStacking(InventoryItemStacking inv)
        {
            resource = new SerializableResource(inv.resource);
            amount = inv.amount;
        }
    }

    [Serializable]
    public class SerializableCharacter : SerializationConverter
    {
        public string characterName;
        public float friendshipLevel;
        public bool hasMetPlayer;
    }

    [Serializable]
    public class SerializableSmuggler : SerializableCharacter
    {
        public SerializableResource[] smugglerOffer;
        public SerializableInventoryStacking[] smugglerInventory;
        private int[] smugglerGenerationRatio;

        public SerializableSmuggler(SerializableResource[] smugglerOffer, SerializableInventoryStacking[] smugglerInventory, int[] smugglerGenerationRatio)
        {
            this.smugglerOffer = smugglerOffer;
            this.smugglerInventory = smugglerInventory;
            this.smugglerGenerationRatio = smugglerGenerationRatio;
        }

        public SerializableSmuggler(Smuggler character)
        {
            characterName = character.GetCharacterName();

            var inventory = character.SmugglerInventory;
            smugglerInventory = new SerializableInventoryStacking[inventory.Count];

            for (int i = 0; i < inventory.Count; i++)
            {
                smugglerInventory[i] = new SerializableInventoryStacking(inventory[i]);
            }

            smugglerGenerationRatio = character.SmugglerGenerationRatio;
        }
    }

    [Serializable]
    public class SerializableShip : SerializationConverter
    {
        // Clave numérica de tipo de barco:
        // 0 - Coastal Sloop
        // 1 - Continental Sloop
        // 2 - Military Balander
        // 3 - Two Mastles Felucca
        // 4 - Mistic Felucca
        // 5 - One-Mastled Tartain
        // 6 - Two-MastledTartain
        // 7 - 12 cannons brig
        // 8 - 16 cannons brig
        // 9 - 10 cannons Lugger
        // 10 - 14 cannons Lugger
        // 11 - Shooner Polacre
        // 12 - Polacre
        // 13 - Corvette
        // 14 - 26 Cannons-Frigate
        // 15 - Military Frigate
        // 16 - Little Gallion / Galeoncete
        // 17 - Gallion
        // 18 - Dutch Gallion
        // 19 - Flyboat
        // 20 - Urca

        // Clave numérica de Mejoras en el barco:
        // 0 - Boardingnets - Redes de Abordaje
        // 1 - Copperhull - Planchas de Cobre
        // 2 - Overblindsail - Sobrecebadera
        // 3 - Boatswainlocker - Pañol de Contramaestre
        // 4 - Staysail - Estay de Mesana
        // 5 - Flyingjib - Foque volante
        // 6 - Foremaststay - Estay de Trinquete
        // 7 - Lateenyard - Segunda Entena

        //Clave numérica de Variantes de Barcos:
        // 100 - Roundbrig - Bergantín Redondo
        // 101 - Snowbrig - Bergantín de Esnón
        // 102 - Xebec - Jabeque-Polacra
        // 103 - Barque - Bribarca

        public string shipName;
        public string captainName;
        public int shipType;
        public string variant;
        public string[] shipImprovements;

        public SerializableShip(Ship ship)
        {
            shipName = ship.name_Ship;
            captainName = ship.name_Captain;
            shipType = ship.GetSubClassID();
            variant = ship.GetVariantKey();
            shipImprovements = ship.GetCurrentImprovementsKeys();
        }

        public Ship GetShipFromSerializedData() {

            Ship newShip;
            switch(shipType)
            {
                case 0:
                    newShip = new ShipSubCategory_CoastalSloop();
                    break;
                case 1:
                    newShip = new ShipSubCategory_ContinentalSloop();
                    break;
                case 2:
                    newShip = new ShipSubCategory_MilitaryBalander();
                    break;
                case 3:
                    newShip = new ShipSubCategory_TwoMastlesFelucca();
                    break;
                case 4:
                    newShip = new ShipSubCategory_MisticFelucca();
                    break;
                case 5:
                    newShip = new ShipSubCategory_TwoMastlesTartain();
                    break;
                case 6:
                    newShip = new ShipSubCategory_ThreeMastlesTartain();
                    break;
                case 7:
                    newShip = new ShipSubCategory_12Brig();
                    newShip.SetVariant(variant);
                    break;
                case 8:
                    newShip = new ShipSubCategory_16Brig();
                    newShip.SetVariant(variant);
                    break;
                case 9:
                    newShip = new ShipSubCategory_10Lugger();
                    break;
                case 10:
                    newShip = new ShipSubCategory_14Lugger();
                    break;
                case 11:
                    newShip = new ShipSubCategory_ShoonerPolacre();
                    newShip.SetVariant(variant);
                    break;
                case 12:
                    newShip = new ShipSubCategory_Polacre();
                    newShip.SetVariant(variant);
                    break;
                case 13:
                    newShip = new ShipSubCategory_Corvette();
                    break;
                case 14:
                    newShip = new ShipSubCategory_26Frigate();
                    break;
                case 15:
                    newShip = new ShipSubCategory_MilitaryFrigate();
                    break;
                case 16:
                    newShip = new ShipSubCategory_LittleGallion();
                    break;
                case 17:
                    newShip = new ShipSubCategory_Gallion();
                    break;
                case 18:
                    newShip = new ShipSubCategory_DutchGallion();
                    break;
                case 19:
                    newShip = new ShipSubCategory_Flyboat();
                    break;
                case 21:
                    newShip = new ShipSubCategory_Urca();
                    newShip.SetVariant(variant); //En el futuro se introducirá el paquebote como variante de la urca
                    break;
                default:
                    newShip = new ShipSubCategory_CoastalSloop();
                    break;
            }
            newShip.name_Ship = shipName;
            newShip.name_Ship = captainName;

            newShip.SetImprovementsFromKeys(shipImprovements);
            return newShip;
        }
    }

    #endregion

    #region SAVE GAME

    [Serializable]
    public class SavedFile : SerializationConverter
    {
        //Basic Game Data & Game Settings

        public DateTime WorldDate; // fecha de la partida
        //public DateTime startingDate; //fecha inicial de la partida;
        public uint playedTime; //tiempo de juego en segundos;
        public float deltaWorldDate; // el contador para llegar al próximo día
        public byte marketTimer; // contador para actualizar mercados.

        public GameDifficulty settings_gameDifficulty;
        public bool settings_MoreEvents;
        public bool settings_AggresiveKingdoms;
        public bool settings_AgressivePatrols;
        public ushort IDCounter; //contador de entidades que hemos creado (indica el siguiente valor de clave primaria cuando instanciamos algo)
        public ushort IDKeyPointCounter; //Contador de ubicaciones en el mundo creadas (ciudades, puntos de misión, etc);

        //Player's basic Data

        public string 
            playerName,
            playerShipName;
        public byte[] 
            playerFlag,
            playerAvatar;

        public SerializableShip playerShipData;
        public int playerGold;
        public float playerReputation;
        public float[] playerPosition;
        public float[] playerRotation;
        public bool
            playerIsInPort,
            playerCanMove;
        public int 
            portID, //id del puerto destino del jugador
            targetID; //id del barco o convoy siendo pereguido por el jugador
        public float[] playerDestination; //posición del destino

        //Player's Inventory & Crew
        public byte crew;
        public byte[] moraleModifiers;
        public bool
            rationingRum,
            rationingMeat,
            rationingRum_TimerLock,
            rationingMeat_TimerLock;
        public DateTime
            unlockDate_RumRationing,
            unlockDate_MeatRationing;
        public float
            moraleSupplies,
            moralePillage,
            moraleResting,
            moraleGlobal;

        public byte[]
            onLoadGuns, //armamento en bodega (no equipado)
            onEquipmentGuns, //armamaneto montado 
            onUseWeaponsSlots; //huecos de armamento usados;

        public int[] inventoryItems;
        public float[] surplus;
        public int disentryCounter;

        //Cities and KeyPoints

        //Ships in Game


        public SavedFile()
        {
            var player = GameObject.FindWithTag("Player").transform;
            var playerMovement = player.GetComponent<PlayerMovement>();

            //Basic Game Data
            WorldDate = TimeManager.WorldDate;
            playedTime = TimeManager.instance.PlayedTime;
            deltaWorldDate = TimeManager.instance.Timer;
            marketTimer = TimeManager.instance.Counter20;

            settings_gameDifficulty = PersistentGameData._GData_Difficulty;
            settings_MoreEvents = PersistentGameData._GDATA_MoreEvents;
            settings_AggresiveKingdoms = PersistentGameData._GDATA_AgressiveKingdoms;
            settings_AgressivePatrols = PersistentGameData._GDATA_AlwaysAttack;
            IDCounter = Convoy.IDConvoyCounter;
            IDKeyPointCounter = KeyPoint.IDKeyPointCounter;

            playerName = PersistentGameData._GData_PlayerName;
            playerShipName = PersistentGameData._GData_ShipName;
            //playerFlag = PersistentGameData._GData_PlayerFlag.texture.EncodeToPNG();
            //playerAvatar = PersistentGameData._GData_PlayerAvatar.texture.EncodeToPNG();

            //Testing:
            //File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", playerFlag);

            //playerFlag = PersistentGameData._GData_PlayerFlag.texture.GetRawTextureData();
            //playerAvatar = PersistentGameData._GData_PlayerAvatar.texture.GetRawTextureData();

            playerShipData = new SerializableShip(PlayerMovement.playership);
            playerGold = PersistentGameData._GData_Gold;
            playerReputation = PersistentGameData._GData_Reputation;

            playerPosition = ConvertV3(player.position);
            playerRotation = ConvertQuaternion(player.rotation);
            playerIsInPort = PlayerMovement.IsInPort();
            playerCanMove = PlayerMovement.canMove;
            portID = playerMovement.GetCurrentPort() ? playerMovement.GetCurrentPort().KeyPointID : -1;
            targetID = playerMovement.ConvoyTarget ? playerMovement.ConvoyTarget.ID : -1;
            playerDestination = ConvertV3(playerMovement.GetDestination());

            crew = (byte)ShipInventory.Crew;
            moraleModifiers = MoraleModifier.ActiveModifiers;
            disentryCounter = ShipInventory.disentryCounter;
            rationingRum = ShipInventory.instance.rationingRum;
            rationingMeat = ShipInventory.instance.rationingMeat;

            if(rationingRum || rationingMeat)
            {
                //var toggles = TimeManager.instance.RationingManagers;
                var toggles = GameObject.FindObjectsOfType<UI.WorldMap.ToggleRationing>(true);

                if (rationingRum )
                {
                    for (int i = 0; i < toggles.Length; i++)
                    {
                        var ratToggle = toggles[i];
                        if (ratToggle.Index == 10)
                        {
                            unlockDate_RumRationing = ratToggle.UnlockerDate;
                            rationingRum_TimerLock = ratToggle.LockedByTiming;
                            break;
                        }
                    }
                }
                if (rationingMeat)
                {
                    for (int i = 0; i < toggles.Length; i++)
                    {
                        var ratToggle = toggles[i];
                        if (ratToggle.Index == 11)
                        {
                            unlockDate_MeatRationing = ratToggle.UnlockerDate;
                            rationingMeat_TimerLock = ratToggle.LockedByTiming;
                            break;
                        }
                    }
                }
            }

            moraleSupplies = ShipInventory.Morale_Supplies;
            moralePillage = ShipInventory.Morale_Pillage;
            moraleResting = ShipInventory.Morale_Resting;
            moraleGlobal = ShipInventory.Morale_Global;

            onLoadGuns = ShipInventory.instance.onLoad;
            onEquipmentGuns = ShipInventory.instance.equipment;
            onUseWeaponsSlots = ShipInventory.instance.onUseWeaponsSlots;

            inventoryItems = ShipInventory.Items;
            surplus = ShipInventory.Surplus;

        }
    }

    [Serializable]
    public class SerializationUtilities : SerializationConverter
    {
        protected string getFileExtension()
        {
            var currentMod = PersistentGameSettings.currentMod;

            if (currentMod == null)
            {
                return ".pirate";
            }
            else if (currentMod.gameLogic != null)
            {
                //extersión de partidas guardadas del mod:
                var ext = currentMod.gameLogic.modFileExt;
                if (ext != string.Empty)
                {
                    return ext;
                }
                else
                {
                    return ".mod";
                }
            }
            else
            {
                return ".mod";
            }
        }

        protected string getRoute()
        {
            var currentMod = PersistentGameSettings.currentMod;
            if (currentMod == null)
            {
                //Ruta por del juego vanilla:
                return Directory.GetCurrentDirectory() + "/Saves/";
            }
            else
            {
                //Ruta del directorio del mod
                return currentMod.ModPath + "Data/Saves/";
            }
        }
    }

    [Serializable]
    public class SavedGameBinaryFormat : SerializationUtilities
    {
        public SavedFile savedFile;

        public SavedGameBinaryFormat(SavedFile savedFile)
        {
            this.savedFile = savedFile;
        }

        public void SaveGame(string fileName, bool overWrite = false)
        {
            var path = getRoute() + fileName + getFileExtension();
            Debug.Log("saving: " + path);

            if (File.Exists(path) && !overWrite)
            {
                //Vamos a sobreescribir, lanzamos aviso al jugador

                //Ahora mismo lo que voy a hacer es sobreescribir:
                SaveGame(fileName, true);
            }
            else
            {
                var binaryFormatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Create);

                //Serialización:
                binaryFormatter.Serialize(stream, this.savedFile);
                stream.Close();
            }

        }
    }

#endregion

#region LOAD
    [Serializable]
    public class LoaderBinaryFormat : SerializationUtilities
    {
        public SavedFile LoadGame(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();
            Debug.Log(path);
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open);
                SavedFile result = binaryFormatter.Deserialize(stream) as SavedFile;
                stream.Close();
                return result;
            }
            return null;
        }
    }

    #endregion

}

