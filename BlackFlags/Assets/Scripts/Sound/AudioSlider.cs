using UnityEngine;
using UnityEngine.UI;

public class AudioSlider : MonoBehaviour
{
    [SerializeField] private byte sliderIndex;
    private Slider thisSlider;
    void Start()
    {
        thisSlider = GetComponent<Slider>();
        thisSlider.value = PersistentAudioSettings.parameterValues[sliderIndex];

        thisSlider.onValueChanged.AddListener(delegate { PersistentAudioSettings.SetParameterST(sliderIndex, thisSlider.value); });
    }
}
