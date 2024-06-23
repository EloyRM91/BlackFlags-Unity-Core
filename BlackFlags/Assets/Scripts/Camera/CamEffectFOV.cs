using DG.Tweening;
using UnityEngine;

public class CamEffectFOV : AdaptiveCamEffect
{
    [SerializeField] protected int initialField, finalField;

    private void OnEnable()
    {
        Effect();
    }

    public override void Effect()
    {
        thisCamera.fieldOfView = initialField;
        thisCamera.DOFieldOfView(finalField, .5f);
    }
}
