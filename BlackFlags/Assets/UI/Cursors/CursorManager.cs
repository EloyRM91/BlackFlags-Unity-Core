using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D[] cursors;
    public static CursorManager t;

    private void Awake()
    {
        t = this;
    }
    public static void SetCursor(int index)
    {
        Cursor.SetCursor(t.cursors[index], Vector2.zero, CursorMode.Auto);
    }

}
