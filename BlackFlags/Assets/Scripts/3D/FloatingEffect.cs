using UnityEngine;
using DG.Tweening;

[ExecuteInEditMode]
public class FloatingEffect : MonoBehaviour
{
    //Parameters
    [Header("Cabeceo y alabeo")]
    [Range(0,10)]
    public float row;
    [Range(0, 10)]
    public float pitch;
    public float offsetRow;

    private void Update()
    {
        float rotRow = offsetRow + row * Mathf.Sin(1.7f * Time.realtimeSinceStartup);
        float rotPitch = pitch * Mathf.Cos(0.8f * Time.realtimeSinceStartup);
        transform.rotation = Quaternion.Euler(rotPitch, transform.rotation.eulerAngles.y, rotRow);
    }

}
