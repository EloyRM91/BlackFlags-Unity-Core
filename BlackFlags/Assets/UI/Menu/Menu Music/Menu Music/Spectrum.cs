using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Spectrum : MonoBehaviour
{
    [SerializeField] private AudioSource _musicAS;
    private float[] spectrum = new float[64];
    private Image[] imgs = new Image[64];

    private void Start()
    {
        for (int i = 0; i < 32; i++)
        {
            imgs[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    private void FixedUpdate()
    {
        _musicAS.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        for (int i = 0; i < 5; i++)
        {
            imgs[i].fillAmount = spectrum[i] * 4;
        }
        for (int i = 4; i < 32; i++)
        {
            imgs[i].fillAmount = spectrum[i] * 10;
        }
    }
}
