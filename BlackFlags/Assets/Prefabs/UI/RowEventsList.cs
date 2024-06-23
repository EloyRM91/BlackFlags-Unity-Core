using UnityEngine.UI;
using UnityEngine;

public class RowEventsList : MonoBehaviour
{
    public void SetRowData(Sprite sp, string date, string body)
    {
        transform.GetChild(0).GetComponent<Image>().sprite = sp;
        transform.GetChild(1).GetComponent<Text>().text = $"***   {date}   ***";
        transform.GetChild(2).GetComponent<Text>().text = body;
    }
}
