using System.Collections;
using System.Collections.Generic;
using GameMechanics.save;
using UnityEngine;

/// <summary>
/// Esta clase contiene los datos de los reinos y ciudades que deben ser entregados a la escena de partida.
/// Tras lo cual el objeto debe ser destruido, evitando así alojar datos estáticos innecesarios
/// </summary>
public class FleetsDataDelivery : Delivery<SerializedGameFleetData>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override SerializedGameFleetData Deliver()
    {
        Kill();
        return shipmentData;
    }
}
