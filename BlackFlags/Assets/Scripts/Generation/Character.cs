using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generation Calls
using Generation.Generators;

namespace GameMechanics.Data
{
    //--------------------------------
    //PERSONAJES
    //--------------------------------
    /// <summary>
    /// Character: Clase abstracta que define los parámetros y métodos básicos de cualquier personaje del mundo.
    /// Parámetros: nombre del personaje (string), nive de amistad con el player (byte)
    ///  ** Constructores:
    ///         public Character() { }
    /// </summary>

    public abstract class BASE_Character
    {
        //Salutations
        protected static readonly Dictionary<EntityType_KINGDOM, string> D_Hello = new Dictionary<EntityType_KINGDOM, string>()
        {
            {EntityType_KINGDOM.KINGDOM_Spain, "Buen día," },
            {EntityType_KINGDOM.KINGDOM_Portugal, "Bom Dia," },
            {EntityType_KINGDOM.KINGDOM_France, "Bonjour," },
            {EntityType_KINGDOM.KINGDOM_Dutch, "Goedendag," },
            {EntityType_KINGDOM.KINGDOM_Britain, "How do you do?,"}
        };


        public string CharacterName;
        public string GetCharacterName()
        {
            return CharacterName;
        }
        public void SetNameByGeneration(EntityType_KINGDOM nationality, bool isPirate = false)
        {
            CharacterName = WorldGenerator.GetCharacterName(nationality, isPirate);
        }
        public abstract string GetRoleName();
    }

    [System.Serializable]
    public abstract class Character : BASE_Character
    {
        public float FriendshipLevel;
        public bool hasMetPlayer; //has this charecter seen the player before?

        protected static readonly Dictionary<AttributeType, List<string>> D_Presentations = new Dictionary<AttributeType, List<string>>()
        {
            {AttributeType.OdiaEspañoles, new List<string>() { "Esos malditos españoles… ¿Tú también los odias, verdad? ", "*Da un trago* ...Sí... al diablo con ellos. Bueno, a beber *Da un trago*." } },
            {AttributeType.OdiaPortugueses, new List<string>() { "Tú… -Sniff- TÚ… TIENES CARA DE PORTUGUÉS. Negrero de los... ", "Espera… creo que he bebido demasiad... O demasiado poco. *Mira la jarra vacía en la mano*" } },
            {AttributeType.OdiaFranceses, new List<string>() { "*Ronquido* … AAAHHHHHHHHH… Qué pesadilla...", "Soñé… que era un marine francés... CON LO QUE LOS ODIOOOO. ... (...) ... ¿No os conozco, verdad?" } },
            {AttributeType.OdiaHolandeses, new List<string>() { "Haaarl… otro gran día para no ser holandés. ¿No es así? ", "? -Sniff- Imagina morir luchando por... no sé ¿Especias? ... Claro que si fuera ron lo entendería. Ha harl!" } },
            {AttributeType.OdiaIngleses, new List<string>() { "*Golpea la mesa* SUCIOS INGLESES. Antes contrataban a los piratas, y ahora que ellos controlan los mares nos persiguen, Y NOS PONEN A BAILAR CON UNA SOGA. ", "*Escupe* QUE LES PARTA UN RAYO A TODOS." } },
            {AttributeType.OdiaNegreros, new List<string>() { "*Escupe* ¡Vos! ¡Vos venga aquí! *Sniff* Mirad a vuestro alrededor", "¿Qué es lo que veis? ¡Camaradería! ¡Ron! ¿Por qué? *Da un trago* Es sencillo: ", "Porque aquí no hay negreros, ni corsarios, ni procuradores. ¡Sólo gente de buen oficio, ha harl!" } },
            {AttributeType.Cruel, new List<string>() { "*Deja la jarra* Oh, ¿Esa oreja de ahí en el plato? Bueno, digamos que hay que saber imponerse.", "Dejar claras las cosas, ¿Sabéis? *Sonríe* Pero a vos os digo que Dios os guarde. De capitán a capitán os lo digo, que no es poco." } },
            {AttributeType.Caballeroso, new List<string>() { "Vos sois una cara nueva. Y parecéis ser capitán. ¿Me equivoco? ", "Brindemos por ello, ¡Por los nuevos camaradas! ¡O por la buena vida! Lo importante es beber." } },
            {AttributeType.Diligente, new List<string>() { "*Mira unos documentos* ¿…Vos no tendréis información sobre las patrullas costeras, verdad? ", "Argh... El ron no sabe bien si no tengo todo bajo control, ¿Sabéis? ", "Por ello sobornar a un procurador o a un guardia mal pagado nunca es demasiado caro, ¡Es una inversión! ¡Ha harl!" } },
            {AttributeType.TieneMigrañas, new List<string>() { "........ *Da un trago*........ ¡Ahhh, mi cabeza! ", "El ron ya no me calma las migrañas. Tendré que pasarme a la ginebra..." } },
            {AttributeType.Sanguinario, new List<string>() { "*Mata a una rata de un tiro* ¡YAHAHARL! ¿Habéis visto? ¡Ahora aquella!", "¡Os reto a matarla del primer intento! *Da un trago* YARL.", "¡La Historia se escribe con sangre, y sangre necesito! Si se acaban las ratas... ", "Quién sabe, tendré que empezar a disparar a personas. HAHAHARL." } },
            {AttributeType.Orgulloso, new List<string>() { "Yarrl!  ¿Quién sois vos? Supongo que habréis oído de mí, pero yo no he oído de vos." } }
        };

        protected static readonly Dictionary<AttributeType, List<string>> D_Presentations_End = new Dictionary<AttributeType, List<string>>()
        {
            {AttributeType.OdiaEspañoles, new List<string>() { "y la pesadilla de los mercantes españoles, ¡Haharl!"} },
            {AttributeType.OdiaPortugueses, new List<string>() { "y... ¡PORTUGUESES! *saca la pistola*"} },
            {AttributeType.OdiaFranceses, new List<string>() { "y enemigo jurado de la corona francesa"} },
            {AttributeType.OdiaHolandeses, new List<string>() { "Si me queréis de buen humor, no me habléis de los holandeses"} },
            {AttributeType.OdiaIngleses, new List<string>() { "y presto a hundir barcos ingleses. *Hace un gesto* A vuestra disposición"} },
            {AttributeType.OdiaNegreros, new List<string>() { "¡Brindemos por la libertad! *Da un trago*" } },
            {AttributeType.Cruel, new List<string>() { "y coleccionista de partes del cuerpo, ¡Pero sólo de gente que se lo ha buscado!", "soy un hombre honrado a fin de cuentas *mira la jarra*" } },
            {AttributeType.Caballeroso, new List<string>() { "Dios os guarde. Y si no... que el diablo os guarde.. Harl!"} },
            {AttributeType.Diligente, new List<string>() { "... ¿Y mis cartas de... Ajá!, Aquí."} },
            {AttributeType.TieneMigrañas, new List<string>() { "¡Ahhh, mi cabeza!" } },
            {AttributeType.Sanguinario, new List<string>() { "Sabed que... ¡RATA! *Pega un tiro*", "Espera, era un paje corriendo... HA HA HARL. (...) ¿Qué estaba diciendo?" } },
            {AttributeType.Orgulloso, new List<string>() { "Algún día mi bandera hondeará de Nassau a Cayo del Ron, hasta entonces, ¡Brindemos por mi éxito!", "¡Haharl!" } }
        };

