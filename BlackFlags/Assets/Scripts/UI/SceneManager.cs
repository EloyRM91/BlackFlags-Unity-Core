using UnityEngine;

//On Editor
using UnityEditor;

//UI - version reading
using UnityEngine.UI;
using System.IO;

public class SceneManager : MonoBehaviour
{
    [SerializeField] private Sprite tutorialAvatar;
    [SerializeField] private Text _TXT_versionText, _TXT_versionLabel;

    [SerializeField] public GameMechanics.Data.CurrentsMapData data;
    public static int sceneAfterAsynLoad;

    private void Awake()
    {
        MatController.isShowingFlags = false; //En esta escena no se muestran banderas independientes al timescale

        //Pruebas:
        //Serialization.SerializationUtils.createDefaultCurrentsMap();
        //data = Serialization.SerializationUtils.Test();
    }
    public void Quit()
    {
        Application.Quit();
    }

    public static void LoadScene(int v)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(v);
        if(v == 1) print(new System.Diagnostics.StackFrame(1).GetMethod().Name);
    }

    public static void SetAsynSceneIndex(int val)
    {
        sceneAfterAsynLoad = val;
    }
    public static void SetAsynSceneAndLoadAsyn(int val)
    {
        sceneAfterAsynLoad = val;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Loading");
    }
    public void IsTutorial(bool val)
    {
        PersistentGameData.SetAsTutorial(val);
        if(val) PersistentGameData._GData_PlayerAvatar = tutorialAvatar;

    }

    //---- MENU ITEM
#if UNITY_EDITOR
    [MenuItem("Escenas/Menú")]
    static void OpenMenu()
    {
        //Open the Scene in the Editor (do not enter Play Mode)
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/Menu.unity");
    }
    [MenuItem("Escenas/Nivel Mundo")]
    static void OpenLevel()
    {
        //Open the Scene in the Editor (do not enter Play Mode)
        UnityEditor.SceneManagement.EditorSceneManager.OpenScene("Assets/Scenes/SampleScene.unity");
    }
#endif

    //---- GET VERSION

    public void GetVersionText()
    {
        if(_TXT_versionText.text == string.Empty)
        {
            try
            {
#if UNITY_EDITOR
            
#endif
                StreamReader sr = new StreamReader("version.txt");
                string line = sr.ReadLine();
                if(line != null)
                {
                    _TXT_versionLabel.text += line;
                    _TXT_versionText.text = "Cambios en esta versión: \n\n";

                    while ((line = sr.ReadLine()) != null)
                    {
                        _TXT_versionText.text += line + "\n";
                    }
                }
                
            }
            catch (System.Exception e)
            {
                print("The file could not be read: " + e.Message);
            }
        }
    }
}
