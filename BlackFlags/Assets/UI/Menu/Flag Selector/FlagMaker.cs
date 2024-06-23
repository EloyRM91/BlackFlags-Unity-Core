using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.CoreModule;

/// <summary>
/// The Flagmaker class manages the player's flag design and create new textures from sprites.
/// </summary>
public class FlagMaker : MonoBehaviour
{
    // Data
    private int currentConfig;
    private int _index1, _index2;

    //Mode Selecion
    [SerializeField] private Transform _selectionBox, _box1, _box2;

    //Swipe
    [SerializeField] private Sprite bottomFlag;
    [SerializeField] private Texture[] _a1, _a2, _b1, _b2;
    [SerializeField] private Sprite[] _a1_Preview, _a2_Preview, _b1_Preview, _b2_Preview;
    [SerializeField] private Image[] images;

    //Scenic Flag
    [SerializeField] private Renderer _flagRenderer;

    //CReate and Serialize Texture
    Texture2D selected1, selected2;
    [SerializeField] private Sprite mask, flagMask;

    //Render Texture Cameras
    [SerializeField] private Camera FlagCamera1, FlagCamera2;

    //Events
    //public delegate void NewFlag(Sprite flag);
    //public static event NewFlag _EVENT_Newflag;

    void Start()
    {
        Randomize();
    }
    public void Swipe1(int val)
    {
        switch (currentConfig)
        {
            case 0:
                if (val == 1 && _index1 == _a1.Length - 1)
                    _index1 = 0;
                else if (val == -1 && _index1 == 0)
                    _index1 = _a1.Length - 1;
                else
                    _index1 += val;
                images[0].sprite = _a1_Preview[_index1];
                _flagRenderer.materials[0].SetTexture("_MainTex",_a1[_index1]);
                break;
            case 1:
                if (val == 1 && _index1 == _b1.Length - 1)
                    _index1 = 0;
                else if (val == -1 && _index1 == 0)
                    _index1 = _b1.Length - 1;
                else
                    _index1 += val;
                images[0].sprite = _b1_Preview[_index1];
                _flagRenderer.materials[0].SetTexture("_MainTex", _b1[_index1]);
                break;
        }
        SetFlag();
    }
    public void Swipe2(int val)
    {
        switch (currentConfig)
        {
            case 0:
                if (val == 1 && _index2 == _a2.Length - 1)
                    _index2 = 0;
                else if (val == -1 && _index2 == 0)
                    _index2 = _a2.Length - 1;
                else
                    _index2 += val;
                images[1].sprite = _a2_Preview[_index2];
                _flagRenderer.materials[1].SetTexture("_MainTex", _a2[_index2]);
                break;
            case 1:
                if (val == 1 && _index2 == _b2.Length - 1)
                    _index2 = 0;
                else if (val == -1 && _index2 == 0)
                    _index2 = _b2.Length - 1;
                else
                    _index2 += val;
                images[1].sprite = _b2_Preview[_index2];
                _flagRenderer.materials[1].SetTexture("_MainTex", _b2[_index2]);
                break;     
        }
        SetFlag();
    }
    public void SetMode(int val)
    {
        if(currentConfig != val)
        {
            currentConfig = val;
            _index1 = 0;
            _index2 = 0;

            _selectionBox.position = val == 1 ? _box2.position : _box1.position;

            images[0].sprite = val == 0 ? _a1_Preview[0] : _b1_Preview[0];
            images[1].sprite = val == 0 ? _a2_Preview[0] : _b2_Preview[0];

            SetFlag();
        }
    }
    /// <summary>
    /// Show a random flag design
    /// </summary>
    public void Randomize()
    {
        currentConfig = Random.Range(0, 6) > 3 ? 1 : 0;
        _selectionBox.position = currentConfig == 1 ? _box2.position : _box1.position;
        _index1 = currentConfig == 0 ? Random.Range(0, _a1.Length) : Random.Range(0, _b1.Length);
        _index2 = currentConfig == 0 ? Random.Range(0, _a2.Length) : Random.Range(0, _b2.Length);
        SetFlag();
        SetPreview();
    }
    private void SetPreview()
    {
        switch (currentConfig)
        {
            case 0:
                images[0].sprite = _a1_Preview[_index1];
                images[1].sprite = _a2_Preview[_index2];
                break;
            case 1:
                images[0].sprite = _b1_Preview[_index1];
                images[1].sprite = _b2_Preview[_index2];
                break;
        }
    }
    private void SetFlag()
    {
        switch (currentConfig)
        {
            case 0:
                _flagRenderer.materials[0].SetTexture("_MainTex", _a1[_index1]);
                _flagRenderer.materials[1].SetTexture("_MainTex", _a2[_index2]);
                break;
            case 1:
                _flagRenderer.materials[0].SetTexture("_MainTex", _b1[_index1]);
                _flagRenderer.materials[1].SetTexture("_MainTex", _b2[_index2]);
                break;
        }
        selected1 = currentConfig == 0 ? (Texture2D)_a1[_index1] : (Texture2D)_b1[_index1];
        selected2 = currentConfig == 0 ? (Texture2D)_a2[_index2] : (Texture2D)_b2[_index2];
    }