        protected static readonly Dictionary<AttributeType, List<string>> D_Greetings_PIRATE_BY_ATTRIBUTE = new Dictionary<AttributeType, List<string>>()
        {
            {AttributeType.OdiaEspañoles, new List<string>() { "Decidme, ¿Habéis atacado algún barco español últimamente?" } },
            {AttributeType.OdiaPortugueses, new List<string>() { "¿Sabéis que unos portugueses estaban traficando con esclavos y sal bajo bandera falsa?", "¿Adivinad quién los ha pasado por la quilla?", "*Golpea la jarra contra la mesa* ¡HA HA HA HARL!" } },
            {AttributeType.OdiaFranceses, new List<string>() { "Vengo de librar batalla contra un patache francés", "No eran gran cosa, tras intercambiar disparos cogieron viento de bolina y huyeron ¡Ha harl!" } },
            {AttributeType.OdiaHolandeses, new List<string>() { "Haaarl… -Sniff-" } },
            {AttributeType.OdiaIngleses, new List<string>() { "Justo estaba pensando que esos balandros ingleses son un dolor de cabeza que el ron no quita", "Están bien arbolados, y los botan en Bermuda, no en Europa" } },
            {AttributeType.OdiaNegreros, new List<string>() { "¿Sabéis? En una trifulca en la que llegó la guardia ayer... *sniff* Bueno, sentáos y os lo cuento." } },
            {AttributeType.Cruel, new List<string>() { "*Deja la jarra* Oh, ¿Esa otra oreja? ¡Me vi obligado! ¡Si te ofenden, tú respondes!" , "¿Y esa cara? Me haces ver como si fuera malvado... *Mira la jarra vacía*" } },
            {AttributeType.Caballeroso, new List<string>() { "Acercáos a beber conmigo." } },
            {AttributeType.Diligente, new List<string>() { "Dadme un momento. Veinte grados, un minuto treinta segundos norte... ", "Setenta y dos grados, cuarenta y dos minuto, veinte segundos oeste... ¡Listo, a beber!" } },
            {AttributeType.TieneMigrañas, new List<string>() { "Parece que la cabeza me está dando un respiro", "Vos sois mi buenaventura, quién lo diría." } },
            {AttributeType.Sanguinario, new List<string>() { "¡Vos! Mirad, tengo aquí un collar hecho con dientes humanos. No es muy bonito, pero transmite un mensaje. HA HA HARRL! *Da un trago*" } },
            {AttributeType.Orgulloso, new List<string>() { "¡Harl! ¿Queréis oir cómo fue mi última victoria?" } },
            {AttributeType.BebedorSocial, new List<string>() { "Siéntase a beber conmigo. El tiempo vuela cuando tieness ron. *Da un trago*" } },
            {AttributeType.BebedorColérico, new List<string>() { "Mis hombres no aguantan nada bebiendo, ¿Por qué no me acompañáis vos?" } },
            {AttributeType.Agresivo, new List<string>() { "Hoy bebemos, y mañana... ¡Acabamos con algún corsario..", "*sniff* ...o con alguna de esas malditas patrullas. Quién sabe." } },
            {AttributeType.Prudente, new List<string>() { "Decidme, ¿Os siguió alguna patrulla viniendo hasta aquí? Nunca se sabe..." } },
            {AttributeType.Callado, new List<string>() { "... *Da un trago* ... Espero no incomodaros, es que no estoy muy hablador...", "A buen entendedor, pocas palabras, ¿No es así?" } },
            {AttributeType.Desconfiado, new List<string>() { "Acercáos si os place (...) ¡Pero no tanto!", "Estas son mis cartas de navegación *mira con recelo*" } },
            {AttributeType.ColeccionaTrofeos, new List<string>() { "Mirad esto, ¡Hola!", "Esta Red Union cosida de agujeros era de un balandro hace unos días", "Tengo más eh... ¡Haharl!"} }

        };

        protected static readonly Dictionary<int, List<string>> D_Greetings_PIRATE_GENERIC = new Dictionary<int, List<string>>()
        {
            {0, new List<string>() { "Acercáos a beber conmigo. ¿Conocéis a Sandy, la fulana tuerta?" } },
            {1, new List<string>() { "Estábamos discutiendo si es mejor emborracharse e ir al burdel,", "o ir al burdel y emborracharse." } },
            {2, new List<string>() { "Saludos, ¿Qué os trae por...¡CALLÁOS PERROS SARNOSOS! ¡Estamos hablando de capitán a capitán!" } },
            {3, new List<string>() { "Hablemos de cosas *sniff* de cosas importantes... EL MATELOTAGE", "¿Es obligatoria la sodomía? ¿SÍ o NO?" } },
            {4, new List<string>() { "¿Havéis visto esa jaula de hierro a la entrada del puerto con un pobre diablo pudriéndose?", "¡Belcebú se los lleve a esos malnacidos! *escupe*" } },
            {5, new List<string>() { "Venid. ¿Conocéis a Sandy, la fulana tuerta? ¡Es una leyenda!" } },
            {6, new List<string>() { "¡Hola! ¿Esa no es Sandy, la fulana tuerta?", "No espera... No *Argh*"} },
            {7, new List<string>() { "Estamos discutiendo... ¡CALLÁOS RATAS!", "Estamos discutiendo si se puede adiestrar a un mono para que se suba al timón del barco enemigo...", "*Escupe*, ya sabes, hacer que asocie el timón con comida...", "¡Y entonces hacer que el mono EXPLOTE!"} }
        };

        protected static readonly Dictionary<int, List<string>> D_Presentations_Smuggler = new Dictionary<int, List<string>>()
        {
            {0, new List<string>() { "*Sonríe* Vos sois capitán, ¿No es así? De los que navegan bajo bandera negra, diría yo.", "Tal vez encontréis útiles los servicios de alguien que compra y vende vituallas, herramientas... *baja la voz* armas..." } },
            {1, new List<string>() { "*Suelta unos papeles* Estoy rodeado de piratas y marineros, pero no parezco uno. ¿Es eso lo que os ha venido a la mente?", "Bueno, en la marina un servidor sería lo que de oficio se llama despensero, solo que yo... más bien compro y vendo mercancía sin preguntar su procedencia." } },
            {2, new List<string>() { "Habéis venido al lugar idóneo. Suelo hacer negocios aquí, y me encanta ver caras nuevas", "Tal vez a vos pueda ayudarle de alguna manera. Podría comprarle vituallas sin hacer preguntas... Algún arma, tal vez"} },
            {3, new List<string>() { "Diría que vos navegáis bajo la jack de cráneo y tibias *Suelta unos papeles* Al final es todo igual ¿Sabe vos?", "Comerciantes locales, las coronas de Europa, hacendados... todos recurren al contrabando.", "Una profesión muy noble, cabe decir." } },
            {4, new List<string>() { "¡Hola! Una cara nueva. Estoy viendo arreciar los vientos de fortuna ahora mismo:", "Vos tenéis un barco, y yo dispongo de gente que puede introducir mercancía tierra adentro."} }
        };

