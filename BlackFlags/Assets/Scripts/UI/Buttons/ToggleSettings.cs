using UnityEngine;
using UnityEngine.UI;

public abstract class ToggleSettings : Toggle
{
    protected override void Start()
    {
        // onValueChanged.AddListener(delegate { PersistentGameSettings.SetValue<ToggleSettings>(isOn); }); no sabe
        onValueChanged.AddListener(delegate { SetVal(); });
    }
    protected abstract void SetVal();
}
