using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;
using MySql.Data.MySqlClient;

public class GameManager : MonoBehaviour
{
    ProfileDataScript profileDataScript;

    public string shopNameString;

    public string shopMail, shopMailPassword, shopSystemMail, shopSystemPassword;

    public string chousenItem;
    public string lastWindow, newWindow;

    public string pathToFolder;

    public List<ItemForCheck> itemsToMail = new List<ItemForCheck>();

    public LoadScript loadScript;
    public MobileNotificationAndroidScript mobileNotificationAndroidScript;
    public LoadPersonInfoFromFilesScript loadPersonInfoFromFilesScript;

    public GameObject startWindow, mainWindow, catalogWindow, busketWindow, profileWindow, itemWindow, buyItemWindow, notificationWindow, persoAllInfoWindow;
    public GameObject lowerPanel, topPanel;
    public InputField searchPanel;

    public Text helpMessageText;
    public GameObject loadingWindow;

    public GameObject objectForTypes;
    public GameObject objectForItems;
    public GameObject objectForItemsToBuy;
    public GameObject objectForProfileInfo;
    public GameObject objectForNotification;

    private void StartSettings()
    {
        Application.targetFrameRate = 60;

        //PlayerSettings.Android.renderOutsideSafeArea = false;
    }

    void Start()
    {
        StartSettings();

        InitializationAllObjects();

        ChouseStartWindow();
        StartCoroutine(CloseStartWindow());

        loadScript.LoadFilesFromServer();

        loadPersonInfoFromFilesScript.LoadFiles();
        loadPersonInfoFromFilesScript.LoadAllDataFromFile();
    }

    private void InitializationAllObjects()
    {
        profileDataScript = GameObject.Find("GameManager").GetComponent<ProfileDataScript>();
        loadScript = GameObject.Find("GameManager").GetComponent<LoadScript>();
        mobileNotificationAndroidScript = GameObject.Find("GameManager").GetComponent<MobileNotificationAndroidScript>();
        loadPersonInfoFromFilesScript = GameObject.Find("GameManager").GetComponent<LoadPersonInfoFromFilesScript>();

        lowerPanel = GameObject.Find("LowerPanelWithButton");
        topPanel = GameObject.Find("TopPanelWithShopName");

        searchPanel = topPanel.transform.GetChild(1).gameObject.GetComponent<InputField>();

        helpMessageText = GameObject.Find("HelpMessageText").GetComponent<Text>();
        helpMessageText.gameObject.SetActive(false);

        loadingWindow = GameObject.Find("LoadingWindow");
        loadingWindow.gameObject.SetActive(false);
    }

    IEnumerator CloseStartWindow()
    {
        StartCoroutine(loadScript.checkInternetConnection());
        startWindow.transform.GetChild(2).GetComponent<Text>().text = "Welcome!";
        yield return new WaitForSeconds(3f);
        while (loadScript.isNetworkAllow != true)
        {
            startWindow.transform.GetChild(2).GetComponent<Text>().text = "No Connection to Internet";
            StartCoroutine(loadScript.checkInternetConnection());
            yield return new WaitForSeconds(1f);
        }
        startWindow.transform.GetChild(2).GetComponent<Text>().text = "Welcome!";
        startWindow.GetComponent<Animator>().SetTrigger("CloseStartWindow");
        yield return new WaitForSeconds(1f);
        ChouseMainWindow("mainWindow");
    }

    public void CreateNewNotififcation()
    {
        //mobileNotificationAndroidScript.CreateAndSentNotification("Lol", "Privet", 5);
        mobileNotificationAndroidScript.CreateAndSentNotificationSecondVer("Lol", "Privet", 5);
        loadPersonInfoFromFilesScript.SaveAllDataToFile();

        ChouseMainWindow("mainWindow");
    }

    public void CloseNotificationWindowAndDeleteAllNotification()
    {
        loadPersonInfoFromFilesScript.personNotification.Clear();
        loadPersonInfoFromFilesScript.SaveAllDataToFile();

        ChouseMainWindow("mainWindow");
    }

    public void SendHelpMessage(string helpMessage)
    {
        helpMessageText.gameObject.SetActive(true);
        helpMessageText.text = helpMessage;
        StartCoroutine(ShowHelpMessage());
    }

    IEnumerator ShowHelpMessage()
    {
        yield return new WaitForSeconds(3f);
        helpMessageText.gameObject.SetActive(false);
    }

