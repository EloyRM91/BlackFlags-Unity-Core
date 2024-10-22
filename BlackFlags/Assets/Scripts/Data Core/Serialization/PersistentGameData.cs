using UnityEngine;
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
            _GData_ShipName = "Seren�ssima";
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
            return new Vector3(values[0], values[1], values [2]);
        }

        public Vector2 ToVector2(float[] values)
        {
            return new Vector3(values[0], values[1]);
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

#endregion

#region SAVE GAME

    [Serializable]
    public class savedFile : SerializationConverter
    {
        public string playerName;
        public string playerShipName;
        public int playerGold;

        public savedFile()
        {
            playerName = PersistentGameData._GData_PlayerName;
            playerShipName = PersistentGameData._GData_ShipName;
            playerGold = PersistentGameData._GData_Gold;
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
        public savedFile savedFile;

        public SavedGameBinaryFormat(savedFile savedFile)
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

    [Serializable]
    public class LoaderBinaryFormat : SerializationUtilities
    {
        public savedFile LoadGame(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();
            Debug.Log(path);
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open);
                Debug.Log("entra aquí");
                savedFile result = binaryFormatter.Deserialize(stream) as savedFile;
                stream.Close();
                return result;
            }
            return null;
        }
    }

    #endregion

}

