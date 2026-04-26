//Core
using UnityEngine;
using System.Collections;
//Lists
using System.Collections.Generic;
using System.Linq;
//Mechanics
using GameMechanics.WorldCities;
using GameMechanics.Ships;
using GameMechanics.Data;


namespace GameMechanics.AI
{
    public abstract class ClassAI : MonoBehaviour  //IAs
    {
        //Components
        protected Collider collider;

        //AI State 
        protected ConvoyNPC _thisConvoy;
        protected Vector3 _destination;

        public abstract void SetStateAs_OnCruisse(KeyPoint currentPort = null);
        public abstract string GetAIRol();
        public abstract string GetGentilism(Kingdom kingdom);
        public virtual void SetStateAs_AtPort()
        {
            collider.enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
            _thisConvoy.Dissappear();
            _thisConvoy.convoyCurrentState = ConvoyState.AtPort;
        }
        //Triggers
        protected abstract Vector3 GetDestination(KeyPoint port);
        public abstract void ArriveToDestination();

        public virtual void Awake()
        {
            _thisConvoy = GetComponent<ConvoyNPC>();
            collider = GetComponent<Collider>();
        }
        public void SetTag(string tag)
        {
            transform.tag = tag;
        }
    }

    //------

    public class AI_Merchant : ClassAI
    {
        [SerializeField] private Settlement[] convoyRoute;
        public Settlement[] getRemainingRoute
        {
            get
            {
                // return convoyRoute[routeIndex..];
                return convoyRoute.Skip(routeIndex).ToArray();
            }
        }
        public byte routeIndex = 0;
        private bool toOcean; //el convoy vuelve al océano y sale del mapa
        public bool ToOcean
        {
            get { return toOcean; }
        }

        private void OnEnable()
        {
            routeIndex = 0;
            convoyRoute = null;
            toOcean = false;
            StartCoroutine("SetOnCruisseSync");
        }

        //-------------------- AI CLASS NAME
        public override string GetAIRol() { return "Convoy Europeo"; }
        public override string GetGentilism(Kingdom kingdom) { return kingdom.GENTILISM_MALESIN; }

        //-------------------- DESTINATION MANAGING
        protected override Vector3 GetDestination(KeyPoint port)
        {
            return port.transform.GetChild(0).position;
        }
        public void SetRoute(Settlement[] route)
        {
            convoyRoute = route;
        }
        public override void ArriveToDestination()
        {
            //¿He recibido la información?
            if (convoyRoute == null)
            {
                StartCoroutine("DestinationSync");
                return;
            }

            //¿Qué hago si llego a destino?
            var iT = GetComponent<IsTarget>();
            if (iT != null)
                iT.Safe();
            if (routeIndex < convoyRoute.Length)
            {
                SetStateAs_AtPort();
            }
            else if (!toOcean) //all cities visited? then go to ocean
            {
                toOcean = true;
                SetStateAs_AtPort();
            }
            else // arrived to ocean killer? then route is completed
            {
                convoyRoute = null;
                _thisConvoy.DissappearAndSleep();
                gameObject.SetActive(false);
            }
        }

        //-------------------- AI STATES
        #region IA STATES
        public override void SetStateAs_OnCruisse(KeyPoint currentPort = null)
        {
            if (toOcean)
            {
                _thisConvoy.currentPort = null;
                _thisConvoy.SetIADestination(OceanicRoute.GetOceanicRouteOut(transform.position));
            }
            else
            {
                _thisConvoy.currentPort = convoyRoute[routeIndex];
                routeIndex++;
                _thisConvoy.SetIADestination(GetDestination(_thisConvoy.currentPort));
            }

            _thisConvoy.convoyCurrentState = ConvoyState.Sailing;
            collider.enabled = true;
        }
        public override void SetStateAs_AtPort()
        {
            base.SetStateAs_AtPort();
            CancelInvoke();
            Invoke("onCruisseAction", 660);

            //Add this convoy to settlement's list
            if (_thisConvoy.currentPort is MB_City)
            {
                var port = (MB_City)_thisConvoy.currentPort;
                port.convoysInThisPort.Add(_thisConvoy);
            }
        }
        private void onCruisseAction()
        {
            SetStateAs_OnCruisse();
        }
        IEnumerator SetOnCruisseSync()
        {
            while (convoyRoute == null)
            {
                yield return null;
            }
            SetStateAs_OnCruisse();
            StopCoroutine(SetOnCruisseSync());
        }
        IEnumerator DestinationSync()
        {
            while (convoyRoute == null)
            {
                yield return null;
            }
            ArriveToDestination();
            StopCoroutine(DestinationSync());
        }
        #endregion
    }
    public class AI_LocalMerchant : ClassAI
    {
        private void Start()
        {
            // SetStateAs_AtPort();
        }