    private void InputSettings()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnButtonFunc();
            }
        }
    }

    void Update()
    {
        SearchPanelFunc();

        InputSettings();

        StartAsyncLoadItems(sortingListOfItemForAsyncLoad);
    }

    public void ChouseStartWindow()
    {
        startWindow.SetActive(true);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(false);
        topPanel.SetActive(false);

        StateOfTopPanel(0);

        ReloadAllText();
    }

    public void ChouseMainWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(true);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(0);

        LastAndNewWindow(windowName);

        ReloadAllText();
        ReloadCountOfNotification();

        GameObject.Find("SecondWindow").transform.GetChild(1).GetComponent<LoadMainWindowScript>().DestroyAllElementsOfMainWindow();
    }

    public void ChouseCatalogWindow(string windowName)
    {
        startWindow.SetActive(true);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(true);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(1);

        LastAndNewWindow(windowName);

        ReloadAllText();
        LoadAllTypesOfItems(loadScript.typeOfItems.typesOfItem);
    }

    public void ChouseChousenItemsWindow(string metodName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(true);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(1);

        ReloadAllText();

        if (metodName != "busketsItems")
        {
            busketWindow.transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            busketWindow.transform.GetChild(2).gameObject.SetActive(true);
        }

        LastAndNewWindow(metodName);

        LoadAllPrefabFromItem(metodName);
    }

    public void ChouseProfileWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(true);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);

        LastAndNewWindow(windowName);

        ReloadAllText();
    }

    public void ChouseItemWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 0);

        ReloadAllText();
    }

    public void ChouseAboutUsWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 1);

        ReloadAllText();
    }

    public void ChouseToContactUsWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 2);

        ReloadAllText();
    }

    public void ChouseLoginWindow()
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(true);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 3);

        ReloadAllText();
    }

    public void ChouseBuyItemWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(true);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        LastAndNewWindow(windowName);

        ReloadAllText();
    }

    public void ChouseNotificationWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(true);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(true);
        persoAllInfoWindow.SetActive(false);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        LastAndNewWindow(windowName);

        ReloadAllText();

        LoadAllNotificationObjects();
    }

    public void ChousePersoAllInfoWindowWindow(string windowName)
    {
        startWindow.SetActive(false);
        mainWindow.SetActive(false);
        catalogWindow.SetActive(false);
        busketWindow.SetActive(false);
        profileWindow.SetActive(false);
        itemWindow.SetActive(false);
        buyItemWindow.SetActive(false);
        notificationWindow.SetActive(false);
        persoAllInfoWindow.SetActive(true);

        lowerPanel.SetActive(true);
        topPanel.SetActive(true);

        LastAndNewWindow(windowName);

        ReloadAllText();

        StateOfTopPanel(2);
        itemWindow.GetComponent<Animator>().SetInteger("numberOfState", 2);

        LoadAllNotificationObjects();
    }

    private void LastAndNewWindow(string windowName)
    {
        if (newWindow != windowName)
        {
            lastWindow = newWindow;
            newWindow = windowName;
        }
    }

    public void ReturnButtonFunc()
    {
        ClearSearchPanel();

        if (lastWindow == "mainWindow") ChouseMainWindow("mainWindow");
        else
        if (lastWindow == "profileWindow") ChouseProfileWindow("profileWindow");
        else
        if (lastWindow == "busketsItems") ChouseChousenItemsWindow("busketsItems");
        else
        if (lastWindow == "catalogItem") ChouseCatalogWindow("catalogItem");
        //else
        //if (lastWindow == "persoAllInfoWindow") ChouseCatalogWindow("persoAllInfoWindow");
    }

    public void ClearSearchPanel()
    {
        searchPanel.text = "";
    }

    private void StateOfTopPanel(int stateOfTopPanel)
    {
        if (stateOfTopPanel == 0)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", false);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", false);
        }
        else if (stateOfTopPanel == 1)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", true);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", false);
        }
        else if (stateOfTopPanel == 2)
        {
            topPanel.GetComponent<Animator>().SetBool("CloseNamePanel", true);
            topPanel.GetComponent<Animator>().SetBool("CloseSearchPanel", true);
        }
    }

    private void ReloadAllText()
    {
        ReloadShopName();
    }

    private void ReloadShopName()
    {
        if (GameObject.FindGameObjectWithTag("ShopNameTag"))
        {
            GameObject.FindGameObjectWithTag("ShopNameTag").GetComponent<Text>().text = shopNameString;
        }
    }

    private void ReloadCountOfNotification()
    {
        GameObject.Find("CountMessageText").GetComponent<Text>().text = loadPersonInfoFromFilesScript.personNotification.Count.ToString();
    }

    public void BuyItemFunc()
    {
        if (newWindow == "busketsItems")
        {
            GameObject content = GameObject.Find("Content");
            int countSelectedItems = 0;
            for (int i = 0; i < content.transform.childCount; i++)
            {
                if (content.transform.GetChild(i).GetChild(8).GetComponent<Toggle>().isOn)
                {
                    countSelectedItems++;
                }
            }

            if (countSelectedItems != 0)
            {

                DestroyAllItemsToBuy();
                for (int i = 0; i < content.transform.childCount; i++)
                {
                    if (content.transform.GetChild(i).GetChild(8).GetComponent<Toggle>().isOn)
                    {
                        GameObject newItemToBuy = Instantiate(objectForItemsToBuy, buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0));
                        newItemToBuy.GetComponent<InfoAboutItemToBuyScript>().item = content.transform.GetChild(i).GetComponent<InfoAboutItem>().item;
                    }
                }

                Instantiate(objectForProfileInfo, buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0));

                // Ограничение на заказы в день
                if (CheckCountRequestToBuy())
                {
                    ChouseBuyItemWindow("buyWindow");
                }
                else
                {
                    SendHelpMessage("Your made request today");
                }

                //ChouseBuyItemWindow("buyWindow");

            }
            else
            {
                SendHelpMessage("Select any items from list");
            }
        }

        if (newWindow == "buyWindow")
        {
            if (ReturnCountItemWithChouseSize() == buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1)
            {
                ProfileDataScript profileDataScript = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1).GetComponent<ProfileDataScript>();

                if (profileDataScript.isProfileMailConfirmed == true)
                {
                    if (profileDataScript.cashlessPaymentToggle.isOn == true || profileDataScript.cashPaymentToggle.isOn == true)
                    {
                        CreatePackageOfItems(itemsToMail);
                        profileDataScript.SendMessageToCreatorMail(profileDataScript.CreateCheckMessage());
                        SendHelpMessage("Your order is accepted. You will be contacted soon");

                        ChouseMainWindow("mainWindow");

                        loadPersonInfoFromFilesScript.personBusket.Clear(); // Сменить на удаление только купленных товаров

                        loadPersonInfoFromFilesScript.SaveAllDataToFile();
                    }
                    else
                    {
                        SendHelpMessage("Chouse payment method");
                    }
                }
                else
                {
                    SendHelpMessage("Your mail is not confirmed");
                    ChouseLoginWindow();
                }
            }
            else
            {
                SendHelpMessage("Chouse size to all items");
            }
        }
    }

    string constr = "Server=remotemysql.com;Database=odrqAeocA5;User ID=odrqAeocA5;Password=pphLr2KUcV;Pooling=true;CharSet=utf8;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;
    MySqlError er = null;
    private bool loading;
    private bool saving;

    void TryConnection()
    {
        try
        {
            con = new MySqlConnection(constr);
            con.Open();
            Debug.Log("Connection State: " + con.State);
            //Debug.Log(con.ServerVersion);
        }
        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    void OnApplicationQuit()
    {
        if (con != null)
        {
            if (con.State.ToString() != "Closed")
                con.Close();
            con.Dispose();
        }
    }

    private bool CheckCountRequestToBuy()
    {
        TryConnection();

        System.DateTime ourTime = System.DateTime.UtcNow;

        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT timeOfOrder FROM BusketOfOrdersSecond WHERE profileMail=?savedProfileMail AND timeOfOrder > date_sub(now(), interval 1 day)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                    oParam.Value = profileDataScript.savedProfileMail;

                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }

        return false;
    }

    public class ItemForCheck
    {
        public string nameItem;
        public string vendorCode;
        public string firstTypeItem;
        public string secondTypeItem;

        public string firstCostItem, secondCostItem;
        public string sizeOfItem;

        public string briefInfoOfItem;
        public string compositionOfItem;
        public string manufacturingFirm;

        public string allCostOfItem;

        public ItemForCheck(string nameItem, string vendorCode, string firstTypeItem, string secondTypeItem, string firstCostItem, string secondCostItem, string sizeOfItem, string briefInfoOfItem, string compositionOfItem, string manufacturingFirm, string allCostOfItem)
        {
            this.nameItem = nameItem;
            this.vendorCode = vendorCode;
            this.firstTypeItem = firstTypeItem;
            this.secondTypeItem = secondTypeItem;
            this.firstCostItem = firstCostItem;
            this.secondCostItem = secondCostItem;
            this.sizeOfItem = sizeOfItem;
            this.briefInfoOfItem = briefInfoOfItem;
            this.compositionOfItem = compositionOfItem;
            this.manufacturingFirm = manufacturingFirm;
            this.allCostOfItem = allCostOfItem;
        }
    }

    private void CreatePackageOfItems(List<ItemForCheck> itemsToMail)
    {
        ClearPackageOfItems(itemsToMail);

        for (int i = 0; i < buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).childCount - 1; i++)
        {
            GameObject itemObject = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0).GetChild(i).gameObject;
            ItemScript item = itemObject.GetComponent<InfoAboutItemToBuyScript>().item;
            itemsToMail.Add(new ItemForCheck(
                item.nameItem,
                item.vendorCode,
                item.firstTypeItem,
                item.secondTypeItem,
                item.firstCostItem.ToString(),
                item.secondCostItem.ToString(),
                itemObject.GetComponent<InfoAboutItemToBuyScript>().chousenSize,
                item.briefInfoOfItem, item.compositionOfItem,
                item.manufacturingFirm,
                itemObject.GetComponent<InfoAboutItemToBuyScript>().summIntText.text.ToString())
            );
        }
    }

    private void ClearPackageOfItems(List<ItemForCheck> itemsToMail)
    {
        if (itemsToMail.Count != 0)
        {
            itemsToMail.Clear();
        }
    }

    private int ReturnCountItemWithChouseSize()
    {
        int countItemsWithChousenSize = 0;

        Transform content = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).name != "ProfileInfoPrefab(Clone)")
            {
                if (content.GetChild(i).GetComponent<InfoAboutItemToBuyScript>().chousenSize != "")
                {
                    countItemsWithChousenSize++;
                }
            }
        }
        return countItemsWithChousenSize;
    }

    private void DestroyAllNotificationObjects(Transform content)
    {
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    private void LoadAllNotificationObjects()
    {
        Transform content = notificationWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        DestroyAllNotificationObjects(content);
        foreach (LoadPersonInfoFromFilesScript.notificationObject notObj in loadPersonInfoFromFilesScript.personNotification)
        {
            GameObject newNotificationObject = Instantiate(objectForNotification, content);
            newNotificationObject.GetComponent<InfoAboutNotificationScript>().message = notObj.message;
            newNotificationObject.GetComponent<InfoAboutNotificationScript>().date = notObj.date;
        }

    }

    private void DestroyAllItemsToBuy()
    {
        Transform content = buyItemWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
            Debug.Log(content.GetChild(i).name);
        }
    }

    public void LoadAllTypesOfItems(List<string> typeOfItems)
    {
        Transform content = catalogWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        DestroyAllTypesOfItems();
        for (int i = 0; i < typeOfItems.Count; i++)
        {
            GameObject newTypeOfItem = Instantiate(objectForTypes, content);
            newTypeOfItem.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = loadScript.typeOfItems.typesOfItem[i];

            newTypeOfItem.GetComponent<ChouseTypeOfItemScript>().nameOfType = loadScript.typeOfItems.typesOfItem[i];
        }
    }

    private void DestroyAllTypesOfItems()
    {
        Transform content = catalogWindow.transform.GetChild(1).GetChild(0).GetChild(0);

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void LoadAllPrefabFromItem(string parameterOfItem)
    {
        if (parameterOfItem == "allItems")
        {
            CreateAllPrefabFromItem(loadScript.items);
        }
        else if (parameterOfItem == "busketsItems")
        {
            FindItemsInBusket();
            CreateAllPrefabFromItem(loadPersonInfoFromFilesScript.sortingListOfItem);
        }
        else if (parameterOfItem == "marksItems")
        {
            FindItemsInMarks();
            CreateAllPrefabFromItem(loadPersonInfoFromFilesScript.sortingListOfItem);
        }
        else
        {
            FindItemByTypes(parameterOfItem);
            CreateAllPrefabFromItem(loadPersonInfoFromFilesScript.sortingListOfItem);
        }
    }

    private void FindItemsInBusket()
    {
        ClearSortingList();

        loadPersonInfoFromFilesScript.ReloadPersonBusket();

        foreach (ItemScript item in loadScript.items)
        {
            for (int i = 0; i < loadPersonInfoFromFilesScript.personBusket.Count; i++)
            {
                if (item.name == loadPersonInfoFromFilesScript.personBusket[i])
                {
                    loadPersonInfoFromFilesScript.sortingListOfItem.Add(item);
                    break;
                }
            }
        }
    }

    private void FindItemsInMarks()
    {
        ClearSortingList();
        foreach (ItemScript item in loadScript.items)
        {
            for (int i = 0; i < loadPersonInfoFromFilesScript.personMarks.Count; i++)
            {
                if (item.name == loadPersonInfoFromFilesScript.personMarks[i])
                {
                    loadPersonInfoFromFilesScript.sortingListOfItem.Add(item);
                    break;
                }
            }
        }
    }

    private void FindItemByTypes(string typeOfitem)  /// Переписать поиск не по слову а по символам 
    {
        ClearSortingList();
        foreach (ItemScript item in loadScript.items)
        {
            if (typeOfitem.Equals(item.firstTypeItem, System.StringComparison.OrdinalIgnoreCase))
            {
                loadPersonInfoFromFilesScript.sortingListOfItem.Add(item);
            }
        }

        foreach (ItemScript item in loadScript.items)
        {
            if (typeOfitem.Equals(item.secondTypeItem, System.StringComparison.OrdinalIgnoreCase))
            {
                loadPersonInfoFromFilesScript.sortingListOfItem.Add(item);
            }
        }
    }

    private void FindTypeByTypes(string typeOfitem)
    {
        ClearSortingList();
        foreach (string type in loadScript.typeOfItems.typesOfItem)
        {
            if (typeOfitem.Equals(type, System.StringComparison.OrdinalIgnoreCase))
            {
                loadPersonInfoFromFilesScript.sortingListOfType.Add(type);
            }
        }
    }

    string lastTextFromSeacrh = "";

    private void SearchPanelFunc()
    {
        while (searchPanel.text != lastTextFromSeacrh)
        {
            Debug.Log(searchPanel.text);
            ClearSortingList();
            SpecialSearchFunc(searchPanel.text);
            lastTextFromSeacrh = searchPanel.text;
        }
    }

    private void SpecialSearchFunc(string textFromSearch)
    {
        if (newWindow != "catalogItem")
        {
            FindItemByTypes(textFromSearch);
            CreateAllPrefabFromItem(loadPersonInfoFromFilesScript.sortingListOfItem);

            if (textFromSearch == "")
            {
                LoadAllPrefabFromItem(newWindow);
            }
        }
        else
        {
            FindTypeByTypes(textFromSearch);
            LoadAllTypesOfItems(loadPersonInfoFromFilesScript.sortingListOfType);

            if (textFromSearch == "")
            {
                LoadAllTypesOfItems(loadScript.typeOfItems.typesOfItem);
            }
        }
    }

    private void ClearSortingList()
    {
        loadPersonInfoFromFilesScript.sortingListOfType.Clear();
        loadPersonInfoFromFilesScript.sortingListOfItem.Clear();
    }

    private void CreateAllPrefabFromItem(List<ItemScript> sortingListOfItem)
    {
        DeleteAllPrefabFromItem();

        sortingListOfItemForAsyncLoad = sortingListOfItem;
        countLoadedItems = 0;
    }

    int countLoadedItems;
    [SerializeField]
    List<ItemScript> sortingListOfItemForAsyncLoad;

    private void StartAsyncLoadItems(List<ItemScript> sortingListOfItem)
    {
        if (sortingListOfItem != null)
        {
            if (sortingListOfItem.Count != countLoadedItems)
            {
                Transform content = GameObject.Find("Content").transform;

                while ((countLoadedItems % 10 != 0 || countLoadedItems == 0 || (410 * (countLoadedItems / 2 - 3) + 15 * (countLoadedItems / 2 - 3)) <= content.localPosition.y) && sortingListOfItem.Count != countLoadedItems)
                {

                    ItemScript item = sortingListOfItem[countLoadedItems];

                    GameObject newObjectForItems = GameObject.Instantiate(objectForItems, content);
                    newObjectForItems.GetComponent<InfoAboutItem>().item = item;

                    countLoadedItems++;
                }
            }
        }
    }

    private void DeleteAllPrefabFromItem()
    {
        if (GameObject.Find("Content") != null)
        {
            if (GameObject.Find("Content").transform.childCount != 0)
            {
                for (int i = 0; i < GameObject.Find("Content").transform.childCount; i++)
                {
                    Destroy(GameObject.Find("Content").transform.GetChild(i).gameObject);
                }
            }
        }
    }

}
