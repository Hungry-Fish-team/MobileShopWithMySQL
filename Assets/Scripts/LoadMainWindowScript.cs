using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainWindowScript : MonoBehaviour
{
    public GameManager gameManager;
    public LoadScript loadScript;

    public GameObject prefabOfElementOfMainWindow;

    [SerializeField]
    bool isElementsLoaded = false;

    void Start()
    {
        InitializationAllItems();
        LoadAllElementsOfMainWindow();
    }

    private void InitializationAllItems()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();
    }

    private void Update()
    {
        LoadAllElementsOfMainWindow();
    }

    public void LoadAllElementsOfMainWindow()
    {
        if (isElementsLoaded == false)
        {
            Transform ourContent = transform.GetChild(0).GetChild(0);

            DestroyAllElementsOfMainWindow();

            for (int i = 0; i < loadScript.typeOfItems.nameOfCategoryForMainWindow.Count; i++)
            {
                GameObject newPrefabOfItemForMainWindow = Instantiate(prefabOfElementOfMainWindow, ourContent);
                if (i < loadScript.typeOfItems.nameOfCategoryForMainWindow.Count)
                {
                    newPrefabOfItemForMainWindow.GetComponent<InfoAboutCategoryScript>().nameOfCategory = loadScript.typeOfItems.nameOfCategoryForMainWindow[i];
                }
                if (i < loadScript.typeOfItems.secondNameOfCategoryForMainWindow.Count)
                {
                    newPrefabOfItemForMainWindow.GetComponent<InfoAboutCategoryScript>().secondNameOfCategory = loadScript.typeOfItems.secondNameOfCategoryForMainWindow[i];
                }
                if (loadScript.typeOfItems.imageOfCategoryForMainWindow != null)
                {
                    if (i < loadScript.typeOfItems.imageOfCategoryForMainWindow.Count)
                    {
                        newPrefabOfItemForMainWindow.GetComponent<InfoAboutCategoryScript>().imageOfCategory = loadScript.typeOfItems.imageOfCategoryForMainWindow[i];
                    }
                }
            }

            isElementsLoaded = true;
        }
    }

    public void DestroyAllElementsOfMainWindow()
    {
        Transform ourContent = transform.GetChild(0).GetChild(0);

        for (int i = 0; i < ourContent.childCount; i++)
        {
            Destroy(ourContent.GetChild(i).gameObject);
        }

        isElementsLoaded = false;
    }
}
