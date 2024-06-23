using System.Collections;
using UnityEngine;
using UnityEngine.UI;
//Mechanics
using GameMechanics.WorldCities;

namespace UI.WorldMap
{
    public class UIPlayerSprites : UIShipSprites
    {
        [SerializeField] private Transform playerTarget;
        private void Start()
        {
            var img = PersistentGameData._GData_PlayerAvatar;
            transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = img;
            transform.GetChild(1).GetChild(0).GetComponent<Image>().sprite = img;

            //Events
            PlayerMovement._EVENT_ArriveToPort += OnPortBannerMode;
            PlayerMovement._EVENT_ExitFromPort += OnCruisseBannerMode;
        }

        private void OnDestroy()
        {
            //Events
            PlayerMovement._EVENT_ArriveToPort -= OnPortBannerMode;
            PlayerMovement._EVENT_ExitFromPort -= OnCruisseBannerMode;
        }

        //public static void SetPortTarget(Transform target)
        //{
        //    portTarget = target;
        //}

        private void OnPortBannerMode(KeyPoint k)
        {
            SetAsTarget(k.transform);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
            //StartCoroutine(Delay(k));
        }
        IEnumerator Delay(KeyPoint k)
        {
            yield return new WaitForSeconds(1.1f);
            SetAsTarget(k.transform);
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        private void OnCruisseBannerMode()
        {
            StopAllCoroutines();
            SetAsTarget(playerTarget);
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}


