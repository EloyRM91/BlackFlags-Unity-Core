using System.Collections.Generic; using UnityEngine;
using System.Linq;

//BBDD
using Mono.Data.Sqlite;
using System.Data;
using Generation.Ships;
//Game Mechanics
using GameMechanics.Data;
using GameMechanics.Ships;
using GameSettings.Core;

namespace Generation.Generators
{
public class WorldGenerator
    {
        #region VARIABLES
        // SQ
        private static IDataReader reader;
        private static IDbConnection dbcon;
        //Nombres de embarcaciones
        private static ShipList
            _names_Merchants_SPAIN = new ShipList(),
            _names_Merchants_PORTUGAL = new ShipList(),
            _names_Merchants_FRANCE = new ShipList(),
            _names_Merchants_DUTCH = new ShipList(),
            _names_Merchants_BRITAIN = new ShipList(),
            _names_Corsairs_SPAIN = new ShipList(),
            _names_Corsairs_PORTUGAL = new ShipList(),
            _names_Corsairs_FRANCE = new ShipList(),
            _names_Corsairs_DUTCH = new ShipList(),
            _names_Corsairs_BRITAIN = new ShipList(),
            _names_Military_SPAIN = new ShipList(),
            _names_Military_PORTUGAL = new ShipList(),
            _names_Military_FRANCE = new ShipList(),
            _names_Military_DUTCH = new ShipList(),
            _names_Military_BRITAIN = new ShipList(),
            _names_Pirate_SPAIN = new ShipList(),
            _names_Pirate_PORTUGAL = new ShipList(),
            _names_Pirate_FRANCE = new ShipList(),
            _names_Pirate_DUTCH = new ShipList(),
            _names_Pirate_BRITAIN = new ShipList();
        //Nombres y apellidos de personajes NPC
        private static List<string>[]
            _names_NPC_SPAIN = new List<string>[2] { new List<string>(), new List<string>()},
            _names_NPC_PORTUGAL = new List<string>[2] { new List<string>(), new List<string>() },
            _names_NPC_FRANCE = new List<string>[2] { new List<string>(), new List<string>() },
            _names_NPC_DUTCH = new List<string>[2] { new List<string>(), new List<string>() },
            _names_NPC_BRITAIN = new List<string>[2] { new List<string>(), new List<string>() },
            _overNames = new List<string>[2] { new List<string>(), new List<string>() };
        #endregion

        #region DB Loading
        public static void Initialize()
        {
            var modding = PersistentGameSettings.currentMod != null;
            GetShipNamesLists(modding);
            GetCharacterNamesLists(modding);
        }
        private static IDbConnection OpenDB(string dir)
        {
            IDbConnection thisdBConn = new SqliteConnection(dir); // Establece conexión con la bbdd
            thisdBConn.Open();
            return thisdBConn;
        }
        private static IDataReader RunReader(string tableName)
        {
            IDbCommand cmdRead = dbcon.CreateCommand();
            cmdRead.CommandText = "SELECT * FROM " + tableName; // query
            IDataReader reader = cmdRead.ExecuteReader();
            return reader;
        }

