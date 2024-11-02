public class ButtonStartNewGame : ButtonIntroAction
{
    protected override void Action()
    {
        SceneManager.SetAsynSceneAndLoadAsyn(3);
    }
}
