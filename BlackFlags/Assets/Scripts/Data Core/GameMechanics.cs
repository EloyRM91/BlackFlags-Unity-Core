//Core
using UnityEngine; using System.Collections.Generic;
//Game Mechanics Data
using GameMechanics.Data;
//Generation
using Generation.Ships;

namespace GameMechanics.Data
{
    //Ship Units
    [System.Serializable] public enum ShipType_ROLE { LocalMerchant = 0, Merchant = 1, Corsair = 2, Patrol = 3, Military = 4, Pirate = 5 }
    [System.Serializable] public enum EntityType_KINGDOM { KINGDOM_Spain = 0, KINGDOM_Portugal = 1, KINGDOM_France = 2, KINGDOM_Dutch = 3, KINGDOM_Britain = 4 }
    [System.Serializable] public enum ShipType_CLASS { Raider = 0, EarlyFighter = 1, HeavyFighter = 2, CargoShip = 3, CLASS_Warship = 4 }
    [System.Serializable] public enum ShipType_MODEL { MODEL_Sloop = 0, MODEL_Felucca = 1, MODEL_Tartain = 2, MODEL_Brig = 3, MODEL_Lugger = 4, MODEL_Polacre = 5, MODEL_Corvette = 6, MODEL_Frigate = 7, MODEL_Gallion = 8, MODEL_Flyboat = 9, MODEL_URCA = 10 }

    //Currents
    [System.Serializable] public enum E_Direction8 { None = 0, North = 1, NorthEast = 2, East = 3, SouthEast = 4, South = 5, SouthWest = 6, West = 7, NorthWest = 8 }
    [System.Serializable] public enum E_CurrentDrag_TYPE { None = 0, Calm = 1, Light = 2, Moderate = 3, Strong = 4, Extreme = 5}

    [System.Serializable]
    public class BoundingBox
    {
        public float minX, maxX, minZ, maxZ;

        public BoundingBox (float minX, float maxX, float minZ, float maxZ)
        {
            this.minX = minX;
            this.maxX = maxX;
            this.minZ = minZ;
            this.maxZ = maxZ;
        }
    }

    [System.Serializable]
    public class CurrentArrow
    {
        public int direction;
        public int dragStrength;

        public CurrentArrow(int direction, int dragStrength)
        {
            //Debug.Log(direction + " -- " + dragStrength);
            this.direction = direction;
            this.dragStrength = dragStrength;
        }

        public CurrentArrow(int bufferValue)
        {
            //this.direction = bufferValue >> 3;
            //this.dragStrength = (bufferValue << 29) >> 29;

            string str = bufferValue == 0 ? "00" : bufferValue.ToString();
            this.direction = (int)char.GetNumericValue(str[0]);
            this.dragStrength = (int)char.GetNumericValue(str[1]);
        }

        public CurrentArrow(string bufferValue)
        {
            bufferValue = bufferValue == "0" ? "00" : bufferValue;
            this.direction = (int)char.GetNumericValue(bufferValue[0]);
            this.direction = (int)char.GetNumericValue(bufferValue[1]);
        }
    }

    [System.Serializable]
    public class CurrentsMapData
    {
        public Dictionary<E_CurrentDrag_TYPE, float> D_CurrentDrag_VALUES = new Dictionary<E_CurrentDrag_TYPE, float>
        {
            {E_CurrentDrag_TYPE.None, 0 }
        };

        public BoundingBox boundingBox;
        public int tilesX, tilesZ;
        public float tileSizeX, tileSizeZ;
        public CurrentArrow[,] arrows;

        public CurrentsMapData(BoundingBox boundingBox, int tilesX, int tilesZ, CurrentArrow[,] arrows)
        {
            this.boundingBox = boundingBox;
            this.tilesX = tilesX;
            this.tilesZ = tilesZ;
            this.arrows = arrows;

            tileSizeX = (this.boundingBox.maxX - this.boundingBox.minX) / tilesX;
            tileSizeZ = (this.boundingBox.maxZ - this.boundingBox.minZ) / tilesZ;
        }

        public CurrentsMapData(BoundingBox boundingBox, int tilesX, int tilesZ, int[] bufferArray)
        {
            this.boundingBox = boundingBox;
            this.tilesX = tilesX;
            this.tilesZ = tilesZ;
            this.arrows = new CurrentArrow[tilesZ, tilesX];

            tileSizeX = (this.boundingBox.maxX - this.boundingBox.minX) / tilesX;
            tileSizeZ = (this.boundingBox.maxZ - this.boundingBox.minZ) / tilesZ;

            for (int i = 0; i < tilesZ; i++)
            {
//#if UNITY_EDITOR
//                Debug.Log(bufferArray.Length);
//                Debug.Log(string.Join("",
//                    new List<int>(bufferArray)
//                    .ConvertAll(i => i.ToString())
//                    .ToArray()));
//#endif

                CurrentArrow[] row = new CurrentArrow[tilesX];
                for (int j = 0; j < tilesX; j++)
                {
                    //Debug.Log(i + ", "  + j + ":");
                    this.arrows[i, j] = new CurrentArrow(bufferArray[ i * tilesX * 2 + 2 * j], bufferArray[ i * tilesX * 2 + 2 * j + 1]);
                }
            }
        }
    }
}

