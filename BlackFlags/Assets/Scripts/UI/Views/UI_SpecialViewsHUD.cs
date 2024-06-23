using UnityEngine;
using System.Collections;
//UI
using UnityEngine.UI;
//Tweening
using DG.Tweening;
//Mechanics
using GameMechanics.Data;
using GameMechanics.Sound;

public class UI_SpecialViewsHUD : MonoBehaviour
{
#region VARIABLES
    //Ref
    [SerializeField] GameObject[] mapUI;

    //Components -- GLOBAL
    private static Image fadePanel;
    private static GameObject UI_ShipFull3D, UI_TavernView, UI_TavernDetails;
    private static Image observerFiller;

    //Components -- TAVERN
    [Header("TAVERN")]
    private static GameObject UI_TAVERN_Header;
    private static GameObject UI_TAVERN_MainPanel;
    private static GameObject UI_TAVERN_CharactersPanel;
    private static GameObject UI_TAVERN_CharacterActions;
    private GameObject UI_TAVERN_CharacterActionsHeader;
    private GameObject
        UI_TAVERN_SmugglerActions,
        UI_TAVERN_PirateActions;

    //Data
    private static Color _COLOR_off = new Color(0, 0, 0, 0);

    //Singleton
    private static UI_SpecialViewsHUD instance;

    //Events
    public delegate void VoidEvent(bool response, float timeResponse);
    public static event VoidEvent EndOfView;

#endregion

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        //Fade panel
        fadePanel = transform.GetChild(2).GetComponent<Image>();

        //Components -- 3D VIEW
        UI_ShipFull3D = transform.GetChild(0).gameObject;
        observerFiller = UI_ShipFull3D.transform.GetChild(2).GetComponent<Image>();

        //Components -- TAVERN
        UI_TavernView = transform.GetChild(1).gameObject;
        UI_TavernDetails = UI_TavernView.transform.GetChild(1).gameObject;
        UI_TAVERN_Header = UI_TavernView.transform.GetChild(3).gameObject;
        UI_TAVERN_MainPanel = UI_TavernView.transform.GetChild(5).gameObject;
        UI_TAVERN_CharactersPanel = UI_TavernView.transform.GetChild(7).gameObject;
        UI_TAVERN_CharacterActions = UI_TavernView.transform.GetChild(8).gameObject;
        UI_TAVERN_CharacterActionsHeader = UI_TAVERN_CharacterActions.transform.GetChild(0).gameObject;
        UI_TAVERN_SmugglerActions = UI_TAVERN_CharacterActions.transform.GetChild(1).gameObject;
        UI_TAVERN_PirateActions = UI_TAVERN_CharacterActions.transform.GetChild(2).gameObject;

        //Events
        DialogManager.ActionByDialog += SetDialogAction;
    }

    private void OnDestroy()
    {
        //Events
        DialogManager.ActionByDialog -= SetDialogAction;
    }

#region FADE PANEL

    public static void FadeOn(float val)
    {
        fadePanel.gameObject.SetActive(true);
        fadePanel.color = _COLOR_off;
        fadePanel.DOFade(1, val).SetUpdate(true); ;
    }
    public static void FadeOff(float val)
    {
        //Sequence
        Sequence sequence = DOTween.Sequence().SetUpdate(true);
        sequence.Append(fadePanel.DOFade(0, val).SetUpdate(true));
        sequence.AppendInterval(1.1f);
        sequence.AppendCallback(() => { fadePanel.gameObject.SetActive(true); });
        sequence.Play();
    }

#endregion

#region DIALOG ACTIONS

    private void SetDialogAction(DialogTrigger dialogAction)
    {
        switch (dialogAction)
        {
            case DialogTrigger.dialogTavern_NOEVENT:
                Action_NOACTION(); break;
            case DialogTrigger.dialogTalkWithSmuggler:
                Action_TALK_SMUGGLER(); break;
            case DialogTrigger.dialogTalkWithPirate:
                Action_TALK_PIRATE(); break;
        }
    }
    private void Action_NOACTION()
    {
        UI_TAVERN_MainPanel.SetActive(true);
    }
    private void Action_TALK_SMUGGLER()
    {
        //¿Hablo con el contrabandista en una taberna?
        if (UI_TavernView.activeSelf)
        {
            //Desactiva cabecera de la interfaz
            UI_TAVERN_Header.SetActive(false);
            //Activa panel de personaje
            UI_TAVERN_CharacterActions.SetActive(true);
            //Activa acciones con este personaje
            UI_TAVERN_SmugglerActions.SetActive(true);
        }
        //¿Hablo con el contrabandista en un escondite de contrabando?
        else
        {

        }
    }

    private void Action_TALK_PIRATE()
    {
        //¿Hablo con el pirata en una taberna?
        if (UI_TavernView.activeSelf)
        {
            //Desactiva cabecera de la interfaz
            UI_TAVERN_Header.SetActive(false);
            //Activa panel de personaje
            UI_TAVERN_CharacterActions.SetActive(true);
            //Activa acciones con este personaje
            UI_TAVERN_PirateActions.SetActive(true);
        }
    }
