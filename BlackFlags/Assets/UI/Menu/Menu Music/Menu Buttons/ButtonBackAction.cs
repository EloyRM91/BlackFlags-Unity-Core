using UnityEngine;
using UnityEngine.UI;

public class ButtonBackAction : MonoBehaviour
{
    public int panelTo;
    private bool KeywordResponsive = true;
    public bool lockBackSpace;

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
        var i = lockBackSpace ? Input.GetKeyDown(KeyCode.Z) : Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.Delete);
        if (i)
        {
            if(KeywordResponsive)
            {
                Action(); KeywordResponsive = false;
                GetComponent<Button>().interactable = false;
            }
        }
    }

    private void Action()
    {
        PanelFade.Back(panelTo);
    }
}
