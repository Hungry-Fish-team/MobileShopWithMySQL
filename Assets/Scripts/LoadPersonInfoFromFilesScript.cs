using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEditor;
using MySql.Data;
using MySql.Data.MySqlClient;

public class LoadPersonInfoFromFilesScript : MonoBehaviour
{
    [SerializeField]
    ProfileDataScript profileDataScript;

    public List<ItemScript> sortingListOfItem;
    public List<string> sortingListOfType;
    public List<string> personBusket, personMarks;

    public class notificationObject
    {
        public string message;
        public string date;

        public notificationObject() { }

        public notificationObject(string newMessage, string newDate)
        {
            this.message = newMessage;
            this.date = newDate;
        }
    }

    public List<notificationObject> personNotification = new List<notificationObject>();

    public string fileForPersonBusket;
    public string fileForMarkSave;
    public string fileForNotificationSave;

    public void LoadFiles()
    {
        if (!File.Exists(Application.persistentDataPath + "/fileForPersonBusket.json"))
        {
            CreateFilesForSave("/fileForPersonBusket.json");
        }
        fileForPersonBusket = Application.persistentDataPath + "/fileForPersonBusket.json";
        if (!File.Exists(Application.persistentDataPath + "/fileForMarkSave.json"))
        {
            CreateFilesForSave("/fileForMarkSave.json");
        }
        fileForMarkSave = Application.persistentDataPath + "/fileForMarkSave.json";
        if (!File.Exists(Application.persistentDataPath + "/fileForNotificationSave.json"))
        {
            CreateFilesForSave("/fileForNotificationSave.json");
        }
        fileForNotificationSave = Application.persistentDataPath + "/fileForNotificationSave.json";
    }

    private void CreateFilesForSave(string nameOfFile)
    {
        FileStream newFile = File.Open(Application.persistentDataPath + nameOfFile, FileMode.OpenOrCreate);
        newFile.Close();
        Debug.Log("create" + nameOfFile);
    }

    public void SaveAllDataToFile()
    {
        //SavePersonBusket();
        SavePersonMarks();
        SavePersonNotification();

        SaveProfileInfoFromServer();
    }

    private void SavePersonBusket()
    {
        JSONObject personDATA = new JSONObject();
        JSONArray personBusketJSON = new JSONArray();
        if (personBusket.Count == 0)
        {
            File.Delete(fileForPersonBusket);
        }
        else
        {
            for (int i = 0; i < personBusket.Count; i++)
            {
                personBusketJSON.Add(personBusket[i]);
            }

            personDATA.Add("personBusket", personBusketJSON);

            if (File.Exists(fileForPersonBusket))
            {
                File.WriteAllText(fileForPersonBusket, personDATA.ToString());
            }
        }
    }

    private void SavePersonMarks()
    {
        JSONObject personDATA = new JSONObject();
        JSONArray personMarksJSON = new JSONArray();
        if (personMarks.Count == 0)
        {
            File.Delete(fileForMarkSave);
        }
        else
        {
            for (int i = 0; i < personMarks.Count; i++)
            {
                personMarksJSON.Add(personMarks[i]);
            }

            personDATA.Add("personMarks", personMarksJSON);

            if (File.Exists(fileForMarkSave))
            {
                File.WriteAllText(fileForMarkSave, personDATA.ToString());
            }
        }
    }

    private void SavePersonNotification()
    {
        JSONObject personDATA = new JSONObject();

        if (personNotification.Count == 0)
        {
            File.Delete(fileForNotificationSave);
        }
        else
        {
            for (int i = 0; i < personNotification.Count; i++)
            {
                JSONArray personNotificatioJSON = new JSONArray();

                personNotificatioJSON.Add("Message", personNotification[i].message);
                personNotificatioJSON.Add("Date", personNotification[i].date);

                personDATA.Add("personNotification" + i.ToString(), personNotificatioJSON);
            }

            if (File.Exists(fileForNotificationSave))
            {
                File.WriteAllText(fileForNotificationSave, personDATA.ToString());
            }
        }
    }

    public void LoadAllDataFromFile()
    {
        //LoadPersonBusket();
        LoadPersonMarks();
        LoadPersonNotification();

        ReadEntries();
    }

    private void LoadPersonBusket()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForPersonBusket)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForPersonBusket));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA["personBusket"].Count; i++)
                {
                    personBusket.Add(personDATA["personBusket"].AsArray[i]);
                }
            }
        }
    }

    private void LoadPersonMarks()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForMarkSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForMarkSave));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA["personMarks"].Count; i++)
                {
                    personMarks.Add(personDATA["personMarks"].AsArray[i]);
                }
            }
        }
    }

    private void LoadPersonNotification()
    {
        if ((JSONObject)JSON.Parse(File.ReadAllText(fileForNotificationSave)) != null)
        {
            JSONObject personDATA = (JSONObject)JSON.Parse(File.ReadAllText(fileForNotificationSave));

            if (personDATA != null)
            {
                for (int i = 0; i < personDATA.Count; i++)
                {
                    personNotification.Add(new notificationObject(personDATA["personNotification" + i.ToString()].AsArray[0], personDATA["personNotification" + i.ToString()].AsArray[1]));
                }
            }
        }
    }

    public void ReloadPersonBusket()
    {
        personBusket.Clear();

        ReadEntries();
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

    void OnEnable()
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
        DeleteEntries();
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
        string query = string.Empty;
        // Вылавливаем ошибки 
        try
        {
            query = "INSERT INTO ProfileBusket (profileMail, itemName) VALUES (?profileMail, ?itemName)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (string itemName in personBusket)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                        oParam.Value = profileDataScript.profileMail;
                        MySqlParameter oParam1 = cmd.Parameters.Add("?itemName", MySqlDbType.VarChar);
                        oParam1.Value = itemName;

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
            query = "DELETE FROM ProfileBusket WHERE profileMail=?savedProfileMail";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                foreach (string itemName in personBusket)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                        oParam.Value = profileDataScript.savedProfileMail;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

    // Чтение всех записей из таблицы 
    void ReadEntries()
    {
        string query = string.Empty;
        // Отлавливаем ошибки 
        try
        {
            query = "SELECT * FROM ProfileBusket WHERE profileMail=?savedProfileMail";
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
                        while (rdr.Read())
                        {
                            personBusket.Add(rdr["itemName"].ToString());
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }
}