        protected static readonly Dictionary<AttributeType, List<string>[]> D_Drinking_PIRATE_BY_ATTRIBUTE = new Dictionary<AttributeType, List<string>[]>()
        {
            {AttributeType.OdiaEspañoles, new List<string>[]
            {
                new List<string>() { "*Da un trago* *Sniff* Toda esa plata y perlas que salen de Cartagena... *Alarga la mano*", "Sin esos galeones armados ¡Todo eso sería mío... oh... quiero decir nuesto, camarada." },
                new List<string>() {"...había allí dos pataches y un bergantín que habían acudido, reflotando el tesoro español...", "...una urca de doce fletada cuando fue a zozobra ¡TONELADAS DE TESOROS! ...Si hubieramos llegado vos y yo antes que nadie a aquel naufragio", "¡Todo eso sería mío... oh... quiero decir nuesto, camarada. *Da un trago*" } }
            },
            {AttributeType.OdiaPortugueses, new List<string>[]
            {
                new List<string>() { "¡Brindemos! *Alaza la jarra* Por las futuras víctimas que nos traerán riqueza...", "...Y por tener mi mano asiendo el gaznate de algún corsario portugués malnacido." },
                new List<string>() { "Aquel negrero portugués había tenido espalda con espalda a doscientas almas para ganar más en el viaje...", " Hasta en la sentina, donde los cuerpos se estaban pudriendo, y vivas quedaban dos mujeres y un hombre..."} }
            },
            {AttributeType.OdiaFranceses, new List<string>[]
            {
                new List<string>() { "...Ese Olonés era un salvaje ¿Sabes lo que les hacía a sus víctimas? ¡Les arrancaba el corazón, y le daba un bocado!" },
                new List<string>() { "Francia tiene buenas armas, pero no buenos hombres... *sniff*", "y lo mismo mismo dan corso a un negrero que arman una tartana de guardiamarinas imberbes..." } }
            },
            {AttributeType.OdiaHolandeses, new List<string>[]
            {
                new List<string>() { "...esa Compañía de las Indias Occidentales tiene los días contados... argh *snif* ...te lo digo yo" },
                new List<string>() { "*Alza la jarra* Un brindis... (...) ¿Por qué brindamos? ..." , "... *Agita la jarra* Por los holandeses, porque desaparezcan de la faz de la Tierra y dejen en paz a las gentes de bien." } }
            },
            {AttributeType.OdiaIngleses, new List<string>[]
            {
                new List<string>() { " *Da un trago* Ahhhj... esos ingleses", "Se la jugaron bien a Bonnet. Ese perro gobernador... Primero le indultaron, pero luego fueron a por él..."},
                new List<string>() { "Estuve en Londres, y allí la ginebra y las aguas sucias habían arruinado los barrios bajos y el puerto...", "*Sniff* Sí... Vi a una mujer tirada en el suelo, con un \"guagua\" tomando del pecho de la madre,", "...que llamaban así unas criadas negras a los niños, \"guagua\". *Da un trago*" } }
            },
            {AttributeType.OdiaNegreros, new List<string>[]
            {
                new List<string>() {"*Da un trago* ... Así que llegó la guardia ayer... *sniff*, y nos salvó a mí a mis hombres un cimarrón fugitivo.","El muy diablo se hizo filibustero cuando huía, y se sabe todos los trucos. Buenas gentes esos negros.", "*Vacía la botella de un trago*" } ,
                new List<string>() {"*Da un trago* ...Esos canallas del almirantazgo llamándonos enemigos de la humanidad...", "¡Peor que ser un pirata es ser un negrero! *escupe*"}}
            },
            {AttributeType.Cruel, new List<string>[]
            {
                new List<string>() { "*Suelta la jarra* El sentido de la crueldad está muy mal interpretado... *escupe*", "En un motín o en una revuelta siempre mueren compañeros, buenos hombres...", " *eructa* ...pero si cuelgas a un agitador a tiempo se evitan desgracias. ¡Ha harl!" } }
            },
            {AttributeType.Caballeroso, new List<string>[]
            {
                new List<string>() { "*Deja la jarra* Un pirata debe ser ante todo un caballero. ¡Como Roberts, como Bonnet!", "Así, cuando la muerte te llame, irás bien vestido, Yaharl!" },
                new List<string>() { "...Entonces... Aquellos dos hombres fueron azotados de acuerdo a un tribunal a bordo.", "Pero a Parris le até yo mismo la soga y se le ahorcó en la mesana, colgado de la verga seca.", "...A un compañero de armas. Pero eso no importa. Incluso los filibusteros debemos seguir una ley de conducta."},
                new List<string>() { "...Sabed vos que no he cerrado mi mente al mundo. *Sniff* Soy amigo de españoles, de los franceses y hasta de Gran Bretaña.", "Siempre lo he sido... Si tan sólo la marina no diese una vida tan miserable, y el ron no existiese...", " hubiese sido alguien honrado. *Da un sorbo* ha-haarl" } }
            },
            {AttributeType.Diligente, new List<string>[]
            {
                 new List<string>() { "Nada como beber después de un pillaje bien hecho.¡Harl!"},
                 new List<string>() { "*Sniff* Un criollo del azúcar me dijo... \"Está la cabeza del hacha y la cabeza de quien la empuña...", "Y si ambos son afilados, el tamaño del árbol no importa\". ¡Ha-harl!"}}
            },
            {AttributeType.TieneMigrañas, new List<string>[]
            {
                new List<string>() { "(...) ...Mi cabeza... *Da un trago*"},
                new List<string>() { "...El caso es que un brujo indígena decía que podía quitarme las migrañas...",  "y que debía sacarme los espíritus del cuerpo... que no sonaba muy católico.", "Hizo un ritual de santería en el que tenía que comer un murciélago, y sangré por abajo dos días.", "Le pegué un tiro, lo despeñé, y le pegué otro tiro... *Da un trago*", "No soy un mal hombre, es que... pufff me dolía la cabeza."},
                new List<string>() { "...Las hierbas de los nativos son buenas para la migraña y el estómago, pero... no es ron. *Mira la botella*", "También tienen un remedio muy potente para la fecundidad *Aprieta el puño*", "HAHAHAaarl--Ay! Mi... *Da un trago*" } }

            },
            {AttributeType.Sanguinario, new List<string>[]
            {
                new List<string>() { "¿Sabéis que si encerráis a alguien en un lugar cerrado con ratas, las ratas se lo comerán vivo?", "De dentro... a fuera. *Mira fijamente* HA HA HARRL! *Da un trago*" },
                new List<string>() { "*Da un trago* Cruzamos los alfanjes... y diré que se desenvolvía bien, pero a aquel infeliz le faltaba nervio.", "Cuando resbaló y se fijó en toda la sangre perdió la mirada... *Sniff*", "Yo no. Yo necesito sangre, así como otros necesitan de alcohol."},
                new List<string>() {"...Trajimos entonces a cubierta al capellán del galeón, y le pregunté si creía que su Dios le salvaría... Ha-Harl", "¿Qué? Créeme: Yo hubiera sido compasivo con el sacerdote, *Sniff* pero...", "eso no hubiera sido justo para todos los que cogieron un alfanje en vez de un crucifijo y murieron dando lucha.", "Al final separé la cabeza del cuerpo porque Pitts dijo que no alcanzaba a lanzar la cabeza hasta la verga de juanete.", "Y tenía razón el perro... me ganó un real de a ocho el hijo de su malnacida madre... *Da un trago*" }}
            },
            {AttributeType.Orgulloso, new List<string>[]
            {
                 new List<string>() { "...Allí había unos cien, no, ¡Doscientos hombres! *eructo*, ¡Qué digo, trescientos!"},
                 new List<string>() { "...El caso: Había rendillo por mi mismo el alcázar y la toldilla, cuando salieron cuatro hombres más..."} }
            },
            {AttributeType.BebedorSocial, new List<string>[]
            {
                new List<string>() { "¡Haharl! ¡Cantemos La doncella de Ámsterdam!" },
                new List<string>() { "*Da un trago* ¡Vamooos! ¡Que los músicos toquen una zarabanda! ¿Qué es esto, un galeón?"} } 
            },
            {AttributeType.BebedorColérico, new List<string>[] 
            {
                new List<string>() { "*Gruñido* ...Arghlrl... ¿QUÉ MIRÁIS VOSOTROS? *Saca la pistola de chispa*" },
                new List<string>() { "HA-HARL, ¡Bebed, cantad! ... *Dispara al techo* CANTAD, HE DICHO." } } 
            },
            {AttributeType.Agresivo, new List<string>[]
            {
                new List<string>() { "¿Sabéis por qué Stede Bonnet era un hombre? *Golpea la mesa* ¡Porque no se rendía!", "En el Revenge lo abandonaron, ¡Y él continuó!. Le capturaron... ¡Y él se fugó! ...Aunque lo atraparon otra vez..." },
                new List<string>() { "Da igual si uno es marino, villanesco... o hasta un haitiano haciendo vudú con dientes y huesos. *Sniff*", "Todo hombre que se precie debe, al menos una vez en la vida, escupirse las manos...", "...izar la bandera negra... ¡Y rajar cuellos! ¡Ha-harl! *Da un trago*"} } 
            },
            {AttributeType.Prudente, new List<string>[] 
            {
                new List<string>() { "Los piratas no duramos más de tres años en esta profesión, y luego... el dulce abrazo del cáñamo \"Quajhrh\" *Saca la lengua*", "Por eso en esta vida hay que tener una parte de gallardía y otra de prudencia *Da un trago*" },
                new List<string>() { "Tened cuidado al cargar las piezas de la regala... *Sniff* ...Falconetes, esmeriles...", "...Si no se limpian bien, pueden explotar en la cara del artillero."} } 
            },
            {AttributeType.Callado, new List<string>[]
            {
                new List<string>() { "... *Da un trago* ... (...) ........ ¿Vos Conocéis a Sandy, la fulana tuerta?" },
                new List<string>() { "........ *Da un trago* ........" } } 
            },
            {AttributeType.Desconfiado, new List<string>[] 
            { 
                new List<string>() {"...*Da un trago* ¿Vos sabéis cómo atraparon a Bonnet? Algún perro canalla hijo del demonio lo delató...", "*Escupe* Sí... hay que tener cuidado con los puñales que tienes delante...", "*Mira fijamente* ...pero más todavía con los que tienes detrás... *Da un trago*" },
                new List<string>() { "*Voz baja* ...Fijáos si hay alguna cara nueva que mire mucho hacia aquí. Puede venir algún soplón de la guardia..."} } 
            },
            {AttributeType.ColeccionaTrofeos, new List<string>[] 
            {
                new List<string>() {"¿Véis este anillo que llevo? De un capitán inglés. ¿Y este otro? De un corsario francés.", "Estos son mis tesoros... el recordar que vas a morir, y que tus cosas terminan en el anular de alguien, ¡Haharl!" },
                new List<string>() { "...Y la de cruces es el gallardete de almirantago. *Deja la jarra* Algún día me haré con una bandera de almirantazgo", "*Sniff*, y la pondré junto a la red Union de un bergantín de Esnón. Isville se llamaba aquel navío..."} } }
        };