        //-------------------- AI CLASS NAME
        public override string GetAIRol() { return "Mercante"; }
        public override string GetGentilism(Kingdom kingdom) { return kingdom.GENTILISM_MALESIN; }

        //-------------------- DETINATION MANAGING
        protected override Vector3 GetDestination(KeyPoint port)
        {
            return port.transform.GetChild(0).position;
        }
        public override void ArriveToDestination()
        {
            //¿Qué hago si llego a destino?
            SetStateAs_AtPort();
            var iT = GetComponent<IsTarget>();
            if (iT != null)
                iT.Safe();
        }

        //-------------------- AI STATES
        #region IA STATES
        public override void SetStateAs_OnCruisse(KeyPoint currentPort = null)
        {
            try
            {
                Kingdom kingdom = transform.parent.parent.GetComponent<Kingdom>();
                //En el futuro, los mercantes tendrán que poder entrar en puertos con los que se tenga un acuerdo comercial
                if (currentPort != null)
                {
                    _thisConvoy.currentPort = currentPort;
                }
                else
                {
                    _thisConvoy.currentPort = _thisConvoy.currentPort.NewDestinationFromThisPort(kingdom);
                }

                _thisConvoy.SetIADestination(GetDestination(_thisConvoy.currentPort));
                _thisConvoy.convoyCurrentState = ConvoyState.Sailing;
                collider.enabled = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("error at " + transform.name + " convoy");
                Debug.Log(GetComponent<ConvoyNPC>().convoyName);
                Debug.Log(transform.parent);
                Debug.Log(transform.parent.parent);
                Debug.LogError(e);
            }
        }
        public override void SetStateAs_AtPort()
        {
            //esto es pa que no compile
            base.SetStateAs_AtPort();
            CancelInvoke();
            Invoke("onCruisseAction", Random.Range(90, 180));

            //Add this convoy to settlement's list
            if (_thisConvoy.currentPort is MB_City)
            {
                var port = (MB_City)_thisConvoy.currentPort;
                port.convoysInThisPort.Add(_thisConvoy);
            }
        }
        private void onCruisseAction()
        {
            SetStateAs_OnCruisse();
        }
        #endregion
    }

    public class AI_Patrol : ClassAI
    {
        private void Start()
        {
            // StartCoroutine(SetStateAs_AtPortSync());
        }
        IEnumerator SetStateAs_AtPortSync()
        {
            while (_thisConvoy.currentPort == null)
            {
                yield return null;
            }
            SetStateAs_AtPort();
            StopAllCoroutines();
        }

        //-------------------- AI CLASS NAME
        public override string GetAIRol() { return "Patrulla"; }
        public override string GetGentilism(Kingdom kingdom) { return kingdom.GENTILISM_FEMSIN; }

        //-------------------- DETINATION MANAGING
        protected override Vector3 GetDestination(KeyPoint port)
        {
            return port.transform.GetChild(0).position;
        }
        public override void ArriveToDestination()
        {
            //¿Qué hago si llego a destino?
            var iT = GetComponent<IsTarget>();
            if (iT != null)
                iT.Safe();
            SetStateAs_AtPort();
        }

        //-------------------- AI CHASE
        public void ChaseTarget(Convoy target)
        {
            //¿Cómo establezco el destino?
        }

        //-------------------- AI STATES
        public override void SetStateAs_OnCruisse(KeyPoint currentPort = null)
        {
            try
            {
                //En el futuro, las patrullas tendrán que poder ir no sólo a ciudades, si no a keypoints de agua (propias, enemigas o alidas)
                Kingdom kingdom = transform.parent.parent.GetComponent<Kingdom>();
                if (currentPort != null)
                {
                    _thisConvoy.currentPort = currentPort;
                }
                else
                {
                    _thisConvoy.currentPort = _thisConvoy.currentPort.NewDestinationFromThisPort(kingdom, true, 250);

                }
                _thisConvoy.SetIADestination(GetDestination(_thisConvoy.currentPort));
                _thisConvoy.convoyCurrentState = ConvoyState.Sailing;
                collider.enabled = true;
            }
            catch (System.Exception e)
            {
                Debug.LogError("error at " + transform.name + " convoy");
                Debug.Log(GetComponent<ConvoyNPC>().convoyName);
                Debug.Log(transform.parent);
                Debug.Log(transform.parent.parent);
                Debug.LogError(e);
            }

        }
        public override void SetStateAs_AtPort()
        {
            base.SetStateAs_AtPort();
            CancelInvoke();
            Invoke("onCruisseAction", 240);

            //Add this convoy to settlement's list
            var port = (MB_City)_thisConvoy.currentPort;
            port.convoysInThisPort.Add(_thisConvoy);
        }

