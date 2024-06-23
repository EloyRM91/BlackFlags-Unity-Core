public class ButtonStartNewGame : ButtonIntroAction
{
    protected override void Action()
    {
        SceneManager.sceneAfterAsynLoad = 3;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
}
