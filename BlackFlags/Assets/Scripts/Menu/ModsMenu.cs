using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;

public class ModsMenu : MonoBehaviour
{
    [SerializeField] private Text 
        _TEXT_path,
        _TEXT_description;
    [SerializeField] private ToggleGroup _group;
    [SerializeField] private Image _IMG_Preview;

    private string selectedPath;
    private string SelectedPath
    {
        set
        {
            selectedPath = value;
            StopCoroutine("SelectMod");
            StartCoroutine("SelectMod");
        }
    }

    //Rows List
    [SerializeField] private Transform _LayoutContainer;
    [SerializeField] private GameObject _PREFAB_modRow;


    void Start()
    {
        var path = Directory.GetParent(Application.dataPath).FullName + "\\Mods";
        var button = transform.GetChild(2).GetComponent<Button>();
        _TEXT_path.text = path;

        if(Directory.Exists(path))
        {
            var folders = Directory.GetDirectories(path, "*", SearchOption.TopDirectoryOnly);

            for (int i = 0; i < folders.Length; i++)
            {
                //New row
                var row = Instantiate(_PREFAB_modRow,_LayoutContainer).transform;

                //Text Settings
                var split = folders[i].Split('\\');
                row.GetChild(1).GetComponent<Text>().text = split[split.Length - 1];

                //Toggle settings
                var toggle = row.GetComponent<Toggle>();
                string modPath = folders[i];
                toggle.onValueChanged.AddListener(delegate { if (toggle.isOn) { SelectedPath = modPath; } });
                toggle.group = _group;
            }

            if(folders.Length > 0)
                _LayoutContainer.GetChild(0).GetComponent<Toggle>().isOn = true;
            
            if(folders.Length > 0)
            {
                button.onClick.AddListener(delegate { LoadMod(); });
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    IEnumerator SelectMod()
    {
        var previewPath = selectedPath + "\\preview";
        if (File.Exists(previewPath + ".jpg"))
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(previewPath + ".jpg");
            request.SendWebRequest();
            while(request.result != UnityWebRequest.Result.Success)
            {
                yield return null;
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            var rec = new Rect(0, 0, texture.width, texture.height);
            _IMG_Preview.sprite = Sprite.Create(texture, rec, new Vector2(0, 0));
        }
        else if (File.Exists(previewPath + ".png"))
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(previewPath + ".png");
            request.SendWebRequest();
            while (request.result != UnityWebRequest.Result.Success)
            {
                yield return null;
            }

            var texture = DownloadHandlerTexture.GetContent(request);
            var rec = new Rect(0, 0, texture.width, texture.height);
            _IMG_Preview.sprite = Sprite.Create(texture, rec, new Vector2(0, 0));
        }
        else
        {
            _IMG_Preview.sprite = Resources.Load<Sprite>("Warnings/nopreview");
        }
        previewPath = selectedPath + "\\readme.txt";
        _TEXT_description.text = File.Exists(previewPath) ? File.ReadAllText(previewPath) : "Sin descripción";
    }

    public void LoadMod()
    {
        if(selectedPath != string.Empty)
        {
            //Crea el archivo tempCheckMod.txt
            var wr = new StreamWriter("TempCheckMod.txt");
            wr.WriteLine(selectedPath);
            wr.Close();

            //Reinicia el juego
            System.Diagnostics.Process.Start(Application.dataPath + "/../BlackFlags.exe");
            Application.Quit();
        }
    }
}