        /// <summary>
        /// Get the complete list of names from "DB_ShipNames.db", reading all the dataase file tables"
        /// </summary>
        private static void GetShipNamesLists(bool isMod = false)
        {
            //Refactorizar: doble array [país][rol]
            string str;
            var path = "URI=file:" + (isMod ?
                PersistentGameSettings.currentMod.ModStreaming 
                : Application.streamingAssetsPath + "/");
            var lists = new ShipList[] {
                _names_Merchants_SPAIN,
                _names_Merchants_PORTUGAL,
                _names_Merchants_FRANCE,
                _names_Merchants_DUTCH,
                _names_Merchants_BRITAIN,
                _names_Corsairs_SPAIN,
                _names_Corsairs_PORTUGAL,
                _names_Corsairs_FRANCE,
                _names_Corsairs_DUTCH,
                _names_Corsairs_BRITAIN,
                _names_Military_SPAIN,
                _names_Military_PORTUGAL,
                _names_Military_FRANCE,
                _names_Military_DUTCH,
                _names_Military_BRITAIN,
                _names_Pirate_SPAIN,
                _names_Pirate_PORTUGAL,
                _names_Pirate_FRANCE,
                _names_Pirate_DUTCH,
                _names_Pirate_BRITAIN
            };
            //Database tables
            var tables = new string[] {
                "t_Merchants_SPAIN",
                "t_Merchants_PORTUGAL",
                "t_Merchants_FRANCE",
                "t_Merchants_DUTCH",
                "t_Merchants_BRITAIN",
                "t_Corsairs_SPAIN",
                "t_Corsairs_PORTUGAL",
                "t_Corsairs_FRANCE",
                "t_Corsairs_DUTCH",
                "t_Corsairs_BRITAIN",
                "t_Military_SPAIN",
                "t_Military_PORTUGAL",
                "t_Military_FRANCE",
                "t_Military_DUTCH",
                "t_Military_BRITAIN",
                "t_Pirate_SPAIN",
                "t_Pirate_PORTUGAL",
                "t_Pirate_FRANCE",
                "t_Pirate_DUTCH",
                "t_Pirate_BRITAIN"
            };
#if UNITY_EDITOR
            path = "URI=file:C:/users/" + System.Environment.UserName + "/Desktop/";
#endif
            //Open database
            dbcon = OpenDB(path + "DB_ShipNames.db");
            for (int i = 0; i < lists.Length; i++)
            {
                reader = RunReader(tables[i]);
                while (reader.Read())
                {
                    str = reader[0].ToString();
                    if (str != "") lists[i].Add(str);
                }
            }
            dbcon.Close();
        }

        /// <summary>
        /// Get the spawn probabilities table por each kingdom and each unit by means of a give table index.
        /// For example: Ratio of light fighters for a british patrol: 60%. Ratio of Brigantines (a light fighter) 
        /// for british patrols: 50%.
        /// Total probability for spawning a brigantine in a british patrol: 30%
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static ShipGenerator GetShipSpawnData(int index, bool isMod = false)
        {
            var statistics = new List<List<float[]>>();
            var spawnFactors = new List<float[]>();
            var path = "URI=file:" + (isMod ?
                PersistentGameSettings.currentMod.ModStreaming
                : Application.streamingAssetsPath + "/");
#if UNITY_EDITOR
            path = "URI=file:C:/users/" + System.Environment.UserName + "/Desktop/";
#endif
            //Database tables
            var tables = new string[]
            {
                "t_SPAIN",
                "t_PORTUGAL",
                "t_FRANCE",
                "t_DUTCH",
                "t_BRITAIN"
            };
            //Open database
            dbcon = OpenDB(path + "DB_ShipSpawns.db");
            reader = RunReader(tables[index]);
            while (reader.Read())
            {
                statistics.Add(DataSplit(reader[0].ToString()));
                spawnFactors.Add(System.Array.ConvertAll(reader[1].ToString().Split(' '), s => float.Parse(s)));
            }
            dbcon.Close();
            return new ShipGenerator(statistics, spawnFactors);
        }

        /// <summary>
        /// Get the spawn probabilities table por each kingdom and each unit by means of a given table name.
        /// For example: Ratio of light fighters for a british patrol: 60%. Ratio of Brigantines for british patrols: 50%
        /// Total probability for spawning a brigantine in a british patrol: 30%
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ShipGenerator GetShipSpawnData(string tableName, bool isMod = false)
        {
            var statistics = new List<List<float[]>>();
            var spawnFactors = new List<float[]>();
            var path = "URI=file:" + (isMod ?
                PersistentGameSettings.currentMod.ModStreaming
                : Application.streamingAssetsPath + "/");
#if UNITY_EDITOR
            path = "URI=file:C:/users/" + System.Environment.UserName + "/Desktop/";
#endif
            //Open dataase
            dbcon = OpenDB(path + "DB_ShipSpawns.db");
            reader = RunReader(tableName);
            while (reader.Read())
            {
                statistics.Add(DataSplit(reader[0].ToString()));
                spawnFactors.Add(System.Array.ConvertAll(reader[1].ToString().Split(' '), s => float.Parse(s)));
            }
            dbcon.Close();
            return new ShipGenerator(statistics, spawnFactors);
        }