namespace GameMechanics.Ships
{
    //--------------------------------
    // SHIPS
    //--------------------------------

    /// <summary>
    /// Abstract class with all the basic related ships info.
    /// Parameters: Ship Name (string), Captain Ship (string)
    /// Static Info: Characteristics (Dictionary), Improvements (Dictionary), Variants (Dictionary)
    ///  ** Constructors: None (abstract)
    /// </summary>
    [System.Serializable]
    public abstract class Ship
    {
        //Características
        protected static readonly Dictionary<string, Modifier> D_Characteristics = new Dictionary<string, Modifier>()
        {
            {"OneMastle", new Modifier("Un Mástil", "La nave es más vulnerable al desarbolado en un combate prolongado", new float[10]{ 0,0,0,0,0,0,0,0,0, -5}) },
            {"TwoMastles", new Modifier("Dos Mástiles", "La nave tiene una arboladura de trinquete y mesana", new float[10]{ 0,0,0,0,0,0,0,0,0, 10}) },
            {"ThreeMastles", new Modifier("Tres Mástiles", "Nave de tres palos, lo que permite mayor velamen", new float[10]{ 0,0,0,0,0,0,0,0,0, 15}) },
            {"BlindSail", new Modifier("Cebadera", "Una vela Cebadera reduce la visibilidad", new float[10]{ 0,0,0,0,-0.65f,0,0,0,0,0}) },
            {"Platform", new Modifier("Cofa", "Una cofa permite mayor alcance de visión", new float[10]{ 0,0,0,0,2,0,0,0,0,0}) },
            {"Quarterdeck", new Modifier("Alcázar", "Una sobrecubierta entre mayor y mesana confiere una mejor defensa", new float[10]{ 0,0,0,0,0.5f,0,0,0,10,0}) },
            {"Poopdeck", new Modifier("Toldilla", "Una sobrecubierta entre mesana y popa confiere una mejor defensa", new float[10]{ 0,0,0,0,1.5f,0,0,0,10,0}) }
        };
        //Mejoras
        protected static readonly Dictionary<string, Improvement> D_Improvements = new Dictionary<string, Improvement>()
        {
            {"Boardingnets", new Improvement("Redes de Abordaje", "Redes de soga endurecida ideadas para defenderse de un abordaje", new float[10]{ 0,0,0,0,0,0,0,0,10,0}, 3000)},
            {"Copperhull", new Improvement("Planchas de Cobre", "Un recubrimiento en la obra viva aumenta la velocidad, pero también el mantenimiento", new float[10]{ .2f,.8f,.2f,.8f,0,0,0,90,0,0}, 35000) },
            {"Overblindsail", new Improvement("Sobrecebadera", "Una verga montada en bauprés sobre la cebadera, que mejora la velocidad a barlovento, pero afecta al coste de mantenimiento y la visibilidad", new float[10]{ 0,0,.2f,.4f,-0.3f,1,0,15,0,0}, 4500) },
            {"Boatswainlocker", new Improvement("Pañol de Contramaestre", "Redistribuyendo el espacio en el sollado se puede preparar un espacio del barco para herramientas y mantenimiento", new float[10]{ 0,0,0,0,0,0,0,-10,0,0},7000) },
            {"Staysail", new Improvement("Estay de Mesana", "Un estay añade más superficie vélica al barco", new float[10]{ 0,.1f,.2f,.3f,0,1,0,10,0,5},3500) },
            {"Flyingjib", new Improvement("Foque volante", "Un foque adicional añade más superficie de velamen", new float[10]{ 0,.1f,.2f,.3f,-0.1f,1,0,10,0,5},3500) },
            {"Foremaststay", new Improvement("Estay de Trinquete", "Un estay añade más superficie vélica al barco", new float[10]{ 0,.1f,.2f,.3f,0,1,0,10,0,5},3000) },
            {"Lateenyard", new Improvement("Segunda Entena", "Una entena corta de reemplazo, ideada para rizar con mal tiempo y sustituir a la principal", new float[10]{ 0,0,0,0,0,1,0,-5,0,6},2700) },
        };
        //Variantes
        protected static readonly Dictionary<string, Improvement> D_Variants = new Dictionary<string, Improvement>()
        {
            {"Roundbrig", new Improvement("Bergantín Redondo", "Un tipo de bergantín en el que la vela mayor es la redonda montada sobre trinquete, y que atrapa más el barlovento",new float[10]{ -0.3f,-0.4f,0.3f,0.5f,0,-2,-4,20,0,0},5500) },
            {"Snowbrig", new Improvement("Bergantín de Esnón", "Un tipo de bergantín que porta un mástil de esnón, lo que facilita las maniobras de la vela cangreja", new float[10]{ 0,0,0.2f,0.3f,0,-1,5,15,0,9}, 9900) },
            {"Xebec", new Improvement("Jabeque polacra", "Polacra con vela de jabeque en trinquete, muy utilizado por piratas del Mediterráneo", new float[10]{ 0.4f,0.4f,-0.4f,-0.6f,0,-2,13,-10,0,0},4100) },
            {"Barque", new Improvement("Bribarca", "Barco de tres palos, similar a la fragata, con velas redondas en los dos primeros mástiles", new float[10]{ 0.5f,0.5f,-0.4f,-0.8f,0,-1,8,-10,0,0},4000) },
        };

