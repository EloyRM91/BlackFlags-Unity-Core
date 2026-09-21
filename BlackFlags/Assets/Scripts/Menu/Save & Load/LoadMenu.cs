using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using GameSettings.Core;

namespace UI.Files
{
    public class LoadMenu : SavedFilesDisplayMenu
    {
        [SerializeField] private InputField fieldText;
        public void Load()
        {
            string fileName = fieldText.text;
            print(fileName);

            PersistentGameSettings.loadingFile = true;
            PersistentGameSettings.selectedFileName = fileName;
            SceneManager.SetAsynSceneAndLoadAsyn(3);
        }

        protected override void Select(SavedFileRow fileRow)
        {
            var currentFileName = fileRow.GetFileName();
            fieldText.text = currentFileName;
            base.Select(fileRow);
        }
    }
}