        /// <summary>
        /// Get the complete list of character names names from "DB_CharacterNames.db", reading all the dataase file tables"
        /// </summary>
        public static void GetCharacterNamesLists(bool isMod)
        {
            string str;
            var path = "URI=file:" + (isMod ?
                PersistentGameSettings.currentMod.ModStreaming 
                : Application.streamingAssetsPath + "/");
            var lists = new List<string>[][]
            {
                _names_NPC_SPAIN,
                _names_NPC_PORTUGAL,
                _names_NPC_FRANCE,
                _names_NPC_DUTCH,
                _names_NPC_BRITAIN,
                _overNames
            };
            //Database tables
            var tables = new string[]
            {
                "t_SPAIN",
                "t_PORTUGAL",
                "t_FRANCE",
                "t_DUTCH",
                "t_BRITAIN",
                "t_Nicknames"
            };
#if UNITY_EDITOR
            path = "URI=file:C:/users/" + System.Environment.UserName + "/Desktop/";
#endif
            dbcon = OpenDB(path + "DB_CharacterNames.db");
            for (int i = 0; i < lists.Length; i++)
            {
                reader = RunReader(tables[i]);
                
                while (reader.Read())
                {
                    str = reader[0].ToString();
                    if (str != "") lists[i][0].Add(str);
                    str = reader[1].ToString();
                    if (str != "") lists[i][1].Add(str);
                }
            }
            dbcon.Close();
        }


        #endregion

        //_________________________________________________________

        #region TASKS

        private static List<float[]> DataSplit(string txt)
        {
            List<float[]> result = new List<float[]>();
            var l = ((string[])txt.Split('-'));
            for (int i = 0; i < l.Length; i++)
            {
                var a = System.Array.ConvertAll(l[i].Split(' '), s => float.Parse(s));
                result.Add(a);
            }
            return result;
        }

        #endregion

        //_________________________________________________________

        #region USING DATA
        public static string GiveShipName(EntityType_KINGDOM kingdom, ShipType_ROLE role, GenerationMode mode = GenerationMode.Sequence)
        {
            ShipList list = new ShipList();
            string pref = "";
            //int val;
            switch (kingdom)
            {
                case EntityType_KINGDOM.KINGDOM_Spain:
                    switch (role)
                    {
                        case ShipType_ROLE.Corsair:
                            list = _names_Corsairs_SPAIN;
                            break;
                        case ShipType_ROLE.Patrol:
                            list = Random.Range(0, 10) == 0 ? _names_Merchants_SPAIN : _names_Corsairs_SPAIN;
                            break;
                        case ShipType_ROLE.Military:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_SPAIN : _names_Military_SPAIN;
                            break;
                        case ShipType_ROLE.Pirate:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_SPAIN : _names_Pirate_SPAIN;
                            break;
                        default: 
                            list = _names_Merchants_SPAIN;
                            break;

                    }
                    break;
                case EntityType_KINGDOM.KINGDOM_Portugal:
                    switch (role)
                    {
                        case ShipType_ROLE.Corsair:
                            list = _names_Corsairs_PORTUGAL;
                            break;
                        case ShipType_ROLE.Patrol:
                            list = Random.Range(0, 10) == 0 ? _names_Merchants_PORTUGAL : _names_Corsairs_PORTUGAL;
                            break;
                        case ShipType_ROLE.Military:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_PORTUGAL : _names_Military_PORTUGAL;
                            break;
                        case ShipType_ROLE.Pirate:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_PORTUGAL : _names_Pirate_PORTUGAL;
                            break;
                        default:
                            list = _names_Merchants_PORTUGAL;
                            break;
                    }
                    break;
                case EntityType_KINGDOM.KINGDOM_France:
                    switch (role)
                    {
                        case ShipType_ROLE.Corsair:
                            list = _names_Corsairs_FRANCE;
                            break;
                        case ShipType_ROLE.Patrol:
                            list = Random.Range(0, 10) == 0 ? _names_Merchants_FRANCE : _names_Corsairs_FRANCE;
                            break;
                        case ShipType_ROLE.Military:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_FRANCE : _names_Military_FRANCE;
                            break;
                        case ShipType_ROLE.Pirate:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_FRANCE : _names_Pirate_FRANCE;
                            break;
                        default:
                            list = _names_Merchants_FRANCE;
                            break;
                    }
                    break;
                case EntityType_KINGDOM.KINGDOM_Dutch:
                    switch (role)
                    {
                        case ShipType_ROLE.LocalMerchant:
                            list = _names_Merchants_DUTCH;
                            break;
                        case ShipType_ROLE.Merchant:
                            pref = "VOC ";
                            list = _names_Merchants_DUTCH;
                            break;
                        case ShipType_ROLE.Corsair:
                            list = _names_Corsairs_DUTCH;
                            break;
                        case ShipType_ROLE.Patrol:
                            list = Random.Range(0, 10) == 0 ? _names_Merchants_DUTCH : _names_Corsairs_DUTCH;
                            break;
                        case ShipType_ROLE.Military:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_DUTCH : _names_Military_DUTCH;
                            break;
                        case ShipType_ROLE.Pirate:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_DUTCH : _names_Pirate_DUTCH;
                            break;
                    }
                    break;
                case EntityType_KINGDOM.KINGDOM_Britain:
                    switch (role)
                    {
                        case ShipType_ROLE.LocalMerchant:
                            list = _names_Merchants_BRITAIN;
                            break;
                        case ShipType_ROLE.Merchant:
                            pref = "HMS ";
                            list = _names_Merchants_BRITAIN;
                            break;
                        case ShipType_ROLE.Corsair:
                            list = _names_Corsairs_BRITAIN;
                            break;
                        case ShipType_ROLE.Patrol:
                            pref = "HMS ";
                            list = Random.Range(0, 10) == 0 ? _names_Merchants_BRITAIN : _names_Corsairs_BRITAIN;
                            break;
                        case ShipType_ROLE.Military:
                            pref = "HMS ";
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_BRITAIN : _names_Military_BRITAIN;
                            break;
                        case ShipType_ROLE.Pirate:
                            list = Random.Range(0, 10) == 0 ? _names_Corsairs_BRITAIN : _names_Pirate_BRITAIN;
                            break;
                    }
                    break;
            }
            return pref + list.GetName(mode);
        }

