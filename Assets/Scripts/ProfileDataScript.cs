using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using SimpleJSON;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Net.Mime;
using MySql.Data;
using MySql.Data.MySqlClient;

public class ProfileDataScript : MonoBehaviour
{
    public GameManager gameManager;

    [SerializeField]
    List<string> allProfileMail;

    public int UID;
    public string profileName;
    public string profileAddress;
    public string phoneNumber;
    public string profileMail;
    public string savedProfileMail;

    string lastProfileName;
    string lastProfileAddress;
    string lastPhoneNumber;
    string lastProfileMail;

    public InputField profileNameInputField, profileAddressInputField, profilePhoneNumberInputField, profileMailInputField;
    public Toggle cashlessPaymentToggle, cashPaymentToggle;
    public Button saveButton;
    public GameObject mailConfirmation;

    public int codeToConfirm = -1;
    public bool isProfileMailConfirmed = false;

    private bool startConfirmFunc = false;

    private void OnEnable()
    {
        InstallConnection();

        InitializationAllObjects();
        LoadAllDataFromFile();

        ReturnAllProfileMail();

        ReadEntries();
        LoadAllDataToObjects();
    }

    void Start()
    {
        SaveStartProfileInfo();
        if (mailConfirmation != null)
        {
            if (isProfileMailConfirmed == false)
            {
                FirstStateOfMailConf();
            }
            else
            {
                ThirdStateOfMailConf();
            }
        }
    }

    private void Update()
    {
        TakeNewProfileInfo();

        CheckingChanges();

        if (mailConfirmation != null)
        {
            if (startConfirmFunc == true)
            {
                WaitingTrueCode();
            }

            if (startConfirmFunc != true)
            {
                if (isProfileMailConfirmed != true)
                {
                    FirstStateOfMailConf();
                }
            }
        }
    }

    private void TakeNewProfileInfo()
    {
        profileName = profileNameInputField.text;
        profileAddress = profileAddressInputField.text;
        phoneNumber = profilePhoneNumberInputField.text;
        profileMail = profileMailInputField.text;
    }

    private void InitializationAllObjects()
    {

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (transform.name != "ProfileInfoPrefab(Clone)")
        {
            //profileNameInputField = GameObject.Find("ProfileNameInputField").GetComponent<InputField>();
            //profileAddressInputField = GameObject.Find("ProfileAddressInputField").GetComponent<InputField>();
            //profilePhoneNumberInputField = GameObject.Find("ProfilePhoneNumberInputField").GetComponent<InputField>();
            //profileMailInputField = GameObject.Find("ProfileMailInputField").GetComponent<InputField>();
            //mailConfirmation = GameObject.Find("MailConfirmation");
            //saveButton = GameObject.Find("SaveButton").GetComponent<Button>();
        }
        else
        {
            profileMailInputField.interactable = false;
        }
    }

    private void SaveStartProfileInfo()
    {
        lastProfileName = profileName;
        lastProfileAddress = profileAddress;
        lastPhoneNumber = phoneNumber;
        lastProfileMail = profileMail;
    }

    public string fileForProfileSave;

    private void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForProfileData.json"))
        {
            CreateFilesForSave("/fileForProfileData.json");
        }
        fileForProfileSave = Application.persistentDataPath + "/fileForProfileData.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        JSONObject personDATA = new JSONObject();

        personDATA.Add("profileName", profileName);
        personDATA.Add("profileAddress", profileAddress);
        personDATA.Add("phoneNumber", phoneNumber);
        personDATA.Add("profileMail", profileMail);
        personDATA.Add("isProfileMailConfirmed", isProfileMailConfirmed);

