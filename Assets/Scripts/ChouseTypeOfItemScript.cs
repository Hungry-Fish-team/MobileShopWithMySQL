using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChouseTypeOfItemScript : MonoBehaviour
{
    public string nameOfType;

    public void OnButtonDown()
    {
        GameObject.Find("GameManager").GetComponent<GameManager>().ChouseChousenItemsWindow(nameOfType);
    }
}
