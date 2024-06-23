public class ButtonFlagSetter : ButtonIntroAction
{
    protected override void Action()
    {
        PanelFade.Next(panelTo);
        transform.parent.GetChild(0).GetChild(0).GetComponent<FlagMaker>().SetFlagImage();
        
    }
}