        if (File.Exists(fileForProfileSave))
        {
            File.WriteAllText(fileForProfileSave, personDATA.ToString());
        }
    }

    public void LoadAllDataFromFile()
    {
        LoadFiles();

        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForProfileSave));

            if (personDATA != null)
            {
                //profileName = personDATA["profileName"];
                //profileAddress = personDATA["profileAddress"];
                //phoneNumber = personDATA["phoneNumber"];
                profileMail = personDATA["profileMail"];
                isProfileMailConfirmed = personDATA["isProfileMailConfirmed"];

                savedProfileMail = profileMail;
            }
            else
            {
                savedProfileMail = string.Empty;
            }
        }
    }

    private void LoadAllDataToObjects()
    {
        if (profileName != null)
        {
            profileNameInputField.text = profileName;
        }
        if (profileAddress != null)
        {
            profileAddressInputField.text = profileAddress;
        }
        if (phoneNumber != null)
        {
            profilePhoneNumberInputField.text = phoneNumber;
        }
        if (profileMail != null)
        {
            profileMailInputField.text = profileMail;
        }
    }

    public void SaveAllDataFromObjects()
    {
        profileName = profileNameInputField.text;
        profileAddress = profileAddressInputField.text;
        phoneNumber = profilePhoneNumberInputField.text;
        profileMail = profileMailInputField.text;

        SaveAllDataToFile();

        InsertEntries();
    }

    public void UpdateAllDataFromObjects()
    {
        profileName = profileNameInputField.text;
        profileAddress = profileAddressInputField.text;
        phoneNumber = profilePhoneNumberInputField.text;
        profileMail = profileMailInputField.text;

        SaveAllDataToFile();

        UpdateEntries();
    }

    private string FindTypeOfProfileMail(string mail)
    {
        if (mail != "")
        {
            string typeOfProfileMail = mail.Remove(0, (mail.IndexOf("@") + 1));
            return typeOfProfileMail;
        }
        return null;
    }

    public void SendMessageToProfileMail(string code)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.Body = CreateMailConfirmMessage(code);

        mailMessage.Subject = "Подтверждение почты. Магазин EGOIST";
        mailMessage.From = new MailAddress(gameManager.shopMail);
        mailMessage.To.Add(profileMail);
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

        SmtpClient client = new SmtpClient();
        client.Host = "smtp." + FindTypeOfProfileMail(gameManager.shopMail);
        client.Port = 587;
        client.Credentials = new NetworkCredential(mailMessage.From.Address, gameManager.shopMailPassword);
        client.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
         delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
         { return true; };

        client.Send(mailMessage);
    }

    public void SendMessageToCreatorMail(string message)
    {
        MailMessage mailMessage = new MailMessage();

        mailMessage.Body = message;

        mailMessage.Subject = "Новый заказ. Магазин EGOIST";
        mailMessage.From = new MailAddress(gameManager.shopSystemMail);
        mailMessage.To.Add(gameManager.shopMail);
        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;

        SmtpClient client = new SmtpClient();
        client.Host = "smtp." + FindTypeOfProfileMail(gameManager.shopMail);
        client.Port = 587;
        client.Credentials = new NetworkCredential(mailMessage.From.Address, gameManager.shopSystemPassword);
        client.EnableSsl = true;

        ServicePointManager.ServerCertificateValidationCallback =
         delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
         { return true; };

        client.Send(mailMessage);
    }

    public string CreateMailConfirmMessage(string code)
    {
        string message = "Код для подтверждения почты: " + code + "\r\n" + "C уважением, интернет-магазин EGOIST";
        return message;
    }

    public string CreateCheckMessage()
    {
        InsertEntriesToBusketOfOrders();
        InsertEntriesOfOrders();

        string newMessageString = "Новый заказ" + System.Environment.NewLine;

        newMessageString += "Имя клиента: " + profileName + System.Environment.NewLine;
        newMessageString += "Указанный город: " + profileAddress + System.Environment.NewLine;
        newMessageString += "Номер телефона клиента: " + phoneNumber + System.Environment.NewLine;
        newMessageString += "Почта клиента: " + profileMail + System.Environment.NewLine;

        string buffString = "";
        if (cashlessPaymentToggle.isOn == true)
        {
            buffString = "'Безналичный рассчет'";
        }
        if (cashPaymentToggle.isOn == true)
        {
            buffString = "'Наличный рассчет'";
        }

        newMessageString += "Оплата типа: " + buffString + System.Environment.NewLine + System.Environment.NewLine;

        for (int i = 0; i < gameManager.itemsToMail.Count; i++)
        {
            string nameItem = "Наименование товара: " + gameManager.itemsToMail[i].nameItem + System.Environment.NewLine;
            string vendorCode = "Код товара: " + gameManager.itemsToMail[i].vendorCode + System.Environment.NewLine;
            string firstTypeItem = "Категория: " + gameManager.itemsToMail[i].firstTypeItem + System.Environment.NewLine;
            string secondTypeItem = "Категория: " + gameManager.itemsToMail[i].secondTypeItem + System.Environment.NewLine;

            string firstCostItem = "Первая цена: " + gameManager.itemsToMail[i].firstCostItem + System.Environment.NewLine;
            string secondCostItem = "Вторая цена: " + gameManager.itemsToMail[i].secondCostItem + System.Environment.NewLine;
            string sizeOfItem = "Выбранные размер: " + gameManager.itemsToMail[i].sizeOfItem + System.Environment.NewLine;

            string briefInfoOfItem = "Краткая информация: " + gameManager.itemsToMail[i].briefInfoOfItem + System.Environment.NewLine;
            string compositionOfItem = "Состав товара: " + gameManager.itemsToMail[i].compositionOfItem + System.Environment.NewLine;
            string manufacturingFirm = "Фирма: " + gameManager.itemsToMail[i].manufacturingFirm + System.Environment.NewLine;

            int countOfItemInt = 0, buffInt = 0;

            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out countOfItemInt);
            int.TryParse(gameManager.itemsToMail[i].secondCostItem, out buffInt);
            countOfItemInt = countOfItemInt / buffInt;

            string countOfItem = "Количество единиц товара: " + countOfItemInt + System.Environment.NewLine;

            string allCostOfItem = "ОБЩАЯ СТОИМОСТЬ ДАННОГО ТОВАРА: " + gameManager.itemsToMail[i].allCostOfItem + System.Environment.NewLine;

            newMessageString = newMessageString + nameItem + vendorCode + firstTypeItem + secondTypeItem + firstCostItem + secondCostItem + sizeOfItem + briefInfoOfItem + compositionOfItem + manufacturingFirm + countOfItem + allCostOfItem + System.Environment.NewLine;
        }

        int buff = 0;
        int allCostInt = 0;

        for (int i = 0; i < gameManager.itemsToMail.Count; i++)
        {
            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out buff);
            allCostInt += buff;
        }
        string allCost = "ОБЩАЯ СТОИМОСТЬ ЗАКАЗА: " + allCostInt.ToString() + System.Environment.NewLine;

        newMessageString += allCost;

        newMessageString += System.Environment.NewLine + "C уважением, интернет-магазин EGOIST";

        return newMessageString;
    }

    void InsertEntriesToBusketOfOrders()
    {
        if (IsThisMailSavedOnServer(profileMail))
        {
            string query = string.Empty;
            // Вылавливаем ошибки 
            try
            {
                query = "INSERT INTO BusketOfOrders (profileName, profileAddress, profileNumber, profileMail, paymantType, nameItem, vendorCode, firstTypeItem, secondTypeItem, firstCostItem, secondCostItem, sizeOfItem, briefInfoOfItem, compositionOfItem, manufacturingFirm, countOfItem, allCostOfItem) VALUES (?profileName, ?profileAddress, ?profileNumber, ?profileMail, ?paymantType, ?nameItem, ?vendorCode, ?firstTypeItem, ?secondTypeItem, ?firstCostItem, ?secondCostItem, ?sizeOfItem, ?briefInfoOfItem, ?compositionOfItem, ?manufacturingFirm, ?countOfItem, ?allCostOfItem)";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    for (int i = 0; i < gameManager.itemsToMail.Count; i++)
                    {
                        using (cmd = new MySqlCommand(query, con))
                        {
                            MySqlParameter oParam = cmd.Parameters.Add("?profileName", MySqlDbType.VarChar);
                            oParam.Value = profileName;
                            MySqlParameter oParam1 = cmd.Parameters.Add("?profileAddress", MySqlDbType.VarChar);
                            oParam1.Value = profileAddress;
                            MySqlParameter oParam2 = cmd.Parameters.Add("?profileNumber", MySqlDbType.VarChar);
                            oParam2.Value = phoneNumber;
                            MySqlParameter oParam3 = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                            oParam3.Value = profileMail;

                            string buffString = "";
                            if (cashlessPaymentToggle.isOn == true)
                            {
                                buffString = "'Безналичный рассчет'";
                            }
                            if (cashPaymentToggle.isOn == true)
                            {
                                buffString = "'Наличный рассчет'";
                            }

                            MySqlParameter oParam4 = cmd.Parameters.Add("?paymantType", MySqlDbType.VarChar);
                            oParam4.Value = buffString;
                            MySqlParameter oParam5 = cmd.Parameters.Add("?nameItem", MySqlDbType.VarChar);
                            oParam5.Value = gameManager.itemsToMail[i].nameItem;
                            MySqlParameter oParam6 = cmd.Parameters.Add("?vendorCode", MySqlDbType.VarChar);
                            oParam6.Value = gameManager.itemsToMail[i].vendorCode;
                            MySqlParameter oParam7 = cmd.Parameters.Add("?firstTypeItem", MySqlDbType.VarChar);
                            oParam7.Value = gameManager.itemsToMail[i].firstTypeItem;
                            MySqlParameter oParam8 = cmd.Parameters.Add("?secondTypeItem", MySqlDbType.VarChar);
                            oParam8.Value = gameManager.itemsToMail[i].secondTypeItem;
                            MySqlParameter oParam9 = cmd.Parameters.Add("?firstCostItem", MySqlDbType.Float);
                            oParam9.Value = gameManager.itemsToMail[i].firstCostItem;
                            MySqlParameter oParam10 = cmd.Parameters.Add("?secondCostItem", MySqlDbType.Float);
                            oParam10.Value = gameManager.itemsToMail[i].secondCostItem;

                            MySqlParameter oParam11 = cmd.Parameters.Add("?sizeOfItem", MySqlDbType.VarChar);
                            oParam11.Value = gameManager.itemsToMail[i].sizeOfItem.ToString();

                            int countOfItemInt = 0, buffInt = 0;

                            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out countOfItemInt);
                            int.TryParse(gameManager.itemsToMail[i].secondCostItem, out buffInt);
                            countOfItemInt = countOfItemInt / buffInt;

                            MySqlParameter oParam12 = cmd.Parameters.Add("?briefInfoOfItem", MySqlDbType.VarChar);
                            oParam12.Value = gameManager.itemsToMail[i].briefInfoOfItem;
                            MySqlParameter oParam13 = cmd.Parameters.Add("?compositionOfItem", MySqlDbType.VarChar);
                            oParam13.Value = gameManager.itemsToMail[i].compositionOfItem;
                            MySqlParameter oParam14 = cmd.Parameters.Add("?manufacturingFirm", MySqlDbType.VarChar);
                            oParam14.Value = gameManager.itemsToMail[i].manufacturingFirm;
                            MySqlParameter oParam15 = cmd.Parameters.Add("?countOfItem", MySqlDbType.Int64);
                            oParam15.Value = countOfItemInt;
                            MySqlParameter oParam16 = cmd.Parameters.Add("?allCostOfItem", MySqlDbType.Int64);
                            oParam16.Value = gameManager.itemsToMail[i].allCostOfItem;

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
    }

    void InsertEntriesOfOrders()
    {
        if (IsThisMailSavedOnServer(profileMail))
        {
            int orderNumber = Random.Range(1000, 9999);

            string query = string.Empty;
            // Вылавливаем ошибки 
            try
            {
                query = "INSERT INTO BusketOfOrdersSecond (orderNumber, profileMail, paymantType, timeOfOrder, allCostOfOrder) VALUES (?orderNumber, ?profileMail, ?paymantType, ?timeOfOrder, ?allCostOfOrder)";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam4 = cmd.Parameters.Add("?orderNumber", MySqlDbType.Int64);
                        oParam4.Value = orderNumber;

                        MySqlParameter oParam = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                        oParam.Value = profileMail;

                        string buffString = "";
                        if (cashlessPaymentToggle.isOn == true)
                        {
                            buffString = "'Безналичный рассчет'";
                        }
                        if (cashPaymentToggle.isOn == true)
                        {
                            buffString = "'Наличный рассчет'";
                        }

                        MySqlParameter oParam1 = cmd.Parameters.Add("?paymantType", MySqlDbType.VarChar);
                        oParam1.Value = buffString;

                        //Debug.Log(System.DateTime.Now.ToFileTimeUtc().ToString());
                        //Debug.Log(System.DateTime.UtcNow);
                        //Debug.Log(System.Diagnostics.Stopwatch.GetTimestamp());

                        MySqlParameter oParam2 = cmd.Parameters.Add("?timeOfOrder", MySqlDbType.DateTime);
                        oParam2.Value = System.DateTime.UtcNow;

                        int buff = 0;
                        int allCostInt = 0;

                        for (int i = 0; i < gameManager.itemsToMail.Count; i++)
                        {
                            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out buff);
                            allCostInt += buff;
                        }

                        MySqlParameter oParam3 = cmd.Parameters.Add("?allCostOfOrder", MySqlDbType.Int64);
                        oParam3.Value = allCostInt;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
            finally { }

            query = string.Empty;
            // Вылавливаем ошибки 
            try
            {
                query = "INSERT INTO InfoAboutOrders (orderNumber, vendorCode, sizeOfItem, countOfItem, allCostOfItem) VALUES (?orderNumber, ?vendorCode, ?sizeOfItem, ?countOfItem, ?allCostOfItem)";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    for (int i = 0; i < gameManager.itemsToMail.Count; i++)
                    {
                        Debug.Log("TrySend i = " + i.ToString());

                        using (cmd = new MySqlCommand(query, con))
                        {
                            MySqlParameter oParam5 = cmd.Parameters.Add("?orderNumber", MySqlDbType.Int64);
                            oParam5.Value = orderNumber;
                            MySqlParameter oParam6 = cmd.Parameters.Add("?vendorCode", MySqlDbType.VarChar);
                            oParam6.Value = gameManager.itemsToMail[i].vendorCode;

                            MySqlParameter oParam7 = cmd.Parameters.Add("?sizeOfItem", MySqlDbType.VarChar);
                            oParam7.Value = gameManager.itemsToMail[i].sizeOfItem.ToString();

                            int countOfItemInt = 0, buffInt = 0;

                            int.TryParse(gameManager.itemsToMail[i].allCostOfItem, out countOfItemInt);
                            int.TryParse(gameManager.itemsToMail[i].secondCostItem, out buffInt);
                            countOfItemInt = countOfItemInt / buffInt;

                            MySqlParameter oParam8 = cmd.Parameters.Add("?countOfItem", MySqlDbType.Int64);
                            oParam8.Value = countOfItemInt;
                            MySqlParameter oParam9 = cmd.Parameters.Add("?allCostOfItem", MySqlDbType.Int64);
                            oParam9.Value = gameManager.itemsToMail[i].allCostOfItem;

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
    }

    private void FirstStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(true);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void SecondStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(true);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(false);
    }

    private void ThirdStateOfMailConf()
    {
        mailConfirmation.transform.GetChild(0).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(1).gameObject.SetActive(false);
        mailConfirmation.transform.GetChild(2).gameObject.SetActive(true);
    }

    public void MailConfirmationFunc()
    {
        if (profileMail != "")
        {
            GenerateNewCode();

            SendMessageToProfileMail(codeToConfirm.ToString());

            InputField profileInputCode = mailConfirmation.transform.GetChild(1).GetComponent<InputField>();
            profileInputCode.text = "";

            SecondStateOfMailConf();

            startConfirmFunc = true;
        }
    }

    private void WaitingTrueCode()
    {
        InputField profileInputCode = mailConfirmation.transform.GetChild(1).GetComponent<InputField>();

        if (profileInputCode.text != codeToConfirm.ToString())
        {
            profileInputCode.transform.GetChild(1).GetComponent<Text>().color = Color.red;
        }
        if (profileInputCode.text == codeToConfirm.ToString())
        {
            startConfirmFunc = false;
            isProfileMailConfirmed = true;
            ThirdStateOfMailConf();

            savedProfileMail = profileMail;
            ReadEntries();
            LoadAllDataToObjects();

            SaveAllDataFromObjects();
        }
    }

    private void GenerateNewCode()
    {
        codeToConfirm = Random.Range(100000, 999999);
    }

    private void CheckingChanges()
    {
        if (lastProfileMail != profileMail)
        {
            lastProfileMail = profileMail;
            isProfileMailConfirmed = false;
        }
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

    void InstallConnection()
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

    void SaveProfileInfoFromServer()
    {
        saving = true;
        //// Для начала очистим таблицу 
        //DeleteEntries();
        //// теперь сохраним информацию о сцене
        InsertEntries();
        // можно также использовать обновление если известен ID уже сохранённого элемента
        saving = false;
    }

    void LoadProfileInfoFromServer()
    {
        loading = true;
        // считаем информацию из БД
        ReadEntries();
        //LogGameItems();
        loading = false;
    }

    // Вставка новой записи в таблицу
    void InsertEntries()
    {
        if (!IsThisMailSavedOnServer(profileMail))
        {
            string query = string.Empty;
            // Вылавливаем ошибки 
            try
            {
                query = "INSERT INTO ProfileData (profileName, profileAddress, profileNumber, profileMail) VALUES (?profileName, ?profileAddress, ?profileNumber, ?profileMail)";
                if (con.State.ToString() != "Open")
                    con.Open();
                using (con)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?profileName", MySqlDbType.VarChar);
                        oParam.Value = profileName;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?profileAddress", MySqlDbType.VarChar);
                        oParam1.Value = profileAddress;
                        MySqlParameter oParam2 = cmd.Parameters.Add("?profileNumber", MySqlDbType.VarChar);
                        oParam2.Value = phoneNumber;
                        MySqlParameter oParam3 = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                        oParam3.Value = profileMail;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.Log(ex.ToString());
            }
            finally { }
        }
    }

    // Обновление существующих записей в таблице 
    void UpdateEntries()
    {
        string query = string.Empty;
        // Вылавливаем ошибки

        //Debug.Log(savedProfileMail);

        try
        {
            query = "UPDATE ProfileData SET profileName=?profileName, profileAddress=?profileAddress, profileNumber=?profileNumber WHERE profileMail=?savedProfileMail";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    MySqlParameter oParam = cmd.Parameters.Add("?profileName", MySqlDbType.VarChar);
                    oParam.Value = profileName;
                    MySqlParameter oParam1 = cmd.Parameters.Add("?profileAddress", MySqlDbType.VarChar);
                    oParam1.Value = profileAddress;
                    MySqlParameter oParam2 = cmd.Parameters.Add("?profileNumber", MySqlDbType.VarChar);
                    oParam2.Value = phoneNumber;
                    MySqlParameter oParam3 = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                    oParam3.Value = profileMail;
                    MySqlParameter oParam4 = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                    oParam4.Value = savedProfileMail;

                    cmd.ExecuteNonQuery();
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
            query = "DELETE FROM ProfileData WHERE profileMail=?savedProfileMail";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                    oParam.Value = savedProfileMail;

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
        if (savedProfileMail != null)
        {
            if (IsThisMailSavedOnServer(profileMail))
            {
                string query = string.Empty;
                // Отлавливаем ошибки 
                try
                {
                    query = "SELECT * FROM ProfileData WHERE profileMail=?savedProfileMail";
                    if (con.State.ToString() != "Open")
                        con.Open();
                    using (con)
                    {
                        using (cmd = new MySqlCommand(query, con))
                        {
                            MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                            oParam.Value = savedProfileMail;

                            rdr = cmd.ExecuteReader();
                            if (rdr.HasRows)
                                while (rdr.Read())
                                {
                                    profileName = rdr["profileName"].ToString();
                                    profileAddress = rdr["profileAddress"].ToString();
                                    phoneNumber = rdr["profileNumber"].ToString();
                                    //profileMail = rdr["profileMail"].ToString();
                                }
                            rdr.Dispose();
                        }
                    }
                }
                catch (IOException ex) { Debug.Log(ex.ToString()); }
                finally { }
            }
        }
    }

    void ReturnAllProfileMail()
    {
        allProfileMail = new List<string>();

        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT profileMail FROM ProfileData";
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
                            allProfileMail.Add(rdr["profileMail"].ToString());
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    bool IsThisMailSavedOnServer(string ourMail)
    {
        foreach (string mail in allProfileMail)
        {
            if (ourMail == mail)
            {
                return true;
            }
        }
        return false;
    }
}
