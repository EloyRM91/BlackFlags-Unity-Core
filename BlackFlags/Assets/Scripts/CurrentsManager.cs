using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentsManager : MonoBehaviour
{
    //[SerializeField] private GameObject[] _arrowsPooling;
    [SerializeField] private GameObject _PREF_currentArrow;
    [SerializeField] private Material[] _colors;
    //private static readonly Color[] _colors = new Color[] {
    //    new Color(1,1,1,0.6f),
    //    new Color(0.576f,0.843f,0.313f,0.7f),
    //    new Color(0.204f,0.49f,0.176f,0.7f),
    //    new Color(0.909f,0.643f,0,0.7f),
    //    new Color(0.949f, 0.2f,0.2f, 0.7f),
    //    new Color(0.566f, 0.05f, 0.05f, 0.7f)
    //};

    void Start()
    {
        CreateArrows();
        DrawCurrentsMap();
    }

    private void CreateArrows()
    {
        var currentsMapData = GameManager.gm.CurrentsMapData;
        var required = (currentsMapData.tilesX * currentsMapData.tilesZ) - transform.childCount;

        if (required <= 0)
            return;

        for (int i = 0; i < required; i++)
        {
            var newArrow = Instantiate(_PREF_currentArrow, transform);
            newArrow.SetActive(false);
        }
    }

    private void DrawCurrentsMap(bool recovery = false)
    {
        var currentsMapData = GameManager.gm.CurrentsMapData;
        var tx = currentsMapData.tilesX;
        var tz = currentsMapData.tilesZ;

        var tileSizeX = currentsMapData.tileSizeX;
        var tileSizeZ = currentsMapData.tileSizeZ;

        for (int j = 0; j < tz; j++)
        {
            for (int i = 0; i < tx; i++)
            {
                var tileData = currentsMapData.arrows[j,i];
                try
                {
                    var currentObject = transform.GetChild(j * (int)tx + i).gameObject;

                    if (tileData.dragStrength > 0)
                        currentObject.SetActive(true);



                    //Establece posición del objeto:
                    float x = currentsMapData.boundingBox.minX + tileSizeX * (i + 0.5f);
                    float z = currentsMapData.boundingBox.minZ + tileSizeZ * (j + 0.5f);

                    currentObject.transform.position = new Vector3(x, 0.09f, z);

                    //Establece color de la flecha:
                    var renderer = currentObject.GetComponent<SpriteRenderer>();
                    if (tileData.dragStrength > 0) 
                        renderer.material = _colors[tileData.dragStrength - 1];

                    //Establece dirección de la flecha:
                    print((tileData.direction + 1) * 45);
                    currentObject.transform.localEulerAngles = new Vector3(0, 0, (tileData.direction - 1) * 45);
                }
                catch(System.Exception e)
                {
                    if(recovery)
                        throw e;

                    CreateArrows();
                    DrawCurrentsMap(true);
                }
                
            }
        }
    }

}
