using UnityEngine;
using UnityEngine.UI;

public class TavernScenic : MonoBehaviour
{
    [SerializeField]
    private Image
        _IMG_shadows_GLOBAL,
        _IMG_shadows_CHARACTERS,
        _IMG_shadows_OBJECTS,
        _IMG_Lights_AMBIENCE,
        _IMG_Light_CANDLES, _IMG_Light_CANDLES2,
        _IMG_Light_DETAILS1, _IMG_Light_DETAILS2;
    [SerializeField]
    private float
        _freq1_Inn, _freq2_Inn, _freq1_Ext, _freq2_Ext, //Armonic1, armonic2, ambience sin
        _amp1, _amp2, _amp3, _amp4;
    private float _candleEffectValue, _ambienceEffectValue;
    void Start()
    {
        
    }
    void FixedUpdate()
    {
        _candleEffectValue = _amp1 * Mathf.Sin(_freq1_Inn * Time.time) + _amp2 * Mathf.Sin(_freq2_Inn * Time.time);
        _ambienceEffectValue = _amp3 * Mathf.Sin(_freq1_Ext * Time.time) + _amp4 * Mathf.Sin(_freq2_Ext * Time.time);

        //Mobile Shadows
        _IMG_shadows_CHARACTERS.transform.localScale += Vector3.one * _candleEffectValue/17;
        _IMG_shadows_OBJECTS.transform.localScale += Vector3.one * _candleEffectValue / 27;

        //Illumination - Background
        _IMG_shadows_GLOBAL.color += new Color(0, 0, 0, -_candleEffectValue / 12 - _ambienceEffectValue / 12);
        _IMG_Lights_AMBIENCE.color += new Color(0, 0, 0, _ambienceEffectValue / 3f);
        _IMG_Light_CANDLES.color += new Color(0, 0, 0, _candleEffectValue / 1.3f);
        _IMG_Light_CANDLES2.color -= new Color(0, 0, 0, 1.3f* _candleEffectValue / 1.1f);

        //Illumination - front
        if(_IMG_Light_DETAILS1.isActiveAndEnabled)
            _IMG_Light_DETAILS1.color += new Color(0, 0, 0, _candleEffectValue / 1.1f);
        if (_IMG_Light_DETAILS2.isActiveAndEnabled)
            _IMG_Light_DETAILS2.color += new Color(0, 0, 0, _candleEffectValue / 1.1f);
    }
}
