using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : MonoBehaviour
{
    private static byte index;
    [SerializeField] private byte thisButtonIndex;
    [SerializeField] private string description;
    private static Transform selectionFrame;
    private static Text descriptionText;
    private static Text currentSelected;
    private Text thisText;
    private static Color selectionColor = new Color(1, 0.734f, 0, 1);
    void Start()
    {
        thisText = GetComponent<Text>();
        if (thisButtonIndex == 2) //is this the first row?
        {
            var x = transform.parent.parent;
            selectionFrame = x.GetChild(1);
            descriptionText = x.GetChild(2).GetChild(0).GetComponent<Text>();
            //Set frame position
            selectionFrame.position = transform.position;
            Invoke("Select", 0.01f);
        }
    }

    public void Select()
    {
        //Set scene index
        index = thisButtonIndex;
        if (currentSelected != null)
            currentSelected.color = Color.white;
        //Select this
        currentSelected = thisText;
        currentSelected.color = selectionColor;
        //Set frame position
        selectionFrame.position = transform.position;
        //Tutorial text banner
        descriptionText.text = description;
    }
    public void LoadTutorial()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
