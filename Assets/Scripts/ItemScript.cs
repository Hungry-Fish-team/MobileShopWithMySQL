using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Create new Item")]

public class ItemScript : ScriptableObject
{
    public string nameItem;
    public string vendorCode;
    public string firstTypeItem;
    public string secondTypeItem;

    public float firstCostItem, secondCostItem;
    public int[] sizeOfItem;

    public string briefInfoOfItem;
    public string compositionOfItem;
    public string manufacturingFirm;

    public string descriptionOfItem;

    public Sprite[] imagesOfItem;

    public bool isThereItemOnStore = true;
}
