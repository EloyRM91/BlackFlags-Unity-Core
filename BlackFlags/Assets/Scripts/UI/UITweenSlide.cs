using UnityEngine;
using DG.Tweening;

public class UITweenSlide : MonoBehaviour
{
    [SerializeField] private int offsetX, offsetY;
    Vector3 iniPos, endPos;

    Tween tween;
    private void Awake()
    {
        endPos = transform.position;
        iniPos = endPos + new Vector3(offsetX, offsetY, 0);
    }
    private void OnEnable()
    {
        transform.position = iniPos;
        tween = transform.DOMoveX(endPos.x, 0.5f);
        tween = transform.DOMoveY(endPos.y, 0.5f);
    }

    private void OnDisable()
    {
        tween.Kill();
    }

}
