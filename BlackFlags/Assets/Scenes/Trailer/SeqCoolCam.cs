using UnityEngine;
using DG.Tweening;

public class SeqCoolCam : MonoBehaviour
{
    [SerializeField] Transform pivot, ship;
    private float speed;
    void Start()
    {
        Invoke("Action",3);
    }

    private void FixedUpdate()
    {
        ship.position += ship.forward * speed * Time.deltaTime;
    }

    private void Action()
    {
        //pivot.DORotate(new Vector3(pivot.rotation.eulerAngles.x, 181, pivot.rotation.eulerAngles.z),1).SetEase(Ease.InOutQuad);
        //pivot.DOLocalMoveZ(pivot.position.z + 30, 1).SetEase(Ease.InOutQuad);

        speed = 9;
        pivot.DOLocalMoveZ(pivot.position.z + 120, 6).SetEase(Ease.InOutQuint);
        pivot.DOLocalMoveX(pivot.position.x -20, 6).SetEase(Ease.InOutQuint);
        pivot.DORotate(new Vector3(pivot.rotation.eulerAngles.x, 120, pivot.rotation.eulerAngles.z), 6).SetEase(Ease.InOutQuint); ;
    }
}