        protected readonly Dictionary<int, List<string>> D_Drinking_PIRATE_GENERIC = new Dictionary<int, List<string>>()
        {
            {0, new List<string>(){ "... ♫ ♬ ¡BelCEbú y lA beBIddda aCAbaROn con su VIdaaa! ♫ ♩ ♫ ¡YO-HO-HO! *Da un trago* ...Mmgrghh... *Snif*" } },
            {1, new List<string>(){ "... Llegamos al fondeadero de Kidd a vísperas apenas cogiendo 3 nudos, porque parecía que el diablo se nos hubiera llevado los alíseos.,", "Allí nos atacaron los indígenas, y Frankly Escorbuto calló allí de una honda... *Levanta la botella* ¡Por Frankly!" } },
            {2, new List<string>(){ "...Allí desde el peñón del catalejo se veía desde la ensenada hasta las tres cruces... *Da un trago*", "...llegamos a la hora en que los creyentes hacen el Ángelus, porque los hombres aún iban ebrios... *sniff*", "y se movían más por miedo a los azotes que otra cosa..."} },
            {3, new List<string>(){ "...Te diré que no hay buenos capitanes negros... no que yo haya conocido *Da un trago*", "Porque cuando un esclavo renegado se hace filibustero... *sniff* forma tripulaciones de negros.*", "En una banda que se precie deben luchar y dormir bajo cofradía los blancos, los nativos y los mulatos a bordo", "...fueren quienes fueren cada cual antes de subir y dar lealtad al código.", "Igual para los cimarrones que los criollos y los fugitivos"} },
            {4, new List<string>(){ "Tengo un caso terrible de gaznate seco, ¡Ha-harl!"} },
            {5, new List<string>(){ "*Da un trago* Arl... El ron es la auténtica salud."} },
            {6, new List<string>(){ "...y cuando los alíseos van más hacia norte rehúyen del paso de la Florida,", "infestada como estaba Nassau de piratas en el buen tiempo en que Roberts navegaba *Da un trago*"} },
            {7, new List<string>(){ "...sin volver a Europa por La Florida los barcos tendrán que poner rumbo en las Bahamas,", "Por Cat Island, el Cayo del Ron, o hasta por el Paso de Islas Turcas, cuando puedes atrapar viento del Sur..."} },
            {8, new List<string>(){ "Como perros de mar que somos hay dos cosas importantes en la vida... *Sniff...*", "Una es el ron... y la otra... es Sandy la fulana tuerta."} },
            {9, new List<string>(){ "El Walrus dió un cañonazo de saludo, y el capitán ordenó dar un cañonazo de respuesta.", "Los barcos se cruzaron, vi las velas cuadras y tiré los zuchos para asomar por la regala, porque me llegaba por la coronilla", "Tenía diez añoz, y nunca había visto un barco tan hermoso... *mira la jarra*"} }
        };

