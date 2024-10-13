using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// An in-menu button animation controller. It displays a description text
/// </summary>
public class UI_FlagButton : MonoBehaviour
{
    [SerializeField] private GameObject[] lockerElements;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Click);
    }

    private void Click()
    {
        if(CanSetOff())
        {
            transform.parent.GetComponent<Animator>().SetTrigger("Click");
            Invoke("SetOff", 4);
        }
    }

    private void SetOff()
    {
        Destroy(gameObject);
    }

    private bool CanSetOff()
    {
        for (int i = 0; i < lockerElements.Length; i++)
        {
            if(lockerElements[i].activeSelf)
                return false;
        }
        return true;
    }
}