        private void onCruisseAction()
        {
            SetStateAs_OnCruisse();
        }
    }
    public class AI_Pirate : ClassAI
    {
        //Character

        public Pirate pirateCharacter;

        private void OnEnable()
        {
            StartCoroutine("OnEnableSync");
        }

        IEnumerator OnEnableSync()
        {
            while (!_thisConvoy.HasSprite())
            {
                yield return null;
            }
            SetStateAs_AtPort();
        }
        private void Start()
        {
            SetStateAs_AtPort();
        }
        public void CreatePirateCharacter(ushort k)
        {
            //pirateCharacter = new Pirate(Random.Range(16, 90), k); //Karma aleatorio?
            pirateCharacter = new Pirate(k);
        }
        public override string GetAIRol() { return "Pirata "; }
        public override string GetGentilism(Kingdom kingdom) { return string.Empty; }
        protected override Vector3 GetDestination(KeyPoint port)
        {
            return port.transform.GetChild(0).position;
        }
        public override void ArriveToDestination()
        {
            //¿Qué hago si llego a destino?
            SetStateAs_AtPort();
            var iT = GetComponent<IsTarget>();
            if (iT != null)
                iT.Safe();
        }
        public void ChaseTarget(Convoy target)
        {
            //¿Cómo establezco el destino?
        }

        //-------------------- AI STATES
        #region IA STATES
        public override void SetStateAs_OnCruisse(KeyPoint currentPort = null)
        {
            if (currentPort != null)
            {
                _thisConvoy.currentPort = currentPort;
            }
            currentPort = _thisConvoy.currentPort;

            if (currentPort is MB_City)
            {
                var city = (MB_City)currentPort;
                city.GetOut(pirateCharacter);
            }
            else if (currentPort is MB_PirateShelter)
            {
                var shelter = (MB_PirateShelter)currentPort;
                shelter.GetOut(pirateCharacter);
            }
            //Si el jugador está en puerto el convoy no se va
            //if ()
            //{
            //    SetStateAs_AtPort();
            //}
            currentPort = Random.Range(0, 2) == 1 ? GameManager.gm.GetCity(currentPort) : GameManager.gm.GetShelter(currentPort);

            //PARCHE: ESTABLECER RUTA A BELIZE DA ERRORES DE PATHFINDING AL SALIR DE LOS PUERTOS DE LAS ANTILLAS
            if (currentPort.cityName == "Belize" && Vector3.Distance(transform.position, currentPort.transform.position) > 150) currentPort = GameManager.gm.GetShelter(currentPort);

            _thisConvoy.currentPort = currentPort;
            _thisConvoy.SetIADestination(GetDestination(currentPort));
            _thisConvoy.convoyCurrentState = ConvoyState.Sailing;
            collider.enabled = true;
        }
        public override void SetStateAs_AtPort()
        {
            base.SetStateAs_AtPort();
            CancelInvoke();
            if (_thisConvoy.currentPort is MB_City)
            {
                var city = (MB_City)_thisConvoy.currentPort;
                city.GetIn(pirateCharacter);
            }
            else if (_thisConvoy.currentPort is MB_PirateShelter)
            {
                var shelter = (MB_PirateShelter)_thisConvoy.currentPort;
                shelter.GetIn(pirateCharacter);
            }
            Invoke("onCruisseAction", 480); //pero si el jugador está en puerto vuelve a contar

            //Add this convoy to settlement's list
            if (_thisConvoy.currentPort is MB_City)
            {
                var port = (MB_City)_thisConvoy.currentPort;
                port.convoysInThisPort.Add(_thisConvoy);
            }
            else if (_thisConvoy.currentPort is MB_PirateShelter)
            {
                var port = (MB_PirateShelter)_thisConvoy.currentPort;
                port.convoysInThisPort.Add(_thisConvoy);
            }
        }

        private void onCruisseAction()
        {
            SetStateAs_OnCruisse();
        }
        #endregion
    }
    //-----------------------------------------------------------------------------------------------------
    public class IsTarget : MonoBehaviour
    {
        public List<Convoy> chasers = new List<Convoy>();

        private void OnEnable()
        {
            GetComponent<Convoy>().isOnTarget = true;
        }
        public void MakeChasersRecalculatePath()
        {
            var temp = new List<Convoy>(chasers);
            foreach (Convoy c in temp)
            {
                c.RecalculateByTargetChanges();
            }
        }
        public void Safe()
        {
            var temp = new List<Convoy>(chasers);
            foreach (Convoy c in temp)
            {
                c.TargetLost();
            }
            Destroy(this);
        }
        private void OnDestroy()
        {
            GetComponent<Convoy>().isOnTarget = false;
        }
    }
}



