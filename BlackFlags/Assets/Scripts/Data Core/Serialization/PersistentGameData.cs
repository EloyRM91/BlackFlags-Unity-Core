using UnityEngine;
using System.Collections.Generic;

//Mechanics
using GameMechanics.Ships;
using GameMechanics.Data;
using GameMechanics.WorldCities;

//Data
using System.IO;
using GameSettings.Core;
using System.Linq;

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
        //public static EntityType_KINGDOM _GDataPlayerNation;
        public static ushort _GDataPlayerNation;
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

        //WORLD - NEW GAME
        //!Datos iniciales al crear una partida nueva
        private Dictionary<byte, SerializableNaturalPort> _D_startGameNatPorts = new Dictionary<byte, SerializableNaturalPort>()
        {
            {0, new SerializableNaturalPort(
                "Turbo",
                "Bahía Turbo",
                new float [2] { 19.542f, 128.45f},
                new float [2] { 20.04f, 128.45f },
                new float [2] { 19.542f, 127.19f },
                1,
                0,
                true
                )
            },
            {1, new SerializableNaturalPort(
                "Higuerote",
                "Carenero de Higuerote",
                new float [2] { -81.03f, 93.5f},
                new float [2] { -81.378f, 93.494f },
                new float [2] { -81.03f, 94.76f },
                3,
                0
                )
            },
            {2, new SerializableNaturalPort(
                "Isla Saona",
                "",
                new float [2] { -52.22f, 21.55f},
                new float [2] { -51.765f, 21.382f },
                new float [2] { -52.22f, 22.81f },
                3,
                0
                )
            },
            {3, new SerializableNaturalPort(
                "Isla de la Gonave",
                "Gonâve",
                new float [2] { -52.22f, 21.55f},
                new float [2] { -51.765f, 21.382f },
                new float [2] { -52.22f, 22.81f },
                3,
                0
                )
            },
            {4, new SerializableNaturalPort(
                "Cat Island",
                "",
                new float [2] { 16.92f, -30.86f},
                new float [2] { 17.238f, -30.56f },
                new float [2] { 16.92f, -32.12f },
                1,
                0,
                true
                )
            },
            {5, new SerializableNaturalPort(
                "Bahía Honda",
                "",
                new float [2] { 84.54f, -12.54f},
                new float [2] { 85.458f, -12.606f },
                new float [2] { 84.54f, -11.28f },
                3,
                0,
                true
                )
            },
            {6, new SerializableNaturalPort(
                "Bahía de Cárdenas",
                "Bahía de Jorge Cárdenas",
                new float [2] { 59.78f, -13.15f},
                new float [2] { 59.888f, -13.342f },
                new float [2] { 59.78f, -11.89f },
                3,
                0,
                true
                )
            },
            {7, new SerializableNaturalPort(
                "Caimanera",
                "La Caimanera",
                new float [2] { 10.16f, 11.38f},
                new float [2] { 10.73f, 12.13f },
                new float [2] { 10.16f, 10.12f },
                3,
                0,
                true
                )
            },
            {8, new SerializableNaturalPort(
                "Laguna Chiquirí",
                "",
                new float [2] { 61f, 122.22f},
                new float [2] { 60.772f, 122.892f },
                new float [2] { 61f, 123.48f },
                2,
                0,
                true
                )
            },
            {9, new SerializableNaturalPort(
                "Isla Cotorra",
                "Delta del Amacuro (Cotorra)",
                new float [2] { -117.46f, 91.2f},
                new float [2] { -117.46f, 91.878f },
                new float [2] { -117.46f, 89.94f },
                1,
                0,
                true
                )
            },
            {10, new SerializableNaturalPort(
                "Delta del Orinoco",
                "",
                new float [2] { -131.16f, 101.18f},
                new float [2] { -132.354f, 101.516f },
                new float [2] { -131.16f, 102.44f },
                1,
                0,
                false
                )
            },
            {11, new SerializableNaturalPort(
                "Nuevitas del Príncipe",
                "",
                new float [2] { 31.15f, -1.31f},
                new float [2] { 30.964f, -1.298f },
                new float [2] { 31.15f, -0.05f },
                2,
                0,
                false
                )
            }
        };

        private Dictionary<byte, SerializableSmugglersPost> _D_startGameSmPosts = new Dictionary<byte, SerializableSmugglersPost>()
        {
            {
                0, new SerializableSmugglersPost(
                    "Isla La Tortuga",
                    "La Tortuga",
                    new float [2] { -88.79f, 87.07f},
                    new float [2] { -88.808f, 87.778f },
                    new float [2] { -88.79f, 85.99f },
                    new int[] {12},
                    0,
                    false
                )
            },
            {
                1, new SerializableSmugglersPost(
                    "Araya",
                    "",
                    new float [2] { -99.23f, 88.54f},
                    new float [2] { -98.312f, 88.402f },
                    new float [2] { -99.23f, 87.46f },
                    new int[] {12},
                    0,
                    true
                )
            },
            {
                2, new SerializableSmugglersPost(
                    "Ponce",
                    "",
                    new float [2] { -72.05f, 19.89f},
                    new float [2] { -72.05f, 20.556f },
                    new float [2] { -72.05f, 18.81f },
                    new int[] {8},
                    0,
                    false
                )
            },
            {
                3, new SerializableSmugglersPost(
                    "Islas Turcas",
                    "Las Turcas",
                    new float [2] { -20.25f, -9.24f},
                    new float [2] { -20.568f, -8.43f },
                    new float [2] { -20.25f, -10.32f },
                    new int[] {12},
                    0,
                    false
                )
            },
            {
                4, new SerializableSmugglersPost(
                    "Baracoa",
                    "",
                    new float [2] { 4.93f, 6.69f},
                    new float [2] { 4.492f, 6.342f },
                    new float [2] { 4.93f, 7.77f },
                    new int[] {7},
                    0,
                    false
                )
            },
            {
                5, new SerializableSmugglersPost(
                    "Tucacas",
                    "Cayos de Tucacas",
                    new float [2] { -59.24f, 93.1f},
                    new float [2] { -60.23f, 93.754f },
                    new float [2] { -59.24f, 92.02f },
                    new int[] {6},
                    0,
                    false
                )
            },
            {
                6, new SerializableSmugglersPost(
                    "Río Unare",
                    "",
                    new float [2] { -90.25f, 95.57f},
                    new float [2] { -90.022f, 95.156f },
                    new float [2] { -90.25f, 96.65f },
                    new int[] {0},
                    0,
                    false
                )
            },
            {
                7, new SerializableSmugglersPost(
                    "Puerto Plata",
                    "Puerto de Plata",
                    new float [2] { -30.93f, 8.491f},
                    new float [2] { -30.906f, 8.185f },
                    new float [2] { -30.9f, 9.571f },
                    new int[] {0, 8},
                    0,
                    false
                )
            },
        };

        private Dictionary<byte, SerializablePirateShelter> _D_startGameShelters = new Dictionary<byte, SerializablePirateShelter>()
        {
            {
                0, new SerializablePirateShelter(
                    "Tortuga",
                    "",
                    new float [2] { -11.49f, 8.491f},
                    new float [2] { -11.53f, 8.89f },
                    new float [2] { -11.49f, 6.62f },
                    "La Dama de Amsterdam",
                    new EntryClass(RequirementEntry.LoyaltyToCodeLowerThan, 40),
                    0,
                    false
                )
            },
            {
                1, new SerializablePirateShelter(
                    "Hôpital",
                    "",
                    new float [2] { -17.53f, 21.58f},
                    new float [2] { -16.62f, 21.85f },
                    new float [2] { -17.53f, 20.28f },
                    "Le Bon Voleur",
                    new EntryClass(RequirementEntry.LoyaltyToCodeBiggerThan, 50),
                    0,
                    false
                )
            },
            {
                2, new SerializablePirateShelter(
                    "George Town",
                    "Isla George Town",
                    new float [2] { 20.7f, -23.07f},
                    new float [2] { 20.7f, -22.195f },
                    new float [2] { 20.7f, -24.37f },
                    "Roberts' Blood",
                    new EntryClass(RequirementEntry.LoyaltyToCodeBiggerThan, 10),
                    0,
                    false
                )
            },
            {
                3, new SerializablePirateShelter(
                    "San Salvador",
                    "Isla de San Salvador",
                    new float [2] { 7.99f, -28.8f},
                    new float [2] { 8.555f, -28.575f },
                    new float [2] { 7.99f, -27.5f },
                    "Taberna San Dimas",
                    new EntryClass(RequirementEntry.LoyaltyToCodeBiggerThan, 30),
                    0,
                    false
                )
            },
            {
                4, new SerializablePirateShelter(
                    "San Andrés",
                    "Isla de San Andrés",
                    new float [2] { 68.42f, 89.06f},
                    new float [2] { 69.035f, 89.06f },
                    new float [2] { 68.42f, 90.36f },
                    "Los Tres Pícaros",
                    new EntryClass(RequirementEntry.ByFame_Spain, 30),
                    0,
                    false
                )
            },
            {
                5, new SerializablePirateShelter(
                    "Roatán",
                    "Isla Roatán",
                    new float [2] { 114.08f, 52.23f},
                    new float [2] { 114.08f, 53.08f },
                    new float [2] { 114.08f, 50.93f },
                    "La Negrita",
                    new EntryClass(RequirementEntry.ByFame_Spain, 40),
                    0,
                    false
                )
            },
            {
                6, new SerializablePirateShelter(
                    "Nassau",
                    "",
                    new float [2] { 37.32f, -31.42f},
                    new float [2] { 37.08f, -31.42f },
                    new float [2] { 37.32f, -32.72f },
                    "La Taberna de Nancy",
                    new EntryClass(RequirementEntry.LoyaltyToCodeBiggerThan, 65),
                    0,
                    false
                )
            },
        };

        private Dictionary<byte, SerializableCity> _D_startGameCities = new Dictionary<byte, SerializableCity>()
        {
            {0, new SerializableCity(
                "San Juan",
                "San Juan de Puerto Rico",
                new float [2] { -75.16f, 14.8f},
                new float [2] { -75.16f, 14.324f },
                new float [2] { -75.16f, 15.92f },
                new int[] {0, 10, 4, 12},
                13700,
                "La Gaviota",
                0
                )
            },
            {1, new SerializableCity(
                "Portobelo",
                "Bahía de Portobelo",
                new float [2] { 47.302f, 116.713f},
                new float [2] { 47.902f, 116.241f },
                new float [2] { 47.302f, 117.833f },
                new int[] {15, 6, 11},
                1930,
                "La Bella Inés",
                0
                )
            },
            {2, new SerializableCity(
                "Cartagena",
                "Ciudad de Cartagena",
                new float [2] { 7.63f, 103.47f},
                new float [2] { 8.498f, 103.27f },
                new float [2] { 7.63f, 104.59f },
                new int[] {6, 15, 8},
                9100,
                "La Morena",
                0
                )
            },
            {3, new SerializableCity(
                "Santiago de Tolú",
                "Coronada de Tolú",
                new float [2] { 8.104f, 113.193f},
                new float [2] { 9.216f, 113.513f },
                new float [2] { 8.104f, 111.833f },
                new int[] {8, 2, 0},
                1800,
                "La Sirena",
                1
                )
            },
            {4, new SerializableCity(
                "Maracaibo",
                "Ciudad de Maracaibo",
                new float [2] { -27.85f, 100.629f},
                new float [2] { -28.902f, 100.773f },
                new float [2] { -27.85f, 99.269f },
                new int[] {6, 7, 0},
                6400,
                "El Faro del Diablo",
                1
                )
            },
            {5, new SerializableCity(
                "Coro",
                "Santa Ana de Coro",
                new float [2] { -48.15f, 88.422f},
                new float [2] { -48.15f, 87.77f },
                new float [2] { -48.15f, 89.542f },
                new int[] {8,1},
                3600,
                "El Delfín Negro",
                0
                )
            },
            {6, new SerializableCity(
                "Gibraltar",
                "San Antonio de Gibraltar",
                new float [2] { -36.35f, 112.708f},
                new float [2] { -36.118f, 112.036f },
                new float [2] { -36.35f, 113.828f },
                new int[] {15, 6, 10},
                3640,
                "La Casa del Tuerto",
                0
                )
            },
            {7, new SerializableCity(
                "Caracas",
                "Ciudad de Caracas",
                new float [2] { -73.91f, 93.31f},
                new float [2] { -73.91f, 92.666f },
                new float [2] { -73.91f, 94.43f },
                new int[] {6, 9, 7},
                32000,
                "Las Tres Marías",
                1
                )
            },
            {8, new SerializableCity(
                "Puerto Cabello",
                "Cala de Puerto Cabello",
                new float [2] { -64.37f, 96.25f},
                new float [2] { -64.266f, 95.586f },
                new float [2] { -64.37f, 97.37f },
                new int[] {11, 6, 10},
                7410,
                "Cala del Muerto",
                1
                )
            },
            {9, new SerializableCity(
                "Cumaná",
                "Puerto de las Perlas de Cumaná",
                new float [2] { -99.76f, 90.31f},
                new float [2] { -99.76f, 89.55f },
                new float [2] { -99.76f, 91.43f },
                new int[] {14, 9, 11},
                2100,
                "El Bribón",
                0
                )
            },
            {10, new SerializableCity(
                "La Asunción",
                "La Margarita",
                new float [2] { -101.7f, 84.14f},
                new float [2] { -102.284f, 84.792f },
                new float [2] { -101.7f, 82.78f },
                new int[] {14, 12, 6},
                7200,
                "La Mulata",
                0
                )
            },
            {11, new SerializableCity(
                "Nueva Barcelona",
                "Barcelona del Cerro",
                new float [2] { -96.08f, 92.97f},
                new float [2] { -96.08f, 92.43f },
                new float [2] { -96.08f, 94.09f },
                new int[] {2, 0, 6},
                4700,
                "Los Dos Hermanos",
                1
                )
            },
            {12, new SerializableCity(
                "Puerto España",
                "Puerto de Isla Trinidad",
                new float [2] { -126.532f, 84.35f},
                new float [2] { -125.544f, 84.47f },
                new float [2] { -126.532f, 82.99f },
                new int[] {8, 4, 6},
                3000,
                "La Capitana",
                0
                )
            },
            {13, new SerializableCity(
                "La Habana",
                "San Cristóbal de La Habana",
                new float [2] { 77.97f, -13.96f},
                new float [2] { 78.034f, -14.636f },
                new float [2] { 77.97f, -12.84f },
                new int[] {8, 10, 4, 5},
                32000,
                "Puerto Capitanes",
                0
                )
            },
            {14, new SerializableCity(
                "Santo Domingo",
                "",
                new float [2] { -40.28f, 20.12f},
                new float [2] { -40.28f, 20.84f },
                new float [2] { -40.28f, 18.76f },
                new int[] {8, 2, 5},
                4510,
                "Cisne Negro",
                0
                )
            },
            {15, new SerializableCity(
                "Santiago de Cuba",
                "",
                new float [2] { 16.55f, 12.59f},
                new float [2] { 16.55f, 13.31f },
                new float [2] { 16.557f, 11.23f },
                new int[] {10, 2, 8},
                2300,
                "El Polvorín",
                0
                )
            },
            {16, new SerializableCity(
                "Trinidad",
                "Santísima Trinidad",
                new float [2] { 53.3f, -1.73f},
                new float [2] { 53.388f, -0.722f },
                new float [2] { 53.3f, -3.09f },
                new int[] {8, 6},
                1850,
                "El Burro",
                1
                )
            },
            {17, new SerializableCity(
                "San Carlos y Severino",
                "Severino de Matanzas",
                new float [2] { 67.935f, -14.24f},
                new float [2] { 67.079f, -14.588f },
                new float [2] { 67.935f, -13.12f },
                new int[] {8},
                3600,
                "La Puta Coja",
                0
                )
            },
            {18, new SerializableCity(
                "Fort Royale",
                "Isla de La Granade",
                new float [2] { -122.33f, 69.69f},
                new float [2] { -122.758f, 70.17f },
                new float [2] { -122.33f, 68.33f },
                new int[] {8, 11},
                1900,
                "La Belle Anne",
                0
                )
            },
            {19, new SerializableCity(
                "Martinica",
                "Martinique",
                new float [2] { -125.53f, 42.95f},
                new float [2] { -125.618f, 43.626f },
                new float [2] { -125.53f, 41.59f },
                new int[] {8},
                1370,
                "Les Quatre Amies",
                0,
                true
                )
            },
            {20, new SerializableCity(
                "Guadalupe",
                "Isla Guadalupe",
                new float [2] { -118.64f, 28.5f},
                new float [2] { -119.6f, 28.88f },
                new float [2] { -118.64f, 27.14f },
                new int[] {10, 8},
                15100,
                "La Licorne",
                1,
                true
                )
            },
            {21, new SerializableCity(
                "Cap-Français",
                "Cap-Haïtien",
                new float [2] { -17.4f, 10.91f},
                new float [2] { -17.428f, 10.486f },
                new float [2] { -17.4f, 9.55f },
                new int[] {8, 4},
                9500,
                "La Sirène Haïtien",
                0
                )
            },
            {22, new SerializableCity(
                "Jacmel",
                "Estuario de Jacmel",
                new float [2] { -15.78f, 25.083f},
                new float [2] { -15.588f, 26.031f },
                new float [2] { -15.78f, 23.723f },
                new int[] {8, 7},
                1600,
                "Le Diable Haïtien",
                1
                )
            },
            {23, new SerializableCity(
                "Aruba",
                "Isla de Aruba",
                new float [2] { -43.178f, 79.1f},
                new float [2] { -42.822f, 79.636f },
                new float [2] { -43.178f, 77.74f },
                new int[] {11, 2, 12},
                1800,
                "De Mulatvrouw",
                2,
                true
                )
            },
            {24, new SerializableCity(
                "Curazao",
                "Isla de Curazao",
                new float [2] { -53.66f, 80.92f},
                new float [2] { -52.996f, 81.464f },
                new float [2] { -53.66f, 79.56f },
                new int[] {12, 4, 6},
                2700,
                "De Zwarte Roos",
                0
                )
            },
            {25, new SerializableCity(
                "Kingston",
                "Ciudad de Kingston",
                new float [2] { 23.43f, 31.88f},
                new float [2] { 23.938f, 32.564f },
                new float [2] { 23.43f, 30.52f },
                new int[] {8, 4, 0},
                13000,
                "Jamaican Donkey",
                0,
                true
                )
            },
            {26, new SerializableCity(
                "Belize",
                "Belize Town",
                new float [2] { 130.96f, 37.11f},
                new float [2] { 129.86f, 37.162f },
                new float [2] { 130.96f, 35.75f },
                new int[] {4, 11},
                7500,
                "Molly's Harbour",
                0
                )
            },
            {27, new SerializableCity(
                "Basseterre",
                "Isla de San Cristóbal",
                new float [2] { -106.45f, 19.87f},
                new float [2] { -105.838f, 20.45f },
                new float [2] { -106.45f, 18.51f },
                new int[] {4, 11, 13},
                7400,
                "Saint Kitts' Devil",
                0,
                true
                )
            },
            {28, new SerializableCity(
                "Charlestown",
                "Isla de Nieves",
                new float [2] { -108.531f, 21.607f},
                new float [2] { -107.851f, 21.623f },
                new float [2] { -108.531f, 22.727f },
                new int[] {10, 13, 11, 4},
                6500,
                "Nevis's Siren",
                2,
                true
                )
            },
            {29, new SerializableCity(
                "Barbados",
                "Isla de Barbados",
                new float [2] { -139.81f, 54.74f},
                new float [2] { -139.594f, 55.776f },
                new float [2] { -139.81f, 53.62f },
                new int[] {8},
                1600,
                "The Sea Dragon",
                1
                )
            },
            {30, new SerializableCity(
                "Montserrat",
                "Isla de Montserrat",
                new float [2] { -112.401f, 24.619f},
                new float [2] { -111.773f, 24.451f },
                new float [2] { -112.401f, 25.739f },
                new int[] {8, 9},
                1750,
                "The Sea Lady",
                0
                )
            },
            {31, new SerializableCity(
                "Antigua",
                "Isla Antigua",
                new float [2] { -116.02f, 20.82f},
                new float [2] { -116.26f, 20.18f },
                new float [2] { -116.02f, 21.94f },
                new int[] {8},
                2000,
                "The Privateer",
                0
                )
            },

        };
        private Dictionary<byte, SerializableTown> _D_startGameVillages = new Dictionary<byte, SerializableTown>()
        {
            {0, new SerializableTown(
                "Pilón",
                "Villa de Pilón",
                new float [2] { 31.09f, 12.031f},
                new float [2] { 31.955f, 11.711f },
                new float [2] { 31.09f, 13.431f },
                new int[] {8},
                210,
                1,
                true
                )
            },
            {1, new SerializableTown(
                "Cubagua",
                "Isla de Cubagua",
                new float [2] { -99.33f, 86.41f},
                new float [2] { -99.905f, 86.66f },
                new float [2] { -99.33f, 85.21001f },
                new int[] {14},
                180,
                0,
                true
                )
            },
            {2, new SerializableTown(
                "Riohacha",
                "Villa de Riohacha",
                new float [2] { -17.48f, 90.34f},
                new float [2] { -16.745f, 90.1f },
                new float [2] { -17.48f, 91.74f },
                new int[] {14},
                600,
                0,
                true
                )
            },
            {3, new SerializableTown(
                "Ciénaga y Santa Marta",
                "Bahía de Santa Marta",
                new float [2] { -3.832f, 95.011f },
                new float [2] { -3.407f, 94.876f },
                new float [2] { -3.832f, 96.411f },
                new int[] {0},
                830,
                0,
                true
                )
            },
            {4, new SerializableTown(
                "Barranquilla",
                "Aldea de Barranquilla",
                new float [2] { 2.175f, 97.991f},
                new float [2] { 2.63f, 97.626f },
                new float [2] { 2.175f, 99.391f },
                new int[] {12},
                550,
                0
                )
            },
            {5, new SerializableTown(
                "Coveñas",
                "Villa de Coveñas",
                new float [2] { 9.51f, 115.66f},
                new float [2] { 2.63f, 97.626f },
                new float [2] { 2.175f, 99.391f },
                new int[] {2},
                650,
                1
                )
            },
            {6, new SerializableTown(
                "Moñitos",
                "Villa de Moñitos",
                new float [2] { 14.254f, 117.695f},
                new float [2] { 14.839f, 119.095f },
                new float [2] { 14.254f, 99.391f },
                new int[] {5},
                160,
                1,
                true
                )
            },
            {7, new SerializableTown(
                "Moín",
                "Bahía de Moín",
                new float [2] { 81.583f, 114.232f},
                new float [2] { 81.59f, 113.757f },
                new float [2] { 81.583f, 115.632f },
                new int[] {0},
                180,
                0
                )
            },
            {8, new SerializableTown(
                "Puerto Viejo de Talamanca",
                "Puerto Viejo",
                new float [2] { 77.293f, 118.196f},
                new float [2] { 76.503f, 117.821f },
                new float [2] { 77.293f, 119.596f },
                new int[] {6},
                260,
                1,
                true
                )
            },
            {9, new SerializableTown(
                "Villa Trujillo",
                "Trujillo",
                new float [2] { 110.29f, 57.919f},
                new float [2] { 110.29f, 57.199f },
                new float [2] { 110.29f, 59.319f },
                new int[] {15},
                250,
                0
                )
            },
            {10, new SerializableTown(
                "Puerto Caballos",
                "Puerto Cortés",
                new float [2] { 127.62f, 58.82f},
                new float [2] { 128.17f, 58.715f },
                new float [2] { 127.62f, 60.22f },
                new int[] {1},
                180,
                0,
                true
                )
            },
            {11, new SerializableTown(
                "Río Dulce",
                "Boca de Río Dulce",
                new float [2] { 136.305f, 58.63f},
                new float [2] { 135.695f, 58.45f },
                new float [2] { 136.305f, 60.03f },
                new int[] {2},
                120,
                0
                )
            },
            {12, new SerializableTown(
                "San Carlos de Macuro",
                "Macuro",
                new float [2] { -119.14f, 84.16f},
                new float [2] { -119.455f, 84.92f },
                new float [2] { -119.14f, 82.86f },
                new int[] {6},
                210,
                1,
                true
                )
            },
            {13, new SerializableTown(
                "San Martín",
                "Isla de San Martín",
                new float [2] { -102.9f, 13.51f},
                new float [2] { -102.7f, 14.11f },
                new float [2] { -102.9f, 12.21f },
                new int[] {12},
                190,
                0,
                true
                )
            },
            {14, new SerializableTown(
                "Roseau ",
                "Isla de Ro-Zó",
                new float [2] { -122.22f, 36.98f},
                new float [2] { -121.52f, 37.205f },
                new float [2] { -122.22f, 35.58f },
                new int[] {5},
                350,
                0
                )
            },
            {15, new SerializableTown(
                "Tobago",
                "Isla de Tobago",
                new float [2] { -132.84f, 75.51f},
                new float [2] { -133.655f, 75.91f },
                new float [2] { -132.84f, 74.11f },
                new int[] {8},
                350,
                1
                )
            },
            {16, new SerializableTown(
                "Bonaire",
                "Isla de Bonaire",
                new float [2] { -59.73f, 79.6f},
                new float [2] { -59.26f, 79.82f },
                new float [2] { -59.73f, 78.2f },
                new int[] {12},
                320,
                0
                )
            },
            {17, new SerializableTown(
                "Barbuda",
                "Isla Barbuda",
                new float [2] { -114.644f, 15.535f},
                new float [2] { -114.619f, 16.225f },
                new float [2] { -114.644f, 16.935f },
                new int[] {0},
                200,
                0,
                true
                )
            },
            {18, new SerializableTown(
                "Providencia",
                "Vieja Providencia",
                new float [2] { 65.067f, 80.338f},
                new float [2] { 65.552f, 80.038f },
                new float [2] { 65.067f, 81.738f },
                new int[] {9},
                180,
                0,
                true
                )
            },
            {19, new SerializableTown(
                "Bluefields",
                "",
                new float [2] { 87.85f, 94.76f},
                new float [2] { 87.625f, 95.535f },
                new float [2] { 87.85f, 93.06f },
                new int[] {11},
                550,
                1,
                true
                )
            },
            {20, new SerializableTown(
                "Corn Island",
                "",
                new float [2] { 81.141f, 93.444f},
                new float [2] { 80.506f, 93.684f },
                new float [2] { 81.141f, 91.744f },
                new int[] {0},
                250,
                0,
                true
                )
            },
            {21, new SerializableTown(
                "George Town",
                "Gran Caimán",
                new float [2] { 66.773f, 22.451f},
                new float [2] { 66.703f, 21.981f },
                new float [2] { 66.773f, 21.151f },
                new int[] {2},
                200,
                1
                )
            },
            {22, new SerializableTown(
                "Saint Vicent",
                "Isla de San Vicente",
                new float [2] { -126.28f, 56.86f},
                new float [2] { -126.905f, 57.43f },
                new float [2] { -126.28f, 55.16f },
                new int[] {1},
                170,
                1
                )
            },
            {23, new SerializableTown(
                "Santa Lucía",
                "Isla de Santa Lucía",
                new float [2] { -127.906f, 48.981f},
                new float [2] { -127.281f, 48.651f },
                new float [2] { -127.906f, 50.381f },
                new int[] {8},
                260,
                0,
                true
                )
            },
        };
        private Dictionary<byte, SerializableKingdom> _D_startGameKingdomsData = new Dictionary<byte, SerializableKingdom>()
        {
            {0, new SerializableKingdom(
                0,
                "España",
                "español",
                "españoles",
                "española",
                "españolas",
                new float[4] { 51, 37, 6, 6},
                1,
                new ushort[0],
                new ushort[0],
                new SerializableCity[0],
                new SerializableTown[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0]
                )
            },
            {1, new SerializableKingdom(
                1,
                "Portugal",
                "portugués",
                "portugueses",
                "portuguesa",
                "portuguesas",
                new float[4] { 53, 27, 12, 8},
                2,
                new ushort[0],
                new ushort[0],
                new SerializableCity[0],
                new SerializableTown[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0]
                )
            },
            {2, new SerializableKingdom(
                2,
                "Francia",
                "francés",
                "franceses",
                "francesa",
                "francesas",
                new float[4] { 54, 20, 11, 15},
                1,
                new ushort[0],
                new ushort[0],
                new SerializableCity[0],
                new SerializableTown[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0]
                )
            },
            {3, new SerializableKingdom(
                3,
                "Holanda",
                "holandés",
                "holandeses",
                "holandesa",
                "holandesas",
                new float[4] { 35, 37, 10, 13},
                2,
                new ushort[0],
                new ushort[0],
                new SerializableCity[0],
                new SerializableTown[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0]
                )
            },
            {4, new SerializableKingdom(
                4,
                "Gran Bretaña",
                "inglés",
                "ingleses",
                "inglesa",
                "inglesas",
                new float[4] { 50, 22, 12, 16},
                2,
                new ushort[0],
                new ushort[0],
                new SerializableCity[0],
                new SerializableTown[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0],
                new SerializableConvoy[0]
                )
            },
        };

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

        public static Vector3 ToVector3(float[] values)
        {
            return new Vector3(values[0], values[1], values[2]);
        }

        public static Vector2 ToVector2(float[] values)
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
        public string
            cityName,
            alternativeName;
        public bool revealed;
        public float[]
            position,
            entryPoint,
            pivotPoint,
            eventsPoint;
        public byte spriteIndex;
        public bool flippedX;
    }

    [Serializable]
    public abstract class SerializableSettlement : SerializableKeyPoint
    {
        //public SerializableResource[] exports;
        public int[] exports;
    }

    [Serializable]
    public class SerializableCity : SerializableSettlement
    {
        public int population;
        public string tavernName;
        //todo ¿personajes en la ciudad?

        //todo | crear un id de elemento temporal para el serializado
        //todo | y así vincular barcos, destinos de barcos y personajes a ciudades

        public SerializableCity(MB_City city)
        {
            cityName = city.cityName;
            revealed = city.revealed;

            position = ConvertV3(city.transform.position);
            entryPoint = ConvertV3(city.transform.GetChild(0).position);
            pivotPoint = ConvertV3(city.transform.GetChild(1).position);
            eventsPoint = null; //todo

            exports = city.exportsIndex;
            population = city.population;
            tavernName = city.tavernName;
            spriteIndex = city.imgIndex;
            flippedX = city.transform.localScale.x < 0;
        }

        public SerializableCity(
            string cityName,
            string alternativeName,
            float[] position,
            float[] entryPoint,
            float[] pivotPoint,
            int[] exports,
            int population,
            string tavernName,
            byte imgIndex = 0,
            bool flippedX = false)
        {
            this.cityName = cityName;
            this.alternativeName = alternativeName;
            revealed = false;

            this.position = new float[3] { position[0], 0.01f, position[1] };
            this.entryPoint = new float[3] { entryPoint[0], 0.01f, entryPoint[1] };
            this.pivotPoint = new float[3] { pivotPoint[0], 0.01f, pivotPoint[1] };
            this.exports = exports;
            this.population = population;
            this.tavernName = tavernName;
            this.spriteIndex = imgIndex;
            this.flippedX = flippedX;
        }
    }

    [Serializable]
    public class SerializableTown : SerializableSettlement
    {
        public int population;

        public SerializableTown(MB_Town town)
        {
            cityName = town.cityName;
            alternativeName = town.alternativeName;
            revealed = town.revealed;

            position = ConvertV3(town.transform.position);
            entryPoint = ConvertV3(town.transform.GetChild(0).position);
            pivotPoint = ConvertV3(town.transform.GetChild(1).position);
            eventsPoint = null; //todo

            population = town.population;
            exports = town.exportsIndex;
            spriteIndex = town.imgIndex;
            flippedX = town.transform.localScale.x < 0;
        }

        public SerializableTown(
            string townName,
            string alternativeName,
            float[] position,
            float[] entryPoint,
            float[] pivotPoint,
            int[] exports,
            int population,
            byte imgIndex = 0,
            bool flippedX = false)
        {
            this.cityName = townName;
            this.alternativeName = alternativeName;
            revealed = false;

            this.position = new float[3] { position[0], 0.01f, position[1] };
            this.entryPoint = new float[3] { entryPoint[0], 0.01f, entryPoint[1] };
            this.pivotPoint = new float[3] { pivotPoint[0], 0.01f, pivotPoint[1] };
            this.eventsPoint = null; //todo
            this.exports = exports;
            this.population = population;
            this.spriteIndex = imgIndex;
            this.flippedX = flippedX;
        }
    }

    [Serializable]
    public class SerializableSmugglersPost : SerializableSettlement
    {
        //todo | personajes en la ciudad
        //todo | generación de id

        public SerializableSmugglersPost(MB_SmugglersPost post)
        {
            cityName = post.cityName;
            alternativeName = post.alternativeName;
            revealed = post.revealed;
            position = ConvertV3(post.transform.position);
            entryPoint = ConvertV3(post.transform.GetChild(0).position);
            pivotPoint = ConvertV3(post.transform.GetChild(1).position);
            eventsPoint = null; //todo
            exports = post.exportsIndex;
            //spriteIndex = post.spriteIndex; //todo
            flippedX = post.transform.localScale.x < 0;
        }

        public SerializableSmugglersPost(
            string townName,
            string alternativeName,
            float[] position,
            float[] entryPoint,
            float[] pivotPoint,
            int[] exports,
            byte imgIndex = 0,
            bool flippedX = false)
        {
            this.cityName = townName;
            this.alternativeName = alternativeName;
            revealed = false;

            this.position = new float[3] { position[0], 0.01f, position[1] };
            this.entryPoint = new float[3] { entryPoint[0], 0.01f, entryPoint[1] };
            this.pivotPoint = new float[3] { pivotPoint[0], 0.01f, pivotPoint[1] };
            this.eventsPoint = null; //todo
            this.exports = exports;
            this.spriteIndex = imgIndex;
            this.flippedX = flippedX;
        }

        public MB_SmugglersPost Deserialize(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("KeyPoints/KeyPoint_Hideout") as GameObject;
            GameObject kp = UnityEngine.Object.Instantiate(prefab, parent);
            if (kp.TryGetComponent<MB_SmugglersPost>(out MB_SmugglersPost port))
            {
                //Añade a este puerto la información serializada
                port.SetFromSerializedData(this);
                return port;
            }

            return null;
        }
    }

    [Serializable]
    public class SerializablePirateShelter : SerializableSettlement
    {
        public string tavernName;
        public EntryClass entryCondition;
        //todo | personajes en la ciudad
        //todo | generación de id

        public SerializablePirateShelter(MB_PirateShelter shelter)
        {
            cityName = shelter.cityName;
            alternativeName = shelter.alternativeName;
            revealed = shelter.revealed;
            position = ConvertV3(shelter.transform.position);
            entryPoint = ConvertV3(shelter.transform.GetChild(0).position);
            pivotPoint = ConvertV3(shelter.transform.GetChild(1).position);
            eventsPoint = null; //todo
            tavernName = shelter.tavernName;
            // this.spriteIndex = shelter.imgIndex; //todo
            this.flippedX = shelter.transform.localScale.x < 0;
        }

        public SerializablePirateShelter(
            string shelterName,
            string alternativeName,
            float[] position,
            float[] entryPoint,
            float[] pivotPoint,
            string tavernName,
            EntryClass condition,
            byte imgIndex = 0,
            bool flippedX = false)
        {
            this.cityName = shelterName;
            this.alternativeName = alternativeName;
            revealed = false;

            this.position = new float[3] { position[0], 0.01f, position[1] };
            this.entryPoint = new float[3] { entryPoint[0], 0.01f, entryPoint[1] };
            this.pivotPoint = new float[3] { pivotPoint[0], 0.01f, pivotPoint[1] };
            this.eventsPoint = null; //todo
            this.tavernName = tavernName;
            this.entryCondition = new EntryClass(condition);
            this.spriteIndex = imgIndex;
            this.flippedX = flippedX;
        }

        public MB_PirateShelter Deserialize(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("KeyPoints/KeyPoint_Shelter") as GameObject;
            GameObject kp = UnityEngine.Object.Instantiate(prefab, parent);
            if (kp.TryGetComponent<MB_PirateShelter>(out MB_PirateShelter port))
            {
                //Añade a este puerto la información serializada
                port.SetFromSerializedData(this);
                return port;
            }

            return null;
        }
    }

    [Serializable]
    public class SerializableNaturalPort : SerializableKeyPoint
    {
        public byte calado;

        public SerializableNaturalPort(MB_NaturalPort port)
        {
            cityName = port.cityName;
            alternativeName = port.alternativeName;
            revealed = port.revealed;
            position = ConvertV3(port.transform.position);
            entryPoint = ConvertV3(port.transform.GetChild(0).position);
            pivotPoint = ConvertV3(port.transform.GetChild(1).position);
            eventsPoint = null; //todo
            //spriteIndex = port.spriteIndex; //todo
            calado = port.calado;

        }

        public SerializableNaturalPort(
            string portName,
            string alternativeName,
            float[] position,
            float[] entryPoint,
            float[] pivotPoint,
            byte calado,
            byte imgIndex = 0,
            bool flippedX = false)
        {
            this.cityName = portName;
            this.alternativeName = alternativeName;
            revealed = false;

            this.position = new float[3] { position[0], 0.01f, position[1] };
            this.entryPoint = new float[3] { entryPoint[0], 0.01f, entryPoint[1] };
            this.pivotPoint = new float[3] { pivotPoint[0], 0.01f, pivotPoint[1] };
            this.eventsPoint = null; //todo

            this.spriteIndex = imgIndex;
            this.flippedX = flippedX;
        }

        public MB_NaturalPort Deserialize(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("KeyPoints/KeyPoint_NatPort") as GameObject;
            GameObject kp = UnityEngine.Object.Instantiate(prefab, parent);
            if (kp.TryGetComponent<MB_NaturalPort>(out MB_NaturalPort port))
            {
                //Añade a este puerto la información serializada
                port.SetFromSerializedData(this);
                return port;
            }

            return null;
        }
    }

    [Serializable]
    public class SerializableKingdom : SerializationConverter
    {
        public ushort tagKey;
        public string
            kingdomName,
            gentilism_MALESIN, gentilism_MALEPLU, gentilism_FEMSIN, gentilism_FEMPLU;
        public float[] roleFleetSpawnStats;
        public byte countryBaseStrength = 1;
        public ushort[]
            atWarWith,
            atTradeAgrrementWith;
        public SerializableCity[] countryCities;
        public SerializableTown[] countryVillages;
        public SerializableConvoy[]
            countryMerchants,
            countryPatrols,
            europeanConvoys;

        public SerializableKingdom(Kingdom kingdom, Transform kTransform)
        {
            tagKey = kingdom.tagKey;
            kingdomName = kingdom.KINGDOMNAME;
            gentilism_MALESIN = kingdom.GENTILISM_MALESIN;
            gentilism_MALEPLU = kingdom.GENTILISM_MALEPLU;
            gentilism_FEMSIN = kingdom.GENTILISM_FEMSIN;
            gentilism_FEMPLU = kingdom.GENTILISM_FEMPLU;
            roleFleetSpawnStats = kingdom.roleFleetsSpawnStatistics;

            var warList = kingdom.atWarWith.ToArray();
            atWarWith = new ushort[warList.Length];
            atWarWith = warList.Select((k, index) => warList[index].tagKey).ToArray();

            //posesiones del reino:
            var countryPossessions = kingdom.GetPortsList();
            var citiesList = countryPossessions.Where(s => s is MB_City).ToList();
            var townsList = countryPossessions.Where(s => s is MB_Town).ToList();

            countryCities = new SerializableCity[citiesList.Count];
            countryVillages = new SerializableTown[townsList.Count];

            for (int i = 0; i < countryCities.Length; i++)
            {
                var city = citiesList[i] as MB_City;
                countryCities[i] = new SerializableCity(city);
            }

            for (int i = 0; i < countryVillages.Length; i++)
            {
                var town = townsList[i] as MB_Town;
                countryVillages[i] = new SerializableTown(town);
            }

            var shipsContainer = kTransform.GetChild(2);
            var shipsList = new List<SerializableConvoy>();
            for (int i = 0; i < shipsContainer.childCount; i++)
            {
                var shipObj = shipsContainer.GetChild(i);
                if (shipObj.gameObject.activeSelf)
                {
                    var s = shipObj.GetComponent<ConvoyNPC>();
                    shipsList.Add(new SerializableConvoy(s));
                }
            }
            countryMerchants = shipsList.ToArray();

            shipsContainer = kTransform.GetChild(3);
            shipsList = new List<SerializableConvoy>();
            for (int i = 0; i < shipsContainer.childCount; i++)
            {
                var shipObj = shipsContainer.GetChild(i);
                if (shipObj.gameObject.activeSelf)
                {
                    var s = shipObj.GetComponent<ConvoyNPC>();
                    shipsList.Add(new SerializableConvoy(s));
                }
            }
            countryPatrols = shipsList.ToArray();

            shipsContainer = kTransform.GetChild(4);
            shipsList = new List<SerializableConvoy>();
            for (int i = 0; i < shipsContainer.childCount; i++)
            {
                var shipObj = shipsContainer.GetChild(i);
                if (shipObj.gameObject.activeSelf)
                {
                    var s = shipObj.GetComponent<ConvoyNPC>();
                    shipsList.Add(new SerializableConvoy(s));
                }
            }
            europeanConvoys = shipsList.ToArray();
        }

        public SerializableKingdom(
            ushort tagKey,
            string kingdomName,
            string gentilism_MALESIN,
            string gentilism_MALEPLU,
            string gentilism_FEMSIN,
            string gentilism_FEMPLU,
            float[] roleFleetSpawnStats,
            byte countryBaseStrength,
            ushort[] atWarWith,
            ushort[] atTradeAgrrementWith,
            SerializableCity[] countryCities,
            SerializableTown[] countryVillages,
            SerializableConvoy[] countryMerchants,
            SerializableConvoy[] countryPatrols,
            SerializableConvoy[] europeanConvoys)
        {
            this.tagKey = tagKey;
            this.kingdomName = kingdomName;
            this.gentilism_MALESIN = gentilism_MALESIN;
            this.gentilism_MALEPLU = gentilism_MALEPLU;
            this.gentilism_FEMSIN = gentilism_FEMSIN;
            this.gentilism_FEMPLU = gentilism_FEMPLU;
            this.roleFleetSpawnStats = roleFleetSpawnStats;
            this.countryBaseStrength = countryBaseStrength;
            this.atWarWith = atWarWith;
            this.atTradeAgrrementWith = atTradeAgrrementWith;
            this.countryCities = countryCities;
            this.countryVillages = countryVillages;
            this.countryMerchants = countryMerchants;
            this.countryPatrols = countryPatrols;
            this.europeanConvoys = europeanConvoys;
        }
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
        //public SerializableResource resource;
        public byte key;
        public int amount;

        //public SerializableInventoryStacking(Resource r, int n)
        //{
        //    resource = new SerializableResource(r);
        //    amount = n;
        //}

        //public SerializableInventoryStacking(SerializableResource r, int n)
        //{
        //    resource = r;
        //    amount = n;
        //}

        //public SerializableInventoryStacking(InventoryItemStacking inv)
        //{
        //    resource = new SerializableResource(inv.resource);
        //    amount = inv.amount;
        //}

        public SerializableInventoryStacking(Resource r, int n)
        {
            key = r.Key;
            amount = n;
        }

        public SerializableInventoryStacking(SerializableResource r, int n)
        {
            key = r.key;
            amount = n;
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
                smugglerInventory[i] = new SerializableInventoryStacking(inventory[i].resource, inventory[i].amount);
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

        public Ship GetShipFromSerializedData()
        {

            Ship newShip;
            switch (shipType)
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

    [Serializable]
    public class SerializableConvoy : SerializationConverter
    {
        public SerializableShip[] convoyShips;
        public float[]
            position,
            rotation,
            destination;
        public ushort
            id,
            targetId; //referencia al convoy/barco al que está persiguiendo
        public bool inOnTarget;

        public SerializableConvoy(ConvoyNPC convoy)
        {
            var ships = convoy.thisConvoyShips;
            convoyShips = new SerializableShip[ships.Length];
            for (int i = 0; i < ships.Length; i++)
            {
                convoyShips[i] = new SerializableShip(ships[i]);
            }

            var tr = convoy.transform;
            position = ConvertV3(tr.position);
            rotation = ConvertQuaternion(tr.rotation);

            //todo: id y target id

            inOnTarget = convoy.isOnTarget;
        }
    }

    #endregion

    #region DATA (DEV ONLY)
    /// <summary>
    /// An object that contains the kingdoms and cities info for starting a new game.
    /// In a sense, creating a new game is like loading a saved game, but such saved game contains the initial kingdoms data only
    /// </summary>
    [Serializable]
    public class StartGameData : SerializationConverter
    {
        public DateTime WorldDate; // fecha de la partida

        //-----------------------------
        //World: Cities and KeyPoints
        //-----------------------------

        public SerializableKingdom[] kingdoms;
        public SerializableNaturalPort[] naturalPorts;
        public SerializableSmugglersPost[] hideouts;
        public SerializablePirateShelter[] shelters;

        public StartGameData()
        {
            //!el constructor nunca debería de llamarse si no estamos en la escena del juego
            //!Sin embargo, el deserializar json llama al constructor si cargamos un mod
            if (GameObject.FindWithTag("Kingdoms") == null)
                return;

            //Basic Game Data
            WorldDate = TimeManager.WorldDate;

            //* World, kingdoms and cities:
            //Kingdoms:
            var kingdomsContainer = GameObject.FindWithTag("Kingdoms").transform;
            kingdoms = new SerializableKingdom[kingdomsContainer.childCount];
            for (int i = 0; i < kingdomsContainer.childCount; i++)
            {
                var k = kingdomsContainer.GetChild(i).GetComponent<Kingdom>();
                kingdoms[i] = new SerializableKingdom(k, k.transform);
            }

            var worldPlacesContainer = GameObject.FindWithTag("WorldPlaces").transform;

            //Natural shelters:
            var naturalPortsContainer = worldPlacesContainer.GetChild(0);
            naturalPorts = new SerializableNaturalPort[naturalPortsContainer.childCount];

            for (int i = 0; i < naturalPortsContainer.childCount; i++)
            {
                var p = naturalPortsContainer.GetChild(i).GetComponent<MB_NaturalPort>();
                naturalPorts[i] = new SerializableNaturalPort(p);
            }

            //Smugglers hideouts
            var smugglersHideoutsContainer = worldPlacesContainer.GetChild(1);
            hideouts = new SerializableSmugglersPost[smugglersHideoutsContainer.childCount];

            for (int i = 0; i < smugglersHideoutsContainer.childCount; i++)
            {
                var p = smugglersHideoutsContainer.GetChild(i).GetComponent<MB_SmugglersPost>();
                hideouts[i] = new SerializableSmugglersPost(p);
            }

            //Pirate shelters
            var pirateSheltersContainer = worldPlacesContainer.GetChild(2);
            shelters = new SerializablePirateShelter[pirateSheltersContainer.childCount];

            for (int i = 0; i < pirateSheltersContainer.childCount; i++)
            {
                var p = pirateSheltersContainer.GetChild(i).GetComponent<MB_PirateShelter>();
                shelters[i] = new SerializablePirateShelter(p);
            }
        }
    }
    #endregion
    #region SAVE GAME

    [Serializable]
    public class SavedFile : SerializationConverter
    {
        //-----------------------------
        //Basic Game Data & Game Settings
        //-----------------------------
        public DateTime WorldDate; // fecha de la partida
        //public DateTime startingDate; //fecha inicial de la partida;
        public uint playedTime; // tiempo de juego en segundos;
        public float deltaWorldDate; // el contador para llegar al próximo día
        public byte
            marketTimer, // contador para actualizar mercados.
            pirateActivity; // número de piratas generados

        public GameDifficulty settings_gameDifficulty;
        public bool settings_MoreEvents;
        public bool settings_AggresiveKingdoms;
        public bool settings_AgressivePatrols;
        public ushort IDCounter; //contador de entidades que hemos creado (indica el siguiente valor de clave primaria cuando instanciamos algo)
        public ushort IDKeyPointCounter; //Contador de ubicaciones en el mundo creadas (ciudades, puntos de misión, etc);

        //-----------------------------
        //Player's basic Data
        //-----------------------------
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

        //-----------------------------
        //Player's Inventory & Crew
        //-----------------------------
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

        //-----------------------------
        //World: Cities and KeyPoints
        //-----------------------------

        public SerializableKingdom[] kingdoms;
        public SerializableNaturalPort[] naturalPorts;
        public SerializableSmugglersPost[] hideouts;
        public SerializablePirateShelter[] shelters;
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
            pirateActivity = GameManager.gm.pirateActivity;

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

            playerFlag = PersistentGameData._GData_PlayerFlag.texture.GetRawTextureData();
            playerAvatar = PersistentGameData._GData_PlayerAvatar.texture.GetRawTextureData();

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

            if (rationingRum || rationingMeat)
            {
                //var toggles = TimeManager.instance.RationingManagers;
                var toggles = GameObject.FindObjectsOfType<UI.WorldMap.ToggleRationing>(true);

                if (rationingRum)
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

            //* World, kingdoms and cities:
            //Kingdoms:
            var kingdomsContainer = GameObject.FindWithTag("Kingdoms").transform;
            kingdoms = new SerializableKingdom[kingdomsContainer.childCount];
            for (int i = 0; i < kingdomsContainer.childCount; i++)
            {
                var k = kingdomsContainer.GetChild(i).GetComponent<Kingdom>();
                kingdoms[i] = new SerializableKingdom(k, k.transform);
            }

            var worldPlacesContainer = GameObject.FindWithTag("WorldPlaces").transform;

            //Natural shelters:
            var naturalPortsContainer = worldPlacesContainer.GetChild(0);
            naturalPorts = new SerializableNaturalPort[naturalPortsContainer.childCount];

            for (int i = 0; i < naturalPortsContainer.childCount; i++)
            {
                var p = naturalPortsContainer.GetChild(i).GetComponent<MB_NaturalPort>();
                naturalPorts[i] = new SerializableNaturalPort(p);
            }

            //Smugglers hideouts
            var smugglersHideoutsContainer = worldPlacesContainer.GetChild(1);
            hideouts = new SerializableSmugglersPost[smugglersHideoutsContainer.childCount];

            for (int i = 0; i < smugglersHideoutsContainer.childCount; i++)
            {
                var p = smugglersHideoutsContainer.GetChild(i).GetComponent<MB_SmugglersPost>();
                hideouts[i] = new SerializableSmugglersPost(p);
            }

            //Pirate shelters
            var pirateSheltersContainer = worldPlacesContainer.GetChild(2);
            shelters = new SerializablePirateShelter[pirateSheltersContainer.childCount];

            for (int i = 0; i < pirateSheltersContainer.childCount; i++)
            {
                var p = pirateSheltersContainer.GetChild(i).GetComponent<MB_PirateShelter>();
                shelters[i] = new SerializablePirateShelter(p);
            }
        }
    }

    [Serializable]
    public class SerializationUtilities : SerializationConverter
    {
        public string extension = ".pirate";

        protected virtual string getFileExtension()
        {
            var currentMod = PersistentGameSettings.currentMod;

            if (currentMod == null)
            {
                return extension;
            }
            else if (currentMod.gameLogic != null)
            {
                //extensión de partidas guardadas del mod:
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
                return currentMod.ModPath + "/Data/Saves/";
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
            // Debug.Log("saving: " + path);

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

    public class StartGameBinaryFormat : SerializationUtilities
    {
        public StartGameData savedFile;

        public StartGameBinaryFormat(StartGameData savedFile)
        {
            this.savedFile = savedFile;
        }

        protected override string getFileExtension()
        {
            extension = ".start";
            return base.getFileExtension();
        }

        public void WorldData(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();

            var binaryFormatter = new BinaryFormatter();
            var stream = new FileStream(path, FileMode.Create);

            //Serialización:
            binaryFormatter.Serialize(stream, this.savedFile);
            stream.Close();
        }
    }

    public class StartGameJSONFormat : SerializationUtilities
    {
        public StartGameData savedFile;

        public StartGameJSONFormat(StartGameData savedFile)
        {
            this.savedFile = savedFile;
        }

        protected override string getFileExtension()
        {
            extension = ".json";
            return base.getFileExtension();
        }

        public void WorldData(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();
            //Serialización:
            File.WriteAllText(path, JsonUtility.ToJson(savedFile, true));
            Debug.Log("SerializeJSON: success");
        }
    }

    #endregion

    #region LOAD
    // [Serializable]
    // public class LoaderBinaryFormat<T> : SerializationUtilities
    // {
    //     public T LoadGame(string fileName)
    //     {
    //         var path = getRoute() + fileName + getFileExtension();
    //         Debug.Log(path);
    //         if (File.Exists(path))
    //         {
    //             var binaryFormatter = new BinaryFormatter();
    //             var stream = new FileStream(path, FileMode.Open);
    //             T result = binaryFormatter.Deserialize(stream) as T; //!esto no compila
    //             stream.Close();
    //             return result;
    //         }
    //         return null;
    //     }
    // }

    [Serializable]
    public class GameLoaderBinaryFormat : SerializationUtilities
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

    [Serializable]
    public class CitiesLoaderBinaryFormat : SerializationUtilities
    {
        public CitiesLoaderBinaryFormat()
        {
            extension = ".start";
        }

        protected string getRoute()
        {
            var currentMod = PersistentGameSettings.currentMod;
            if (currentMod == null)
            {
                //Ruta por del juego vanilla:
                return Directory.GetCurrentDirectory() + "/Campaigns/";
            }
            else
            {
                //Ruta del directorio del mod
                return currentMod.ModPath + "/Data/Campaigns/";
            }
        }

        protected override string getFileExtension()
        {
            var currentMod = PersistentGameSettings.currentMod;

            if (currentMod == null)
            {
                return extension;
            }
            else if (currentMod.gameLogic != null)
            {
                //extensión de partida para mods:
                // return currentMod.gameLogic.modExtension;
                //(usaremos un .json)
                return ".json";
            }
            else
            {
                return extension;
            }
        }

        public StartGameData LoadWorldData(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();
            Debug.Log(path);
            if (File.Exists(path))
            {
                var binaryFormatter = new BinaryFormatter();
                var stream = new FileStream(path, FileMode.Open);
                StartGameData result = binaryFormatter.Deserialize(stream) as StartGameData;
                stream.Close();
                return result;
            }
            return null;
        }
    }

    [Serializable]
    public class CitiesLoaderJsonFormat : SerializationUtilities
    {
        public CitiesLoaderJsonFormat()
        {
            extension = ".json";
        }

        protected string getRoute()
        {
            var currentMod = PersistentGameSettings.currentMod;
            if (currentMod == null)
            {
                //Ruta por del juego vanilla:
                return Directory.GetCurrentDirectory() + "/Campaigns/";
            }
            else
            {
                //Ruta del directorio del mod
                return currentMod.ModPath + "/Data/Campaigns/";
            }
        }

        protected override string getFileExtension()
        {
            var currentMod = PersistentGameSettings.currentMod;

            if (currentMod == null)
            {
                return extension;
            }
            else if (currentMod.gameLogic != null)
            {
                //extensión de partida para mods:
                // return currentMod.gameLogic.modExtension;
                //(usaremos un .json)
                return ".json";
            }
            else
            {
                return extension;
            }
        }

        public StartGameData LoadWorldData(string fileName)
        {
            var path = getRoute() + fileName + getFileExtension();
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                StartGameData result = JsonUtility.FromJson<StartGameData>(json) as StartGameData;
                return result;
            }
            return null;
        }
    }

    #endregion

}

namespace Serialization
{
    public class SerializationUtils
    {
        public static bool SerializeJSON<T>(T data, string pathFile)
        {
            try
            {
                File.WriteAllText(pathFile, JsonUtility.ToJson(data, true));
                Debug.Log("SerializeJSON: success");
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        public static T LoadJson<T>(string path)
        {
            T data;
            data = JsonUtility.FromJson<T>(File.ReadAllText(path));
            return data;
        }

        //--------------------------------
        // MAPA DE CORRIENTES
        //--------------------------------


        //aquí voy a poner un ejemplo de serialización de mapa de corrientes con los datos por defecto
        public static void createDefaultCurrentsMap()
        {
            int[] bufferArray =
                {
                22, 32, 21, 32, 22, 21, 0, 0, 0, 15, 61, 0, 22, 32, 42, 41, 31, 41, 62, 51, 53, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                23, 33, 42, 22, 33, 22, 31, 0, 0, 15, 61, 0, 81, 62, 42, 61, 22, 32, 42, 41, 43, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                13, 0, 51, 13, 31, 44, 22, 21, 0, 15, 71, 0, 0, 61, 62, 22, 23, 33, 43, 42, 51, 42, 0, 0, 0, 0, 0, 0, 71, 81, 71, 81,
                83, 73, 63, 83, 25, 45, 35, 35, 25, 14, 82, 0, 11, 0, 61, 13, 13, 61, 52, 62, 41, 81, 42, 21, 81, 61, 21, 22, 32, 81, 81, 61,
                72, 82, 83, 25, 83, 63, 22, 22, 13, 82, 82, 81, 81, 0, 0, 81, 83, 83, 62, 23, 33, 82, 81, 71, 71, 21, 12, 21, 41, 52, 61, 62,
                82, 72, 83, 15, 72, 62, 0, 0, 0, 0, 0, 0, 81, 22, 42, 0, 71, 62, 72, 83, 0, 43, 82, 81, 71, 71, 82, 81, 61, 62, 81, 42,
                71, 71, 84, 85, 62, 0, 0, 23, 43, 0, 0, 0, 0, 82, 82, 71, 0, 61, 71, 83, 73, 63, 82, 82, 62, 72, 81, 82, 72, 61, 81, 81,
                0, 0, 12, 15, 85, 23, 23, 32, 32, 21, 0, 0, 0, 0, 0, 81, 61, 61, 71, 71, 82, 82, 82, 72, 72, 61, 72, 82, 82, 72, 82, 82,
                0, 0, 81, 83, 84, 23, 13, 23, 62, 82, 82, 81, 31, 61, 61, 62, 0, 0, 0, 0, 0, 0, 81, 71, 71, 82, 71, 71, 0, 81, 81, 82,
                0, 0, 11, 12, 83, 84, 84, 84, 72, 71, 81, 21, 41, 41, 41, 51, 0, 0, 0, 0, 22, 42, 81, 0, 71, 61, 71, 82, 0, 0, 82, 81,
                0, 0, 11, 82, 82, 83, 84, 74, 84, 72, 81, 11, 0, 0, 32, 42, 61, 61, 81, 22, 33, 43, 42, 62, 72, 72, 82, 82, 81, 0, 82, 82,
                0, 21, 11, 82, 82, 72, 72, 83, 74, 84, 22, 12, 21, 82, 63, 53, 42, 22, 22, 82, 72, 62, 62, 41, 61, 71, 71, 81, 82, 81, 81, 82,
                0, 0, 81, 81, 71, 82, 82, 72, 73, 84, 74, 84, 84, 83, 73, 64, 74, 74, 74, 63, 63, 62, 72, 81, 62, 72, 72, 72, 82, 11, 0, 82,
                0, 0, 0, 0, 0, 81, 82, 61, 82, 73, 83, 84, 74, 74, 64, 74, 74, 83, 83, 74, 84, 73, 72, 72, 81, 81, 71, 82, 71, 82, 81, 62,
                0, 0, 0, 0, 0, 0, 51, 61, 61, 72, 82, 72, 82, 74, 73, 82, 82, 72, 72, 82, 73, 83, 83, 73, 72, 82, 72, 72, 73, 83, 73, 82,
                0, 0, 0, 0, 0, 0, 61, 62, 72, 72, 72, 81, 72, 82, 62, 81, 81, 81, 71, 71, 72, 73, 82, 62, 63, 83, 72, 72, 63, 74, 85, 83,
                0, 0, 0, 0, 0, 0, 52, 63, 73, 83, 83, 82, 81, 71, 81, 11, 21, 81, 0, 0, 0, 0, 41, 71, 71, 61, 61, 61, 61, 0, 0, 85,
                0, 0, 0, 0, 0, 0, 52, 53, 62, 82, 13, 12, 62, 72, 82, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 82,
                0, 0, 0, 0, 0, 0, 42, 43, 42, 32, 23, 23, 52, 0, 23, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 41, 43, 33, 33, 23, 32, 43, 33, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0, 0, 0, 21, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
            };
            //var box = new BoundingBox(4.635f, -4.72f, 4.1672f, -1.9784f);
            var box = new BoundingBox(139f, -141.5f, -59.5f, 124.8f);
            var defaultDataMap = new serializable_CurrentsMapData(box, 32, 21, bufferArray);

            string filename = "CurrentsMapData.json";
            Debug.Log(defaultDataMap.bufferArray);
            SerializeJSON(defaultDataMap, filename);
        }

        public static CurrentsMapData Test()
        {
            string path = "CurrentsMapData.json";
            var currentsMap = LoadJson<serializable_CurrentsMapData>(path);

            return new CurrentsMapData(currentsMap.boundingBox, currentsMap.tilesX, currentsMap.tilesZ, currentsMap.bufferArray);
        }

        public static CurrentsMapData LoadCurrentsMap(string path)
        {
            var currentsMap = LoadJson<serializable_CurrentsMapData>(path);
            return new CurrentsMapData(currentsMap.boundingBox, currentsMap.tilesX, currentsMap.tilesZ, currentsMap.bufferArray);
        }

        //--------------------------------
        // DATOS INICIALES DE PARTIDA (CIUDADES Y REINOS)
        //--------------------------------

        /// <summary>
        /// Función encargada de generar un json con datos iniciales del mundo para una partida.
        /// Esta función se debe utilizar como desarrollador o para modding
        /// </summary>
        // public static void GenerateStartGameJson(StartGameData startGameData)
        // {
        //     var startGameJSONFormat = new StartGameJSONFormat(startGameData);
        //     startGameJSONFormat.WorldData("worldData_Campaign_1720");
        // }

        /// <summary>
        /// Función encargada de generar un binaryfile con datos iniciales del mundo para una 
        /// partida. Esta función se debe utilizar como desarrollador o para modding
        /// </summary>
        // public static void GenerateStartGameBin(StartGameData startGameData)
        // {
        //     var startGameBinaryFormat = new StartGameBinaryFormat(startGameData);
        //     startGameBinaryFormat.WorldData("worldData_Campaign_1720");
        // }

        public static Transform[] LoadCitiesDataBin(string fileName)
        {
            var loaderBinaryFormat = new CitiesLoaderBinaryFormat();
            StartGameData CampaignCitiesData = loaderBinaryFormat.LoadWorldData(fileName);
            Debug.Log(CampaignCitiesData);
            Debug.Log(CampaignCitiesData.kingdoms);
            //Reinos
            var kingdomsContainer = new GameObject();
            kingdomsContainer.name = "Countries";
            kingdomsContainer.tag = "Kingdoms";
            kingdomsContainer.SetActive(false);

            var banners = new GameObject().transform;
            banners.gameObject.SetActive(false);

            SerializableKingdom[] kingdoms = CampaignCitiesData.kingdoms;

            for (int i = 0; i < kingdoms.Length; ++i)
            {
                SerializableKingdom k = kingdoms[i];
            }

            //Keypoints:
            var kpsContainer = new GameObject();
            kpsContainer.name = "World KeyPoints -- holi :3";
            kpsContainer.tag = "WorldPlaces";
            kpsContainer.SetActive(false);

            //Puertos naturales
            var naturalPorts = CampaignCitiesData.naturalPorts;
            var naturalPortsContainer = new GameObject();
            naturalPortsContainer.transform.parent = kpsContainer.transform;
            naturalPortsContainer.name = "Natural Docks";

            //Banners
            var naturalPortsBanners = new GameObject();
            naturalPortsBanners.name = "Natural piers";
            naturalPortsBanners.transform.SetParent(banners);
            for (int i = 0; i < naturalPorts.Length; ++i)
            {
                SerializableNaturalPort port = naturalPorts[i];
                MB_NaturalPort serializedPort = port.Deserialize(naturalPortsContainer.transform);
                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            //Escondites de contrabando
            var hideouts = CampaignCitiesData.hideouts;
            var hideOutsContainer = new GameObject();
            hideOutsContainer.transform.parent = kpsContainer.transform;
            hideOutsContainer.name = "Smuggglers Posts";
            hideOutsContainer.tag = "Pirate";

            //Banners
            var hideOutsBanners = new GameObject();
            hideOutsBanners.name = "Smuggglers hideout";
            hideOutsBanners.transform.SetParent(banners);
            for (int i = 0; i < hideouts.Length; ++i)
            {
                SerializableSmugglersPost port = hideouts[i];
                MB_SmugglersPost serializedPort = port.Deserialize(hideOutsContainer.transform);

                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            var shelters = CampaignCitiesData.shelters;
            var sheltersContainer = new GameObject();
            sheltersContainer.transform.parent = kpsContainer.transform;
            sheltersContainer.name = "Pirate Shelters";
            sheltersContainer.tag = "Pirate";

            var sheltersBanners = new GameObject();
            sheltersBanners.name = "Pirate Shelters";
            sheltersBanners.transform.SetParent(banners);
            for (int i = 0; i < shelters.Length; ++i)
            {
                SerializablePirateShelter port = shelters[i];
                MB_PirateShelter serializedPort = port.Deserialize(sheltersContainer.transform);

                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            return new Transform[3] { kingdomsContainer.transform, kpsContainer.transform, banners };
        }

        //todo: refactorizar esto (usar un parámetro genérico <T> en lugar de duplicar la función)
        public static Transform[] LoadCitiesDataJson(string fileName)
        {
            var jsonFormatter = new CitiesLoaderJsonFormat();
            StartGameData CampaignCitiesData = jsonFormatter.LoadWorldData(fileName);

            //Reinos
            var kingdomsContainer = new GameObject();
            kingdomsContainer.name = "Countries";
            kingdomsContainer.tag = "Kingdoms";
            kingdomsContainer.SetActive(false);

            var banners = new GameObject().transform;
            banners.gameObject.SetActive(false);

            SerializableKingdom[] kingdoms = CampaignCitiesData.kingdoms;

            for (int i = 0; i < kingdoms.Length; ++i)
            {
                SerializableKingdom k = kingdoms[i];
            }

            //Keypoints:
            var kpsContainer = new GameObject();
            kpsContainer.name = "World KeyPoints -- holi :3";
            kpsContainer.tag = "WorldPlaces";
            kpsContainer.SetActive(false);

            //Puertos naturales
            var naturalPorts = CampaignCitiesData.naturalPorts;
            var naturalPortsContainer = new GameObject();
            naturalPortsContainer.transform.parent = kpsContainer.transform;
            naturalPortsContainer.name = "Natural Docks";

            //Banners
            var naturalPortsBanners = new GameObject();
            naturalPortsBanners.name = "Natural piers";
            naturalPortsBanners.transform.SetParent(banners);
            for (int i = 0; i < naturalPorts.Length; ++i)
            {
                SerializableNaturalPort port = naturalPorts[i];
                MB_NaturalPort serializedPort = port.Deserialize(naturalPortsContainer.transform);
                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            //Escondites de contrabando
            var hideouts = CampaignCitiesData.hideouts;
            var hideOutsContainer = new GameObject();
            hideOutsContainer.transform.parent = kpsContainer.transform;
            hideOutsContainer.name = "Smuggglers Posts";
            hideOutsContainer.tag = "Pirate";

            //Banners
            var hideOutsBanners = new GameObject();
            hideOutsBanners.name = "Smuggglers hideout";
            hideOutsBanners.transform.SetParent(banners);
            for (int i = 0; i < hideouts.Length; ++i)
            {
                SerializableSmugglersPost port = hideouts[i];
                MB_SmugglersPost serializedPort = port.Deserialize(hideOutsContainer.transform);

                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            var shelters = CampaignCitiesData.shelters;
            var sheltersContainer = new GameObject();
            sheltersContainer.transform.parent = kpsContainer.transform;
            sheltersContainer.name = "Pirate Shelters";
            sheltersContainer.tag = "Pirate";

            var sheltersBanners = new GameObject();
            sheltersBanners.name = "Pirate Shelters";
            sheltersBanners.transform.SetParent(banners);
            for (int i = 0; i < shelters.Length; ++i)
            {
                SerializablePirateShelter port = shelters[i];
                MB_PirateShelter serializedPort = port.Deserialize(sheltersContainer.transform);

                //Asigna un banner
                Transform bannerTransform = serializedPort.GetKeyPointBanner(naturalPortsBanners.transform).transform;
            }

            return new Transform[3] { kingdomsContainer.transform, kpsContainer.transform, banners };
        }
    }

    public class serializable_CurrentsMapData
    {
        public BoundingBox boundingBox;
        public int tilesX, tilesZ;
        public int[] bufferArray;

        public serializable_CurrentsMapData(BoundingBox boundingBox, int tilesX, int tilesZ, int[] bufferArray)
        {
            this.boundingBox = boundingBox;
            this.tilesX = tilesX;
            this.tilesZ = tilesZ;
            this.bufferArray = bufferArray;
        }
    }
}

