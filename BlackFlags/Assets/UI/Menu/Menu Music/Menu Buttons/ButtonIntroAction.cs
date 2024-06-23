using UnityEngine;
using UnityEngine.UI;

public class ButtonIntroAction : MonoBehaviour
{
    public int panelTo;
    private bool KeywordResponsive = true;

    private void OnEnable()
    {
        CancelInvoke();
        KeywordResponsive = true;
        GetComponent<Button>().interactable = true;
    }
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Action());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (KeywordResponsive)
            {
                Action(); 
                KeywordResponsive = false;
                GetComponent<Button>().interactable = false;
            }
        }
    }

    protected virtual void Action()
    {
        PanelFade.Next(panelTo);
    }
}
