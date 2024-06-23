using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// An in-menu flag image controller class. It sets the flag texture and description.
/// </summary>
public class PirateFlag : MonoBehaviour
{
    //<>
    [SerializeField] private Image _helpImage;
    [SerializeField] private Text _helpText_name, _helpText_date, _helpText_info;
    [SerializeField] private FlagInfo[] info;
    void Start()
    {
        int index = Random.Range(0, 5);
        GetComponent<Renderer>().material.SetTexture("_MainTex", Resources.Load<Texture>("pflag " + index));
        if(_helpImage != null)
            _helpImage.sprite = Resources.Load<Sprite>("pinfo " + index);
        _helpText_name.text = info[index].name;
        _helpText_date.text = info[index].date;
        _helpText_info.text = info[index].des;
    }
}

[System.Serializable]
public class FlagInfo
{
    public string name, date, des;
}
