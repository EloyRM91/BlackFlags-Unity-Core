using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameMechanics.Data
{
    public class ShipInventory : EconomyBehaviour
    {
        //Singleton
        private static ShipInventory instance;

        //Ship data
        public int maxCapacity;
        public static float shipLoad;
        private int[] items = new int[21];
        private float[] averageCost = new float[21];
        private float[] surplus = new float[6];
        

        //Morale and crew
        private int crew;
        private int[] moralBySupplies = new int[6];
        private float morale_BYSUPPLIES, morale_BYPILLAGE, morale_BYRESTING, morale_GLOBAL;

        //Morale: Modifiers and Rationing
        [SerializeField] private MoraleModifier[] modifiers;
        public static int disentryCounter;
        public static bool rationingRum, rationingMeat;

        public static float Morale_Supplies
        {
            get { return instance.morale_BYSUPPLIES; }
        }

        public static float Morale_Pillage
        {
            get { return instance.morale_BYPILLAGE; }
        }

        public static float Morale_Resting
        {
            get { return instance.morale_BYRESTING; }
        }

        public static float Morale_Global
        {
            get { return instance.morale_GLOBAL; }
        }

        //Supplies Consumption
        private static readonly Dictionary<byte, float> D_SupplyConsumption = new Dictionary<byte, float>()
        {
            {0, .9f},
            {1, 1.5f},
            {2, .8f},
            {3,.3f},
            {4,.3f},
            {5, .2f}
        };

        //Supplies moral weight points
        private static readonly Dictionary<byte, int> D_MoralBySupplies = new Dictionary<byte, int>()
        {
            {0, 1},
            {1, 2},
            {2, 2},
            {3, 2},
            {4, 3},
            {5, 1}
        };

        public static int[] Items
        {
            get { return instance.items; }
        }
        public static float[] Surplus
        {
            get { return instance.surplus; }
        }
        public static float[] AverageCost
        {
            get { return instance.averageCost; }
        }
        public static int Crew
        {
            get { return instance.crew; }
            set { instance.crew = value; }
        }

        public static int[] onLoad = new int[4];
        public static int[] equipment = new int[4];
        public static int[] onUseWeaponsSlots = new int[4];
        private static int[] armoryCapacity = new int[4];

        //Events
        public delegate void OnEquipmentChange(byte index);
        public static event OnEquipmentChange OnEquipmentEditRefresh; //Equipa o desequipa armamento

        public delegate void OnInventoryChange();

        /// <summary>
        /// updateLoad event is launched on ship's load changes
        /// Listeners: Convoy HUD display, trading panels
        /// </summary>
        public static OnInventoryChange updateLoad;
        /// <summary>
        /// updateMorale event is launched on crew's morale changes
        /// Listeners: Convoy HUD display
        /// </summary>
        public static OnInventoryChange updateMorale;
        /// <summary>
        /// updateMoraleModifiers event is launched if a modifiers is enabled or disabled
        /// Listeners: Convoy HUD display
        /// </summary>
        public static OnInventoryChange updateMoraleModifiers;

        private void Awake()
        {
            //Singleton
            instance = this;

            //Events
            TimeManager.DailyConsumptionEvent += DailyConsumption;
            //TimeManager.DailyConsumptionEvent += SetMorale_Resting;
        }
        private void Start()
        {
            maxCapacity = PersistentGameData._GData_PlayerShip.GetCapacity();
            StartingInventory(LevelOfDifficulty.easy);
            StartingArmory();
            armoryCapacity = PlayerMovement.playership.GetPowerCapacity();
            UpdateLoad();

            morale_BYSUPPLIES = 0.8f;
            morale_BYRESTING = 0.7f;
            morale_BYPILLAGE = 0.3f;
            SetMorale_Global();
        }

        private void OnDestroy()
        {
            TimeManager.DailyConsumptionEvent -= DailyConsumption;
            //TimeManager.DailyConsumptionEvent -= SetMorale_Resting;
        }

        //Initialize and deserialization
        private void StartingInventory(LevelOfDifficulty level)
        {
            //items = D_InitialResources[level];
            Array.Copy(D_InitialResources[level], items, 21);
        }

        private void StartingArmory()
        {
            for (byte i = 0; i < 4; i++)
            {
                onLoad[i] = Convert.ToByte(items[i + 17]);
            }
        }

        //Supply
        public static float GetNecessarySupplies7(byte key)
        {
            return D_SupplyConsumption[key]/2 * Crew;
        }
        public static float GetNecessarySupplies15(byte key)
        {
            return D_SupplyConsumption[key] * Crew;
        }

        public static int GetNecessaryGuns(byte index)
        {
            if(index == 16)
            {
                return Mathf.Clamp(2 * Crew - instance.items[index], 0, 999);
            }
            else
            {
                int capacity = PersistentGameData._GData_PlayerShip.GetPowerCapacity()[index - 17];
                return Mathf.Clamp(capacity - instance.items[index], 0,999);
            }
        }

        //Consumption

        private void DailyConsumption()
        {
            if (!PlayerMovement.IsInPort())
            {
                for (byte i = 0; i < 6; i++)
                {
                    var consumptionValue = crew * D_SupplyConsumption[i] / 15f;
                    //Coste de mantenimiento por navegar:
                    if (i == 6) consumptionValue += PersistentGameData._GData_PlayerShip.GetManteinance() / 45;

                    if(consumptionValue <= surplus[i])
                    {
                        //Resta del excedentes
                        surplus[i] -= consumptionValue;
                    }
                    else
                    {
                        //Resta del excedente
                        consumptionValue -= surplus[i];
                        surplus[i] = 0;

                        //Para consumir el resto toma directamente del inventario
                        int val = (int)Math.Ceiling(consumptionValue);
                        items[i] -= val;

                        //Reajusta el nuevo excedente
                        surplus[i] = val - consumptionValue;

                        moralBySupplies[i] = items[i] + surplus[i] < 0 ? 0 : D_MoralBySupplies[i];

                        if(items[i] < 0)
                        {
                            items[i] = 0;
                            surplus[i] = 0;
                        }
                    }
                }
                SetMorale_Supplies();
                updateLoad();
            }
        }

        private void SetMorale_Supplies()
        {
            if (!PlayerMovement.IsInPort())
            {
                float sum = 0;
                for (int i = 0; i < 6; i++)
                {
                    sum += moralBySupplies[i];
                }
                morale_BYSUPPLIES = (morale_BYSUPPLIES + (sum / 11f)) / 2;
                GetSuppliesModiffiers();
                SetMorale_Resting();
                SetMorale_Global();
                if (updateMorale != null) updateMorale();
            }
        }

        private void GetSuppliesModiffiers()
        {
            //Starvation
            if (!MoraleModifier.Contains(modifiers[0]))
            {
                if (moralBySupplies[0] == 0 && moralBySupplies[1] == 0 && moralBySupplies[2] == 0)
                {
                    MoraleModifier.Add(modifiers[0]);
                }
            }
            else
            {
                if (moralBySupplies[0] > 0 || moralBySupplies[1] > 0 || moralBySupplies[2] > 0)
                    MoraleModifier.Remove(modifiers[0]);
            }

            //Disentry
            if (!MoraleModifier.Contains(modifiers[1]))
            {
                if (moralBySupplies[3] == 0 && moralBySupplies[4] == 0)
                {
                    if (UnityEngine.Random.Range(0, 9) == 0)
                    {
                        disentryCounter = 8;
                        MoraleModifier.Add(modifiers[1]);
                    }     
                }
            }
            else
            {
                if (disentryCounter > 0)
                    disentryCounter++;
                else MoraleModifier.Remove(modifiers[1]);
            }

            //Scurvy
            if (!MoraleModifier.Contains(modifiers[2]))
            {
                if(moralBySupplies[1] == 0)
                    if (UnityEngine.Random.Range(0, 10) == 0)
                        MoraleModifier.Add(modifiers[2]);
            }
            else
            {
                if (moralBySupplies[1] != 0)
                    if (UnityEngine.Random.Range(0, 3) == 0)
                        MoraleModifier.Remove(modifiers[2]);
            }

            //Drunken
            if (!MoraleModifier.Contains(modifiers[3]))
            {
                if(!MoraleModifier.Contains(modifiers[11]))
                    if(items[4] > 15)
                        if (UnityEngine.Random.Range(0, 6) == 0)
                            MoraleModifier.Add(modifiers[3]);
            }
            else
            {
                MoraleModifier.Remove(modifiers[3]);
            }
        }

        public static void Rationing(bool isOn, int modifierIndex)
        {
            if (isOn) MoraleModifier.Add(instance.modifiers[modifierIndex]);
            else MoraleModifier.Remove(instance.modifiers[modifierIndex]);




            if (updateMorale != null) updateMorale();
        }

        private void SetMorale_Resting()
        {
            morale_BYRESTING += PlayerMovement.IsInPort() ? 0.1f : -0.01f;
            morale_BYRESTING = Mathf.Clamp(morale_BYRESTING, 0, 1);
            SetMorale_Pillaging();
            //if (updateMorale != null) updateMorale();
        }

        private void SetMorale_Pillaging()
        {
            morale_BYPILLAGE = Mathf.Clamp(morale_BYPILLAGE - 0.005f, 0, 1);
        }

        private void SetMorale_Pillaging(int value)
        {
            morale_BYPILLAGE = Mathf.Clamp(morale_BYPILLAGE + value / (crew * 600), 0, 1.3f);
            //if (updateMorale != null) updateMorale();
        }

        private void SetMorale_Global()
        {
            morale_GLOBAL = (morale_BYSUPPLIES + morale_BYPILLAGE + morale_BYRESTING)/3;
        }

//Trading
#region TRADING
        public static void SetInventoryChange(int resourceIndex, int amount)
        {
            if (amount < 0) //vende o consume
                instance.RemoveItem(resourceIndex, amount);
            else           //compra u obtiene
                instance.AddItem(resourceIndex,amount);
        }

        public static void SetInventoryChange(int resourceIndex, int amount, int cost)
        {
            if (amount > 0) //compra
                instance.AddItem(resourceIndex, amount, cost);

        }
        private void UpdateLoad()
        {
            shipLoad = 0;
            for (int i = 0; i < 21; i++)
            {
                shipLoad += D_WorldResources[i].weight * items[i];
            }
            if (updateLoad != null) updateLoad();
        }

        public void AddItem(int index, int amount, int costPerUnit = 0)
        {
            var additionalWeight = D_WorldResources[index].weight * amount;
            if (HasCapacity(additionalWeight))
            {
                //Precio de insumo
                int v1 = amount * costPerUnit;
                //Precio de existencias
                float v2 = items[index] * averageCost[index];

                //Modifica inventario
                items[index] += amount;
                shipLoad += additionalWeight;

                //Nuevo coste promedio
                averageCost[index] = (v1 + v2) / items[index];

                if (index >= 17)
                {
                    onLoad[index - 17] += amount;
                }

                if (updateLoad != null) updateLoad();
            }
        }
        public void RemoveItem(int index, int amount)
        {
            amount = items[index] >= amount ? amount : items[index];

            items[index] -= Math.Abs(amount);
            shipLoad -= shipLoad + D_WorldResources[index].weight * amount;

            if (index >= 17)
            {
                ReduceWeaponsOnBoard((byte)(index - 17), (byte)Math.Abs(amount));
            }

            UpdateLoad();
        }
        public static bool HasCapacity(byte resourceIndex, int amount)
        {
            var newVal = shipLoad + D_WorldResources[resourceIndex].weight * amount;
            return newVal < instance.maxCapacity;
        }
        public bool HasCapacity(float weight) { return shipLoad + weight < maxCapacity; }
#endregion

        //Armory & set up equipment
        #region ARMORY: ONLOAD & EQUIPMENT
        private void ReduceWeaponsOnBoard(byte index, byte amount)
        {
            int AmountToReduce = amount <= items[index] ? amount : items[index];

            //if(AmountToReduce > 0)
            //{
            //Manage load/equipment armory
            //if (onLoad[index] > 0)
            //{
            //    if (onLoad[index] >= amount)
            //    {
            //        onLoad[index] -= AmountToReduce;
            //    }
            //    else
            //    {
            //        onLoad[index] = 0;
            //        var amountToSetOff = AmountToReduce - onLoad[index];
            //        equipment[index] -= amountToSetOff;
            //    }
            //}
            //else
            //{
            //    equipment[index] -= (byte)AmountToReduce;
            //    onUseWeaponsSlots[index] -= AmountToReduce;
            //}
            //}


            if (AmountToReduce > 0)
            {
                //Change equipment amount
                if (onLoad[index] > 0)
                {
                    if (onLoad[index] < amount)
                    {
                        var amountToSetOff = AmountToReduce - onLoad[index];
                        equipment[index] -= amountToSetOff;
                    }

                }
                else
                {
                    equipment[index] -= (byte)AmountToReduce;
                }

                //Get backup
                int[] equipmentBackUp = equipment;

                //ResetAll
                for (byte i = 0; i < 4; i++)
                {
                    onUseWeaponsSlots[i] = 0;
                    equipment[i] = 0;
                    onLoad[i] = items[i + 17];
                    ToEquipment(i, (byte)equipmentBackUp[i]);
                }
            }
        } 
        public static void ToLoad(byte index, byte amount)
        {
            if(equipment[index] >= amount)
            {

                if (equipment[index] == onUseWeaponsSlots[index]) //I'm not using other calibber racks  
                {
                    equipment[index] -= amount;
                    onLoad[index] += amount;
                    onUseWeaponsSlots[index] = Convert.ToByte(Mathf.Clamp(onUseWeaponsSlots[index] - amount, 0, 200));
                    OnEquipmentEditRefresh(index);
                    //print($"{onUseWeaponsSlots[0]}  {onUseWeaponsSlots[1]}  {onUseWeaponsSlots[2]}  {onUseWeaponsSlots[3]} // {equipment[index]} ");
                }
                else //there're  cannons in other racks 
                {
                    var ncaliber = 3;
                    while (ncaliber >= 0)
                    {
                        if (armoryCapacity[ncaliber] > 0 && onUseWeaponsSlots[ncaliber] > 0)
                        {
                            if (onUseWeaponsSlots[ncaliber] > equipment[ncaliber])
                            {
                                equipment[index] -= amount;
                                onLoad[index] += amount;
                                onUseWeaponsSlots[ncaliber] = Convert.ToByte(Mathf.Clamp(onUseWeaponsSlots[ncaliber] - amount, 0, 200));
                                OnEquipmentEditRefresh(index);
                                //print($"{onUseWeaponsSlots[0]}  {onUseWeaponsSlots[1]}  {onUseWeaponsSlots[2]}  {onUseWeaponsSlots[3]} // {equipment[index]}");
                                return;
                            }
                            else
                            {
                                ncaliber--;
                            }
                        }
                        else
                        {
                            ncaliber--;
                        }
                    }
                    OnEquipmentEditRefresh(index);
                }
            }
        }

        public static void ToEquipment(byte index, byte amount)
        {
            int ncaliber = index;
            var slotsVal = onUseWeaponsSlots[index] + amount;
            //byte newAmount;

            if (onLoad[index] > 0)
            {
                if (onLoad[index] >= amount)
                {
                    if (slotsVal <= armoryCapacity[index]) // if ship has capacity to install all demanded weapons
                    {
                        equipment[index] += amount;
                        onLoad[index] = onLoad[index] - amount;
                        onUseWeaponsSlots[index] = onUseWeaponsSlots[index] + amount;
                        OnEquipmentEditRefresh(index);
                        //print($"{onUseWeaponsSlots[0]}  {onUseWeaponsSlots[1]}  {onUseWeaponsSlots[2]}  {onUseWeaponsSlots[3]} // {equipment[index]}");
                    }
                    else if (index < 3) //try another artillery slot, except if caliber is 24'
                    {
                        ncaliber++;
                        while (ncaliber < 4)
                        {
                            if(armoryCapacity[ncaliber] > 0)
                            {
                                slotsVal = onUseWeaponsSlots[ncaliber] + amount;

                                if (slotsVal <= armoryCapacity[ncaliber])
                                {
                                    equipment[index] += amount;
                                    onLoad[index] -= amount;
                                    onUseWeaponsSlots[ncaliber] = onUseWeaponsSlots[ncaliber] + amount;
                                    OnEquipmentEditRefresh(index);
                                    //print($"{onUseWeaponsSlots[0]}  {onUseWeaponsSlots[1]}  {onUseWeaponsSlots[2]}  {onUseWeaponsSlots[3]} // {equipment[index]}");
                                    return;
                                }
                                else
                                {
                                    ncaliber++;
                                }
                            }
                            else
                            {
                                ncaliber++;
                            }
                        }
                    }
                }
            }

            

        }
#endregion
    }
}

