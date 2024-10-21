using UnityEngine;
//Mechanics
using GameMechanics.Ships;
using GameMechanics.Data;
using GameMechanics.WorldCities;

//Data
using System.IO;
using GameSettings.Core;

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

#region SERIALIZATION STRUCTURE - SAVE + LOAD


#endregion
    }
}

