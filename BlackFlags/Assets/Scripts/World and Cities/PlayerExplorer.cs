//Core
using System.Collections.Generic;
using UnityEngine;
//Mechanics
using GameMechanics.WorldCities;
using GameMechanics.Ships;
using GameMechanics.AI;
//UI
using UI.WorldMap;

/// <summary>
/// The player's trigger manager
/// Parameters: visibleConvoys (List)
/// Constructors: None (Monobehaviour)
/// </summary>
public class PlayerExplorer : MonoBehaviour
{
    private PlayerMovement _playermovement;
    private static List<Transform> visibleConvoys = new List<Transform>();

    private void Start()
    {
        _playermovement = transform.parent.GetComponent<PlayerMovement>();
        //Events
        PlayerMovement._EVENT_ArriveToPort += ArriveToPort;
        PlayerMovement._EVENT_ExitFromPort += ExitFromPort;
    }
    private void OnDestroy()
    {
        //Events
        PlayerMovement._EVENT_ArriveToPort -= ArriveToPort;
        PlayerMovement._EVENT_ExitFromPort -= ExitFromPort;
    }

    private void ArriveToPort(KeyPoint k)
    {
        GetComponent<SphereCollider>().enabled = false;
        if(visibleConvoys.Count > 0)
        {

            foreach (Transform visibleConvoy in visibleConvoys)
            {
                print(visibleConvoy);
            }

            foreach (Transform visibleConvoy in visibleConvoys)
            {
                if(visibleConvoy != null)
                {
                    var convoy = visibleConvoy.GetComponent<Convoy>();
                    convoy.Dissappear();
                    if (UISelector.instance.IsTarget(visibleConvoy.transform))
                    {
                        UISelector.instance.Unselect();
                    }
                    visibleConvoy.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }
        visibleConvoys = new List<Transform>();
    }

    private void ExitFromPort()
    {
        GetComponent<SphereCollider>().enabled = true;
        _playermovement.Appear();
    }


    private void OnTriggerEnter(Collider other)
    {
        var point = other.GetComponent<KeyPoint>();
        if(point != null)   //-- A keypoint
        {
            if (!point.revealed)
            {
                if (point != null)
                    point.Reveal();
            }
        }
        else 
        {  
            var convoy = other.GetComponent<Convoy>();
            if (convoy != null)
            {
                convoy.Appear();
                other.transform.GetChild(0).gameObject.SetActive(true);
            }
            if(other.gameObject != gameObject && !other.CompareTag("Respawn")) 
            {
                visibleConvoys.Add(other.transform);
                var AI = other.GetComponent<AI_Pirate>();
                if (AI != null)
                {
                    AI.pirateCharacter.seenByPlayer = true;
                }
            }
        }

        
    }
    private void OnTriggerExit(Collider other)
    {
        var convoy = other.GetComponent<Convoy>();
        if (convoy != null)
        {
            visibleConvoys.Remove(other.transform);
            convoy.Dissappear();
            other.transform.GetChild(0).gameObject.SetActive(false);

            if (UISelector.instance.IsTarget(other.transform))
            {
                UISelector.instance.Unselect();
            }

            var t = _playermovement.GetTarget();
            if (t != null)
            {
                if (other == t)
                {
                    _playermovement.TargetLost();
                }
            }
        }   
    }
    /// <summary>
    /// Is the convoy target visible by player?
    /// </summary>
    /// <param name="x">convoy target</param>
    /// <returns>true if target is visible</returns>
    public static bool IsVisible(Transform x)
    {
        return visibleConvoys.Contains(x);
    }
}