#endregion
    public static void StartFullShipView()
    {
        MatController.RunTiming();
        Ambience.TransitionToAmbience(true, AmbientType.OnSea);

        //Sequence
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { FadeOn(0.5f); });
        sequence.AppendInterval(.7f);
        sequence.AppendCallback(() => { SetUIMode_ST(false); Cameras.Full3DMode(true); });
        sequence.AppendCallback(() => { UI_ShipFull3D.SetActive(true); });
        sequence.AppendCallback(() => { FadeOff(.7f); });
        sequence.AppendCallback(() => { _ExtendedViewPause(true); });
        sequence.AppendCallback(() => { instance.StartCoroutine("ObservationFiller"); });
        sequence.Play();
    }
    public static void EndFullShipView()
    {
        Ambience.TransitionToAmbience(false, AmbientType.OnSea);
        TimeManager.NormalizeST();

        //Sequence
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { FadeOn(0.5f); });
        sequence.AppendInterval(.6f);
        sequence.AppendCallback(() => { Cameras.Full3DMode(false); SetUIMode_ST(true); });
        sequence.AppendCallback(() => { UI_ShipFull3D.SetActive(false); });
        sequence.AppendCallback(() => { FadeOff(.7f); });
        sequence.AppendCallback(() => { _ExtendedViewPause(false);});
        sequence.Play();

    }
    public static void SetUIMode_ST(bool val)
    {
        instance.SetUIMode(val);
    }
    private void SetUIMode(bool val)
    {
        for (int i = 0; i < mapUI.Length; i++)
        {
            mapUI[i].SetActive(val);
        }
    }
    private static void _ExtendedViewPause(bool val)
    {
        GameManager.forcedPause = val;
        if (val)
        {
            Time.timeScale = 0;
        }
    }

    //Observation Filler
    IEnumerator ObservationFiller()
    {
        //Restart filler
        observerFiller.gameObject.SetActive(true);
        observerFiller.fillAmount = 0;
        //Progressive filling
        while (observerFiller.fillAmount < 1)
        {
            observerFiller.fillAmount += 0.25f * Time.unscaledDeltaTime;
            yield return new WaitForEndOfFrame();
        }
        //Deactivate object
        observerFiller.gameObject.SetActive(false);
    }

    public static void StartTavernView()
    {
        MapCamera.isLocked = true;
        PlayerMovement.canMove = false;
        TimeManager.NormalizeST();
        FadeOn(0.6f);

        //Sequence
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { FadeOn(0.5f); });
        sequence.AppendInterval(.7f);
        sequence.AppendCallback(() => { SetUIMode_ST(false); Cameras.ScenicViewMode(true); });
        sequence.AppendCallback(() => { UI_TavernView.SetActive(true); UI_TAVERN_MainPanel.SetActive(true); UI_TAVERN_Header.SetActive(true); });
        sequence.AppendCallback(() => { Ambience.TransitionToAmbience(true, AmbientType.OnTavern); });
        sequence.AppendCallback(() => { FadeOff(.7f); });
        sequence.Play();
    }

    public static void EndTavernView()
    {
        TimeManager.NormalizeST();
        PlayerMovement.canMove = true;

        //Sequence
        Sequence sequence = DOTween.Sequence();
        sequence.AppendCallback(() => { FadeOn(0.5f); });
        sequence.AppendInterval(.7f);
        sequence.AppendCallback(() => { Cameras.ScenicViewMode(false); SetUIMode_ST(true); });
        sequence.AppendCallback(() => { UI_TavernView.SetActive(false); SetTavernViewDetails(); });
        sequence.AppendCallback(() => { FadeOff(.7f); EndOfView(false, 0); });
        sequence.InsertCallback(1.6f, () => { Ambience.TransitionToAmbience(false, AmbientType.OnTavern); MapCamera.isLocked = false; });
        sequence.Play();
    }

    private static void SetTavernViewDetails()
    {
        UI_TavernDetails.transform.GetChild(0).gameObject.SetActive(false);
        UI_TavernDetails.transform.GetChild(1).gameObject.SetActive(false);
        UI_TavernDetails.transform.GetChild(Random.Range(0,2)).gameObject.SetActive(true);
    }
    public static void ClearTavernUI_ST()
    {
        instance.ClearTavernUI();
    }
    private void ClearTavernUI()
    {
        UI_TAVERN_MainPanel.SetActive(false);
        UI_TAVERN_CharactersPanel.SetActive(false);
    }
}
