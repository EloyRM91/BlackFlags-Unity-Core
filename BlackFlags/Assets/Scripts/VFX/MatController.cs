using System.Collections;
using UnityEngine;
using GameMechanics.Data;
/// <summary>
/// This class adjusts some material's behaviour
/// </summary>
public class MatController : MonoBehaviour
{
    //instance
    private static MatController instance;

    //Materiales
    [Header("Materials: Water")]
    [SerializeField] private Material waterMat1;

    [Header("Materials: Sails")]
    [SerializeField] private Material[] Sails;

    [Header("Materials: Flags")]
    [SerializeField] private Material[] flags;

    //TimeOffset
    private float offset = 0;

    //Parameters
    public static bool running = true, isShowingFlags = true;

    private void Awake()
    {
        //Singleton
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this);
    }
    void Start()
    {
        //Events
        //TimeManager.onTimeScaleChange += SetTimeScaleOnMaterials;
        StartCoroutine("UnscaledBehaviour");
    }

    private void OnDestroy()
    {
        //Events
        //TimeManager.onTimeScaleChange -= SetTimeScaleOnMaterials;
    }

    IEnumerator UnscaledBehaviour()
    {
        while (true)
        {
            //Time Offset
            offset += 0.05f * Time.unscaledDeltaTime;
            //Water materials
            waterMat1.SetFloat("_TimeOffset", offset);
            //Sails
            for (int i = 0; i < Sails.Length; i++)
            {
                Sails[i].SetFloat("_TimeOffset", offset);
            }
            //flags
            if (isShowingFlags)
            {
                for (int i = 0; i < flags.Length; i++)
                {
                    flags[i].SetFloat("_TimeOffset", offset);
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    IEnumerator ScaledDeltaTime()
    {
        while (true)
        {
            //Time Offset
            offset += 0.05f * Time.deltaTime;
            //Water materials
            waterMat1.SetFloat("_TimeOffset", offset);
            //Sails
            for (int i = 0; i < Sails.Length; i++)
            {
                Sails[i].SetFloat("_TimeOffset", 100 * offset);
            }
            //flags
            for (int i = 0; i < flags.Length; i++)
            {
                flags[i].SetFloat("_TimeOffset", offset);
            }
            yield return new WaitForEndOfFrame();
        }
    }

    //External calls
    public static void RunTiming(bool unscaled = true)
    {
        if (!running)
        {
            running = !running;
            instance.StartCoroutine(unscaled ? "UnscaledBehaviour" : "ScaledDeltaTime");
        }
    }
    public static void StopTiming()
    {
        running = false;
        instance.StopAllCoroutines();
    }

}
