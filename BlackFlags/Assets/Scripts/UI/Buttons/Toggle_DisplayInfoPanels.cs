using GameSettings.Core;

public class Toggle_DisplayInfoPanels : ToggleSettings
{
    protected override void SetVal()
    {
        PersistentGameSettings._advisorPanel = !isOn;
    }
}
