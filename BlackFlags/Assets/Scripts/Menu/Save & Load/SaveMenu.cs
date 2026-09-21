using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics.Data;

using UnityEngine.EventSystems;

namespace UI.Files
{
    public class SaveMenu : SavedFilesDisplayMenu
    {
        [SerializeField] private InputField fieldText;

        protected override void Start()
        {
            base.Start();

            string GetFileName()
            {
                var playerName = PlayerMovement.playerName;
                var date = TimeManager.WorldDate.ToString("dd-MM-yyyy");

                return $"Partida de {playerName} -- {date}";
            }

            fieldText.text = GetFileName();

            fieldText.onValueChanged.AddListener(OnInputFieldSelected);
        }

        public void Save()
        {
            //todo: detectar si ya existe una partida con ese nombre

            //todo: detectar si el nombre es válido

            //todo: mostrar aviso "Guardando..."

            string fileName = fieldText.text;
            GameManager.gm.SaveGame(fileName);
            print("Guardando: " + fileName);
            Invoke("done", 150);
        }

        protected override void Select(SavedFileRow fileRow)
        {
            var currentFileName = fileRow.GetFileName();
            fieldText.text = currentFileName;
            base.Select(fileRow);
        }

        private void OnInputFieldSelected(string text)
        {
            Unselect();
        }

        private void done()
        {
            // gameObject.SetActive(false);

            //todo: lanzar evento que será escuchado por un mini panel que mostrará el texto "Partida guardada" con un fade off
        }

    }
}


