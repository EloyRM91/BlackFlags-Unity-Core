using System.Collections;
using System.Collections.Generic;
using UnityEngine;using UnityEngine.UI;
using DG.Tweening;

public class Seq4 : MonoBehaviour
{
    public Transform ship1, ship2, ship3, cam;
    public CannonBehaviour cannon1, cannon2, cannon3, cannon4, cannon5, cannon6, cannon7, cannon8, cannon9, cannon10, cannon11, cannon12;
    public ParticleSystem impact;
    //public ParticleSystem impact;

    private float speed1 = 0, speed2 = 0, speed3;

    void Start()
    {
        Invoke("Sequence", 2f);
    }

    private void FixedUpdate()
    {
        ship1.position += ship1.forward * speed1 * Time.deltaTime;
        ship2.position += ship2.forward * speed2 * Time.deltaTime;
        ship3.position += ship3.forward * speed3 * Time.deltaTime;

    }

    private void Sequence()
    {

        cam.DORotate(new Vector3(cam.rotation.eulerAngles.x, 152, cam.rotation.eulerAngles.z),3);
        //ship1.DOLocalRotate(new Vector3(0, 4, 0), 3f);
        //ship2.DOLocalRotate(new Vector3(0, -10, 0), 7);
        

        Sequence sequence = DOTween.Sequence();

        //T1
        sequence.AppendCallback(() => { speed1 = 7; speed2 = 7; speed3 = 5; });
        sequence.AppendCallback(() => { cannon1.Shoot(); cannon2.Shoot(); });
        sequence.AppendInterval(0.5f);

        //T2
 
        sequence.AppendCallback(() => { ship3.DOLocalRotate(new Vector3(0, 9, 0), 6); });
        sequence.AppendCallback(() => { ship2.DOLocalRotate(new Vector3(0, -146, 0), 6); });
        sequence.AppendCallback(() => { ship1.DOLocalRotate(new Vector3(0, 166, 0), 6); });
        sequence.AppendCallback(() => { cannon3.Shoot(); cannon4.Shoot(); });
        sequence.AppendInterval(0.21f);

        //T3
        sequence.AppendCallback(() => { cannon5.Shoot(); });
        sequence.AppendInterval(0.21f);

        //T4
        sequence.AppendCallback(() => { cannon6.Shoot();});
        sequence.AppendCallback(() => { cannon7.Shoot(); cannon8.Shoot(); });
        sequence.AppendInterval(0.21f);

        //T5
        sequence.AppendCallback(() => { cannon9.Shoot(); cannon10.Shoot(); });
        sequence.AppendInterval(0.21f);

        //T6
        sequence.AppendCallback(() => { cannon11.Shoot(); cannon12.Shoot(); });

    }

}
