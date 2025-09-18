using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Esta clase contiene los datos de los reinos y ciudades que deben ser entregados a la escena de partida.
/// Tras lo cual el objeto debe ser destruido, evitando así alojar datos estáticos innecesarios
/// </summary>
public class CitiesDataDelivery : Delivery<Transform[]>
{
    protected override void Awake()
    {
        base.Awake();
        // foreach(Transform t in shipmentData)
        // {
        //     t.parent = gameObject.transform;
        // }  
    }

    public override Transform[] Deliver()
    {
        foreach(Transform t in shipmentData)
        {
            t.SetParent(null);
        }  
        Kill();
        return shipmentData;
    }
}
