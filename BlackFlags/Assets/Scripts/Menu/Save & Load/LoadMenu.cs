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
        [SerializeField] private GameObject warningsPanel;

        [SerializeField] private SavedFileError errorRow;

        protected virtual void OnEnable()
        {
            base.OnEnable();
            warningsPanel.SetActive(false);
        }
        public void Load()
        {
            var statusOK = selectedRow.isOk();

            if (statusOK)
            {
                Load_Force();
            }
            else
            {
                // var message = selectedRow.getErrorMsg();
                warningsPanel.SetActive(true);
            }
        }

        public void Load_Force()
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
            errorRow.updateErrorCause(fileRow.getErrorMsg());
            base.Select(fileRow);
        }
    }
}

