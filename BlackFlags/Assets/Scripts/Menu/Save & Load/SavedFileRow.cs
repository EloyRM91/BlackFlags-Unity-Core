using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Files
{
    public class SavedFileRow : MonoBehaviour
    {
        [SerializeField] private Text _TEXT_version;
        [SerializeField] private Text _TEXT_fileName;
        [SerializeField] private Text _TEXT_gameDate;
        [SerializeField] private Text _TEXT_fileDate;
        [SerializeField] private Text _TEXT_fileSize;
        [SerializeField] Image _IMG_Flag;

        //Events
        public delegate void Selection(SavedFileRow row);
        public static event Selection SelectFile;

        private string errorCause;

        void Start()
        {
            var btn = GetComponent<Button>();
            btn.onClick.AddListener(() => OnFileSelected());
        }

        public string GetFileName()
        {
            return _TEXT_fileName.text.Split('.')[0];
        }

        public void SetData(SavedFileInfo file, SavedMetaInfo meta)
        {
            _TEXT_fileName.text = file.name;
            _TEXT_fileDate.text = file.modifiedDate;
            _TEXT_fileSize.text = file.size;

            if (meta != null)
            {
                _TEXT_version.text = meta.version;
                _TEXT_gameDate.text = meta.gameDate;

                //Crea el sprite de la bandera sampleada:
                byte[] flagMeta = meta.bytes;
                Sprite sp = GetSpriteFromBytes(flagMeta, 200, 133);

                _IMG_Flag.sprite = sp;
            }
            else
            {
                errorCause = "No se encontró el archivo meta de esta partida";
            }

        }

        public string getErrorCause()
        {
            return errorCause;
        }

        private Sprite GetSpriteFromBytes(byte[] bytes, ushort witdh, ushort height)
        {
            // var flagTex = new Texture2D(witdh, height);
            // flagTex.LoadRawTextureData(bytes);
            // flagTex.Apply();
            // return Sprite.Create(flagTex, new Rect(0, 0, witdh, height), new Vector2(0, 0), 1);

            var flagTex = new Texture2D(2, 2);
            flagTex.LoadImage(bytes);
            return Sprite.Create(flagTex, new Rect(0, 0, flagTex.width, flagTex.height), new Vector2(0.5f, 0.5f), 100);
        }

        private void OnFileSelected()
        {
            SelectFile(this);
            GetComponent<Image>().enabled = true;
        }

        public void UnselectThisRow()
        {
            GetComponent<Image>().enabled = false;
        }
    }
}