        /// <summary>
        /// ShipList class: This class is a list of strings with an extended functionality
        /// </summary>
        private class ShipList: List<string>
        {
            public int counterIndex = 0;
            public string GetName(GenerationMode mode = GenerationMode.Sequence)
            {
                if (mode == GenerationMode.Random) return (this[Random.Range(0, this.Count)]);
                var n = this[counterIndex];
                counterIndex = counterIndex >= this.Count - 1 ? 0 : counterIndex + 1;
                return n;
            }
        }

        public static string GetCharacterName(EntityType_KINGDOM country, bool isPirate = false)
        {
            string str = string.Empty;
            switch (country)
            {
                case EntityType_KINGDOM.KINGDOM_Spain:
                    str = _names_NPC_SPAIN[0][Random.Range(0, _names_NPC_SPAIN[0].Count)] + " " + _names_NPC_SPAIN[1][Random.Range(0, _names_NPC_SPAIN[1].Count)];
                    break;
                case EntityType_KINGDOM.KINGDOM_Portugal:
                    str = _names_NPC_PORTUGAL[0][Random.Range(0, _names_NPC_PORTUGAL[0].Count)] + " " + _names_NPC_PORTUGAL[1][Random.Range(0, _names_NPC_PORTUGAL[1].Count)];
                    break;
                case EntityType_KINGDOM.KINGDOM_France:
                    str = _names_NPC_FRANCE[0][Random.Range(0, _names_NPC_FRANCE[0].Count)] + " " + _names_NPC_FRANCE[1][Random.Range(0, _names_NPC_FRANCE[1].Count)];
                    break;
                case EntityType_KINGDOM.KINGDOM_Dutch:
                    str = _names_NPC_DUTCH[0][Random.Range(0, _names_NPC_DUTCH[0].Count)] + " " + _names_NPC_DUTCH[1][Random.Range(0, _names_NPC_DUTCH[1].Count)];
                    break;
                case EntityType_KINGDOM.KINGDOM_Britain:
                    str = _names_NPC_BRITAIN[0][Random.Range(0, _names_NPC_BRITAIN[0].Count)] + " " + _names_NPC_BRITAIN[1][Random.Range(0, _names_NPC_BRITAIN[1].Count)];
                    break;
            }
            if (isPirate)
                str = Random.Range(0, 3) == 2 ? str + " " + _overNames[1][Random.Range(0, _overNames[1].Count)] : str;
            return str;     
        }

