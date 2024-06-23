using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoadingAnimation : MonoBehaviour
{
    [SerializeField] Image[] imgs;
    void Start()
    {
        StartCoroutine("Sequence");
    }

    IEnumerator Sequence()
    {
        while (true)
        {
            for (int i = 0; i < 3; i++)
            {
                imgs[i].enabled = false;
            }
            yield return new WaitForSeconds(0.4f);
            for (int i = 0; i < 3; i++)
            {
                imgs[i].enabled = true;
                yield return new WaitForSeconds(0.4f);
            }
        }
    }
}
