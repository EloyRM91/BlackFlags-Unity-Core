using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameMechanics.Data;
using DG.Tweening;
public class WorldEvents : MonoBehaviour
{
    [SerializeField] private Sprite[] _EventSprites;
    [Header ("Registro de Eventos del Mundo")]
    [SerializeField] private Transform _TR_WorldEventsContainer;
    [SerializeField] private GameObject _PREFAB_EventRow;
    //World Events
    private Dictionary<int, Event> _D_WorldEvents;

    //Components
    [Header("Componentes: Panel de Eventos")]
    [SerializeField] private Transform _PANEL_messagePanel;
    [SerializeField] private Text _TEXT_bodyText, _TEXT_Label;
    [SerializeField] private Image _IMG_kindOfEvent, _IMG_BackgroundEvent;

    //References (External components)
    [Header("Componentes: Vista inferior de información")]
    [SerializeField] private Text _TEXT_LastEventSummary;


    void Start()
    {
        _D_WorldEvents = new Dictionary<int, Event>()
        {
            { 0, new NoEffectsEvent("Un Nuevo Pirata", $"El pirata {PersistentGameData._GData_PlayerName} inicia su carrera de fechorías y pillaje",
                $"La vida al servicio de la marina fue de lo más miserable: Leva forzada, vituallas en mal estado, y costear armas y uniforme a la par con tu propio dinero cuando el salario se atrasa durante meses." +
                $"\n\nTras un tiempo en servicio, tú y tus hombres lleváis a cabo un motín a bordo, haciendo acopio de un destartalado {PersistentGameData._GData_PlayerShip.GetModelName()} de cabotaje, más presto a perseguir a metedores por un río a tiro de mosquete, que a prestar batalla real ante fuego de cañón. Aún así, lo apodásteis con orgullo con el nombre de {PersistentGameData._GData_PlayerShip.name_Ship}." +
                $"\n\nCapitán {PersistentGameData._GData_PlayerName}, ante vos se extiene un mar de tempestades y enemigos. Conocerás a convictos, cimarrones y a los piratas más temidos que navegan los mares. Que tu nombre y tu bandera sean reconocidos en cada punta y cabo de la creación, y sean recordados durante siglos.",
                GetPathByShipModel(PersistentGameData._GData_PlayerShip.GetModelName()[0]))
            }
        };

        OpenEvent(0, PersistentGameData._GData_PlayerAvatar, PersistentGameData._GData_PlayerFlag);

        //----EVENTS
        UIMap.clearPanels += Close;
    }
    private void OnDestroy()
    {
        UIMap.clearPanels -= Close;
    }

    private void OpenEvent(int key, Sprite customEventSprite = null, Sprite customEventOnPanelSprite = null)
    {
        TimeManager.PauseST();
        //Obtiene evento por clave de diccionario
        //---------------------------------------

        var currentEvent = _D_WorldEvents[key];

        //Establece datos en el hud
        //---------------------------------------

        //Título de evento
        _TEXT_Label.text = currentEvent.title;

        //Descripción del evento
        _TEXT_bodyText.text = currentEvent.body;

        //Imágenes de fonto y tipo de evento
        _IMG_BackgroundEvent.sprite = Resources.Load<Sprite>($"Events/{currentEvent.sprite_Background_Index}");
        _IMG_kindOfEvent.sprite = customEventOnPanelSprite;

        //Activa panel
        //---------------------------------------
        _PANEL_messagePanel.gameObject.SetActive(true);
        UIMap.panelON = true;


        //Crea una nueva fila en la lista de eventos en el panel "Información del Mundo"
        //---------------------------------------
        GameObject row = Instantiate(_PREFAB_EventRow,_TR_WorldEventsContainer);
        var spr = customEventSprite ??= _EventSprites[currentEvent.sprite_News_Index];
        var date = TimeManager.GetDateString();
        row.GetComponent<RowEventsList>().SetRowData(spr, date, currentEvent.summary);

        //Actualiza la información de último evento:
        //---------------------------------------
        FadeText(_TEXT_LastEventSummary, $"{TimeManager.GetDateString()}: {currentEvent.summary}");
    }

    private void FadeText(Text text, string str)
    {
        DOTween.defaultTimeScaleIndependent = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(text.DOFade(0, 2.2f).SetUpdate(true));
        sequence.AppendInterval(0.6f).SetUpdate(true);
        sequence.AppendCallback(() => { text.text = str;});
        sequence.Append(text.DOFade(1, 2.2f).SetUpdate(true));

        sequence.Play();
    }

    private void Close()
    {
        if (_PANEL_messagePanel.gameObject.activeSelf)
        {
            _PANEL_messagePanel.gameObject.SetActive(false);
            TimeManager.NormalizeST();
        }
        
    }

    private byte GetPathByShipModel(char c)
    {
        switch(c)
        {
            case 'B': return 1;
            case 'F': return 2;
            case 'T': return 3;
            default: return 1;
        }
    }
}

//-----------------------------------------
public abstract class Event
{
    [TextArea(1, 20)]
    public string
        title,
        summary,
        body,
        action;
    public byte sprite_News_Index, sprite_Img_Index, sprite_Background_Index;
    public abstract void Effects();
}

[System.Serializable]
public class NoEffectsEvent : Event
{
    public override void Effects() { }
    public NoEffectsEvent(string tt, string ss, string bb, byte bgIndex = 1,byte indexNews = 0, byte indexImg = 0)
    {
        title = tt;
        summary = ss;
        body = bb;
        sprite_News_Index = indexNews;
        sprite_Img_Index = indexImg;
        sprite_Background_Index = bgIndex;
    }
}

[System.Serializable]
public class WorldwideEvent : Event
{
    public override void Effects() { }
}

[System.Serializable]
public class NationalEvent : Event
{
    public Kingdom affectedKingdom;
    public override void Effects() { }
}

public class WarEvent : Event
{
    public Kingdom[] kingdoms;
    public override void Effects() { }
}
