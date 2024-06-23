using System.Collections;
using System.Collections.Generic;
using UnityEngine; using UnityEngine.UI;
using GameSettings.Core;
//Tweening
using DG.Tweening;

namespace GameMechanics.Data
{
    /// <summary>
    /// DialogManager. Special-View located script. Public singleton.
    /// </summary>
    public class DialogManager : MonoBehaviour
    {
        //Dialogs
        private readonly Dictionary<int, List<Dialog>> _D_tavernDialogs_NOEFFECT = new Dictionary<int, List<Dialog>>()
        {
            {
                0, new List<Dialog>() 
                {
                    new Dialog("Marinero 1", new List<string>() { "*Snif* Ese... ese Roberts era un caballero. *Da un trago* ...Trataba bien a sus hombres...", "... y era delicado con las mujeres ¿Sabes?" }), 
                    new Dialog("Marinero 2", new List<string>() { "¿Mujeres? ¿Y las tomaba? ... ¿No tenía un amante a bordo del Rover? Sí ese... uno bajito."}), 
                    new Dialog("Marinero 1", new List<string>() { "*Eructa* Pero eso era en alta amar. Ya sabes... cuando la tempestad aprieta cualquier agujero es aspillera *snif*" }) 
                }
            },
            {
                1, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "*Da un trago* ...¿Tú lo harías, atacarías un barco que tiene izado el peñol de oficios?", "*snif ¿Entonces no tienes respeto por la fé?*" }),
                    new Dialog("Marinero 2", new List<string>() { "Mira mameluco. Yo respeto la religión, pero no a los que quieren reunirse con su Dios demasiado rápido.", " *Snif* Si llevas un cargamento apura a atrapar el viento. Y ya en tierra vaya vos a una capilla.", }),
                    new Dialog("Marinero 1", new List<string>() { "Te ha salido un pareado."}),
                    new Dialog("Marinero 2", new List<string>() { "¿Eh?"}),
                    new Dialog("Marinero 1", new List<string>() { "\"Si llevas un cargamen-TO apura a atrapar el vien-TO\"."}),
                    new Dialog("Marinero 2", new List<string>() { "...Haces que me duela la cabeza... *Da un trago* ... (...)"})
                }
            },
            {
                2, new List<Dialog>()
                {
                    new Dialog("Marineros:", new List<string>() { "♩ ♫ ♪ ¡I'll go no more a-rovin' with you, fair maid! ♫ ♫", " ♫ ♪ ¡A-ROVING, A-ROVING, SINCE ROVINGS'S BEEN MY RU-IN! ♬ ♫", " ♫♪ ♪ I'll go no more a-roving with you, FAIR MAIIIIID  ♩ ♩♫ ♪" }),
                }
            },
            {
                3, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "... ¿Entonces sois inglés?" }),
                    new Dialog("Marinero 2", new List<string>() { "No soy inglés, señor. Soy galés."}),
                    new Dialog("Marinero 1", new List<string>() { "*Sniff* ¿Pero no es... es lo mismo?" }),
                    new Dialog("Marinero 2", new List<string>() { "*Golpea la jarra* NO. NO ES LO MISMO." }),
                    new Dialog("Marinero 1", new List<string>() { "...Vale, vale... Dios... *Da un trago*" })
                }
            },
            {
                4, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "...A maitines con la llena vimos de nuevo la presa en el horizonte. Y el capitán nos gritó como si fuera el mismo diablo...", "Habíamos hecho media jornada de repiquete para no perder la distancia y él nos rugía como un animal.", "...Estaba calado porque ni con el magón picado se había movido del sitio..." }),
                }
            },
            {
                5, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "...Bueno, continúa." }),
                    new Dialog("Marinero 2", new List<string>() { "No, que la cuente Pierre que lo cuenta mejor."}),
                    new Dialog("Marinero 3", new List<string>() { "¡Calláos perros!" }),
                    new Dialog("Marinero 4", new List<string>() { "...Bueno. Aquel canalla echó a las negras al agua. Dijo que no podía alimentarlas ni tampoco venderlas.","Eso fue antes de que Pitts estuviera a bordo. Pitts le hubiera rajado de oreja a oreja." })
                }
            },
            {
                6, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "...El capitán nos estaba maldiciendo cuando sentí como si el cielo se partiera por la mitad...", "Sólo sabía que estaba en el suelo, pero el capitán seguía firme, nos seguía mirando con aquellos ojos...", "El mismo rostro del diablo. Pero cuando nos acercamos, ¡Estaba muerto! ¡Un rayo lo había matado!" }),
                    new Dialog("Marinero 2", new List<string>() { "¿Un rayo?"}),
                    new Dialog("Marinero 3", new List<string>() { "Un rayo..." }),
                    new Dialog("Marinero 4", new List<string>() { "Es como dice... hacia Maracaibo caen rayos todo el tiempo. Es como si el cielo se volviera loco." }),
                    new Dialog("Marinero 1", new List<string>() { "No hay forma de morir igual que esa. Te quedas en el sitio, de pie...", "incluso con algo asido de la mano, sin soltarlo. Pero ya estás muerto..." }),
                }
            },
            {
                7, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "...¿Sabes entonces lo que le hizo?" }),
                    new Dialog("Marinero 2", new List<string>() { "No, pero seguro que hace que la bebida me sepa mal"}),
                    new Dialog("Marinero 1", new List<string>() { "Cogió un zucho... y se lo ensartó por el ano. Pero no lo mató al momento. Cogió entonces su cuchillo..." }),
                    new Dialog("Marinero 2", new List<string>() { "Agghh... *Suspiro* .... *Deja la jarra*" }),
                }
            },
            {
                8, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "¿Es cierto que El Olonés era caníbal?" }),
                    new Dialog("Marinero 2", new List<string>() { "*Da un trago* No, eso no es cierto. Si hubiera sido caníbal sería sabido por todo el mundo.", "Eso es porque una vez pruebas el sabor de la carne humana, ...no puedes parar...", "...Me lo dijo un negro haitiano que practicaba vudú:", "No puedes parar... y mientras lo decía mostraba los dientes, y gesticulaba la mandíbula.", "Ñam... Ñam... con esos dientes amarillos... Ñam... Ñam... *Mira fijamente*"}),
                    new Dialog("Marinero 1", new List<string>() { "... O_O" })
                }
            },
            {
                9, new List<Dialog>()
                {
                    new Dialog("Marinero 1", new List<string>() { "Creía que esta la tenía ganada..." }),
                    new Dialog("Marinero 2", new List<string>() { "Y vuelves a perder. Harl! ... Tiene gracia porque es el resumen de tu vida."}),
                    new Dialog("Marinero 1", new List<string>() { "Tu cara sí que es el resumen de mi entrepierna ... Que reparta Darién, que tú eres más malo que El Olonés repartiendo."})
                }
            }
        };
        
        //--------------------------------------------------
        
