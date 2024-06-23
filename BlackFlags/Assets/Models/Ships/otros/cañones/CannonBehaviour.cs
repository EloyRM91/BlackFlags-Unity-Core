using UnityEngine;
using DG.Tweening;

public class CannonBehaviour : MonoBehaviour
{
    [SerializeField] private float offset;
    private static GameObject projectile;

    private void Start()
    {
        projectile = Resources.Load<GameObject>("bullet");
    }
    public void Shoot()
    {
        //Bullet
        Instantiate(projectile, transform.GetChild(2).position, transform.GetChild(2).rotation);
        //Particles
        transform.GetChild(2).GetComponent<ParticleSystem>().Play();
        //Tweening (only for cinematics)
        TweenShoot();
    }

    private void TweenShoot()
    {

        transform.DOLocalMoveX(transform.localPosition.x - offset, .3f).SetEase(Ease.OutQuad);
    }
}
