using System.Collections.Generic;
using UnityEngine;
namespace GameMechanics.Data
{
    public class MoraleModifier : MonoBehaviour
    {
        public float[] modifiers; //modificador de moral, velocidad, y eficacia en abordaje
        public string description;
        private static List<MoraleModifier> activeModifiers = new List<MoraleModifier>();

        public delegate void UpdateValue();
        public static UpdateValue onSpeedModifier;

        private void OnEnable()
        {
            //cosas
        }
        private void OnDisable()
        {
            //cosas
        }

        public static void Add(MoraleModifier mod)
        {
            activeModifiers.Add(mod);
            mod.gameObject.SetActive(true);

            //Is speed being modified?
            if (mod.modifiers[1] != 0) onSpeedModifier();
        }

        public static void Remove(MoraleModifier removed)
        {
            activeModifiers.Remove(removed);
            removed.gameObject.SetActive(false);
        }

        public static bool Contains(MoraleModifier modifier)
        {
            return activeModifiers.Contains(modifier);
        }

        public static float GetMoraleModifier()
        {
            float val = 0;
            foreach(MoraleModifier m in activeModifiers)
            {
                val += m.modifiers[0];
            }
            return val;
        }

        public static float GetSpeedModifier()
        {
            float val = 0;
            foreach (MoraleModifier m in activeModifiers)
            {
                val += m.modifiers[1];
            }
            return val;
        }

        public static float GetLootingModifier()
        {
            float val = 0;
            foreach (MoraleModifier m in activeModifiers)
            {
                val += m.modifiers[2];
            }
            return val;
        }
    }
}