        #endregion
    }

    public enum GenerationMode { Sequence, Random }
}

namespace Generation.Ships
{
    public class ShipGenerator
    {
        private ShipSpawner_ByRole[] Generators = new ShipSpawner_ByRole[5];
        public Ship GenerateShipData(ShipType_ROLE r, EntityType_KINGDOM k)
        {
            if(r == ShipType_ROLE.Pirate)
            {
                return Generators[3].CreateShip(r, k);
            }
            return Generators[(int)r].CreateShip(r, k);
        }

        private static readonly Dictionary<ShipType_CLASS, ShipType_MODEL[]> modelsByClass = new Dictionary<ShipType_CLASS, ShipType_MODEL[]>()
        {
            { ShipType_CLASS.Raider, new ShipType_MODEL[]{ ShipType_MODEL.MODEL_Sloop, ShipType_MODEL.MODEL_Felucca, ShipType_MODEL.MODEL_Tartain } },
            { ShipType_CLASS.EarlyFighter, new ShipType_MODEL[]{ ShipType_MODEL.MODEL_Brig, ShipType_MODEL.MODEL_Lugger, ShipType_MODEL.MODEL_Polacre } },
            { ShipType_CLASS.HeavyFighter, new ShipType_MODEL[]{ ShipType_MODEL.MODEL_Corvette, ShipType_MODEL.MODEL_Frigate } },
            { ShipType_CLASS.CargoShip, new ShipType_MODEL[]{ ShipType_MODEL.MODEL_Gallion, ShipType_MODEL.MODEL_Flyboat, ShipType_MODEL.MODEL_URCA} }
        };

        private static readonly Dictionary<ShipType_ROLE, ShipType_CLASS[]> classesByRole = new Dictionary<ShipType_ROLE, ShipType_CLASS[]>()
        {
            { ShipType_ROLE.LocalMerchant, new ShipType_CLASS[]{ ShipType_CLASS.Raider, ShipType_CLASS.EarlyFighter} },
            { ShipType_ROLE.Merchant, new ShipType_CLASS[]{ ShipType_CLASS.EarlyFighter, ShipType_CLASS.HeavyFighter, ShipType_CLASS.CargoShip} },
            { ShipType_ROLE.Patrol, new ShipType_CLASS[]{ ShipType_CLASS.Raider, ShipType_CLASS.EarlyFighter } },
            { ShipType_ROLE.Corsair, new ShipType_CLASS[]{ ShipType_CLASS.Raider, ShipType_CLASS.EarlyFighter } }
        };

        public ShipGenerator(List<List<float[]>> statistics, List<float[]> f, float[] f2)
        {
            for (int i = 0; i < 4; i++) { Generators[i] = new ShipSpawner_ByRole((ShipType_ROLE)i, statistics[i], f[i], f2[i]); }
        }

        public ShipGenerator(List<List<float[]>> statistics, List<float[]> f)
        {
            for (int i = 0; i < 4; i++)
            {
                //Debug.Log($"Creating Role Spawn Controller for: {((ShipType_ROLE)i).ToString()} ");
                Generators[i] = new ShipSpawner_ByRole((ShipType_ROLE)i, statistics[i], f[i]);
            }
        }

        private abstract class Spawner
        {
            public float thisSpawnFactor;
            public Spawner[] subClassSpawners;

            /// <summary>
            /// Returns a generated ship class, according to its convoy role and nationality
            /// </summary>
            /// <param name="r"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public abstract Ship CreateShip(ShipType_ROLE r, EntityType_KINGDOM k);

