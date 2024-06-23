using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics.Data;
using System.IO;


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
            catch(System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
            
        }

        public static T LoadJson<T>( string path)
        {
            T data;
            data = JsonUtility.FromJson<T>(File.ReadAllText(path));
            return data;
        }


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