        /// <summary>
        /// Return a list of strings as a dialog chain
        /// </summary>
        /// <returns></returns>
        public abstract List<string> GetPresentation();
        public abstract List<string> GetGreetings();
        public abstract List<string> GetGreetings_Responsive(out List<string> response);
        public virtual List<string> GetDrinkingDialog(out List<string> response) { response = null; return null; }

        public virtual void ModifyFriendship(float amount)
        {
            FriendshipLevel = Mathf.Clamp(FriendshipLevel + amount,0,100);
        }
        public virtual void ModifyFriendship() { }
    }
    /// <summary>
    /// Baddies are smugglers, pirates and criminals. This class allows an easy tavern-view filtering 
    /// </summary>
    public abstract class Baddy : Character { }
    /// <summary>
    /// Goodies are neutral non-criminal characters
    /// </summary>
    public abstract class Goody : Character { }

    /// <summary>
    /// Contrabandista: Personaje que posee una lista de inventario.
    ///  ** Constructores:
    ///         public Smuggler(string name, List<Resources> resources)
    ///         Smuggler(EntityType_KINGDOM country, List<Resources> resources)
    /// </summary>
    [System.Serializable]
    public class Smuggler : Character
    {
        //----------------------
        //DICTIONARIES
        //----------------------
        private static readonly Dictionary<byte, int[]> D_BaseRatio = new Dictionary<byte, int[]>()
        {
            {0, new int[] {7,12} },
            {1, new int[] {7,12} },
            {2, new int[] {1,4} },
            {3, new int[] {1,3} }
        };

        //----------------------
        //VARIABLES
        //----------------------
        //Resources the character trade with
        [SerializeField] public List<Resource> SmugglerOffer;
        //Current character inventory
        [SerializeField] public List<InventoryItemStacking> SmugglerInventory;
        //Resource spawn ratio by category
        public int[] SmugglerGenerationRatio = new int[4];
        //This character base ratio
        public int ratio = 1;

        //Overrides
        public override string GetRoleName() { return "Contrabandista"; }
        public override List<string> GetPresentation()
        {
            //character has met the player
            hasMetPlayer = true;

            //Get Presentation by ranom key
            var p1 = new List<string>(D_Presentations_Smuggler[Random.Range(0, 5)]);

            //Get dialog body
            var n = Random.Range(0, 4);
            var p2 =
                n == 0 ? new List<string>() { $"Me llamo {CharacterName}, humilde contrabandista. Necesitará darle salida a la mercancía de los botines, ¿Me equivoco?" } :
                n == 1 ? new List<string>() { $"Mi nombre es {CharacterName}. Me dedico a proveer bienes a la gente de bien.", "Mis hombres van a los pueblos y venden todo lo que la corona regula", "Aceite, vino, acucias... ¡No hay profesión más noble que el contrabando!" } :
                n == 2 ? new List<string>() { $"Soy {CharacterName}. Para vos {CharacterName.Split(' ')[0]}, o el bueno de \"{CharacterName.Split(' ')[0]}\". Tal vez podamos hacer negocios." } :
                new List<string>() { $"Mi nombre es {CharacterName}. Por aquí me conocen por ser alguien que provee lo que es difícil de conseguir...", "o al menos es así cuando han puesto precio a tu cabeza." };
            //Merge
            p1.AddRange(p2);

            //Return a list of strings as a dialog chain
            return p1;
        }
        public override List<string> GetGreetings()
        {
            var name = PersistentGameData._GData_PlayerName;
            var n = Random.Range(0, 4);
            //(Random.Range(0, 2) == 0 ? name : string.Empty)
            var txt =
                n == 0 ? new List<string>() { $"Capitán { name }, ¿Qué le trae por aquí? Tal vez quiera hacer negocios con el bueno de {CharacterName.Split(' ')[0]}." } :
                n == 1 ? new List<string>() { $"{GetSalutation()} Capitán {name}. ¿Algún negocio en mente? Vamos, no sea reservado.", "Yo haría todos los atavíos y vos sigue de correrías en alta mar, ¿Qué os parece?" } :
                n == 2 ? new List<string>() { $"¿En qué le puedo ayudar, Capitán {name}?, ¿Puedo llamarlo {name.Split(' ')[0]}, verdad? Estamos entre socios." } :
                new List<string>() { $"Capitán {name}. Qué casualidad, tengo unos zuchos y hachas de abordaje que os vendrían bien" };

            return txt;
        }
        public override List<string> GetGreetings_Responsive(out List<string> response)
        {
            response = new List<string>() { "Como sigáis rimando os pego un tiro en la cara." };

            return new List<string>() { $"{GetSalutation()} Capitán {PersistentGameData._GData_PlayerName}. No seáis testarudos y no os quedéis mudos. Sed amable y sensato, puede que hagamos un trato." };


        }

        private string GetSalutation()
        {
            var currentPort = GameObject.FindWithTag("Player").GetComponent<PlayerMovement>().GetPort();
            if (currentPort != null)
            {
                if (Random.Range(0, 3) != 0)
                {
                    return D_Hello[GameManager.gm.GetKingdombyTag(currentPort.transform.parent.tag).thisKingdom];
                }
            }
            return "Saludos,";
        }

        //Friendship
        public float GetDiscountModifier()
        {
            return Mathf.Clamp(1 + (FriendshipLevel - 0.4f) / 2, 1, 1.2f);
        }
        public override void ModifyFriendship(float amount)
        {
            ModifyFriendship_ByTrading(amount);
        }
        public void ModifyFriendship_ByTrading(float money)
        {
            FriendshipLevel = Mathf.Clamp(FriendshipLevel + money / 15000, 0, 1);
            //Debug.Log($"Nuevo valor de amistad (por comercio) {FriendshipLevel}");
            UI_TavernView.SetFriendshipLevelST(this);
        }

