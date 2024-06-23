using UnityEngine;
using DG.Tweening;

public class Seq11 : MonoBehaviour
{
    public Transform cam, camTargetPos, ship;
    public float t, freezeT;
    private float speed;
    void Start()
    {
        Invoke("Action", freezeT);
    }

    private void FixedUpdate()
    {
        ship.position += ship.forward * speed * Time.deltaTime;
    }

    private void Action()
    {
        speed = 13;
        cam.DOLocalMove(camTargetPos.position, t).SetEase(Ease.InOutQuad);
        cam.DORotateQuaternion(camTargetPos.rotation, t).SetEase(Ease.InOutQuad);
    }
}
