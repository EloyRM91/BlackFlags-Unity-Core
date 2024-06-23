using UnityEngine;
using System.IO;

namespace GameSettings.VFX
{
    public class GraphicSettings : MonoBehaviour
    {
#region VARIABLES

        //Reading/Writing
        private readonly string path = "GraphicSettings.txt";

        private static bool nightLight;

        public static bool NightLight
        {
            get { return nightLight; }
            set
            {
                nightLight = value;
                if (lightNightChange != null) lightNightChange();
            }
        }

        private static bool postprocessing;

        public static bool Postprocessing
        {
            get { return postprocessing; }
            set
            {
                postprocessing = value;
                postProcessingChange();
            }
        }

#endregion

#region EVENTS

        //Events
        public delegate void OnValueChange();
        public static event OnValueChange lightNightChange;
        public static event OnValueChange postProcessingChange;

#endregion

        void Start()
        {
            ReadSettings();
        }

        void OnDestroy()
        {
            WriteSettings();
        }

        private void ReadSettings()
        {
            
            if (File.Exists(path))
            {
                
                var reader = new StreamReader(path);

                Postprocessing = reader.ReadLine().Split(' ')[1] == "True";
                NightLight = reader.ReadLine().Split(' ')[1] == "True";

                reader.Close();
            }
            else
            {
                Postprocessing = true;
                NightLight = false;
            }
                
        }

        private void WriteSettings()
        {
            var writer = new StreamWriter(path);

            writer.WriteLine("Postprocessing " + Postprocessing);
            writer.WriteLine("NighLighting " + Postprocessing);

            writer.Close();
        }

    }
}