        //Common data for all ship classes
        public string name_Ship, name_Captain;
        public static readonly float
            visibility_BaseValue = 3,
            sailsHealth_BaseValue = 40,
            boardingDefence = 100;

        //Virtual methods: Basic Data
        public virtual List<Modifier> GetCharacteristics() { return null; }
        public virtual List<Improvement> GetVariants() { return null; }
        public virtual string GetVariantName() { return null; }
        public virtual string GetModelName() { return null; }
        public virtual string GetSubClassName() { return null; }

        //Virtual methods: Parameters
        public virtual float GetMinUpwindSpeed() { return 0; }
        public virtual float GetMaxUpwindSpeed() { return 0; }
        public virtual float GetMinSmoothSpeed() { return 0; }
        public virtual float GetMaxSmoothSpeed() { return 0; }
        public virtual float GetManeuverability() { return 0; }
        public virtual int[] GetPowerCapacity() { return null; }
        public virtual int[] GetCrewData() { return null; }
        public virtual int GetManteinance() { return 0; }
        public virtual int GetCapacity() { return 0; }
        public virtual int GetFirePower() { return 0; }
        public virtual float GetVisibility() { return visibility_BaseValue; }
        public virtual byte GetSpriteIndex() { return 0; }
        public virtual int GetShellPoints() { return 0; }

        //External calls:
        public static string GetCompleteName(Ship shipmodel) 
        {
            var str = shipmodel.GetVariantName() == null ? shipmodel.GetSubClassName() : shipmodel.GetVariantName();
            return str;
        }
    }

    //--------------------------------
    //SHIP CLASSES
    //--------------------------------
    #region SHIP MAIN CLASSES
    /// <summary>
    /// Raider / Embarcación menor: A minor group of ships with several common parameters
    /// Parameters: Ship Type Class (ShipType_CLASS), Stability(float[]), Maneuverability (float[])
    ///  ** Constructors: None (abtract)
    /// </summary>
    public abstract class ShipClass_Raider : Ship
    {
        public static readonly ShipType_CLASS sclass = ShipType_CLASS.Raider;
        private static readonly float[] weatherStability = new float[2] { -15, -40 };
        private static readonly float[] weatherManeuverability = new float[3] {0, 0, -15 };
    }
    /// <summary>
    /// Early Fighter / Escolta ligero: A minor group of ships with several common parameters
    /// Parameters: Ship Type Class (ShipType_CLASS), Stability(float[]), Maneuverability (float[])
    ///  ** Constructors: None (abtract)
    /// </summary>
    public abstract class ShipClass_EarlyFighter : Ship
    {
        public static readonly ShipType_CLASS sclass = ShipType_CLASS.EarlyFighter;
        private static readonly float[] weatherStability = new float[2] { -5, -30 };
        private static readonly float[] weatherManeuverability = new float[3] { 0, 0, -10 };
    }
    /// <summary>
    /// Heavy Fighter / Barco de dos puentes: A minor group of ships with several common parameters
    /// Parameters: Ship Type Class (ShipType_CLASS), Stability(float[]), Maneuverability (float[])
    ///  ** Constructors: None (abtract)
    /// </summary>
    public abstract class ShipClass_HeavyFighter : Ship
    {
        public static readonly ShipType_CLASS sclass = ShipType_CLASS.HeavyFighter;
        private static readonly float[] weatherStability = new float[2] { -5, -10 };
        private static readonly float[] weatherManeuverability = new float[3] { -10, 0, -10 };
    }
    /// <summary>
    /// Cargo Ships / Cargueros: A minor group of ships with several common parameters
    /// Parameters: Ship Type Class (ShipType_CLASS), Stability(float[]), Maneuverability (float[])
    ///  ** Constructors: None (abtract)
    /// </summary>
    public abstract class ShipClass_CargoShip : Ship
    {
        public static readonly ShipType_CLASS sclass = ShipType_CLASS.CargoShip;
        private static readonly float[] weatherStability = new float[2] { -10, -50 };
        private static readonly float[] weatherManeuverability = new float[3] { -25, 0, -15 };
    }

