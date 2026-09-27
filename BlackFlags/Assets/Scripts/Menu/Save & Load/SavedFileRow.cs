using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI.WorldMap;

namespace UI.Files
{
    public class SavedFileRow : MonoBehaviour
    {
        [SerializeField] private Text _TEXT_version;
        [SerializeField] private Text _TEXT_fileName;
        [SerializeField] private Text _TEXT_fileLocation;
        [SerializeField] Image _IMG_LocIcon;
        [SerializeField] private Text _TEXT_gameDate;
        [SerializeField] private Text _TEXT_fileDate;
        [SerializeField] private Text _TEXT_fileSize;
        [SerializeField] Image _IMG_Flag;
        [SerializeField] Image _IMG_WarningSprite;

        //Catálogos
        [SerializeField] private SpriteCatalog spriteCatalog;
        [SerializeField] private WarningsCatalog warningsCatalog;

        //Events
        public delegate void Selection(SavedFileRow row);
        public static event Selection SelectFile;

        private string errorCause = "";

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
            _TEXT_fileName.text = file.Name;
            _TEXT_fileDate.text = file.ModifiedDate;
            _TEXT_fileSize.text = file.Size;

            if (meta != null)
            {
                //Comprueba la versión
                _TEXT_version.text = meta.Version;
                var currentVersion = $"v{Application.version}";

                if (currentVersion != meta.Version)
                {
                    SetError("oldVersion");
                    _IMG_WarningSprite.enabled = true;
                }

                _TEXT_fileLocation.text = meta.Location;

                //Accede a sprite por catálogo
                var key = meta.Location == "En el mar" ? "OnSail" : "OnPort";
                _IMG_LocIcon.sprite = spriteCatalog.Get(key);


                //Crea el sprite de la bandera sampleada:
                byte[] flagMeta = meta.Bytes;
                Sprite sp = GetSpriteFromBytes(flagMeta, 200, 133);

                _IMG_Flag.sprite = sp;

                _TEXT_gameDate.text = meta.GameDate;
            }
            else
            {
                SetError("noMeta");
            }

            _IMG_WarningSprite.enabled = errorCause != "";
        }

        public bool isOk()
        {
            return errorCause == "";
        }

        public string getErrorMsg()
        {
            return warningsCatalog.GetText(errorCause);
        }

        private void SetError(string code)
        {
            errorCause = code;
            var ButtonInfo = _IMG_WarningSprite.transform.GetComponent<ButtonInfo>();
            var message = warningsCatalog.GetWarn(code);
            ButtonInfo.txt = message;
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

