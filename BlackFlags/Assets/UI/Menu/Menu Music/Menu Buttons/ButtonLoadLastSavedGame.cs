using UnityEngine.UI;
using GameSettings.Core;
using System.IO;
using System;

public class ButtonLoadLastSavedGame : ButtonIntroAction
{
    protected override void OnEnable()
    {
        base.OnEnable();

        if (Directory.GetFiles(GetDirectory()).Length == 0)
        {
            GetComponent<Button>().interactable = false;
        }
    }
    protected override void Start()
    {
        if (Directory.GetFiles(GetDirectory()).Length != 0)
        {
            base.Start();
        }
    }

    protected override void Action()
    {
        string directory = GetDirectory();

        //var files = Directory.GetFiles(directory);
        var files = new DirectoryInfo(directory).GetFiles("*.*");
        var latestFileName = GetLastGetLatestFile(files);

        PersistentGameSettings.loadingFile = true;
        PersistentGameSettings.selectedFileName = latestFileName;
        SceneManager.SetAsynSceneAndLoadAsyn(3);
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

    private string GetLastGetLatestFile(FileInfo[] files)
    {
        DateTime lastModified = DateTime.MinValue;
        int latestFileIndex = 0;
        for (int i = 0; i < files.Length; i++)
        {
            var modifiedTime = files[i].LastWriteTime;
            if (modifiedTime > lastModified)
            {
                lastModified = modifiedTime;
                latestFileIndex = i;
            }
        }

        return files[latestFileIndex].Name;
    }
}
