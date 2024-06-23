using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// An in-menu button animation controller. It displays a description text
/// </summary>
public class UI_FlagButton : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        transform.parent.GetComponent<Animator>().SetTrigger("Click");
        Invoke("SetOff", 4);
    }

    private void SetOff()
    {
        Destroy(gameObject);
    }
}