    //--------------------------------
    // CREATING AN IMAGE FILE WITH CUSTOM FLAG
    //--------------------------------

    public void SetFlagImage()
    {
        //Avatar
        var newTex = new Texture2D(300, 300);
        var maskTex = mask.texture;
        for (int i = 0; i < 300; i++)
            for (int j = 0; j < 300; j++)
            {
                newTex.SetPixel(i, j, maskTex.GetPixel(i, j));
            }
        for (int i = 0; i < 300; i++)
            for (int j = 0; j < 300; j++)
            {
                if (maskTex.GetPixel(i, j).a != 0)
                {
                    var p = selected1.GetPixel(i + 140, j + 120);
                    if (p.a != 0) newTex.SetPixel(i, j, p);
                    else
                    {
                        p = selected2.GetPixel(i + 140, j + 120);
                        if (p.a != 0) newTex.SetPixel(i, j, p);
                    }
                }
            }
        newTex.Apply();
        var avatar = Sprite.Create(newTex, new Rect(0, 0, 300, 300), new Vector2(0, 0), 1);
        PersistentGameData._GData_PlayerAvatar = avatar;

        //Botón personalizado
        newTex = new Texture2D(600, 400);
        maskTex = flagMask.texture;

        for (int i = 0; i < 600; i++)
            for (int j = 0; j < 400; j++)
            {
                newTex.SetPixel(i, j, maskTex.GetPixel(i, j));
            }
        for (int i = 0; i < 600; i++)
        {
            var distortionY = (int)(18 * Mathf.Sin((float)i/40));

            for (int j = 0; j < 400; j++)
            {
                if (maskTex.GetPixel(i, j).a != 0)
                {
                    var p = selected1.GetPixel(i + 90, j + 80 + distortionY);
                    if (p.a != 0)
                    {
                        newTex.SetPixel(i, j, p.a == 1 ? p : new Color(p.r * p.a, p.g * p.a, p.b * p.a));
                    }

                    else
                    {
                        p = selected2.GetPixel(i + 90, j + 80 + distortionY);
                        if (p.a != 0)
                        {
                            newTex.SetPixel(i, j, p.a == 1 ? p : new Color(p.r * p.a, p.g * p.a, p.b * p.a));
                        }
                    }
                }
            }
        }
            
        newTex.Apply();
        avatar = Sprite.Create(newTex, new Rect(0, 0, 600, 400), new Vector2(0, 0), 1);
        PersistentGameData._GData_PlayerFlag = avatar;
    }
    public static void WriteFlagImage(Sprite sprite, string path = "SavedImg.png")
    {
        byte[] tex2bytes = sprite.texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../" + path, tex2bytes);
    }
    public static void WriteFlagImage(Texture2D tex, string path = "SavedImg.png")
    {
        byte[] tex2bytes = tex.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../" + path, tex2bytes);
    }
    public static void WriteFlagImage(string path = "SavedImg.png")
    {
        byte[] tex2bytes = PersistentGameData._GData_PlayerFlag.texture.EncodeToPNG();
        File.WriteAllBytes(Application.dataPath + "/../" + path, tex2bytes);
    }

}