            protected int GetRandomIndex(Spawner[] probabilities)
            {
                //Set Data
                var percentArray = new float[probabilities.Length];
                for (int i = 0; i < probabilities.Length; i++)
                {
                    percentArray[i] = probabilities[i].thisSpawnFactor;
                }
                //Get Random Exit Index
                float n = Random.Range(0,100);
                if (n <= percentArray[0]) return 0;
                for (int i = 1; i < percentArray.Length; i++)
                {
                    if (n <= percentArray[i] + percentArray[i - 1] ) return i;
                }
                return percentArray.Length - 1;
            }
        }
        private class ShipSpawner_ByRole : Spawner
        {
            private ShipType_ROLE role;
            public ShipSpawner_ByRole(ShipType_ROLE r, List<float[]> statistics, float[] f, float f2)
            {
                role = r;
                var classes = classesByRole[r];
                subClassSpawners = new ShipSpawner_ByClass[classes.Length];
                for (int i = 0; i < subClassSpawners.Length; i++)
                {
                    subClassSpawners[i] = new ShipSpawner_ByClass(classes[i], statistics[i], f[i]);
                }
                thisSpawnFactor = f2;
            }
            public ShipSpawner_ByRole(ShipType_ROLE r, List<float[]> statistics, float[] f)
            {
                role = r;
                var classes = classesByRole[r];
                subClassSpawners = new ShipSpawner_ByClass[classes.Length];
                for (int i = 0; i < subClassSpawners.Length; i++)
                {
                    //Debug.Log($"Calling {classes[i].ToString()} for role: {role.ToString()}");
                    subClassSpawners[i] = new ShipSpawner_ByClass(classes[i], statistics[i], f[i]);
                }
            }
            /// <summary>
            /// Returns a generated ship class, according to its convoy role and nationality
            /// </summary>
            /// <param name="r"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public override Ship CreateShip(ShipType_ROLE r, EntityType_KINGDOM k)
            {
                var i = GetRandomIndex(subClassSpawners);
                return subClassSpawners[i].CreateShip(r, k);
            }
        }
        private class ShipSpawner_ByClass : Spawner
        {
            public ShipSpawner_ByClass(ShipType_CLASS c, float[] statistics,float f)
            {
                var models = modelsByClass[c];
                subClassSpawners = new ShipSpawner_ByModel[models.Length];
                for (int i = 0; i < models.Length; i++)
                {
                    subClassSpawners[i] = new ShipSpawner_ByModel(models[i], statistics[i]);
                }
                thisSpawnFactor = f;
            }
            /// <summary>
            /// Returns a generated ship class, according to its convoy role and nationality
            /// </summary>
            /// <param name="r"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public override Ship CreateShip(ShipType_ROLE r, EntityType_KINGDOM k)
            {
                var i = GetRandomIndex(subClassSpawners);
                return subClassSpawners[i].CreateShip(r, k);
            }
        }
        private class ShipSpawner_ByModel: Spawner
        {
            public ShipType_MODEL model;