    #endregion

    //--------------------------------
    //SHIP MODELS
    //--------------------------------
    #region SHIP MODELS
    /// <summary>
    /// Sloop / Balandro: A subfamily of raider ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Sloop : ShipClass_Raider
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Sloop;
        private static readonly string name_Model = "Balandro";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 9;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 12;
        private static readonly float maneuverability = 100;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Flyingjib"], D_Improvements["Foremaststay"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }

    }
    /// <summary>
    /// Tartain / Tartana: A subfamily of raider ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Tartain : ShipClass_Raider
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Tartain;
        private static readonly string name_Model = "Tartana";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 8f;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 11f;
        private static readonly float maneuverability = 85;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() {  };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Felucca / Falucho: A subfamily of raider ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Felucca : ShipClass_Raider
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Felucca;
        private static readonly string name_Model = "Falucho";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 8;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 11;
        private static readonly float maneuverability = 90;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Lateenyard"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Brigantine / Bergantín: A subfamily of early fighter ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Brigantine : ShipClass_EarlyFighter
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Brig;
        private static readonly string name_Model = "Bergantín";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 7;
        private static readonly float speed_SmoothwindMIN = 4;
        private static readonly float speed_SmoothwindMAX = 10;
        private static readonly float maneuverability = 80;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Boardingnets"], D_Improvements["Staysail"] };
        private static List<Improvement> variants = new List<Improvement>() { D_Variants["Roundbrig"], D_Variants["Snowbrig"] };
        public override List<Improvement> GetVariants() { return variants; }

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Lugger / Lugre: A subfamily of early fighter ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Lugger : ShipClass_EarlyFighter
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Lugger;
        private static readonly string name_Model = "Lugre";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 7;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 9;
        private static readonly float maneuverability = 80;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Boardingnets"], D_Improvements["Boatswainlocker"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
        //public float GetVisibility() { return visibility; }
    }
    /// <summary>
    /// Polacre /Polacra: A subfamily of early fighter ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Polacre : ShipClass_EarlyFighter
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Polacre;
        private static readonly string name_Model = "Polacra";
        private static readonly float speed_UpwindMIN = 2;
        private static readonly float speed_UpwindMAX = 8;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 9;
        private static readonly float maneuverability = 80;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Lateenyard"] };
        private static List<Improvement> variants = new List<Improvement>() { D_Variants["Xebec"] };
        public override List<Improvement> GetVariants() { return variants; }

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Frigate / Fragata: A subfamily of heavy ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Frigate : ShipClass_HeavyFighter
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Frigate;
        private static readonly string name_Model = "Fragata";
        private static readonly float speed_UpwindMIN = 1;
        private static readonly float speed_UpwindMAX = 5;
        private static readonly float speed_SmoothwindMIN = 2;
        private static readonly float speed_SmoothwindMAX = 8;
        private static readonly float maneuverability = 65;

        //Improvements & Variants
        protected static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Boardingnets"], D_Improvements["Boatswainlocker"], D_Improvements["Copperhull"], D_Improvements["Staysail"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
        //public float GetVisibility() { return visibility; }
    }
    /// <summary>
    /// Corvette / Corbeta: A subfamily of heavy ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Corvette : ShipClass_HeavyFighter
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Corvette;
        private static readonly string name_Model = "Corbeta";
        private static readonly float speed_UpwindMIN = 1;
        private static readonly float speed_UpwindMAX = 6;
        private static readonly float speed_SmoothwindMIN = 3;
        private static readonly float speed_SmoothwindMAX = 9;
        private static readonly float maneuverability = 75;

        //Improvements & Variants
        private static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Boardingnets"], D_Improvements["Boatswainlocker"], D_Improvements["Copperhull"]};
        private static List<Improvement> variants = new List<Improvement>() { D_Variants["Barque"] };
        public override List<Improvement> GetVariants() { return variants; }

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Gallion / Galeón: A subfamily of cargo ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Gallion : ShipClass_CargoShip
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Gallion;
        private static readonly string name_Model = "Galeón";
        private static readonly float speed_UpwindMIN = 0;
        private static readonly float speed_UpwindMAX = 3;
        private static readonly float speed_SmoothwindMIN = 1;
        private static readonly float speed_SmoothwindMAX = 7;
        private static readonly float maneuverability = 45;

        //Improvements & Variants
        private static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Overblindsail"], D_Improvements["Boatswainlocker"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    /// <summary>
    /// Gallion / Filibote: A subfamily of cargo ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Flyboat : ShipClass_CargoShip
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_Flyboat;
        private static readonly string name_Model = "Filibote";
        private static readonly float speed_UpwindMIN = 1;
        private static readonly float speed_UpwindMAX = 3;
        private static readonly float speed_SmoothwindMIN = 1;
        private static readonly float speed_SmoothwindMAX = 6;
        private static readonly float maneuverability = 40;

        //Improvements & Variants
        private static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Overblindsail"], D_Improvements["Boatswainlocker"] };

        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
        //public float GetVisibility() { return visibility; }
    }
    /// <summary>
    /// Urca: A subfamily of cargo ships with common characteristics
    /// Parameters: Model type (Shiptype_MODEL), Model Name (string), Speed (float) and Base Maneuverability (float), Improvements (List)
    ///  ** Constructors: Empty
    /// </summary>
    public class ShipModel_Urca : ShipClass_CargoShip
    {
        //THIS MODEL BASIC DATA
        public static readonly ShipType_MODEL model = ShipType_MODEL.MODEL_URCA;
        private static readonly string name_Model = "Urca";
        private static readonly float speed_UpwindMIN = 1;
        private static readonly float speed_UpwindMAX = 3;
        private static readonly float speed_SmoothwindMIN = 2;
        private static readonly float speed_SmoothwindMAX = 8;
        private static readonly float maneuverability = 55;

        //Improvements & Variants
        private static List<Improvement> improvements = new List<Improvement>() { D_Improvements["Boardingnets"], D_Improvements["Boatswainlocker"], D_Improvements["Copperhull"] };
        //As describable:
        public override string GetModelName() { return name_Model; }

        //As mobile entity:
        public override float GetMinUpwindSpeed() { return speed_UpwindMIN; }
        public override float GetMaxUpwindSpeed() { return speed_UpwindMAX; }
        public override float GetMinSmoothSpeed() { return speed_SmoothwindMIN; }
        public override float GetMaxSmoothSpeed() { return speed_SmoothwindMAX; }
        public override float GetManeuverability() { return maneuverability; }
    }
    #endregion
    //--------------------------------
    // SUBCATEGORÍAS
    //--------------------------------
    #region SUBCATEGORY
    /// <summary>
    /// Coastal Sloop / Balandro Costero: A kind of sloop ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_CoastalSloop : ShipModel_Sloop
    {
        private static readonly string name_subModel = "Balandro costero";
        private static readonly int capacity = 15, minCrew = 7, maxCrew = 18, manteinance = 15;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["OneMastle"]};
        private static readonly int[] artilleryCapacity = new int[4] {2,4,0,0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 10; }
        public override int GetShellPoints() { return 9; }
        //---------
        public override byte GetSpriteIndex() { return 1; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Continental Sloop / Balandro de Bermuda: A kind of sloop ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_ContinentalSloop : ShipModel_Sloop
    {
        private static readonly string name_subModel = "Balandro de Bermuda";
        private static readonly int capacity = 20, minCrew = 10, maxCrew = 24, manteinance = 20;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["OneMastle"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 8, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 20; }
        public override int GetShellPoints() { return 12; }
        //---------
        public override byte GetSpriteIndex() { return 2; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Military Banander / Balandra militar: A kind of sloop ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_MilitaryBalander : ShipModel_Sloop
    {
        private static readonly string name_subModel = "Balandra militar";
        private static readonly int capacity = 30, minCrew = 12, maxCrew = 30, manteinance = 40;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["OneMastle"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 12, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 28; }
        public override int GetShellPoints() { return 16; }
        //---------
        public override byte GetSpriteIndex() { return 3; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Two Mastles Felucca / Falucho de dos palos: A kind of felucca ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_TwoMastlesFelucca : ShipModel_Felucca
    {
        private static readonly string name_subModel = "Falucho de dos palos";
        private static readonly int capacity = 20, minCrew = 6, maxCrew = 21, manteinance = 10;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"] };
        private static readonly int[] artilleryCapacity = new int[4] { 2, 4, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 10; }
        public override int GetShellPoints() { return 11; }
        //---------
        public override byte GetSpriteIndex() { return 4; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Mistic Felucca / Místico: A kind of felucca ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_MisticFelucca : ShipModel_Felucca
    {
        private static readonly string name_subModel = "Místico de Tres Palos";
        private static readonly int capacity = 30, minCrew = 10, maxCrew = 27, manteinance = 16;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 8, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 22; }
        public override int GetShellPoints() { return 16; }
        //---------
        public override byte GetSpriteIndex() { return 5; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// One Mastled-Tartain / Tartana de un palo: A kind of tartain ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_TwoMastlesTartain : ShipModel_Tartain
    {
        private static readonly string name_subModel = "Tartana de un palo";
        private static readonly int capacity = 25, minCrew = 8, maxCrew = 18, manteinance = 16;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["OneMastle"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 4, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 12; }
        public override int GetShellPoints() { return 13; }
        //---------
        public override byte GetSpriteIndex() { return 6; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Two Mastled-Tartain / Tartana de dos palos: A kind of tartain ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_ThreeMastlesTartain : ShipModel_Tartain
    {
        private static readonly string name_subModel = "Tartana de dos palos";
        private static readonly int capacity = 45, minCrew = 10, maxCrew = 20, manteinance = 30;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 8, 0, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 22; }
        public override int GetShellPoints() { return 15; }
        //---------
        public override byte GetSpriteIndex() { return 7; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    [System.Serializable]
    /// <summary>
    /// 12 cannons brig / Bergantín de 12 cañones: A kind of brigantine ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors:
    ///         public ShipSubCategory_12Brig(EntityType_KINGDOM k)
    ///         public ShipSubCategory_16Brig(Improvement v)
    /// </summary>
    public class ShipSubCategory_12Brig : ShipModel_Brigantine
    {
        private static readonly string name_subModel = "Bergantín de 14 cañones";
        private static readonly int capacity = 55, minCrew = 13, maxCrew = 38, manteinance = 40;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 10, 4, 0 };

        //This ship variant modifier
        public Improvement variant;
        public override string GetVariantName() { var str = variant == null ? null : variant.mod_Name; return str; }

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 36; }
        public override int GetShellPoints() { return 18; }
        //---------
        public override byte GetSpriteIndex() { return 8; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }

        //Contructors
        public ShipSubCategory_12Brig(EntityType_KINGDOM k)
        {
            if(k == EntityType_KINGDOM.KINGDOM_Dutch || k == EntityType_KINGDOM.KINGDOM_Britain)
            {
                if (Random.Range(0, 3) == 0) variant = D_Variants["Snowbrig"];
            }
            else
            {
                if (Random.Range(0, 4) == 0) variant = D_Variants["Roundbrig"];
            }
        }
        public ShipSubCategory_12Brig(Improvement v)
        {
            variant = v;
        }
    }
    /// <summary>
    /// 16 cannons brig / Bergantín de 16 cañones: A kind of brigantine ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors:
    ///         public ShipSubCategory_16Brig(EntityType_KINGDOM k)
    ///         public ShipSubCategory_16Brig(Improvement v)
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_16Brig : ShipModel_Brigantine
    {
        private static readonly string name_subModel = "Bergantín de 16 cañones";
        private static readonly int capacity = 80, minCrew = 15, maxCrew = 45, manteinance = 55;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 10, 6, 0 };

        //This ship variant modifier
        public Improvement variant;
        public override string GetVariantName() { var str = variant == null ? null : variant.mod_Name;  return str; }

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 44; }
        public override int GetShellPoints() { return 19; }
        //---------
        public override byte GetSpriteIndex() { return 9; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }

        //Contructors
        public ShipSubCategory_16Brig(EntityType_KINGDOM k)
        {
            if (k == EntityType_KINGDOM.KINGDOM_Dutch || k == EntityType_KINGDOM.KINGDOM_Britain)
            {
                if (Random.Range(0, 3) == 0) variant = D_Variants["Snowbrig"];
            }
            else
            {
                if (Random.Range(0, 3) == 0) variant = D_Variants["Roundbrig"];
            }
        }
        public ShipSubCategory_16Brig(Improvement v)
        {
            variant = v;
        }
    }
    /// <summary>
    /// 10 cannons Lugger / Lugre de 10 cañones: A kind of lugger ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_10Lugger : ShipModel_Lugger
    {
        private static readonly string name_subModel = "Lugre de 10 cañones";
        private static readonly int capacity = 60, minCrew = 11, maxCrew = 40, manteinance = 70;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 6, 4, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 28; }
        public override int GetShellPoints() { return 16; }
        //---------
        public override byte GetSpriteIndex() { return 7; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// 14 cannons Lugger / Lugre de 14 cañones: A kind of lugger ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_14Lugger : ShipModel_Lugger
    {
        private static readonly string name_subModel = "Lugre de 14 cañones";
        private static readonly int capacity = 85, minCrew = 14, maxCrew = 44, manteinance = 80;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 10, 4, 0 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 38; }
        public override int GetShellPoints() { return 17; }
        //---------
        public override byte GetSpriteIndex() { return 7; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Shooner Polacre / Goleta polacra: A kind of polacre ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors:
    ///         public ShipSubCategory_ShoonerPolacre(EntityType_KINGDOM k)
    ///         public ShipSubCategory_ShoonerPolacre(Improvement v)
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_ShoonerPolacre : ShipModel_Polacre
    {
        private static readonly string name_subModel = "Goleta-Polacra";
        private static readonly int capacity = 55, minCrew = 10, maxCrew = 38, manteinance = 35;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["TwoMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 4, 8, 4, 0 };

        //This ship variant modifier
        public Improvement variant;
        public override string GetVariantName() { var str = variant == null ? null : variant.mod_Name; return str; }

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 32; }
        public override int GetShellPoints() { return 17; }
        //---------
        public override byte GetSpriteIndex() { return 8; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }

        //Contructors
        public ShipSubCategory_ShoonerPolacre(EntityType_KINGDOM k)
        {
            if (k == EntityType_KINGDOM.KINGDOM_Spain || k == EntityType_KINGDOM.KINGDOM_France)
            {
                if (Random.Range(0, 3) != 0) variant = D_Variants["Xebec"];
            }
        }
        public ShipSubCategory_ShoonerPolacre(Improvement v)
        {
            variant = v;
        }
    }
    /// <summary>
    /// Polacre / Polacra: A kind of polacre ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors:
    ///         public ShipSubCategory_Polacre(EntityType_KINGDOM k)
    ///         public ShipSubCategory_Polacre(Improvement)
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_Polacre : ShipModel_Polacre
    {
        private static readonly string name_subModel = "Polacra";
        private static readonly int capacity = 80, minCrew = 12, maxCrew = 44, manteinance = 45;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 12, 6, 0 };

        //This ship variant modifier
        public Improvement variant;
        public override string GetVariantName() { var str = variant == null ? null : variant.mod_Name; return str; }

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 48; }
        public override int GetShellPoints() { return 18; }
        //---------
        public override byte GetSpriteIndex() { return 8; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }

        //Contructors
        public ShipSubCategory_Polacre(EntityType_KINGDOM k)
        {
            if (k == EntityType_KINGDOM.KINGDOM_Spain || k == EntityType_KINGDOM.KINGDOM_France)
            {
                if (Random.Range(0, 3) != 0) variant = D_Variants["Xebec"];
            }
        }

        public ShipSubCategory_Polacre(Improvement v)
        {
            variant = v;
        }
    }
    /// <summary>
    /// Corvette / Corbeta: A kind of corvette ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors:
    ///         public ShipSubCategory_Corvette()
    ///         public ShipSubCategory_Corvette(Improvement v)
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_Corvette : ShipModel_Corvette
    {
        private static readonly string name_subModel = "Corbeta";
        private static readonly int capacity = 160, minCrew = 16, maxCrew = 48, manteinance = 110;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 8, 12, 0 };

        //This ship variant modifier
        public Improvement variant;
        public override string GetVariantName() { var str = variant == null ? null : variant.mod_Name; return str; }

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 60; }
        public override int GetShellPoints() { return 24; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }

        //Contructors
        public ShipSubCategory_Corvette()
        {
            if (Random.Range(0, 4) == 0) variant = D_Variants["Barque"];
        }
        public ShipSubCategory_Corvette(Improvement v)
        {
            variant = v;
        }
    }
    /// <summary>
    /// 26 Cannons-Frigate / Fragata de 26 cañones: A kind of frigate ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_26Frigate : ShipModel_Frigate
    {
        private static readonly string name_subModel = "Fragata de 26 cañones";
        private static readonly int capacity = 200, minCrew = 16, maxCrew = 55, manteinance = 140;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"]};
        private static readonly int[] artilleryCapacity = new int[4] { 6, 0, 20, 6 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 90; }
        public override int GetShellPoints() { return 28; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Military Frigate / Fragata Militar: A kind of frigate ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_MilitaryFrigate : ShipModel_Frigate
    {
        private static readonly string name_subModel = "Fragata Militar";
        private static readonly int capacity = 280, minCrew = 20, maxCrew = 70, manteinance = 200;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"] };
        private static readonly int[] artilleryCapacity = new int[4] { 8, 0, 30, 10 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 138; }
        public override int GetShellPoints() { return 32; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Little Gallion / Galeoncete: A kind of gallion ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_LittleGallion : ShipModel_Gallion
    {
        private static readonly string name_subModel = "Galeoncete";
        private static readonly int capacity = 320, minCrew = 14, maxCrew = 40, manteinance = 170;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"], D_Characteristics["Poopdeck"], D_Characteristics["BlindSail"] };
        private static readonly int[] artilleryCapacity = new int[4] { 8, 0, 6, 8 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 58; }
        public override int GetShellPoints() { return 33; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Gallion / Galeón: A kind of gallion ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_Gallion : ShipModel_Gallion
    {
        private static readonly string name_subModel = "Galeón";
        private static readonly int capacity = 650, minCrew = 16, maxCrew = 110, manteinance = 260;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"], D_Characteristics["Poopdeck"], D_Characteristics["BlindSail"] };
        private static readonly int[] artilleryCapacity = new int[4] { 8, 0, 10, 40 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 198; }
        public override int GetShellPoints() { return 45; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Dutch Gallion / Galeón Holandés: A kind of gallion ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_DutchGallion : ShipModel_Gallion
    {
        private static readonly string name_subModel = "Galeón Holandés";
        private static readonly int capacity = 550, minCrew = 14, maxCrew = 65, manteinance = 180;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"], D_Characteristics["Poopdeck"], D_Characteristics["BlindSail"] };
        private static readonly int[] artilleryCapacity = new int[4] { 8, 0, 16, 8 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 88; }
        public override int GetShellPoints() { return 36; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Flyboat / Filibote: A kind of flyboat ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_Flyboat : ShipModel_Flyboat
    {
        private static readonly string name_subModel = "Filibote";
        private static readonly int capacity = 700, minCrew = 15, maxCrew = 45, manteinance = 190;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"], D_Characteristics["Poopdeck"], D_Characteristics["BlindSail"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 0, 10, 6 };

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 60; }
        public override int GetShellPoints() { return 34; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach (Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    /// <summary>
    /// Urca: A kind of urca ship
    /// Parameters: Capacity (int), Crew (int), mantainence (int), Artillery (int[4])
    ///  ** Constructors: Empty
    /// </summary>
    [System.Serializable]
    public class ShipSubCategory_Urca : ShipModel_Urca
    {
        private static readonly string name_subModel = "Urca";
        private static readonly int capacity = 250, minCrew = 16, maxCrew = 44, manteinance = 110;
        private static readonly List<Modifier> characteristics = new List<Modifier>() { D_Characteristics["ThreeMastles"], D_Characteristics["Platform"], D_Characteristics["Quarterdeck"], D_Characteristics["BlindSail"] };
        private static readonly int[] artilleryCapacity = new int[4] { 6, 6, 6, 0 };

        //This ship variant modifier
        public Improvement variant;

        //Overriding:
        public override List<Modifier> GetCharacteristics() { return characteristics; }
        public override string GetSubClassName() { return name_subModel; }
        public override int[] GetPowerCapacity() { return artilleryCapacity; }
        public override int[] GetCrewData() { return new int[2] { minCrew, maxCrew }; }
        public override int GetCapacity() { return capacity; }
        public override int GetManteinance() { return manteinance; }
        public override int GetFirePower() { return 36; }
        public override int GetShellPoints() { return 20; }
        //---------
        public override byte GetSpriteIndex() { return 16; }
        //Visibility
        public override float GetVisibility()
        {
            var val = visibility_BaseValue;
            foreach(Modifier characteristic in characteristics)
            {
                val += characteristic.visibility_Modifier;
            }
            return val;
        }
    }
    #endregion

    //--------------------------------
    // MEJORAS
    //--------------------------------
    #region MODIFIERS & IMPROVEMENTS
    /// <summary>
    /// A set of parameters which alter basic ship values
    /// Parameters: Modifier name (string), description (string), modifiers (float)
    ///  ** Constructors:
    ///         public Modifier(string n, string d, float[] values)
    /// </summary>
    public class Modifier
    {
        public string mod_Name, description;
        public float
            minUpwindSpeed_Modifier,
            maxUpwindSpeed_Modifier,
            minSmoothSpeed_Modifier,
            maxSmoothSpeed_Modifier,
            visibility_Modifier,
            minCrew_Modifier,
            maneuverability_Modifier,
            manteinance_Modifier,
            boardingDefence_Modifier,
            sailsHealth_Modifier;

        public Modifier(string n, string d, float[] values)
        {
            mod_Name = n;
            description = d;
            minUpwindSpeed_Modifier = values[0];
            maxUpwindSpeed_Modifier = values[1];
            minSmoothSpeed_Modifier = values[2];
            maxSmoothSpeed_Modifier = values[3];
            visibility_Modifier = values[4];
            minCrew_Modifier = values[5];
            maneuverability_Modifier = values[6];
            manteinance_Modifier = values[7];
            boardingDefence_Modifier = values[8];
            sailsHealth_Modifier = values[9];

        }
    }
    //Modifications and variants

    /// <summary>
    /// Improvements are modifiers witha monetary cost.
    /// Parameters: Improvement Price (int)
    ///  ** Constructors:
    ///         public Improvement(string n, string d, float[] values, int price) : base( n, d, values)
    /// </summary>
    public class Improvement : Modifier
    {
        public int improvementPrice;
        public Improvement(string n, string d, float[] values, int price) : base( n, d, values)
        {
            improvementPrice = price;
        }
    }
#endregion
}
