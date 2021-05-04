using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;

public class LoadScript : MonoBehaviour
{
    public Image progressCircle;

    private string bundleURL = "https://drive.google.com/uc?export=download&id=1xkNqCPtBRhUDblqQF3C5uciJvv5Iff6P";
    public int version = 0;
    public int countItemOnServer = 0;
    public List<ItemScript> items;
    public TypeOfItemScript typeOfItems;

    public string chousenVenderCode;

    public bool isNetworkAllow = false;
    public bool isItemsLoaded = false;

    private void Start()
    {
        progressCircle = GameObject.Find("FirstWindow").transform.GetChild(3).GetChild(0).GetComponent<Image>();
    }

    public IEnumerator checkInternetConnection()
    {
        WWW www = new WWW("http://google.com");
        yield return www;
        if (www.error != null)
        {
            isNetworkAllow = false;
        }
        else
        {
            Debug.Log("Connected");
            isNetworkAllow = true;
        }
    }

    public void LoadFilesFromServer()
    {
        StartCoroutine(checkInternetConnection());

        typeOfItems = new TypeOfItemScript();
        typeOfItems.typesOfItem = new List<string>();
        typeOfItems.nameOfCategoryForMainWindow = new List<string>();
        typeOfItems.secondNameOfCategoryForMainWindow = new List<string>();

        StartCoroutine(LoadInfoFromServer());
        Caching.ClearCache();
        StartCoroutine(DownloadAndCache());
    }

    IEnumerator LoadInfoFromServer()
    {
        ReadEntries();
        yield return new WaitUntil(() => isItemsLoaded == true);
        ReadEntriesOfCategory();
        ReadEntriesOfCategoryForMainWindow();
    }

    public void OnApplicationExit()
    {
        //SaveInfoOnServer();
    }

    void SaveInfoOnServer()
    {
        DeleteEntries();
        InsertEntries();

        DeleteEntriesFromCategoriesMainWIndow();
        InsertEntriesToCategoriesMainWIndow();

        DeleteEntriesFromCategories();
        InsertEntriesToCategories();
    }

    IEnumerator DownloadAndCache()
    {
        while (!Caching.ready)
        {
            yield return null;
        }

        var www = WWW.LoadFromCacheOrDownload(bundleURL, version);

        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log(www.error);
            yield break;
        }
        Debug.Log("IsLoaded");
        var assetBundle = www.assetBundle;

        var itemRequest = assetBundle.LoadAllAssetsAsync(typeof(TypeOfItemScript));
        while (!itemRequest.isDone)
        {
            float progressFloat = itemRequest.progress / 0.9f;
            progressCircle.fillAmount = progressFloat;
            yield return itemRequest;
        }

        Debug.Log("TypeOfItemLoaded");

        //typeOfItems = itemRequest.asset as TypeOfItemScript;

        itemRequest = assetBundle.LoadAllAssetsAsync(typeof(ItemScript));

        progressCircle.fillAmount = 0;

        while (!itemRequest.isDone)
        {
            float progressFloat = itemRequest.progress / 0.9f;
            progressCircle.fillAmount = progressFloat;

            yield return itemRequest;
        }

        Debug.Log("ItemLoaded");

        Debug.Log(itemRequest.allAssets.Length);
        //for (int i = 0; i < itemRequest.allAssets.Length; i++)
        //{
        //    items.Add(itemRequest.allAssets[i] as ItemScript);
        //}

