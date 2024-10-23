using UnityEngine; using System.Collections;

//Pathfinding
using UnityEngine.AI;

//Game Mechanics
using GameMechanics.Data;
using GameMechanics.WorldCities;
using UI.WorldMap;
using GameMechanics.AI;
//Tweening
using DG.Tweening;

namespace GameMechanics.Ships
{
    public abstract class Convoy : MonoBehaviour, IsSelectable
    {
        //UI Sprite & Pooling
        protected UIShipSprites _thisConvoySpriteController;

        //Serialization
        private int _ID;
        public int ID { get { return ID; } }
        private static int IDCounter = 1; // El id 0 es el id del jugador;

        //This Convoy Data
        public float convoySpeed;

        //OnGame Dynamic Data
        public bool isOnTarget; //is this group being intercepted?
        public KeyPoint currentPort;
        private Sequence sequence;

        //Navmesh
        //protected NavMeshAgent _thisAgent;
        public NavMeshPath path;
        protected Vector3 _destination;
        protected int pathProgressIndex = 0;

        //Raycast
        protected Ray _clickRay;
        protected RaycastHit _clickHit;

        //Delegates
        public delegate void RotationEvent(Quaternion rotation); //this entity has changed its rotation
        public delegate void VoidEvent(); // this entity did something, like arriving to destination
        public delegate void DestinationEvent(Vector3 destination); //this entity has set a destination

#region Interface: As Selectable
        //As an IsSelectable implementer
        public virtual void OnMouseEnter() { }
        public virtual void OnMouseExit() { }
        public virtual void OnMouseDown() { }
#endregion

        //------------ BODY
        protected virtual void Start() { SetID();}

        // HUD Pooling
        public void GetSprite(EntityType_KINGDOM k, byte n)
        {
            _thisConvoySpriteController = PoolingShipSprites.GetSprite();
            _thisConvoySpriteController.SetAsTarget(transform);
            _thisConvoySpriteController.SetValues(UIMap.ui.GetFlag(k), n, GetComponent<ClassAI>());
        }
        public void GetSprite(string tag, byte n)
        {
            _thisConvoySpriteController = PoolingShipSprites.GetSprite();
            _thisConvoySpriteController.SetAsTarget(transform);
            _thisConvoySpriteController.ForcePosition();
            _thisConvoySpriteController.SetValues(UIMap.ui.GetFlag(tag), n, GetComponent<ClassAI>());
        }
        public bool HasSprite()
        {
            return _thisConvoySpriteController != null;
        }

        //Serialization
        #region serialization
        public void SetID()
        {
            _ID = IDCounter;
            IDCounter++;
        }

        public void SetID(int val)
        {
            _ID = val;
            if (val > IDCounter) IDCounter = val;
        }

#endregion
        // Basic
        public void SetTag(EntityType_KINGDOM kingdom)
        {
            switch (kingdom)
            {
                case EntityType_KINGDOM.KINGDOM_Spain: gameObject.tag = "KingdomSpain"; break;
                case EntityType_KINGDOM.KINGDOM_Portugal: gameObject.tag = "KingdomPortugal"; break;
                case EntityType_KINGDOM.KINGDOM_France: gameObject.tag = "KingdomFrance"; break;
                case EntityType_KINGDOM.KINGDOM_Dutch: gameObject.tag = "KingdomDutch"; break;
                case EntityType_KINGDOM.KINGDOM_Britain: gameObject.tag = "KingdomBritain"; break;
            }
        }
        public void SetTag(string tag)
        {
            gameObject.tag = tag;
        }

#region TWEENING
        public virtual void PointedOut()
        {
            sequence.Kill();
            var shipSprite = _thisConvoySpriteController.transform.GetComponent<UnityEngine.UI.Image>();
            sequence = DOTween.Sequence().SetUpdate(true);
            sequence.Append(shipSprite.DOFade(0, 0.1f));
            sequence.AppendInterval(0.2f);
            sequence.Append(shipSprite.DOFade(1, 0.1f));
            sequence.AppendInterval(0.2f);
            sequence.Append(shipSprite.DOFade(0, 0.1f));
            sequence.AppendInterval(0.2f);
            sequence.Append(shipSprite.DOFade(1, 0.1f));
            sequence.AppendInterval(0.2f);
            sequence.Play();
        }

