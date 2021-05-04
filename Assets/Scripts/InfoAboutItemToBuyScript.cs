using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoAboutItemToBuyScript : MonoBehaviour
{
    public GameManager gameManager;
    public LoadScript loadScript;
    public ItemScript item;

    public Image itemImage;
    public Text nameOfItemForBuyText;
    public GameObject buttonsToSize;

    public GameObject prefabOfSizeButton;
    public string chousenSize = "";

    public Button plusButton;
    public Button minusButton;
    public Text countOfItemToBuy;
    public Text summIntText;

    void Start()
    {
        InitializationAllItems();
        LoadAllInfo();
    }

    private void InitializationAllItems()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();

        itemImage = transform.GetChild(1).GetComponent<Image>();
        nameOfItemForBuyText = transform.GetChild(2).GetComponent<Text>();
        buttonsToSize = transform.GetChild(3).gameObject;
        plusButton = transform.GetChild(5).GetComponent<Button>();
        minusButton = transform.GetChild(6).GetComponent<Button>();
        countOfItemToBuy = transform.GetChild(7).GetComponent<Text>();
        summIntText = transform.GetChild(9).GetComponent<Text>();
    }

    private void LoadAllInfo()
    {
        if (item.imagesOfItem != null)
        {
            if (item.imagesOfItem.Length > 1)
            {
                itemImage.color = Color.white;
                itemImage.sprite = item.imagesOfItem[0];
            }
        }

        nameOfItemForBuyText.text = item.nameItem;
        LoadAllSizesOfItem();
    }

    private void LoadAllSizesOfItem()
    {
        foreach (int size in item.sizeOfItem)
        {
            GameObject newButtonOfSize = Instantiate(prefabOfSizeButton, buttonsToSize.transform);
            newButtonOfSize.transform.GetChild(0).GetComponent<Text>().text = size.ToString();

            newButtonOfSize.GetComponent<SizeButtomScript>().sizeOfItem = size.ToString();
        }
    }

    public void Update()
    {
        int countOfItem = 1;
        int.TryParse(countOfItemToBuy.text, out countOfItem);

        summIntText.text = (item.secondCostItem * countOfItem).ToString();

        ChangeColorOfSizeButton();
    }

    public void PlusButtonPress()
    {
        int countOfItem = 1;
        int.TryParse(countOfItemToBuy.text, out countOfItem);

        if (countOfItem < 5)
        {
            countOfItemToBuy.text = (countOfItem + 1).ToString();
        }
    }

    public void MinusButtonPress()
    {
        int countOfItem = 1;
        int.TryParse(countOfItemToBuy.text, out countOfItem);

        if (countOfItem > 1)
        {
            countOfItemToBuy.text = (countOfItem - 1).ToString();
        }
    }

    private void ChangeColorOfSizeButton()
    {
        for(int i = 0; i < buttonsToSize.transform.childCount; i++)
        {
            if(chousenSize == buttonsToSize.transform.GetChild(i).GetComponent<SizeButtomScript>().sizeOfItem)
            {
                buttonsToSize.transform.GetChild(i).GetComponent<Image>().color = Color.red;
            }
            else
            {
                buttonsToSize.transform.GetChild(i).GetComponent<Image>().color = Color.grey;
            }
        }
    }
}
