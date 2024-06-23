using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void BackToMenu()
    {
        SceneManager.SetAsynSceneAndLoadAsyn(2);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
