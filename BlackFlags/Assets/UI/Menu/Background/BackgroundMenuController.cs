using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMenuController : MonoBehaviour
{
    [SerializeField] private Texture[] imgs;
    public Material bgMaterial;

    //void Start()
    //{
    //    bgMaterial = GetComponent<Image>()?.material;

    //    if(bgMaterial == null)
    //    {
    //        bgMaterial = GetComponent<MeshRenderer>()?.materials[0];
    //    }
    //}

    void Start()
    {
        bgMaterial = GetComponent<Image>().material = bgMaterial;
    }

    void LateUpdate()
    {
        //bgMaterial.SetFloat("_Opacity", Mathf.Round(Mathf.Cos(Time.time) * 128 + 128));
        //print(Mathf.Round(Mathf.Cos(Time.time) * 128 + 128));
    }

    IEnumerator DynamicBackground()
    {
        yield return null;
    }
}
