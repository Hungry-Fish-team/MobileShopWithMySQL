using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using UnityEngine.UI;

public class PersonAllInfoScript : MonoBehaviour
{
    GameManager gameManager;
    ProfileDataScript profileDataScript;

    [SerializeField]
    GameObject prefabOfRequestInfo;
    [SerializeField]
    GameObject showRequestInfoWindow;
    [SerializeField]
    int numberOfRequest = 0;

    Text infoFromRequestText;

    [SerializeField]
    List<string> requestInfo = new List<string>();

    void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        profileDataScript = GameObject.Find("GameManager").GetComponent<ProfileDataScript>();
    }

    public void LoadPersonInfoAbout()
    {
        TryConnection();

        requestInfo.Clear();

        ReturnRequestInfo();
        showRequestInfoWindow.SetActive(true);
    }

    private string ChousePersonRequest()
    {
        string query = string.Empty;
        switch (numberOfRequest)
        {
            case 0:
                {
                    query = "SELECT profileName, profileMail FROM ProfileData";
                    break;
                }
            case 1:
                {
                    query = "SELECT profileMail, paymantType, timeOfOrder, allCostOfOrder FROM BusketOfOrdersSecond";
                    break;
                }
            case 2:
                {
                    query = "SELECT profileMail, paymantType, timeOfOrder, allCostOfOrder FROM BusketOfOrdersSecond WHERE profileMail = ?savedProfileMail";
                    break;
                }
            case 3:
                {
                    query = "SELECT profileMail, InfoAboutOrders.orderNumber, vendorCode, sizeOfItem, countOfItem, allCostOfItem FROM InfoAboutOrders INNER JOIN BusketOfOrdersSecond ON InfoAboutOrders.orderNumber = BusketOfOrdersSecond.orderNumber WHERE profileMail = ?savedProfileMail";
                    break;
                }
            case 4:
                {
                    query = "SELECT profileName, profileMail FROM ProfileData WHERE profileMail LIKE 'vova%'";
                    break;
                }
            case 5:
                {
                    query = "SELECT profileMail, orderNumber FROM BusketOfOrdersSecond WHERE timeOfOrder > date_sub(now(), interval 1 week)";
                    break;
                }
            case 6:
                {
                    query = "SELECT profileMail, orderNumber FROM BusketOfOrdersSecond WHERE timeOfOrder > date_sub(now(), interval 1 year)";
                    break;
                }
            case 7:
                {
                    //query = "SELECT profileMail, allCostOfOrder, SUM(allCostOfOrder) AS allCost FROM BusketOfOrdersSecond GROUP BY profileMail, allCostOfOrder";
                    query = "SELECT profileMail, SUM(allCostOfOrder) AS allCost FROM BusketOfOrdersSecond WHERE profileMail = ?savedProfileMail GROUP BY profileMail";
                    break;
                }
            case 8:
                {
                    query = "SELECT profileMail, COUNT(profileMail) AS countOrders FROM BusketOfOrdersSecond WHERE profileMail = ?savedProfileMail";
                    break;
                }
            case 9:
                {
                    query = "SELECT profileMail, COUNT(profileMail) AS countOrders FROM BusketOfOrdersSecond GROUP BY profileMail";
                    break;
                }
        }

        return query;
    }

    private void ReturnRequestInfo()
    {
        string info = string.Empty;
        string query = string.Empty;

        try
        {
            query = ChousePersonRequest();

            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                using (cmd = new MySqlCommand(query, con))
                {
                    if (ReadQueryForReturnInfo(query, "WHERE") || ReadQueryForReturnInfo(query, "HAVING"))
                    {
                        if (ReadQueryForReturnInfo(QueryForInputInfo(query), "?savedProfileMail"))
                        {
                            MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                            oParam.Value = profileDataScript.savedProfileMail;
                        }
                    }

                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            string str = string.Empty;
                            if (ReadQueryForReturnInfo(query, "ProfileData"))
                            {
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "profileName"))
                                {
                                    str += rdr["profileName"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "profileAddress"))
                                {
                                    str += rdr["profileAddress"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "profileNumber"))
                                {
                                    str += rdr["profileNumber"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "profileMail"))
                                {
                                    str += rdr["profileMail"].ToString() + " ";
                                }
                            }
                            if (ReadQueryForReturnInfo(query, "BusketOfOrdersSecond"))
                            {
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "orderNumber"))
                                {
                                    str += rdr["orderNumber"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "profileMail"))
                                {
                                    str += rdr["profileMail"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "paymantType"))
                                {
                                    str += rdr["paymantType"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "timeOfOrder"))
                                {
                                    str += rdr["timeOfOrder"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "allCostOfOrder"))
                                {
                                    str += rdr["allCostOfOrder"].ToString() + " ";
                                }
                            }
                            if (ReadQueryForReturnInfo(query, "InfoAboutOrders"))
                            {
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "orderNumber"))
                                {
                                    str += rdr["orderNumber"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "vendorCode"))
                                {
                                    str += rdr["vendorCode"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "sizeOfItem"))
                                {
                                    str += rdr["sizeOfItem"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "countOfItem"))
                                {
                                    str += rdr["countOfItem"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "allCostOfItem"))
                                {
                                    str += rdr["allCostOfItem"].ToString() + " ";
                                }
                            }
                            if (ReadQueryForReturnInfo(query, "AS"))
                            {
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "allCost"))
                                {
                                    str += rdr["allCost"].ToString() + " ";
                                }
                                if (ReadQueryForReturnInfo(QueryForReturnInfo(query), "countOrders"))
                                {
                                    str += rdr["countOrders"].ToString() + " ";
                                }
                            }
                            requestInfo.Add(str);
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }

        //return info;
        LoadAllRequestObjects();
    }

    private bool ReadQueryForReturnInfo(string query, string word)
    {
        if (query.Contains(word) && !query.Contains("SUM("+word+")") && !query.Contains("COUNT(" + word + ")"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private string QueryForReturnInfo(string query)
    {
        string str = query.Substring(0, query.IndexOf("FROM"));
        return str;
    }

    private string QueryForInputInfo(string query)
    {
        string str = string.Empty;

        if (ReadQueryForReturnInfo(query, "WHERE"))
        {
            str = query.Substring(query.IndexOf("WHERE"));
        }
        else if (ReadQueryForReturnInfo(query, "HAVING"))
        {
            str = query.Substring(query.IndexOf("HAVING"));
        }
        return str;
    }

    //private string ReadQueryForInputField(string query, string word)
    //{
    //    if (query.Contains(word))
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    private void ReloadAllText()
    {
        //infoFromRequestText.text = "";

        //infoFromRequestText.text = ReturnRequestInfo();
    }

    private void ClearAllRequestObjects()
    {
        GameObject content = showRequestInfoWindow.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

        if (content.transform.childCount != 0)
        {
            for (int i = 0; i < content.transform.childCount; i++)
            {
                Destroy(content.transform.GetChild(i).gameObject);
            }
        }
    }

    private void LoadAllRequestObjects()
    {
        ClearAllRequestObjects();

        GameObject content = showRequestInfoWindow.transform.GetChild(1).GetChild(0).GetChild(0).gameObject;

        foreach (string request in requestInfo)
        {
            GameObject requestInfo = Instantiate(prefabOfRequestInfo, content.transform);
            requestInfo.transform.GetChild(1).GetComponent<Text>().text = request;
        }
    }

    string constr = "Server=remotemysql.com;Database=odrqAeocA5;User ID=odrqAeocA5;Password=pphLr2KUcV;Pooling=true;CharSet=utf8;";
    MySqlConnection con = null;
    MySqlCommand cmd = null;
    MySqlDataReader rdr = null;
    MySqlError er = null;
    private bool loading;
    private bool saving;

    void OnEnable()
    {
        InitializationAllObjects();
    }

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

    private void OnDisable()
    {
        showRequestInfoWindow.SetActive(false);
    }

    void SaveProfileInfoFromServer()
    {
        saving = true;
        DeleteEntries();
        InsertEntries();
        saving = false;
    }

    void LoadProfileInfoFromServer()
    {
        loading = true;
        ReadEntries();
        loading = false;
    }

    void InsertEntries()
    {
        string query = string.Empty;
        try
        {
            query = "INSERT INTO ProfileBusket (profileMail, itemName) VALUES (?profileMail, ?itemName)";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                //foreach (string itemName in personBusket)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        //MySqlParameter oParam = cmd.Parameters.Add("?profileMail", MySqlDbType.VarChar);
                        //oParam.Value = profileDataScript.profileMail;
                        //MySqlParameter oParam1 = cmd.Parameters.Add("?itemName", MySqlDbType.VarChar);
                        //oParam1.Value = itemName;

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

    void DeleteEntries()
    {
        string query = string.Empty;
        try
        {
            query = "DELETE FROM ProfileBusket WHERE profileMail=?savedProfileMail";
            if (con.State.ToString() != "Open")
                con.Open();
            using (con)
            {
                //foreach (string itemName in personBusket)
                {
                    using (cmd = new MySqlCommand(query, con))
                    {
                        //MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                        //oParam.Value = profileDataScript.savedProfileMail;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }

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
                    //MySqlParameter oParam = cmd.Parameters.Add("?savedProfileMail", MySqlDbType.VarChar);
                    //oParam.Value = profileDataScript.savedProfileMail;

                    rdr = cmd.ExecuteReader();
                    if (rdr.HasRows)
                        while (rdr.Read())
                        {
                            //personBusket.Add(rdr["itemName"].ToString());
                        }
                    rdr.Dispose();
                }
            }
        }
        catch (IOException ex) { Debug.Log(ex.ToString()); }
        finally { }
    }
}