        //SaveInfoOnServer();
    }

    bool saving = false;
    bool loading = false;

    // MySQL настройки 
    string constr = "Server=remotemysql.com;Database=odrqAeocA5;User ID=odrqAeocA5;Password=pphLr2KUcV;Pooling=true;CharSet=utf8;";
    // соединение 
    MySqlConnection con = null;
    // команда к БД
    MySqlCommand cmd = null;
    // чтение
    MySqlDataReader rdr = null;
    // ошибки
    MySqlError er = null;

    void Awake()
    {
        try
        {
            // установка элемента соединения 
            con = new MySqlConnection(constr);

            // посмотрим, сможем ли мы установить соединение 
            con.Open();
            Debug.Log("Connection State: " + con.State);
        }
        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
    }

    void OnApplicationQuit()
    {
        Debug.Log("killing con");
        if (con != null)
        {
            // Конечно, правильнее использовать:
            // if (con.State != ConnectionState.Closed) 
            // но из-за проблем с версиями сборок приходится использовать костыли
            if (con.State.ToString() != "Closed")
                con.Close();
            con.Dispose();
        }
    }

    // Вставка новой записи в таблицу
    void InsertEntries()
    {
        string query = string.Empty;
        // Вылавливаем ошибки 
        try
        {
            query = "INSERT INTO ItemList (nameItem, vendorCode, firstTypeItem, secondTypeItem, firstCostItem, secondCostItem, sizeOfItem, briefInfoOfItem, compositionOfItem, manufacturingFirm, descriptionOfItem, imagesOfItem, isThereItemOnStore) VALUES (?nameItem, ?vendorCode, ?firstTypeItem, ?secondTypeItem, ?firstCostItem, ?secondCostItem, ?sizeOfItem, ?briefInfoOfItem, ?compositionOfItem, ?manufacturingFirm, ?descriptionOfItem, ?imagesOfItem, ?isThereItemOnStore)";
            //query = "INSERT INTO ItemList (nameItem, vendorCode, firstTypeItem, secondTypeItem, firstCostItem, secondCostItem, briefInfoOfItem, compositionOfItem, manufacturingFirm, descriptionOfItem) VALUES (?nameItem, ?vendorCode, ?firstTypeItem, ?secondTypeItem, ?firstCostItem, ?secondCostItem, ?briefInfoOfItem, ?compositionOfItem, ?manufacturingFirm, ?descriptionOfItem)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (ItemScript item in items)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?nameItem", MySqlDbType.VarChar);
                        oParam.Value = item.nameItem;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?vendorCode", MySqlDbType.VarChar);
                        oParam1.Value = item.vendorCode;
                        MySqlParameter oParam2 = cmd.Parameters.Add("?firstTypeItem", MySqlDbType.VarChar);
                        oParam2.Value = item.firstTypeItem;
                        MySqlParameter oParam3 = cmd.Parameters.Add("?secondTypeItem", MySqlDbType.VarChar);
                        oParam3.Value = item.secondTypeItem;
                        MySqlParameter oParam4 = cmd.Parameters.Add("?firstCostItem", MySqlDbType.Float);
                        oParam4.Value = item.firstCostItem;
                        MySqlParameter oParam5 = cmd.Parameters.Add("?secondCostItem", MySqlDbType.Float);
                        oParam5.Value = item.secondCostItem;

                        string size = string.Empty;
                        for (int i = 0; i < item.sizeOfItem.Length; i++)
                        {
                            if (i - 1 != item.sizeOfItem.Length)
                            {
                                size += item.sizeOfItem[i].ToString() + " ";
                            }
                            else
                            {
                                size += item.sizeOfItem[i].ToString();
                            }
                        }
                        MySqlParameter oParam6 = cmd.Parameters.Add("?sizeOfItem", MySqlDbType.VarChar);
                        oParam6.Value = size;

                        MySqlParameter oParam7 = cmd.Parameters.Add("?briefInfoOfItem", MySqlDbType.VarChar);
                        oParam7.Value = item.briefInfoOfItem;
                        MySqlParameter oParam8 = cmd.Parameters.Add("?compositionOfItem", MySqlDbType.VarChar);
                        oParam8.Value = item.compositionOfItem;
                        MySqlParameter oParam9 = cmd.Parameters.Add("?descriptionOfItem", MySqlDbType.VarChar);
                        oParam9.Value = item.descriptionOfItem;
                        MySqlParameter oParam10 = cmd.Parameters.Add("?manufacturingFirm", MySqlDbType.VarChar);
                        oParam10.Value = item.manufacturingFirm;
                        MySqlParameter oParam11 = cmd.Parameters.Add("?imagesOfItem", MySqlDbType.Byte);
                        oParam11.Value = item.isThereItemOnStore;//item.imagesOfItem;
                        MySqlParameter oParam12 = cmd.Parameters.Add("?isThereItemOnStore", MySqlDbType.Byte);
                        oParam12.Value = item.isThereItemOnStore;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
        finally { }
    }

    // Обновление существующих записей в таблице 
    void UpdateEntries()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            query = "UPDATE ItemList SET nameItem = ?nameItem, vendorCode = ?vendorCode, firstTypeItem = ?firstTypeItem, secondTypeItem = ?secondTypeItem, firstCostItem = ?firstCostItem, secondCostItem = ?secondCostItem, sizeOfItem = ?sizeOfItem, briefInfoOfItem = ?briefInfoOfItem, compositionOfItem = ?compositionOfItem, manufacturingFirm = ?manufacturingFirm, descriptionOfItem = ?descriptionOfItem, imagesOfItem = ?imagesOfItem, isThereItemOnStore = ?isThereItemOnStore WHERE vendorCode = ?chousenVenderCode";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (ItemScript item in items)
                {
                    if (item.vendorCode == chousenVenderCode) { }
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?nameItem", MySqlDbType.VarChar);
                        oParam.Value = item.nameItem;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?vendorCode", MySqlDbType.VarChar);
                        oParam1.Value = item.vendorCode;
                        MySqlParameter oParam2 = cmd.Parameters.Add("?firstTypeItem", MySqlDbType.VarChar);
                        oParam2.Value = item.firstTypeItem;
                        MySqlParameter oParam3 = cmd.Parameters.Add("?secondTypeItem", MySqlDbType.VarChar);
                        oParam3.Value = item.secondTypeItem;
                        MySqlParameter oParam4 = cmd.Parameters.Add("?firstCostItem", MySqlDbType.Float);
                        oParam4.Value = item.firstCostItem;
                        MySqlParameter oParam5 = cmd.Parameters.Add("?secondCostItem", MySqlDbType.Float);
                        oParam5.Value = item.secondCostItem;
                        MySqlParameter oParam6 = cmd.Parameters.Add("?sizeOfItem", MySqlDbType.Float);
                        oParam6.Value = item.sizeOfItem;
                        MySqlParameter oParam7 = cmd.Parameters.Add("?briefInfoOfItem", MySqlDbType.Float);
                        oParam7.Value = item.briefInfoOfItem;
                        MySqlParameter oParam8 = cmd.Parameters.Add("?compositionOfItem", MySqlDbType.Float);
                        oParam8.Value = item.compositionOfItem;
                        MySqlParameter oParam9 = cmd.Parameters.Add("?descriptionOfItem", MySqlDbType.Float);
                        oParam9.Value = item.descriptionOfItem;
                        MySqlParameter oParam10 = cmd.Parameters.Add("?manufacturingFirm", MySqlDbType.Float);
                        oParam10.Value = item.manufacturingFirm;
                        MySqlParameter oParam11 = cmd.Parameters.Add("?imagesOfItem", MySqlDbType.Float);
                        oParam11.Value = true;//item.imagesOfItem;
                        MySqlParameter oParam12 = cmd.Parameters.Add("?isThereItemOnStore", MySqlDbType.Float);
                        oParam12.Value = item.isThereItemOnStore;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Удаляем запись из таблицы 
    void DeleteEntries()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            // лучше всего если вы знаете ID записи, которую необходимо удалить
            //----------------------------------------------------------------------- 
            // query = "DELETE FROM demo_table WHERE iddemo_table=?UID"; 
            // MySqlParameter oParam = cmd.Parameters.Add("?UID", MySqlDbType.Int32); 
            // oParam.Value = 0; 
            //----------------------------------------------------------------------- 
            query = "DELETE FROM ItemList";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    //MySqlParameter oParam = cmd.Parameters.Add("?isThereItemOnStore", MySqlDbType.Float);
                    //oParam.Value = chousenVenderCode;

                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Чтение всех записей из таблицы 
    void ReadEntries()
    {
        if (ReturnCountItemOnServer() > 0)
        {
            items.Clear();

            string query = string.Empty;
            // Отлавливаем ошибки 
            try
            {
                query = "SELECT * FROM ItemList";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                ItemScript newItem = new ItemScript();

                                newItem.nameItem = rdr["nameItem"].ToString();
                                newItem.vendorCode = rdr["vendorCode"].ToString();
                                newItem.firstTypeItem = rdr["firstTypeItem"].ToString();
                                newItem.secondTypeItem = rdr["secondTypeItem"].ToString();
                                newItem.firstCostItem = float.Parse(rdr["firstCostItem"].ToString());
                                newItem.secondCostItem = float.Parse(rdr["secondCostItem"].ToString());

                                string size = rdr["sizeOfItem"].ToString();
                                //Debug.Log(size + " " + size.Length.ToString());
                                string[] sizeMass = size.Split(' ');
                                newItem.sizeOfItem = new int[sizeMass.Length - 1];
                                //Debug.Log(sizeMass.Length);
                                for (int i = 0; i < sizeMass.Length - 1; i++)
                                {
                                    int.TryParse(sizeMass[i], out newItem.sizeOfItem[i]);
                                }

                                newItem.briefInfoOfItem = rdr["briefInfoOfItem"].ToString();
                                newItem.compositionOfItem = rdr["compositionOfItem"].ToString();
                                newItem.manufacturingFirm = rdr["manufacturingFirm"].ToString();
                                newItem.descriptionOfItem = rdr["descriptionOfItem"].ToString();

                                //newItem.imagesOfItem = rdr["imagesOfItem"].ToString();
                                bool image = bool.Parse(rdr["imagesOfItem"].ToString());

                                newItem.isThereItemOnStore = bool.Parse(rdr["isThereItemOnStore"].ToString());

                                newItem.name = newItem.vendorCode;
                                items.Add(newItem);
                            }
                        }
                        rdr.Dispose();
                    }
                }
            }
            catch (IOException ex) { Debug.Log(ex.ToString()); }
            finally { }

            isItemsLoaded = true;
        }
    }

    int ReturnCountItemOnServer()
    {
        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT vendorCode FROM ItemList";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            countItemOnServer++;
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }

        return countItemOnServer;
    }

    // Вставка новой записи в таблицу
    void InsertEntriesToCategoriesMainWIndow()
    {
        string query = string.Empty;
        // Вылавливаем ошибки 
        try
        {
            query = "INSERT INTO CategoryForMainWindow (nameOfCategoryForMainWindow, secondNameOfCategoryForMainWindow) VALUES (?nameOfCategoryForMainWindow, ?secondNameOfCategoryForMainWindow)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                for (int i = 0; i < typeOfItems.nameOfCategoryForMainWindow.Count; i++)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?nameOfCategoryForMainWindow", MySqlDbType.VarChar);
                        oParam.Value = typeOfItems.nameOfCategoryForMainWindow[i];
                        MySqlParameter oParam1 = cmd.Parameters.Add("?secondNameOfCategoryForMainWindow", MySqlDbType.VarChar);
                        oParam1.Value = typeOfItems.secondNameOfCategoryForMainWindow[i];

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
        finally { }
    }

    // Обновление существующих записей в таблице 
    void UpdateEntriesOfCategoriesMainWIndow()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            query = "UPDATE CategoryForMainWindow SET nameOfCategoryForMainWindow = ?nameOfCategoryForMainWindow, secondNameOfCategoryForMainWindow = ?secondNameOfCategoryForMainWindow";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                for (int i = 0; i < typeOfItems.nameOfCategoryForMainWindow.Count; i++)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?nameOfCategoryForMainWindow", MySqlDbType.VarChar);
                        oParam.Value = typeOfItems.nameOfCategoryForMainWindow[i];
                        MySqlParameter oParam1 = cmd.Parameters.Add("?secondNameOfCategoryForMainWindow", MySqlDbType.VarChar);
                        oParam1.Value = typeOfItems.secondNameOfCategoryForMainWindow[i];

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Удаляем запись из таблицы 
    void DeleteEntriesFromCategoriesMainWIndow()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            // лучше всего если вы знаете ID записи, которую необходимо удалить
            //----------------------------------------------------------------------- 
            // query = "DELETE FROM demo_table WHERE iddemo_table=?UID"; 
            // MySqlParameter oParam = cmd.Parameters.Add("?UID", MySqlDbType.Int32); 
            // oParam.Value = 0; 
            //----------------------------------------------------------------------- 
            query = "DELETE FROM CategoryForMainWindow";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Чтение всех записей из таблицы 
    void ReadEntriesOfCategoryForMainWindow()
    {
        typeOfItems.nameOfCategoryForMainWindow.Clear();
        typeOfItems.secondNameOfCategoryForMainWindow.Clear();

        if (ReturnCountCategoriesOfMainWindow() > 0)
        {
            string query = string.Empty;
            // Отлавливаем ошибки 
            try
            {
                query = "SELECT * FROM CategoryForMainWindow";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                typeOfItems.nameOfCategoryForMainWindow.Add(rdr["nameOfCategoryForMainWindow"].ToString());
                                typeOfItems.secondNameOfCategoryForMainWindow.Add(rdr["secondNameOfCategoryForMainWindow"].ToString());
                            }
                        }
                        rdr.Dispose();
                    }
                }
            }
            catch (IOException ex) { Debug.Log(ex.ToString()); }
            finally { }
        }
    }

    int ReturnCountCategoriesOfMainWindow()
    {
        int countCategoriesOfMainwindowOnServer = 0;
        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT nameOfCategoryForMainWindow FROM CategoryForMainWindow";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            countCategoriesOfMainwindowOnServer++;
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }

        return countCategoriesOfMainwindowOnServer;
    }

    // Вставка новой записи в таблицу
    void InsertEntriesToCategories()
    {
        string query = string.Empty;
        // Вылавливаем ошибки 
        try
        {
            query = "INSERT INTO TypeOfItemData (typesOfItem) VALUES (?typesOfItem)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                for (int i = 0; i < typeOfItems.typesOfItem.Count; i++)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?typesOfItem", MySqlDbType.VarChar);
                        oParam.Value = typeOfItems.typesOfItem[i];

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex)
        {
            Debug.Log(ex.ToString());
        }
        finally { }
    }

    // Обновление существующих записей в таблице 
    void UpdateEntriesOfCategories()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            query = "UPDATE TypeOfItemData SET typesOfItem = ?typesOfItem WHERE ID = ?index";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                for (int i = 0; i < typeOfItems.typesOfItem.Count; i++)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?typesOfItem", MySqlDbType.VarChar);
                        oParam.Value = typeOfItems.typesOfItem[i];
                        MySqlParameter oParam1 = cmd.Parameters.Add("?index", MySqlDbType.Int32);
                        oParam1.Value = i;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Удаляем запись из таблицы 
    void DeleteEntriesFromCategories()
    {
        string query = string.Empty;
        // Вылавливаем ошибки
        try
        {
            // лучше всего если вы знаете ID записи, которую необходимо удалить
            //----------------------------------------------------------------------- 
            // query = "DELETE FROM demo_table WHERE iddemo_table=?UID"; 
            // MySqlParameter oParam = cmd.Parameters.Add("?UID", MySqlDbType.Int32); 
            // oParam.Value = 0; 
            //----------------------------------------------------------------------- 
            query = "DELETE FROM TypeOfItemData";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Чтение всех записей из таблицы 
    void ReadEntriesOfCategory()
    {
        if (ReturnCountCategories() > 0)
        {
            typeOfItems.typesOfItem.Clear();

            string query = string.Empty;
            // Отлавливаем ошибки 
            try
            {
                query = "SELECT * FROM TypeOfItemData";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        rdr = cmd.ExecuteReader();
                        if (rdr.HasRows)
                        {
                            while (rdr.Read())
                            {
                                typeOfItems.typesOfItem.Add(rdr["typesOfItem"].ToString());
                            }
                        }
                        rdr.Dispose();
                    }
                }
            }
            catch (IOException ex) { Debug.Log(ex.ToString()); }
            finally { }
        }
    }

    int ReturnCountCategories()
    {
        int countCategoriesOnServer = 0;
        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT typesOfItem FROM TypeOfItemData";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            countCategoriesOnServer++;
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }

        return countCategoriesOnServer;
    }
}
