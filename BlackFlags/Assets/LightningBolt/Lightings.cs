using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Lightings : MonoBehaviour
{
    private GameObject rayo;
    public Light dirLight;
    public Material skyboxMat, waterMat;

    private Tween tween;

    public Color refColor, flashColor;

    void OnEnable()
    {
        StartCoroutine("Lighting");
        skyboxMat.SetColor("_Tint", refColor);
        waterMat.SetFloat("_Darkness", 0.327f);
    }

    void Start()
    {
        rayo = transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        var p = skyboxMat.GetFloat("_Rotation");
        if (p >= 360) skyboxMat.SetFloat("_Rotation", 0);
        skyboxMat.SetFloat("_Rotation", p + 1.5f * Time.deltaTime);
    }

    private void Thunder()
    {
        tween.Kill();
        dirLight.intensity = 4f;
        tween = dirLight.DOIntensity(0.69f, 0.2f);
        skyboxMat.SetColor("_Tint", flashColor);
        Invoke("Off", 0.2f);
    }

    private void Off()
    {
        skyboxMat.SetColor("_Tint", refColor);
    }

    IEnumerator Lighting()
    {
        yield return new WaitForSeconds(1);
        while (true)
        {
            Thunder();
            rayo.SetActive(true);
            rayo.transform.position = transform.GetChild(Random.Range(1,4)).position;
            yield return new WaitForSeconds(Random.Range(0.15f, 0.3f));
            rayo.SetActive(false);
            yield return new WaitForSeconds(Random.Range(8,12));
        }
    }

}
