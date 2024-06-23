using UnityEngine;
using UnityEngine.UI;
using GameSettings.Core;

namespace GameSettings.Mods
{
    public class ModResponseMenu : MonoBehaviour
    {
        [SerializeField]
        private Text
            _TEXT_modVersion,
            _TEXT_modName;

        [SerializeField] private Image menuBackground;
        [SerializeField] private Transform cam;
        [SerializeField] private GameObject flag;

        void Start()
        {
            var mod = PersistentGameSettings.currentMod;
            if (mod != null)
            {
                _TEXT_modVersion.text = mod.ModVersion;
                if (mod.ModName != null && mod.ModName != "")
                    _TEXT_modName.text = mod.ModName;

                AdjustBackground(mod);
                SetScene(mod.jsonMenu);
            }
            else
                Destroy(gameObject);
        }

        private void AdjustBackground(ModData mod)
        {
            var menuData = mod.jsonMenu;
            var img = mod.menuBackground;
            if (menuData.customMenu)
            {
                //Establece nueva imagen
                menuBackground.sprite = img;
            }
            
            menuBackground.color = new Color(1,1,1, menuData.opacity);

            //Desplazamiento del fonto
            menuBackground.transform.position += new Vector3(
                menuData.xOffset,
                menuData.yOffset,
                menuData.zOffset);

            menuBackground.transform.localScale = new Vector3(
                menuData.xReScaled,
                menuData.yRescaled,
                menuData.zRescaled);
        }

        private void SetScene(JSON_Menu menuData)
        {
            flag.SetActive(menuData.showFlag);
            AdjustCamera(menuData);
        }

        private void AdjustCamera(JSON_Menu menuData)
        {
            cam.position += -cam.forward * menuData.cameraDistance;
        }
    }
}