        public void ModifyFriendship_ByTime()
        {
            FriendshipLevel = Mathf.Clamp(FriendshipLevel - 0.02f, 0.25f, 1);
            //UI_TavernView.SetFriendshipLevelST(this);
        }

        //Constructor
        public Smuggler(string name, List<Resource> resources)
        {
            CharacterName = name;
            SmugglerOffer = resources;
            SmugglerInventory = new List<InventoryItemStacking>();
            SetFriendshipLevel();
            SetGenerationRatio();
            GetEvents();
        }
        //Constructor
        public Smuggler(EntityType_KINGDOM country, List<Resource> resources)
        {
            SetNameByGeneration(country);
            SmugglerOffer = resources;
            SmugglerInventory = new List<InventoryItemStacking>();
            SetFriendshipLevel();
            SetGenerationRatio();
            GetEvents();
        }

        private void GetEvents()
        {
            TimeManager.UpdateMarketDay += UpdateOffer;
            TimeManager.UnsubscribeMarketEvent += UnsubscribeFromMarket;
        }
        private void UnsubscribeFromMarket()
        {
            TimeManager.UpdateMarketDay -= UpdateOffer;
            TimeManager.UnsubscribeMarketEvent -= UnsubscribeFromMarket;
        }

        private void SetFriendshipLevel()
        {
            FriendshipLevel = 0.2f;
        }

#region INVENTORY
        private void SetGenerationRatio()
        {
            for (byte i = 0; i < 4; i++)
            {
                SmugglerGenerationRatio[i] = Random.Range(D_BaseRatio[i][0], D_BaseRatio[i][1]);
            }
        }

        private int GetGenerationRatio(ResourceType type)
        {
            return SmugglerGenerationRatio[(int)type];
        }

        private void UpdateOffer()
        {
            SetInventory();
        }

        public void SetInventory(bool reset = false)
        {

            if (reset)
            {
                foreach (Resource res in SmugglerOffer)
                {
                    SmugglerInventory.Add(new InventoryItemStacking(res, 0));
                }
                SetInventory();
            }
            else
            {
                if (SmugglerInventory.Count == 0) SetInventory(true);
                var remover = new List<InventoryItemStacking>();
                for (int i = 0; i < SmugglerInventory.Count; i++)
                {

                    //Ratio base: ratio base del tipo de recurso multiplicado por el ratio del personaje (mayor en ciudades grandes y puestos de contrabando)
                    var baseValue = GetGenerationRatio(SmugglerInventory[i].resource.ResourceType);

                    //Base lineal: Aumenta o disminuye la oferta de inventario en base a la oferta actual;
                    var adaptiveValue = ratio * baseValue  - SmugglerInventory[i].amount * ratio;

                    //Aumenta o disminuye el inventario
                    // ¿El comerciante vemde este recurso? Actualiza la oferta
                    if (SmugglerOffer.Contains(SmugglerInventory[i].resource))
                    {
                        SmugglerInventory[i].amount += adaptiveValue * (Random.Range(-1, 3));
                        SmugglerInventory[i].amount = Mathf.Clamp(SmugglerInventory[i].amount, 0, 999);
                    }
                    else //El comerciante tiene en inventario un recurso con el que no comercia? Lo vende todo
                    {
                        remover.Add(SmugglerInventory[i]);
                    }

                    //Limpia la lista de elementos vacíos.
                    foreach(InventoryItemStacking e in remover)
                    {
                        SmugglerInventory.Remove(e);
                    }
                }
            }
        }
#endregion
    }
    /// <summary>
    /// Pirata: Personaje que se desplaza entre ciudades y refugios.
    /// Posee un nivel de karma, con el que se mide la afinidad on el jugador en base a su reputación, y una lista de atributos
    ///  ** Constructores: 
    ///         public Pirate(string name, byte Karma)
    ///         public Pirate (byte karma, EntityType_KINGDOM country)
    ///         public Pirate (byte karma)
    /// </summary>
    [System.Serializable]
    public class Pirate : Character
    {
        //Variables
        public string shipName;
        private static List<Pirate> _pirateList = new List<Pirate>();
        public int reputation;
        public CharacterAttribute[] attributes = new CharacterAttribute[2];
        public bool seenByPlayer, knownByPlayer;

        //Seed Generation (Pirate aspect)
        private int[] characterSeed = new int[10];
        private static readonly int[] seedLength = new int[10] { 9, 5, 5, 4, 1, 5, 9, 5, 10, 4 };
        private static readonly int[] seedLengthBIG = new int[10] { 9, 5, 1, 5, 1, 3, 9, 4, 10, 3 };
        public bool big;
        public static List<Pirate> GetPiratesList() { return _pirateList; }
        private void SetRandomAttributes()
        {
            attributes[0] = CharacterAttribute.D_Attributes_Pirate1[Random.Range(0, 12)];
            attributes[1] = CharacterAttribute.D_Attributes_Pirate2[Random.Range(0, 7)];
            SetRandomSeed();
        }
        private void SetRandomSeed()
        {
            big = Random.Range(0, 8) == 0;
            int[] seedL = big ? seedLengthBIG : seedLength;
            for (int i = 0; i < 10; i++)
            {
                characterSeed[i] = Random.Range(0, seedL[i]);
            }
        }
        public int[] GetSeed() { return characterSeed; }

        //Overrides
        public override string GetRoleName() { return "Pirata"; }
        public override List<string> GetPresentation()
        {
            //character has met the player
            hasMetPlayer = true;

            //Get this character's main attribute
            var typeAtr = attributes[0].attribute;
            string adjective = (int)typeAtr > 10 ? typeAtr.ToString() : string.Empty;

            //Access to dictionaries
            var p1 = new List<string>(D_Presentations[attributes[0].attribute]);
            var n = Random.Range(0, 2);
            var p2 = n == 0 ?
                new List<string>() { (Random.Range(0, 2) == 0 ? "En fin. " : "*Escupe* ") + $"Soy {CharacterName}, el {adjective} capitán del {shipName}. " + (Random.Range(0,2) == 0 ? "Haharl" : "*Da un trago*") } :
                new List<string>() { $"Soy {CharacterName.Split(' ')[0]}, el {adjective} capitán del {shipName}.", (Random.Range(0, 2) == 0 ? "*Sniff* " : string.Empty) + $"{CharacterName}, ese soy yo." };
            p1.AddRange(p2);
            if (n == 0 && D_Presentations_End[attributes[0].attribute] != null) p1.AddRange(D_Presentations_End[attributes[0].attribute]);

            //Return a list of strings as a dialog chain
            return p1;
        }
        public override List<string> GetGreetings_Responsive(out List<string> response)
        {
            response = new List<string>() { " *Levanta la jarra* Y generoso no seas." };
            return new List<string>() { $"Saludos, Capitán {PersistentGameData._GData_PlayerName}. *Levanta la jarra* Arrasa con lo que veas..." };
        }

