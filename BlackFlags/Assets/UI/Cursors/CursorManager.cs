using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSettings.Core;
using System.IO;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursors;
    public static CursorManager t;

    private void Awake()
    {
        t = this;
    }

    private void Start()
    {
        var mod = PersistentGameSettings.currentMod;
        if (mod != null)
        {
            var modSpritePath = mod.ModPath + "/Data/Cursors/";

            if (Directory.Exists(modSpritePath))
            {
                //Cargamos los sprites del mod
                string[] files = Directory.GetFiles(modSpritePath, "*.png");

                if (files.Length > 1)
                {
                    if (files.Length > 6)
                    {
                        cursors = new Texture2D[files.Length];
                    }
                    for (int i = 0; i < files.Length; i++)
                    {
                        cursors[i] = Resources.Load<Texture2D>(files[i]) as Texture2D;
                    }

                }
            }
        }
    }
    public static void SetCursor(int index)
    {
        Cursor.SetCursor(t.cursors[index], Vector2.zero, CursorMode.Auto);
    }

    public static void HideCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public static void ShowCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

}
