using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullInfoAboutItem : MonoBehaviour
{
    public GameManager gameManager;
    public LoadScript loadScript;
    public ItemScript item;
    public ItemScript lastItem;

    public GameObject imagePrefab;
    public GameObject prefabOfSize;

    public GameObject imageScrollViewForItem, ownInfoAboutItem;

    public GameObject firstCostText, secondCostText, nameItemText, briefInfoOfItemText, compositionOfItemText, manufacturingFirmText, contentForSizes;

    [SerializeField]
    bool isInfoWasLoaded = false;

    void Start()
    {
        InitializationAllItems();
        FindItemFromFile(gameManager.chousenItem);
        LoadAllInfo();
    }

    void Update()
    {
        FindItemFromFile(gameManager.chousenItem);
        LoadAllInfo();
    }

    private void InitializationAllItems()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();

        imageScrollViewForItem = GameObject.Find("ImageScrollViewForItem");
        ownInfoAboutItem = GameObject.Find("OwnInfoAboutItem");

        firstCostText = ownInfoAboutItem.transform.GetChild(0).gameObject;
        secondCostText = ownInfoAboutItem.transform.GetChild(1).gameObject;
        nameItemText = ownInfoAboutItem.transform.GetChild(2).gameObject;
        briefInfoOfItemText = ownInfoAboutItem.transform.GetChild(3).gameObject;
        compositionOfItemText = ownInfoAboutItem.transform.GetChild(4).gameObject;
        manufacturingFirmText = ownInfoAboutItem.transform.GetChild(5).gameObject;
        contentForSizes = ownInfoAboutItem.transform.GetChild(6).gameObject;
    }

    private void FindItemFromFile(string nameOfItem)
    {
        if (lastItem == null || lastItem.nameItem != nameOfItem)
        {
            foreach (ItemScript itemFromMass in loadScript.items)
            {
                if (itemFromMass.name == nameOfItem)
                {
                    item = itemFromMass;
                    break;
                }
            }

            isInfoWasLoaded = false;
        }
    }

    private void LoadAllInfo()
    {
        if (isInfoWasLoaded == false)
        {
            lastItem = item;

            LoadAllImageOfItem();
            if (item != null)
            {
                firstCostText.GetComponent<Text>().text = item.firstCostItem.ToString();
                secondCostText.GetComponent<Text>().text = item.secondCostItem.ToString();
                nameItemText.GetComponent<Text>().text = item.nameItem.ToString();
                briefInfoOfItemText.GetComponent<Text>().text = item.briefInfoOfItem.ToString();
                compositionOfItemText.GetComponent<Text>().text = item.compositionOfItem.ToString();
                manufacturingFirmText.GetComponent<Text>().text = item.manufacturingFirm.ToString();

                LoadAllSizeOfItem();

                isInfoWasLoaded = true;
            }
        }
    }

    private void LoadAllImageOfItem()
    {
        DestroyAllImageOfItem();

        if (item != null)
        {
            if (item.imagesOfItem != null)
            {
                for (int i = 0; i < item.imagesOfItem.Length; i++)
                {
                    GameObject newImagePrefab = Instantiate(imagePrefab, imageScrollViewForItem.transform.GetChild(0).GetChild(0).GetChild(0));
                    newImagePrefab.GetComponent<Image>().sprite = item.imagesOfItem[i];
                }
            }
        }
    }

private void DestroyAllImageOfItem()
{
    for (int i = 0; i < imageScrollViewForItem.transform.GetChild(0).GetChild(0).GetChild(0).childCount; i++)
    {
        Destroy(imageScrollViewForItem.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(i).gameObject);
    }
}

private void LoadAllSizeOfItem()
{
    DestroyAllPrefabOfSize();

    for (int i = 0; i < item.sizeOfItem.Length; i++)
    {
        GameObject newPrefabOfSize = Instantiate(prefabOfSize, contentForSizes.transform);
        newPrefabOfSize.transform.GetChild(0).GetComponent<Text>().text = item.sizeOfItem[i].ToString();
        //newPrefabOfSize.GetComponent<Button>().interactable = false;
    }
}

private void DestroyAllPrefabOfSize()
{
    for (int i = 0; i < contentForSizes.transform.childCount; i++)
    {
        Destroy(contentForSizes.transform.GetChild(i).gameObject);
    }
}
}