        public override List<string> GetGreetings()
        {
            //var list = List<string>(){ }
            return (Random.Range(0, 4) == 0 ? D_Greetings_PIRATE_GENERIC[Random.Range(0, 8)] : D_Greetings_PIRATE_BY_ATTRIBUTE[attributes[Random.Range(0,2)].attribute]);
        }

        public override List<string> GetDrinkingDialog(out List<string> response)
        {
            if (Random.Range(0, 2) == 0)
            {
                AttributeType at = attributes[Random.Range(0, attributes.Length)].attribute;
                switch (at)
                {
                    case AttributeType.OdiaEspañoles:
                        response = new List<string>() { "Sí... nuestro *cruzan miradas* HA-HA-HARL" };
                        break;
                    case AttributeType.Orgulloso:
                        response = new List<string>() { "En esa historia hay más enemigos cada vez que la cuentas..." };
                        break;
                    case AttributeType.Sanguinario:
                        response = new List<string>() { $"Capitán { CharacterName.Split(' ')[0]}... Hablar con vos siempre es una delicia... *Da un trago largo* *Argh* ..." };
                        break;
                    case AttributeType.Desconfiado:
                        response = new List<string>() { "Vos siempre tan desconfiado... *Da un trago*" };
                        break;
                    case AttributeType.ColeccionaTrofeos:
                        response = new List<string>() { "La gente suele... No sé, tocar el violín o leer romances. Pero... bonita colección de recuerdos... *Da un trago*" };
                        break;
                    default: response = null; break;
                }
                if (response == null)
                    response = Random.Range(0, 3) == 0 ? new List<string>() { "*Da un trago*" } : null;
                var array = D_Drinking_PIRATE_BY_ATTRIBUTE[at];
                return array[Random.Range(0, array.Length)];
            }
            else
            {
                response = Random.Range(0, 3) == 0 ? new List<string>() { "*Da un trago*" } : null;
                return D_Drinking_PIRATE_GENERIC[Random.Range(0,10)];
            }

        }

        public override void ModifyFriendship()
        {
            ModifyFriendship_Drinking();
        }

        private void ModifyFriendship_Drinking()
        {
            float amount = 1;
            float limit = 1;
            bool canIncrease = true;
            for (int i = 0; i < attributes.Length; i++)
            {
                switch (attributes[i].attribute)
                {
                    case AttributeType.BebedorSocial: 
                        amount += 1; 
                        break;
                    case AttributeType.BebedorColérico:
                        amount += Random.Range(-2, 3);
                        break;
                    case AttributeType.Diligente:
                        amount -= 0.4f;
                        break;
                    case AttributeType.Callado:
                        amount -= 0.4f;
                        break;
                }
                //Default clamping
                switch (attributes[i].attribute)
                {
                    case AttributeType.Desconfiado:
                        limit = 0.8f;
                        break;
                }
                //Conditioned clamping
                switch (attributes[i].attribute)
                {
                    case AttributeType.Caballeroso:
                        canIncrease = PersistentGameData._GData_Reputation >= 0.3f;
                        break;
                    case AttributeType.Cruel:
                        canIncrease = PersistentGameData._GData_Reputation <= 0.8f;
                        break;
                    case AttributeType.Sanguinario:
                        canIncrease = PersistentGameData._GData_Reputation <= 0.65f;
                        break;
                }

            }
            FriendshipLevel = Mathf.Clamp(FriendshipLevel + amount/100, 0.18f, limit);
            Debug.Log($"Nuevo valor de amistad (por beber) {FriendshipLevel}");
            UI_TavernView.SetFriendshipLevelST(this);
        }

        //Constructor
        public Pirate(string name, int Karma)
        {
            CharacterName = name;
            reputation = Karma;
            SetRandomAttributes();
            SetFriendshipLevel();
            _pirateList.Add(this);
        }
        //Constructor
        public Pirate (int karma, EntityType_KINGDOM country)
        {
            SetNameByGeneration(country, true);
            reputation = karma;
            SetRandomAttributes();
            SetFriendshipLevel();
            _pirateList.Add(this);
        }
        //Constructor
        public Pirate ()
        {
            var n = Random.Range(0, 6);
            var country = n > 3 ? EntityType_KINGDOM.KINGDOM_Britain : (EntityType_KINGDOM)n;
            SetNameByGeneration(country, true);
            SetRandomAttributes();
            SetFriendshipLevel();
            _pirateList.Add(this);
        }
        //Constructor
        public Pirate( EntityType_KINGDOM country)
        {
            SetNameByGeneration(country, true);
            SetRandomAttributes();
            SetFriendshipLevel();
            _pirateList.Add(this);
        }

        private void SetFriendshipLevel()
        {
            for (int i = 0; i < attributes.Length; i++)
            {
                switch (attributes[i].attribute)
                {
                    case AttributeType.Desconfiado:
                        FriendshipLevel = 0.18f;
                        break;
                    case AttributeType.Caballeroso:
                        FriendshipLevel = 0.25f;
                        break;
                    default: 
                        FriendshipLevel = 0.2f;
                        break;
                }
            }
        }
    }
    /// <summary>
    /// Personaje adicional que puede aparecer en tabernas, vender información y comprar armas de mano
    ///   ** Constructores: 
    ///          public Criminal(string name)
    ///          public Criminal(EntityType_KINGDOM country)
    /// </summary>
    [System.Serializable]
    public class Criminal : Baddy
    {
        //Constructor
        public Criminal(string name)
        {
            CharacterName = name;
        }
        //Constructor
        public Criminal(EntityType_KINGDOM country)
        {
            SetNameByGeneration(country, true);
        }
        //Overrides
        public override string GetRoleName() { return "Criminal"; }
        public override List<string> GetPresentation()
        {
            return null;
        }
        public override List<string> GetGreetings()
        {
            return null;
        }
        public override List<string> GetGreetings_Responsive(out List<string> response)
        {
            response = null;
            return null;
        }
    }
    /// <summary>
    /// Armador: Personaje que permite reparaciones en astillero o compra de mejoras según la amistad con el personaje
    ///  ** Constructores: 
    ///          
    /// </summary>
    [System.Serializable]
    public class ShipyardMan : Character
    {
        //Overrides
        public override string GetRoleName() { return "Armador"; }
        public override List<string> GetPresentation()
        {
            return null;
        }
        public override List<string> GetGreetings()
        {
            return null;
        }
        public override List<string> GetGreetings_Responsive(out List<string> response)
        {
            response = null;
            return null;
        }
    }
    //--------------------------------
    //ATRIBUTOS DE PERSONAJES
    //--------------------------------
    #region CHARACTER ATTRIBUTES
    /// <summary>
    /// Atributo: Clase que contiene un dato descriptivo de un Character, y que modifica la interacción con él
    /// Parámetros: nombre (string), descripción (string) y tipo de atributo (AttributeType)
    ///  ** Constructores: 
    ///   public CharacterAttribute(string n, string des, byte index)   
    ///   public CharacterAttribute(string n, string des, AttributeType type)
    /// </summary>
    [System.Serializable]
    public class CharacterAttribute
    {
        public string name, description;
        public AttributeType attribute;

