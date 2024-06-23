using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics.Data;
using GameMechanics.Ships;
using UI.WorldMap;

//Lists
using System.Linq;

//Interaction
using UnityEngine.EventSystems;

//Tweening
using DG.Tweening;

namespace GameMechanics.WorldCities
{
    /// <summary>
    /// A key point is a physic point in game world where the player and npcs can get into/go out.
    /// </summary>
    public abstract class KeyPoint : MonoBehaviour, IsSelectable 
    {
        #region VARIABLES
        //Data & performance
        public string cityName;
        [SerializeField] private Sprite spriteNonSelected, spriteHighlighted;
        [SerializeField] private GameObject linkedUIBanner;
        public bool revealed = false;
        protected static KeyPoint currentSelected;

        //Events
        public delegate void Selection();
        public static event Selection KPSelected;
        #endregion

        //Unselect
        public static void Unselect()
        {
            if(currentSelected != null)
            {
                currentSelected.Highlight(false);
                currentSelected = null;
                var cs = MinimapSprite.currentSelected;
                if (cs != null)
                {
                    cs.Unselect();
                }
            }
        }
        public void Highlight(bool h)
        {
            GetComponent<SpriteRenderer>().sprite = h ? spriteHighlighted : spriteNonSelected;
        }

        //On Map Discovery
        public void Reveal()
        {
            revealed = true;
            GetComponent<Collider>().enabled = true;
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            transform.GetChild(2).gameObject.SetActive(true);
            linkedUIBanner.SetActive(true);

            //Tweening :3
            Sequence sequence = DOTween.Sequence();
            sequence.Append(GetComponent<SpriteRenderer>().DOFade(1, 1.2f).SetUpdate(true));
            sequence.Play();
        }

#region INTERFACES
        // As selectable object
        public virtual void OnMouseEnter()
        {
            if (!UIMap.panelON)
            {
                if (revealed)
                {
                    CursorManager.SetCursor(1);
                    Highlight(true);
                }
            }     
        }
        public virtual void OnMouseExit()
        {
            //if (!UIMap.panelON)
            //{
            //    if (revealed)
            //    {
            //        CursorManager.SetCursor(0);
            //        Highlight(false);
            //    }
            //}
            if (revealed)
            {
                CursorManager.SetCursor(0);
                Highlight(false);
            }
        }
        public virtual void OnMouseDown()
        {
            if (revealed)
            {
                if (!UIMap.GetGraphicRaycastResult())
                {
                    StartCoroutine("AvoidUIRaycasting");
                    //Ask UISelector to throw an event
                    UISelector.instance.Unselect();
                    CursorManager.SetCursor(0);
                    if (!UIMap.panelON && UIMap.ui.gameObject.activeSelf)
                    {
                        currentSelected = this;
                        var minimapSprite = currentSelected.transform.GetChild(2).GetComponent<MinimapSprite>();
                        if (minimapSprite != null)
                        {
                            minimapSprite.Select();
                        }

                        //Event: Keypoint is selected
                        KPSelected();

                        //Delay to avoid ui conflicts on clicks
                        StopCoroutine(ClickActionDelay());
                        StartCoroutine(ClickActionDelay());
                    }
                }
            }
        }

        private IEnumerator ClickActionDelay()
        {
            yield return null;

            yield return null;
            if (PersistentGameData.playerIsOnPort && PersistentGameData.playerCurrentPort == this)
            {
                DisplayKeypointPanel();
            }
            else
            {
                DisplayInfo();
            }
        }

        protected abstract void DisplayKeypointPanel();
        protected abstract void DisplayInfo();
#endregion

        //Fixing GUIRaycasting
        IEnumerator AvoidUIRaycasting()
        {
            UIMap.ui.canGraphicRaycasting = false;
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            UIMap.ui.canGraphicRaycasting = true;
        }
        //IA
        public virtual Settlement NewDestinationFromThisPort(string tag, bool onlyCities = false, int dis = 100) { return null; }
        public virtual Settlement NewDestinationFromThisPort(Kingdom kingdom, bool onlyCities = false, int dis = 100) { return null; }
        public Sprite GetSprite() { return spriteNonSelected; }
    }

    /// <summary>
    /// A settlement is a keypoint with particular functions, like producing resources
    /// </summary>
    public abstract class Settlement : KeyPoint
    {
        public int[] exportsIndex;
        public List<Resource> exports;

        public void SetExportsFromIndex()
        {
            exports = new List<Resource>();
            for (int i = 0; i < exportsIndex.Length; i++)
            {
                exports.Add(EconomyBehaviour.GetResource((byte)exportsIndex[i]));
            }
        }
        public bool Contains(int index)
        {
            for (int i = 0; i < exportsIndex.Length; i++)
            {
                if (exportsIndex[i] == index) return true;
            }
            return false;
        }

        /// <summary>
        /// Get a settleent to go from this current port. It will go to a same kingdom city-town, or anothr countries if one or more trade agreement has been set
        /// </summary>
        /// <param name="onlyCities"></param>
        /// <returns></returns>
        public override Settlement NewDestinationFromThisPort(string tag, bool onlyCities = false, int dis = 100)
        {
            Kingdom kingdom = GameManager.gm.GetKingdombyTag(tag);

            var list = onlyCities ? new List<Settlement>(kingdom.GetPortsList().Where(x => x is MB_City).ToList()) : new List<Settlement>(kingdom.GetPortsList());
            list.Remove(this); //current port is not a valid destination

            List<Settlement> tempList;
            foreach (Kingdom traderK in kingdom.atTradeAgreementWith)
            {
                 tempList = onlyCities ? traderK.GetPortsList().Where(x => x is MB_City).ToList() : traderK.GetPortsList();
                list.AddRange(tempList.AsEnumerable());
            }
            tempList = new List<Settlement>();
            foreach (Settlement port in list)
            {
                if (Vector3.Distance(transform.position, port.transform.position) < dis)
                    tempList.Add(port);
            }
            return tempList[Random.Range(0, tempList.Count)];
        }

        public override Settlement NewDestinationFromThisPort(Kingdom kingdom, bool onlyCities = false, int dis = 100)
        {
            var list = onlyCities ? new List<Settlement>(kingdom.GetPortsList().Where(x => x is MB_City).ToList()) : new List<Settlement>(kingdom.GetPortsList());
            list.Remove(this); //current port is not a valid destination

            List<Settlement> tempList;
            foreach (Kingdom traderK in kingdom.atTradeAgreementWith)
            {
                tempList = onlyCities ? traderK.GetPortsList().Where(x => x is MB_City).ToList() : traderK.GetPortsList();
                list.AddRange(tempList.AsEnumerable());
            }
            tempList = new List<Settlement>();
            foreach (Settlement port in list)
            {
                if (Vector3.Distance(transform.position, port.transform.position) < dis)
                    tempList.Add(port);
            }
            if(tempList.Count != 0)
                return tempList[Random.Range(0, tempList.Count)];
            return null;
        }
    }
    

}

