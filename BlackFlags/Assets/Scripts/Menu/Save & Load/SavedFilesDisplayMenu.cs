using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using GameSettings.Core;
using GameMechanics.save;

namespace UI.Files
{
    public class SavedFilesDisplayMenu : MonoBehaviour
    {
        [SerializeField] private GameObject savedFileRow;
        [SerializeField] private Transform container;

        protected virtual void Start()
        {
            string[] savedFiles = GetSavedFiles();
            var dir = GetDirectory();

            foreach (string file in savedFiles)
            {
                SavedFileInfo fileInfo = new SavedFileInfo(file);
                SavedMetaInfo metaInfo = null;

                //Para cada archivo, busca sus metadatos:
                var name = fileInfo.name.Split('.')[0];
                string[] metaResult = Directory.GetFiles(dir, $"{name}.meta");
                string metaFile = null;

                if (metaResult.Length > 0)
                {
                    metaFile = metaResult[0];
                }

                if (metaFile != null)
                {
                    var metaBinInfo = LoadMeta(metaFile);
                    metaInfo = new SavedMetaInfo(metaBinInfo);
                }
                else
                {
                    print("no hay metadato para la partida: " + name);
                }

                //Crea una fila en el layout de partidas:
                var row = GameObject.Instantiate(savedFileRow, container);
                var rowComponent = row.GetComponent<SavedFileRow>();
                rowComponent.SetData(fileInfo, metaInfo);
            }
        }


        private string GetDirectory()
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

        private string GetExtension()
        {
            var currentMod = PersistentGameSettings.currentMod;

            if (currentMod == null)
            {
                return ".pirate";
            }
            else if (currentMod.gameLogic != null)
            {
                //extensión de partidas guardadas del mod:
                var ext = currentMod.gameLogic.modFileExt;
                return ext != string.Empty ? ext : ".mod";
            }
            else
            {
                return ".mod";
            }
        }

        private string[] GetSavedFiles()
        {
            var extension = GetExtension();
            var dir = GetDirectory();
            return Directory.GetFiles(dir, $"*{extension}");
        }

        private SavedMetaFile LoadMeta(string fileName)
        {
            var loaderBinaryFormat = new MetaLoaderBinaryFormat();
            SavedMetaFile savedMetaData = loaderBinaryFormat.LoadMeta(fileName);
            return savedMetaData;
        }
    }

    /** Clase que contiene los datos que se necesitan de un archivo guardado*/
    public class SavedFileInfo
    {
        public string name;
        public string modifiedDate;
        public string size;

        public SavedFileInfo(string filePath)
        {
            //Obtiene la información del archivo
            // name = Path.GetFileName(file).Split('.')[0];
            modifiedDate = File.GetLastWriteTime(filePath).ToString();

            FileInfo fileInfo = new FileInfo(filePath);
            name = fileInfo.Name;
            size = (fileInfo.Length / 1024.0).ToString("F0") + "kb";
        }
    }

    /** Clase que contiene los datos que se necesitan de un archivo meta*/
    public class SavedMetaInfo
    {
        public string version;
        public string gameDate;
        public string location;

        public byte[] bytes;

        public SavedMetaInfo(SavedMetaFile file)
        {
            version = file.gameVersion;
            gameDate = file.WorldDate;
            location = "texto prueba";
            bytes = file.playerFlag_Meta;
        }
    }
}
