using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameSettings.Core;

public class UIAdvisorPanel : MonoBehaviour
{
    //------------------------------- Tips
    private static readonly Dictionary<string, HelpTip> _D_TIPS = new Dictionary<string, HelpTip>()
    {
        {
            "NatPort",
            new HelpTip("Has atracado en un puerto natural. Estos puertos actúan como escondite frente a corsarios o patrullas y en ellos se puede carenar el barco para pequeñas reparaciones.\n\nEncontrarás estos escondites a lo largo y ancho del mundo.")
        },
        {
            "Tavern",
            new HelpTip("")
        }
    };

    //------------------------------- Components
    private static GameObject _PANEL_advisor;
    private static Text _TEXT_body;

    void Start()
    {
        //references
        _PANEL_advisor = transform.GetChild(0).gameObject;
        _TEXT_body = _PANEL_advisor.transform.GetChild(0).GetComponent<Text>();
        Invoke("PreDelay", 0.2f);
        
    }
    private void PreDelay()
    {
        Invoke("Delay", 2);
    }

    private void Delay()
    {
        OpenTip("NatPort");
    }

    private void OpenTip(string key)
    {
        //Must the game show the advisor Panel according to settings?
        if (PersistentGameSettings._advisorPanel)
        {
            //Access to tip by key
            var tip = _D_TIPS[key];
            //Has this tip been shown before?
            if (!tip.alreadyShownThisTip)
            {
                //Open panel
                _PANEL_advisor.SetActive(true);
                //Modify tip's text
                _TEXT_body.text = tip.text;
                //This this now has been shown
                tip.alreadyShownThisTip = true;
            }
        }
    }


}
public class HelpTip
{
    public string text;
    public AudioClip audio;
    public bool alreadyShownThisTip;

    public HelpTip(string txt)
    {
        text = txt;
    }
}
