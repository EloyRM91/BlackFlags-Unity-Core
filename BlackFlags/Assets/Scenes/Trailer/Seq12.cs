using UnityEngine;
using DG.Tweening;

public class Seq12 : MonoBehaviour
{
    public Transform cam, ship1, ship2, camTarget;
    private float speed;
    void Start()
    {
        Invoke("Action", 3.4f);
    }

    //private void FixedUpdate()
    //{
    //    ship1.position += ship1.forward * speed * Time.deltaTime;
    //    ship2.position += ship2.forward * speed * Time.deltaTime;
    //}

    private void Action()
    {
        //speed = 9;
        //cam.DOLocalMoveY(cam.localPosition.y + 32, 2.5f).SetEase(Ease.InOutQuad);
        //cam.DORotate(new Vector3(cam.rotation.eulerAngles.x, 11, cam.rotation.eulerAngles.z),2).SetEase(Ease.InOutQuad);
        //ship1.DORotate(new Vector3(ship1.rotation.eulerAngles.x, 210, ship1.rotation.eulerAngles.z), 7);
        //ship2.DORotate(new Vector3(ship2.rotation.eulerAngles.x, -130, ship2.rotation.eulerAngles.z),7);

        cam.DORotate(new Vector3(0, cam.rotation.eulerAngles.y, cam.rotation.eulerAngles.z), 2.2f).SetEase(Ease.InOutQuint);
        cam.DOLocalMoveY(cam.localPosition.y + 33, 2.2f).SetEase(Ease.InOutQuint);
        cam.DOLocalMoveZ(cam.localPosition.z - 1, 2.2f).SetEase(Ease.InOutQuad);
        cam.DOLocalMoveX(cam.localPosition.x - 4, 2.2f).SetEase(Ease.InOutQuad);
    }
}
