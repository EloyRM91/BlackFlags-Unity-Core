using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Postprocessing
using UnityEngine.Rendering.PostProcessing;

public class VFXController1 : MonoBehaviour
{
    [SerializeField] private PostProcessVolume volume;
    private DepthOfField depth;

    void Awake()
    {
        depth = volume.profile.GetSetting<DepthOfField>();
    }
    private void OnEnable()
    {
        depth.aperture.value = 0;
        StartCoroutine("Focus");
    }

    IEnumerator Focus()
    {
        while(depth.aperture.value < 1.22f)
        {
            depth.aperture.value += 0.8f * Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(0.1f);
        }
    }
}
