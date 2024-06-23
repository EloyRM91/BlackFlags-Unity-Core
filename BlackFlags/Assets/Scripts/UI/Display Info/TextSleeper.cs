using UnityEngine.UI;

public class TextSleeper : Text
{
    protected override void Start()
    {
        base.Start();
    }
    protected override void OnDisable()
    {
        text = "  ";
        //UnityEngine.Canvas.ForceUpdateCanvases();
    }
}
