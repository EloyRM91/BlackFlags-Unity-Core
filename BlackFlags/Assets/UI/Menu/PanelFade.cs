using UnityEngine;
using UnityEngine.UI;
//Tweening
using DG.Tweening;

public class PanelFade : MonoBehaviour
{
    private static Transform t;
    private static Image _panel;
    private static float[] refPos = new float[4];

    private void OnEnable()
    {
        _panel.color = new Color(1, 1, 1, 0);
        FadeOn();
        SlidePanel(0);
    }
    void Awake()
    {
        t = this.transform;
        _panel = GetComponent<Image>();
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            refPos[i] = transform.GetChild(i).position.x;
        }
    }
    private void FadeOn()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Append(_panel.DOFade(0.788f, 0.4f));
        sequence.Play();
    }
    public static void Close()
    {
        _panel.color = new Color(1, 1, 1, 0);
        for (int i = 0; i < t.childCount - 1; i++)
        {
            t.GetChild(i).gameObject.SetActive(false);
            t.GetChild(i).position = new Vector3(refPos[i], t.GetChild(i).position.y, 0);
        }
        t.gameObject.SetActive(false);
    }
    public void SetOffPanel(int val) { transform.GetChild(val).gameObject.SetActive(false); }
    private void SlidePanel(int index)
    {
        var target = transform.GetChild(index);
        target.gameObject.SetActive(true);
        target.position += Vector3.right * Screen.width / 2;
        Sequence sequence = DOTween.Sequence();
        sequence.Append(target.DOMove(target.position - Vector3.right * refPos[index], 0.3f));
        sequence.Play();
    }
    public static void Next(int val)
    {
        t.GetChild(val - 1).gameObject.SetActive(false);
        t.GetChild(val -1).position = new Vector3(refPos[val - 1], t.GetChild(val - 1).position.y, 0);
        t.GetComponent<PanelFade>().SlidePanel(val);
    }
    public static void Back(int val)
    {
        if(val < 0)
        {
            Close();
            t.parent.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            t.GetChild(val + 1).gameObject.SetActive(false);
            t.GetChild(val + 1).position = new Vector3(refPos[val + 1], t.GetChild(val + 1).position.y, 0);
            t.GetComponent<PanelFade>().SlidePanel(val);
        }
    }
}