        public static readonly Dictionary<int, CharacterAttribute> D_Attributes_Pirate1 = new Dictionary<int, CharacterAttribute>()
        {
            {0, new CharacterAttribute("Odia a los españoles", "Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes españoles", AttributeType.OdiaEspañoles) },
            {1, new CharacterAttribute("Odia a los portugueses", "Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes portugueses", AttributeType.OdiaPortugueses) },
            {2, new CharacterAttribute("Odia a los franceses", "Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes franceses", AttributeType.OdiaFranceses) },
            {3, new CharacterAttribute("Odia a los holandeses", "Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes holandeses", AttributeType.OdiaHolandeses) },
            {4, new CharacterAttribute("Odia a los ingleses", "Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes ingleses", AttributeType.OdiaIngleses) },
            {5, new CharacterAttribute("Odia a los negreros", "Atacar barcos negreros aumenta el nivel de amistad con el personaje", AttributeType.OdiaNegreros) },
            {6, new CharacterAttribute("Cruel", "El nivel de amistad no aumenta si tu lealtad al código es mayor al 80%", AttributeType.Cruel) },
            {7, new CharacterAttribute("Caballeroso", "El nivel de amistad no aumenta si tu lealtad al código es menor al 30%", AttributeType.Caballeroso) },
            {8, new CharacterAttribute("Diligente", "El nivel de amistad aumenta más lentamente al beber con el personaje", AttributeType.Diligente) },
            {9, new CharacterAttribute("Tiene migrañas", "Pobre hombre... -Sin efecto-", AttributeType.TieneMigrañas) },
            {10, new CharacterAttribute("Sanguinario", "El nivel de amistad no aumenta si tu lealtad al código es mayor al 65%", AttributeType.Sanguinario) },
            {11, new CharacterAttribute("Vanidoso", "-Sin efecto-", AttributeType.Orgulloso) },
        };

        public static readonly Dictionary<int, CharacterAttribute> D_Attributes_Pirate2 = new Dictionary<int, CharacterAttribute>()
        {
            {0, new CharacterAttribute("Bebedor social", "Aumenta el nivel de amistad más rápido al beber con el perosnaje", AttributeType.BebedorSocial) },
            {1, new CharacterAttribute("Bebedor colérico", "El nivel de amistad puede aumentar o disminuir al beber con el personaje", AttributeType.BebedorColérico) },
            {2, new CharacterAttribute("Agresivo", "Se requiere menos afinidad y nivel de amistad para aceptar ataques en grupo", AttributeType.Agresivo) },
            {3, new CharacterAttribute("Prudente", "Se requiere mayor afinidad y nivel de amistad para aceptar ataques en grupo", AttributeType.Prudente) },
            {4, new CharacterAttribute("Callado", "El nivel de amistad aumenta más lentamente al beber con el personaje", AttributeType.Callado) },
            {5, new CharacterAttribute("Desconfiado", "El nivel de amistad no puede superar el 80%", AttributeType.Desconfiado) },
            {6, new CharacterAttribute("Coleccionista", "Colecciona trofeos de sus víctimas - Sin Efecto-", AttributeType.ColeccionaTrofeos) }
        };

        //Constructor
        public CharacterAttribute(string n, string des, byte index)
        {
            name = n;
            description = des;
            attribute = (AttributeType)index;
        }
        //Constructor
        public CharacterAttribute(string n, string des, AttributeType type)
        {
            name = n;
            description = des;
            attribute = type;
        }
    }

    public enum AttributeType
    {
        //"Odia a los españoles: Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes españoles.
        OdiaEspañoles = 0,
        //"Odia a los portugueses: Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes portugueses.
        OdiaPortugueses = 1,
        //"Odia a los franceses: Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes franceses.
        OdiaFranceses = 2,
        //"Odia a los holandeses: Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes holandeses.
        OdiaHolandeses = 3,
        //"Odia a los ingleses: Requiere menos afinidad y amistad para aceptar ataques en grupo sobre convoyes ingleses.
        OdiaIngleses = 4,
        //"Odia a los negreros: Aumenta la amistad por atacar barcos negreros"
        OdiaNegreros = 5,
        //Bebedor Social: El nivel de amistad aumenta más rápidamente al beber con el personaje
        BebedorSocial = 6,
        //Bebedor colérico: Rasgo negativo. El nivel de amistad puede aumentar o disminuir al beber con el personaje.
        BebedorColérico = 7,
        //Agresivo: Se requiere menos afinidad y nivel de amistad para aceptar ataques en grupo
        Agresivo,
        //Prudente: Rasgo negativo. Se requiere más afinidad y nivel de amistad para aceptar ataques en grupo.
        Prudente,
        //Callado. Rasgo negativo. El nivel de amistad aumenta más lentamente al beber con el personaje.
        Callado,
        //Cruel: El nivel de amistad no aumenta si tu lealtad al código es mayor al 80%
        Cruel,
        //Caballeroso: El nivel de amistad no aumenta si tu lealtad al código es menor al 30%
        Caballeroso,
        //Desconfiado: Rasgo negativo. El nivel de amistad no puede superar el 80%
        Desconfiado,
        //Diligente: Rasgo Negativo. Como callado
        Diligente,
        //Tiene migrañas: Sin efecto
        TieneMigrañas,
        //Sanguinario: como cruel, pero más
        Sanguinario,
        //Orgulloso: Sin efecto
        Orgulloso,
        //Colecciona trofeos: Sin efecto
        ColeccionaTrofeos
    }
#endregion
    /// <summary>
    /// ¿Dónde se encuentra el personaje?
    /// </summary>
    public enum CharacterLocation { atPort, atTavern}

    /// <summary>
    /// Los personajes-evento actúan como trigger de misión. No se puede tener nivel de amistad ya que desaparecerán
    /// </summary>
    [System.Serializable]
    public abstract class EventCharacter : BASE_Character
    {

    }

    /// <summary>
    /// Fujitivo que puede aparecer en tabernas ofreciendo al jugador una misión para ir a otra ciudad.
    /// </summary>
    public class Suspicious : BASE_Character
    {
        public override string GetRoleName() { return "Hombre sospechoso"; }
    }
    /// <summary>
    /// Capitán retirado que puede aparecer en tabernas ofreciendo al jugador información básica.
    /// </summary>
    public class RetiredCaptain : BASE_Character
    {
        public override string GetRoleName() { return "Capitán Retirado"; }
        public RetiredCaptain(string name)
        {
            CharacterName = name;
        }
        public RetiredCaptain(EntityType_KINGDOM country)
        {
            SetNameByGeneration(country);
            CharacterName += CharacterName.Split(' ')[0] + " El Viejo";
        }
    }
}

