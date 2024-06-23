using System.Collections;
using UnityEngine;
using GameMechanics.Data;
using UnityEngine.AI;
using GameMechanics.AI;

namespace GameMechanics.Ships
{
    public class ConvoyNPC : Convoy
    {
        //public ShipType_ROLE convoyRole;
        public ConvoyState convoyCurrentState;
        [SerializeField] public Ship[] thisConvoyShips;
        private Convoy thisConvoyTarget;
        public string convoyName, convoyCaptain;

        protected override void Start() { }

        //Set data from ships convoy
        public void SetConvoyData()
        {
            convoySpeed = thisConvoyShips[0].GetMinSmoothSpeed();
            var leadership = thisConvoyShips[0];
            var power = thisConvoyShips[0].GetFirePower();
            for (int i = 1; i < thisConvoyShips.Length; i++)
            {
                var tempP = thisConvoyShips[i].GetFirePower();
                if (tempP > power)
                {
                    leadership = thisConvoyShips[i];
                    power = tempP;
                }
                var tempS = thisConvoyShips[i].GetMinSmoothSpeed();
                if (tempS < convoySpeed)
                    convoySpeed = tempS;
            }
            //convoySpeed = Mathf.Clamp(convoySpeed *= 0.25f, 0.5f, 2); 07/12/2022
            convoySpeed*= 0.33f;
            convoyName = leadership.name_Ship;
            convoyCaptain = leadership.name_Captain;

            GetSprite(transform.tag, (byte)thisConvoyShips.Length);
            _thisConvoySpriteController.SetSpritesSet(leadership.GetSpriteIndex());
        }

        public override Vector3 GetDestination() { return _destination; }

#region Tweening & Effects
        public override void Appear()
        {
           if(convoyCurrentState != ConvoyState.AtPort)
                base.Appear();
        }
#endregion
#region IA Movement
        public void SetIADestination(Vector3 point)
        {
            StopAllCoroutines();
            if (CanGoTo(point))
            {
                _destination = point;
                if (point == transform.position) Debug.Log("puto genio");
                if (path.corners.Length == 1) SetIADestination(point);
                Move(path.corners);
            }
        }
        protected override void Move(Vector3[] corners)
        {
            StartCoroutine(CheckDestination(corners));
            StartCoroutine("Movement");
        }
        IEnumerator CheckDestination(Vector3[] waypoints)
        {
            transform.rotation = Quaternion.LookRotation(waypoints[1] - transform.position);
            _thisConvoySpriteController.SetSprite();

            int i = 1;
            while (i < waypoints.Length)
            {
                _destination = waypoints[i];
                if (Vector3.Distance(transform.position, _destination) < 0.245f)
                {
                    if (thisConvoyTarget != null)
                    {
                        if (Vector3.Distance(transform.position, thisConvoyTarget.transform.position) < 0.245f)
                        {
                            //alcanza el objetivo
                            StopAllCoroutines();
                            print("ataco al jugador o algo");
                        }
                        else if (i < waypoints.Length - 1 && i != 1)
                        {
                            if (Vector3.Distance(_destination, waypoints[i + 1]) > 0.5f)
                            {
                                var tempDest = Intercept(thisConvoyTarget);
                                transform.rotation = Quaternion.LookRotation(waypoints[i + 1] - transform.position);
                                _thisConvoySpriteController.SetSprite();
                            }
                        }
                    }
                    i++;
                    if (i < waypoints.Length)
                    {
                        transform.rotation = Quaternion.LookRotation(waypoints[i] - transform.position);
                        _thisConvoySpriteController.SetSprite();
                        if (isOnTarget) // chasers have to recalculate their path
                        {
                            print("Cambio rumbo y aviso a perseguidor");
                            GetComponent<IsTarget>().MakeChasersRecalculatePath();
                            //chaser.RecalculateByTargetChanges();
                        }
                    }
                    else //llega a destino
                    {
                        isOnTarget = false;
                        GetComponent<ClassAI>().ArriveToDestination();
                        StopAllCoroutines();
                    }
                }
                if (Time.timeScale == 20)
                    yield return new WaitForEndOfFrame();
                else
                {
                    yield return new WaitForSeconds(0.04f);
                }               
            }
            isOnTarget = false;
            GetComponent<ClassAI>().ArriveToDestination();
            StopAllCoroutines();
        }
#endregion
    }
}