        public virtual void Appear()
        {
            DOTween.defaultTimeScaleIndependent = true;
            var shipSprite = _thisConvoySpriteController.transform.GetComponent<UnityEngine.UI.Image>();
            sequence.Kill();
            sequence = DOTween.Sequence();
            sequence.AppendCallback(() => _thisConvoySpriteController.gameObject.SetActive(true));
            sequence.Append(shipSprite.DOFade(0, 0));
            sequence.Append(shipSprite.DOFade(1, 1.2f).SetUpdate(true));
            sequence.Play();
        }
        public virtual void Dissappear()
        {
            DOTween.defaultTimeScaleIndependent = true;
            if (_thisConvoySpriteController.gameObject.activeSelf)
            {
                var shipSprite = _thisConvoySpriteController.transform.GetComponent<UnityEngine.UI.Image>();
                sequence = DOTween.Sequence();
                sequence.Append(shipSprite.DOFade(0, 1.3f).SetUpdate(true));
                sequence.AppendCallback(() => _thisConvoySpriteController.gameObject.SetActive(this is PlayerMovement));
                sequence.Play();
            }
            if (UISelector.instance.IsTarget(transform)) UISelector.instance.Unselect();
        }
        public virtual void DissappearAndSleep()
        {
            if (_thisConvoySpriteController.gameObject.activeSelf)
            {
                var shipSprite = _thisConvoySpriteController.transform.GetComponent<UnityEngine.UI.Image>();
                sequence = DOTween.Sequence();
                sequence.Append(shipSprite.DOFade(0, 1.3f).SetUpdate(true));
                sequence.AppendCallback(() => _thisConvoySpriteController.gameObject.SetActive(false));
                sequence.AppendCallback(() => _thisConvoySpriteController.Release());
                sequence.AppendCallback(() => gameObject.SetActive(this is PlayerMovement));
                sequence.Play();
            }
        }
        public virtual void DissapearALL()
        {
            _thisConvoySpriteController.gameObject.SetActive(false);
        }
#endregion

#region routes
        public float CalculateRouteTime()
        {
            var time = 0f;
            time += Vector3.Distance(transform.position, path.corners[pathProgressIndex]) / convoySpeed;
            for (int i = pathProgressIndex; i < path.corners.Length - 1; i++)
            {
                time += Vector3.Distance(path.corners[i], path.corners[i + 1]) / convoySpeed;
            }
            return Mathf.Round(time/24);
        }
        protected virtual Vector3 Intercept(Convoy target)
        {
            target.isOnTarget = true;
            var D = Vector3.Distance(target.transform.position, transform.position);
            var alfa = Quaternion.Angle(target.transform.rotation, Quaternion.LookRotation(transform.position - target.transform.position));
            float C1 = 2 * target.convoySpeed * Mathf.Cos(alfa * Mathf.Deg2Rad);
            float C2 = Mathf.Pow(target.convoySpeed, 2) - Mathf.Pow(convoySpeed, 2);
            float C3 = Mathf.Pow(C1, 2) - 4 * C2;
            if (C3 < 0)
            {
                //No solution
                return transform.position + target.transform.forward * 15;
            }
            else
            {
                //Destination
                float t = Mathf.Abs(2 * D / (C1 + Mathf.Sqrt(C3)));

                if(t < 0)
                {
                    //No solution
                    return transform.position + target.transform.forward * 15;
                }

                return target.transform.position + target.transform.forward * t * target.convoySpeed;
            }
        }
        public virtual void RecalculateByTargetChanges() { }
        public virtual void TargetLost() { }

        protected bool CanGoTo(Vector3 d)
        {
            path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, d, NavMesh.AllAreas, path);
            return path.status == NavMeshPathStatus.PathComplete;
        }
#endregion
        //Basic movement
        protected abstract void Move(Vector3[] destination);
        public abstract Vector3 GetDestination();
        IEnumerator Movement()
        {
            float sawSpeed = 0;
            yield return new WaitForSeconds(0.1f);
            bool tick = true;
            float t;
            while (true)
            {
                if(Time.timeScale == 20)
                {
                    transform.position += transform.forward * 0.15f * Time.deltaTime * convoySpeed;
                    yield return new WaitForEndOfFrame();
                }
                else
                {
                    tick = !tick;
                    sawSpeed = tick ? 0.08f : 0;
                    transform.position += transform.forward * sawSpeed * convoySpeed;
                    t = tick ? 0.04f : 0.12f;
                    yield return new WaitForSeconds(t);
                }
            }
        }
    }
    [System.Serializable]
    public enum ConvoyState { Sailing, Chase, AtPort}
}