            public ShipSpawner_ByModel(ShipType_MODEL m, float f)
            {
                model = m;
                thisSpawnFactor = f;
            }
            /// <summary>
            /// Returns a generated ship class, according to its convoy role and nationality
            /// </summary>
            /// <param name="r"></param>
            /// <param name="k"></param>
            /// <returns></returns>
            public override Ship CreateShip(ShipType_ROLE r, EntityType_KINGDOM k)
            {
                //Debug.Log(model.ToString());
                switch (model)
                {
                    case ShipType_MODEL.MODEL_Sloop:
                        switch (r)
                        {
                            case ShipType_ROLE.LocalMerchant:
                                if (Random.Range(0, 7) == 0)
                                {
                                    return new ShipSubCategory_ContinentalSloop();
                                }
                                else
                                {
                                    return new ShipSubCategory_CoastalSloop();
                                }
                            case ShipType_ROLE.Corsair:
                                if(Random.Range(0,4) == 0)
                                {
                                    return new ShipSubCategory_CoastalSloop();
                                }
                                else
                                {
                                    return new ShipSubCategory_ContinentalSloop();
                                }
                            case ShipType_ROLE.Patrol:
                                if (Random.Range(0, 2) == 0)
                                {
                                    return new ShipSubCategory_MilitaryBalander();
                                }
                                else
                                {
                                    return new ShipSubCategory_ContinentalSloop();
                                }
                            case ShipType_ROLE.Pirate:
                                if (Random.Range(0, 3) == 0)
                                {
                                    return new ShipSubCategory_MilitaryBalander();
                                }
                                else
                                {
                                    return new ShipSubCategory_ContinentalSloop();
                                }
                            default:
                                return new ShipSubCategory_ContinentalSloop();
                        }
                    case ShipType_MODEL.MODEL_Felucca:
                        {
                            switch (r)
                            {
                                case ShipType_ROLE.LocalMerchant:
                                    return new ShipSubCategory_TwoMastlesFelucca();
                                case ShipType_ROLE.Corsair:
                                    if (Random.Range(0, 3) == 0)
                                    {
                                        return new ShipSubCategory_TwoMastlesFelucca();
                                    }
                                    else
                                    {
                                        return new ShipSubCategory_MisticFelucca();
                                    }
                                case ShipType_ROLE.Patrol:
                                    return new ShipSubCategory_MisticFelucca();
                                case ShipType_ROLE.Pirate:
                                    if (Random.Range(0, 3) == 0)
                                    {
                                        return new ShipSubCategory_TwoMastlesFelucca();
                                    }
                                    else
                                    {
                                        return new ShipSubCategory_MisticFelucca();
                                    }
                                default:
                                    return new ShipSubCategory_TwoMastlesFelucca();
                            }
                        }
                    case ShipType_MODEL.MODEL_Tartain:
                        {
                            switch (r)
                            {
                                case ShipType_ROLE.LocalMerchant:
                                    return new ShipSubCategory_TwoMastlesTartain();
                                case ShipType_ROLE.Corsair:
                                    if (Random.Range(0, 2) == 0)
                                    {
                                        return new ShipSubCategory_TwoMastlesTartain();
                                    }
                                    else
                                    {
                                        return new ShipSubCategory_ThreeMastlesTartain();
                                    }
                                case ShipType_ROLE.Patrol:
                                    return new ShipSubCategory_ThreeMastlesTartain();
                                case ShipType_ROLE.Pirate:
                                    return new ShipSubCategory_ThreeMastlesTartain();
                                default:
                                    return new ShipSubCategory_TwoMastlesTartain();
                            }
                        }
                    case ShipType_MODEL.MODEL_Brig:
                        switch (r)
                        {
                            case ShipType_ROLE.LocalMerchant:
                                return new ShipSubCategory_12Brig(k);
                            default:
                                if (Random.Range(0, 3) == 0)
                                {
                                    return new ShipSubCategory_16Brig(k);
                                }
                                else
                                {
                                    return new ShipSubCategory_12Brig(k);
                                }

                        }
                    case ShipType_MODEL.MODEL_Lugger:
                        switch (r)
                        {
                            case ShipType_ROLE.LocalMerchant:
                                return new ShipSubCategory_10Lugger();
                            default:
                                if (Random.Range(0, 3) == 0)
                                {
                                    return new ShipSubCategory_10Lugger();
                                }
                                else
                                {
                                    return new ShipSubCategory_14Lugger();
                                }
                        }
                    case ShipType_MODEL.MODEL_Polacre:
                        switch (r)
                        {
                            case ShipType_ROLE.LocalMerchant:
                                return new ShipSubCategory_ShoonerPolacre(k);
                            default:
                                if (Random.Range(0, 3) == 0)
                                {
                                    return new ShipSubCategory_ShoonerPolacre(k);
                                }
                                else
                                {
                                    return new ShipSubCategory_Polacre(k);
                                }
                        }
                    case ShipType_MODEL.MODEL_Corvette:
                        return new ShipSubCategory_Corvette();
                    case ShipType_MODEL.MODEL_Frigate:
                        switch (r)
                        {
                            case ShipType_ROLE.Military:
                                return new ShipSubCategory_MilitaryFrigate();
                            default:
                                return new ShipSubCategory_26Frigate();
                        }
                    case ShipType_MODEL.MODEL_Gallion:
                        switch (r)
                        {
                            case ShipType_ROLE.Military:
                                return new ShipSubCategory_Gallion();
                            default:
                                if(k == EntityType_KINGDOM.KINGDOM_Dutch)
                                {
                                    return new ShipSubCategory_DutchGallion();
                                }
                                else
                                {
                                    if (Random.Range(0, 3) == 0)
                                    {
                                        return new ShipSubCategory_LittleGallion();
                                    }
                                    else
                                    {
                                        return new ShipSubCategory_Gallion();
                                    }
                                }
                        }
                    case ShipType_MODEL.MODEL_Flyboat:
                        return new ShipSubCategory_Flyboat();
                    case ShipType_MODEL.MODEL_URCA:
                        return new ShipSubCategory_Urca();
                }
                return null;
            }
        }
    }
}