#region VARIABLES

        //Singleton
        public static DialogManager DM;

        //Components Reference
        [SerializeField] private GameObject _TR_DialogBox;
        [SerializeField] private Text _TEXT_DialogText, _TEXT_Continue;
        private Image _IMG_DialogBox;
        [SerializeField] private Toggle _TOG_Auto;

        //?
        Coroutine co;

        //Tweening
        Tween blinkerTween;

        //Inputs
        private bool skip; //has the player pressed 'Space'?

        //Data
        public Character currentCharacter;
        private bool playingDialog; //A dialog sequence in progress
        private DialogTrigger currentDialogTrigger; //what kind of post-dialog UI actions are required in next event call

        //Settings
        public bool skipDialog;

        //Events
        public delegate void SetDialogUIEvent(DialogTrigger dTrigger);
        /// <summary>
        /// This event is called when a dialog box is clossed or suspended
        /// </summary>
        public static event SetDialogUIEvent ActionByDialog;

#endregion

        private void Awake()
        {
            DM = this;
        }
        void Start()
        {
            _IMG_DialogBox = _TR_DialogBox.GetComponent<Image>();
            _TOG_Auto.isOn = PersistentGameSettings._autoSkipDialog;
            _TOG_Auto.onValueChanged.AddListener(delegate { PersistentGameSettings._autoSkipDialog = _TOG_Auto.isOn; });
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) )
            {
                if(playingDialog) skip = true;
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
            {
                if (playingDialog) CloseDialogBox();
            }
        }

        private void OnDisable()
        {
            playingDialog = false;
        }

        /// <summary>
        /// Open a dialog boxwirh a character for a presentation or greetings
        /// </summary>
        /// <param name="character"></param>
        /// <param name="dialogType"></param>
        public void OpenDialogBox(Character character, DialogTrigger dialogType)
        {
            //Set actions after dialog sequence or when dialog box is closed/suspended
            currentDialogTrigger = dialogType;

            //Set currentCharacter: this is needed for future actions
            currentCharacter = character;

            //Reset Dialog box
            playingDialog = true;
            _IMG_DialogBox.DOFade(0, 0);
            _TR_DialogBox.SetActive(true);
            _TEXT_DialogText.text = string.Empty;

            //Tween Fading
            _IMG_DialogBox.DOFade(.65f, .5f).SetUpdate(true);

            //Creates a new dialog sequence:
            var val = Random.Range(0, 5);
            List<string> response = null;
            var dialogSequence = new List<Dialog>()
            {
                new Dialog(character.CharacterName, character.hasMetPlayer ? 
                ( val == 0 ? character.GetGreetings_Responsive(out response): character.GetGreetings()) : character.GetPresentation()) 
            };

            if (val == 0) dialogSequence.Add(new Dialog("Capitán " + PersistentGameData._GData_PlayerName, response));

            //Start New Dialog:
            co = StartCoroutine(RunDialog(dialogSequence));
        }

        public void OpenDialogBox(DialogMode mode = DialogMode.listening)
        {
            if (!playingDialog)
            {
                //Set actions after dialog sequence or when dialog box is closed/suspended
                currentDialogTrigger = DialogTrigger.dialogTavern_NOEVENT;

                //Reset Dialog box
                playingDialog = true;
                _IMG_DialogBox.DOFade(0, 0);
                _TR_DialogBox.SetActive(true);
                _TEXT_DialogText.text = string.Empty;

                //Tween Fading
                _IMG_DialogBox.DOFade(.65f, .5f).SetUpdate(true);

                var dialogSequence = new List<Dialog>();

                //switch
                switch (mode)
                {
                    //Escuchando conversaciones en la taberna:
                    case DialogMode.listening: 
                        if (Random.Range(0, 5) == 0)
                        {
                            dialogSequence.Add(new Dialog("Tripulante del " + PersistentGameData._GData_ShipName, new List<string>() { " *Snif* Capitaaaaaaan... Este sitio es estupendo... *Da un trago*" }));
                        }
                        else
                        {
                            dialogSequence = new List<Dialog>(_D_tavernDialogs_NOEFFECT[Random.Range(0, 10)]);
                        }

                        dialogSequence.Add(new Dialog("Capitán " + PersistentGameData._GData_PlayerName, new List<string>() { " - (Nada interesante por aquí)..." }));
                        break;
                    //Bebiendo con un personaje
                    case DialogMode.drinkingCharacter:
                        //bebe con personaje
                        currentDialogTrigger = DialogTrigger.dialogTalkWithPirate;
                        List<string> response = null;
                        if (Random.Range(0, 4) != 0)
                        {

                            dialogSequence.Add(new Dialog(currentCharacter.CharacterName, currentCharacter.GetDrinkingDialog(out response)));
                            if(response != null) dialogSequence.Add(new Dialog("Capitán " + PersistentGameData._GData_PlayerName, response));
                        }
                        else
                        {
                            var n = Random.Range(0, 3);
                            List<string> t = null;
                            switch (n)
                            {
                                case 0:
                                    t = new List<string>() { "♫ ♪  ¡Ey-ooh! ♫ ¡Levamos anclas! ♫ ♬ ¡Ey-ooh! ¡El viento crece! ♪ ♫ ♩ ♫" };
                                    break;
                                case 1:
                                    t = new List<string>() { "♩ ♫ ♪ ¡Then a great big Dutchman rammed my bow ... Mark well what I do say! ♬ ♫", "♫ ♪ For a great big Dutchman rammed my bow ... And said, \"Young man, dit is main frau\" ♩ ♫♪", "♫ ♫ ♩ I'll go no more a roving... with you fair maiiiiid. ♪ ♪ ♩" };
                                    break;
                                case 2:
                                    t = new List<string>() { "♫ ♪ ¡Ahora sí ques-tán un unidoooos el Viejo y el Nuevo Mundoooo! ♩ ♬ ♩", "♫ ♬ ♩ ¡Y sólo están divididos, ♬ ♪ y sólo están divididos por un viejo mar profundo!", };
                                    break;
                            }

                            dialogSequence.Add(new Dialog("Ambos", t));
                        }
                        break;
                    case DialogMode.drinkingAll:
                        dialogSequence.Add(new Dialog("Capitán " + PersistentGameData._GData_PlayerName, new List<string>() { (Random.Range(0, 3) == 0 ? "¡Bebed, canallas! ¡Bebed!" : Random.Range(0, 2) == 0 ? "¡Esta es una orden de vuestro capitán! ¡Bebed hasta hartaros!" : "¡Dios sabe qué nos guarda el mañana, así que ahora a beber se ha dicho!") }));
                        dialogSequence.Add(new Dialog("Tripulantes del " + PersistentGameData._GData_ShipName, new List<string>() { (Random.Range(0, 3) == 0 ? "¡Ya habéis oído al capitán! ¡Yaharl!" : Random.Range(0, 2) == 0 ? "¡Ese es nuestro capitán! ¡Yarl!" : "¡Sin pensar en el mañana! ¡Ahoy!") }));
                        break;
                }


                //Start New Dialog:
                co = StartCoroutine(RunDialog(dialogSequence));
            }

        }
        public void DialogButton_Drink()
        {
            OpenDialogBox();
        }
        public void DialogButton_WithAllDrink()
        {
            OpenDialogBox(DialogMode.drinkingAll);
        }
        public void DialogButton_DrinkWithCharacter()
        {
            OpenDialogBox(DialogMode.drinkingCharacter);
            currentCharacter.ModifyFriendship();
        }

        public void CloseDialogBox(bool suspendedByExit = false)
        {
            //Close panel
            playingDialog = false;
            blinkerTween.Kill();
            StopAllCoroutines();
            _TR_DialogBox.SetActive(false);

            //Call event when dialog panel is closed or sequence is over, not when view window is closed
            if(!suspendedByExit) ActionByDialog(currentDialogTrigger);
        }
        IEnumerator RunDialog(List<Dialog> sequence)
        {
            
            int indexSequence = 0, indexDialog;
            yield return new WaitForSeconds(1);
            while (indexSequence < sequence.Count)
            {
                //new dialog block
                var currentDialog = sequence[indexSequence];
                if (currentDialog == null)
                {
                    print("derp1");
                    CloseDialogBox();
                    StopCoroutine(co);
                    StopCoroutine(RunDialog(sequence));
                    while (true)
                    {
                        yield return null;
                    }
                }
                else if (currentDialog.text == null)
                {
                    print("derp2");
                    CloseDialogBox();
                    StopCoroutine(co);
                    StopCoroutine(RunDialog(sequence));
                    while (true)
                    {
                        yield return null;
                    }

                }
                //Set counter
                indexDialog = 0;

                while(indexDialog < currentDialog.text.Count)
                {
                    //Current message
                    string msg = currentDialog.text[indexDialog];
                    //Character counter
                    int c = 0;
                    //Set Dialog's head
                    _TEXT_DialogText.text = $"<color=#FFBB00>{currentDialog.headerDialog}: </color>";
                    //reset input bool
                    skip = false;
                    while (c < msg.Length)
                    {
                        if (skip)
                        {
                            skip = false;
                            _TEXT_DialogText.text = $"<color=#FFBB00>{currentDialog.headerDialog}: </color>" + msg;
                            break;
                        }
                        _TEXT_DialogText.text += msg[c];
                        yield return new WaitForSeconds(0.03f);
                        c++;
                    }
                    indexDialog++;

                    //Jump 
                    if (_TOG_Auto.isOn)
                    {
                        yield return new WaitForSeconds(0.6f);
                    }
                    else
                    {
                        BlinkContinueText();
                        while (!skip)
                        {
                            yield return null;
                        }
                        skip = false;
                        BlinkContinueText(false);
                    }
                }
                indexSequence++;
            }
            CloseDialogBox();
        }

        private void BlinkContinueText(bool val = true)
        {
            blinkerTween.Kill();
            _TEXT_Continue.DOFade(0, 0);
            if(val)blinkerTween = _TEXT_Continue.DOFade(0.9f, 0.6f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    public enum DialogTrigger
    {
        //Secuencia de diálogo que no dispara ninguna acción
        dialogNull = 0,
        //El jugador escucha un diálogo, al terminar se reestablece la interfaz de taberna a modo normal
        dialogTavern_NOEVENT = 1,
        //El jugador inicia diálogo con el contrabandista
        dialogTalkWithSmuggler = 3,
        //El jugador inicia diálogo con el pirata
        dialogTalkWithPirate = 4,
    }
    public enum DialogMode
    {
        //Conocer/Saludar personaje
        greetings,
        //escuchar una conversación
        listening,
        //beber con la tripulación
        drinkingAll,
        //Beber con un personaje
        drinkingCharacter
    }

    public class Dialog
    {
        public string headerDialog;
        public List<string> text;

        public Dialog(string h, List<string> t)
        {
            headerDialog = h;
            text = t;
        }

    }

}
