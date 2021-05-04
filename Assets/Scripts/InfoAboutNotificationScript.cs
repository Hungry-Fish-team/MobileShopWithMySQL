using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoAboutNotificationScript : MonoBehaviour
{
    public Text ownText;
    public Text dateText;

    public string message;
    public string date;

    private void InitializationAllObjects()
    {
        ownText = transform.GetChild(1).GetComponent<Text>();
        dateText = transform.GetChild(2).GetComponent<Text>();
    }

    private void LoadAllText()
    {
        ownText.text = message;
        dateText.text = date;
    }

    private void Start()
    {
        InitializationAllObjects();
        LoadAllText();
    }
}
