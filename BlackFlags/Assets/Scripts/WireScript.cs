using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WireScript : MonoBehaviour
{
    private LineRenderer _line;
    public static bool detailedTexture = false;
    private void Awake()
    {
//#if UNITY_EDITOR
//        _line = GetComponent<LineRenderer>();
//        _line.positionCount = transform.childCount;
//        for (int i = 0; i < transform.childCount; i++)
//        {
//            _line.SetPosition(i, transform.GetChild(i).position);
//            _line.SetPosition(i, transform.GetChild(i).position);
//        }
//#endif
    }
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _line.positionCount = transform.childCount;
        _line.startWidth = 0.25f;
        _line.endWidth = 0.25f;
        if (detailedTexture)
        {
            _line.sharedMaterial.mainTextureScale = new Vector2(Vector3.Distance(transform.GetChild(0).position, transform.GetChild(transform.childCount - 1).position), 1);
            _line.startWidth *= 5;
            _line.endWidth *= 5;
        }
    }

    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _line.SetPosition(i, transform.GetChild(i).position);
            _line.SetPosition(i, transform.GetChild(i).position);
        }
    }
}
