using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace GameSettings.Mods
{
    public class TestJsonModding : MonoBehaviour
    {
        private string[] quotes =
        {
        "WHERE IS THE RUM GONE?! >:@ - Jack Sparrow",
        "La religión es el opio de la sociedad, pero en Gran Bretaña el opio de la sociedad es literalmente opio",
        "Este juego puede provocar epilepsia. O tal vez no. No lo sé, soy una pantalla de carga"
        };

        void Start()
        {
            //JSON_Loading loading = new JSON_Loading(quotes, true, true);
            //File.WriteAllText("loading.json", JsonUtility.ToJson(loading, true));

            //JSON_Menu menu = new JSON_Menu();
            //File.WriteAllText("menu.json", JsonUtility.ToJson(menu, true));




            //var group = new FontStylesGroup[7]
            //{
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(0, 80),new SerializableFontStyle(0, 85), new SerializableFontStyle(2, 75) }),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(0, 50),new SerializableFontStyle(0, 55), new SerializableFontStyle(2, 42)}),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(1, 40),new SerializableFontStyle(1, 45), new SerializableFontStyle(2, 35)}),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(1, 22),new SerializableFontStyle(1, 26), new SerializableFontStyle(2, 21)}),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(1, 27),new SerializableFontStyle(1, 30), new SerializableFontStyle(2, 26)}),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(0, 67),new SerializableFontStyle(0, 75), new SerializableFontStyle(2, 60)}),
            //    new FontStylesGroup(new SerializableFontStyle[3]{ new SerializableFontStyle(1, 35),new SerializableFontStyle(1, 37), new SerializableFontStyle(2, 26)})
            //};

            //JSON_Fonts jsonFonts = new JSON_Fonts("Cardinal", "PiratesBay", "sylfaen", group, true);
            //File.WriteAllText("fonts.json", JsonUtility.ToJson(jsonFonts, true));
        }

    }
}

