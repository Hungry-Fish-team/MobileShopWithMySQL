using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoAboutItem : MonoBehaviour
{
    public GameManager gameManager;
    public LoadPersonInfoFromFilesScript loadPersonInfoFromFilesScript;

    public ItemScript item;

    public UnityEngine.UI.Image firstImage;

    public Text firstCostOfItemText, secondCostItemText;
    public Text firmsNameText;
    public Text secondTypeOfItemText;

    public Button busketButton, markButton;

    public Toggle chousenItemForBuyToggle;

    public Text isThereItemInShopText;

    public void Start()
    {
        InitializationAllItems();
        LoadInfo();
    }

    private void InitializationAllItems()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadPersonInfoFromFilesScript = GameObject.Find("GameManager").GetComponent<LoadPersonInfoFromFilesScript>();

        //item = gameObject.GetComponent<ItemScript>();

        firstImage = transform.GetChild(1).GetComponent<Image>();
        firstCostOfItemText = transform.GetChild(2).GetComponent<Text>();
        secondCostItemText = transform.GetChild(3).GetComponent<Text>();
        firmsNameText = transform.GetChild(4).GetComponent<Text>();
        secondTypeOfItemText = transform.GetChild(5).GetComponent<Text>();

        busketButton = transform.GetChild(6).GetComponent<Button>();
        markButton = transform.GetChild(7).GetComponent<Button>();

        chousenItemForBuyToggle = transform.GetChild(8).GetComponent<Toggle>();

        isThereItemInShopText = transform.GetChild(9).GetComponent<Text>();
    }

    private void LoadInfo()
    {
        if (item.imagesOfItem != null)
        {
            if (item.imagesOfItem.Length > 0)
            {
                firstImage.color = Color.white;
                firstImage.sprite = item.imagesOfItem[0];
            }
        }

        firstCostOfItemText.text = item.firstCostItem.ToString();
        secondCostItemText.text = item.secondCostItem.ToString();
        firmsNameText.text = item.manufacturingFirm.ToString();
        secondTypeOfItemText.text = item.secondTypeItem.ToString();

        if (FindThisItemInBusket() == -1)
        {
            busketButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            busketButton.GetComponent<Image>().color = Color.red;
        }

        if (FindThisItemInMark() == -1)
        {
            markButton.GetComponent<Image>().color = Color.grey;
        }
        else
        {
            markButton.GetComponent<Image>().color = Color.red;
        }

        if (item.isThereItemOnStore == false)
        {
            isThereItemInShopText.gameObject.SetActive(true);
            busketButton.interactable = false;
        }
        else
        {
            isThereItemInShopText.gameObject.SetActive(false);
            busketButton.interactable = true;
        }

        if (gameManager.newWindow != "busketsItems")
        {
            chousenItemForBuyToggle.gameObject.SetActive(false);
        }
        else
        {
            if (item.isThereItemOnStore == true)
            {
                chousenItemForBuyToggle.gameObject.SetActive(true);
                busketButton.interactable = true;
            }
            else
            {
                chousenItemForBuyToggle.gameObject.SetActive(false);
                busketButton.interactable = false;
            }
        }
    }

    public void OpenItemWindow()
    {
        gameManager.chousenItem = item.name.ToString();
        gameManager.ChouseItemWindow();
    }

    public void OwnWorkWithBusket()
    {
        if (FindThisItemInBusket() == -1)
        {
            busketButton.GetComponent<Image>().color = Color.red;
            AddToBusketThisItem();
        }
        else
        {
            busketButton.GetComponent<Image>().color = Color.grey;
            DeleteItemFromBusket();
        }

        if (gameManager.newWindow == "busketsItems") gameManager.LoadAllPrefabFromItem("busketsItems");
    }

    public void OwnWorkWithMarks()
    {
        if (FindThisItemInMark() == -1)
        {
            AddToMarkThisItem();
        }
        else
        {
            DeleteItemFromMark();
        }

        if (gameManager.newWindow == "marksItems") gameManager.LoadAllPrefabFromItem("marksItems");
    }

    private void AddToBusketThisItem()
    {
        busketButton.GetComponent<Image>().color = Color.red;
        loadPersonInfoFromFilesScript.personBusket.Add(item.vendorCode);
        loadPersonInfoFromFilesScript.SaveAllDataToFile();
    }

    private void AddToMarkThisItem()
    {
        markButton.GetComponent<Image>().color = Color.red;
        loadPersonInfoFromFilesScript.personMarks.Add(item.vendorCode);
        loadPersonInfoFromFilesScript.SaveAllDataToFile();
    }

    private void DeleteItemFromBusket()
    {
        busketButton.GetComponent<Image>().color = Color.grey;
        loadPersonInfoFromFilesScript.personBusket.RemoveAt(FindThisItemInBusket());
        loadPersonInfoFromFilesScript.SaveAllDataToFile();
    }

    private void DeleteItemFromMark()
    {
        markButton.GetComponent<Image>().color = Color.grey;
        loadPersonInfoFromFilesScript.personMarks.RemoveAt(FindThisItemInMark());
        loadPersonInfoFromFilesScript.SaveAllDataToFile();
    }

    private int FindThisItemInBusket()
    {
        for (int i = 0; i < loadPersonInfoFromFilesScript.personBusket.Count; i++)
        {
            if(item.vendorCode == loadPersonInfoFromFilesScript.personBusket[i])
            {
                return i;
            }
        }
        return -1;
    }

    private int FindThisItemInMark()
    {
        for (int i = 0; i < loadPersonInfoFromFilesScript.personMarks.Count; i++)
        {
            if (item.vendorCode == loadPersonInfoFromFilesScript.personMarks[i])
            {
                return i;
            }
        }
        return -1;
    }
}
