using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoAboutCategoryScript : MonoBehaviour
{
    public GameManager gameManager;
    public LoadScript loadScript;

    public string nameOfCategory;
    public string secondNameOfCategory;
    public Sprite imageOfCategory;

    void Start()
    {
        InitializationAllItems();

        LoadAllTextOfPrefab();
    }

    private void InitializationAllItems()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();
    }

    private void LoadAllTextOfPrefab()
    {
        transform.GetChild(1).GetComponent<Image>().sprite = imageOfCategory;
        transform.GetChild(2).GetComponent<Text>().text = nameOfCategory;
        transform.GetChild(3).GetComponent<Text>().text = secondNameOfCategory;
    }

    public void LoadThisCategory()
    {
        gameManager.searchPanel.text = nameOfCategory;
        gameManager.ChouseCatalogWindow("catalogItem");
    }

}
