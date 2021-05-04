using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeButtomScript : MonoBehaviour
{
    public string sizeOfItem = "";

    public void ChouseThisSize()
    {
        transform.parent.parent.GetComponent<InfoAboutItemToBuyScript>().chousenSize = sizeOfItem;
    }
}
