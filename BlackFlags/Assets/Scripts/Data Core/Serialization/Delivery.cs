using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Delivery es una estructura de clase que contiene un dato y luego se destruye.
/// La utilidad radica en guardar un dato entre escenas y destruirlo con la entrega del dato,
/// evitando reservar memoria para datos estáticos persistentes.
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class Delivery<T> : MonoBehaviour
{
    public T shipmentData;

    void Awake()
    {
        transform.tag = "Delivery";
        DontDestroyOnLoad(gameObject);
    }

    public T Deliver()
    {
        Kill();
        return shipmentData;
    }
    public void Kill() { Destroy(gameObject); } //Destruye el objeto

}
