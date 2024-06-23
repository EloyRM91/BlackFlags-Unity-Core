using UnityEngine;

[RequireComponent(typeof(Camera))]
public abstract class AdaptiveCamEffect : MonoBehaviour
{
    protected Camera thisCamera;

    private void Awake()
    {
        thisCamera = GetComponent<Camera>();
    }

    public abstract void Effect();
}
