using UnityEngine;

//Mixers
using UnityEngine.Audio;

//Reading/Writing
using System.IO;
using System.Text;

public class PersistentAudioSettings : MonoBehaviour
{
    #region VARIABLES
    //Singleton
    private static PersistentAudioSettings instance;

    //Audio mixer
    [SerializeField] private AudioMixer masterMixer;

    //Reading/Writing
    private readonly string path = "AudioSettings.txt";

    //Exposed audio mixers parameters
    private string[] parameterNames = new string[] {"Volume_Music", "Volume_Ambience", "Volume_Effects", "Volume_Voices"};
    public static float[] parameterValues = new float[4];
#endregion
    void Awake()
    {
        //Seat
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        GetSettings();
    }

    private void OnDisable()
    {
        WriteSettings();
    }

    private void OnDestroy()
    {
        WriteSettings();
    }

    private void ReadMixerParameters()
    {
        for (int i = 0; i < 4; i++)
            masterMixer.GetFloat(parameterNames[i], out parameterValues[i]);
    }

    //Reading / Writing
#region FILE R/W
    private void GetSettings()
    {
        if (File.Exists(path))
        {
            StreamReader reader = new StreamReader(path);
            int i = 0;
            while (!reader.EndOfStream)
            {
                string[] substrings = reader.ReadLine().Split(' ');
                var val = substrings[1];
                masterMixer.SetFloat(parameterNames[i], float.Parse(val));
                i++;
            }
            reader.Close();
            ReadMixerParameters();
        }
    }
    private void WriteSettings()
    {
        FileStream fs = File.Create(path);

        string txtFile = string.Empty;
        for (int i = 0; i < parameterNames.Length; i++)
        {
            masterMixer.GetFloat(parameterNames[i], out float value);
            txtFile += $"{(i > 0 ? "\n" : "")}{parameterNames[i]} {value}";
        }

        byte[] info = new UTF8Encoding(true).GetBytes(txtFile);
        fs.Write(info, 0, info.Length);
        fs.Close();
    }

    public static void SetParameterST(int index, float val)
    {
        instance.SetParameter(index, val == -30 ? -80 : val);
    }

    private void SetParameter(int index, float val)
    {
        masterMixer.SetFloat(parameterNames[index], val);
        parameterValues[index] = val;
    }

    public static void SaveAudioSettings()
    {
        instance.WriteSettings();
    }
#endregion
}
