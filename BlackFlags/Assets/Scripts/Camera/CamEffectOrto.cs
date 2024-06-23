using DG.Tweening;
using UnityEngine;

public class CamEffectOrto : AdaptiveCamEffect
{
    [SerializeField] protected float initialSize, finalSize;
    public override void Effect()
    {
        thisCamera.orthographicSize = initialSize;
        thisCamera.DOOrthoSize(finalSize, 0.3f);
    }
}
