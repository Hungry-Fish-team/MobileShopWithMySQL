#if UNITY_ANDROID
using System;
using Unity.Notifications.Android;
using NotificationSamples;
using UnityEngine;

public class MobileNotificationAndroidScript : MonoBehaviour
{
    public GameNotificationsManager gameNotificationManager;
    public GameManager gameManager;
    public LoadPersonInfoFromFilesScript loadPersonInfoFromFilesScript;

    public AndroidNotificationChannel notificationChannel;
    public GameNotificationChannel notificationChannelSecondVer;
    int identifier = 0;

    private void Start()
    {
        InitializationAllObjects();
        //CreateNotificationChannel();
        CreateNotificationChannelSecondVer();
    }

    private void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadPersonInfoFromFilesScript = GameObject.Find("GameManager").GetComponent<LoadPersonInfoFromFilesScript>();
    }

    private void CreateNotificationChannel()
    {
        notificationChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
    }

    private void CreateNotificationChannelSecondVer()   /// Рабочая версия
    {
        notificationChannelSecondVer = new GameNotificationChannel("default_channel", "Default Channel", "Generic notifications");
        gameNotificationManager.Initialize(notificationChannelSecondVer);
    }

    public void CreateAndSentNotification(string title, string textOfMessage, double time)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = textOfMessage;
        notification.SmallIcon = "default";     // Поменять название!
        notification.LargeIcon = "default";     // Поменять название!
        notification.FireTime = DateTime.Now.AddSeconds(time);

        AndroidNotificationCenter.SendNotification(notification, notificationChannel.Id);

        loadPersonInfoFromFilesScript.personNotification.Add(new LoadPersonInfoFromFilesScript.notificationObject(textOfMessage, DateTime.Now.AddSeconds(time).ToShortDateString() + " " + DateTime.Now.AddSeconds(time).ToShortTimeString()));
    }

    public void CreateAndSentNotificationSecondVer(string title, string textOfMessage, double time) /// Рабочая версия
    {
        loadPersonInfoFromFilesScript.personNotification.Add(new LoadPersonInfoFromFilesScript.notificationObject(textOfMessage, DateTime.Now.AddSeconds(time).ToShortDateString() + " " + DateTime.Now.AddSeconds(time).ToShortTimeString()));

        IGameNotification notification = gameNotificationManager.CreateNotification();
        if (notification != null)
        {
            notification.Title = title;
            notification.Body = textOfMessage;
            notification.DeliveryTime = DateTime.Now.AddSeconds(time);
            gameNotificationManager.ScheduleNotification(notification);
        }
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
    //    {
    //        var notification = new AndroidNotification();
    //        notification.Title = "SomeTitle";
    //        notification.Text = "SomeText";
    //        notification.SmallIcon = "default";     // Поменять название!
    //        notification.LargeIcon = "default";     // Поменять название!
    //        notification.FireTime = DateTime.Now;

    //        AndroidNotificationCenter.UpdateScheduledNotification(identifier, notification, "default_channel");
    //    }
    //    else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
    //    {
    //        AndroidNotificationCenter.CancelNotification(identifier);
    //    }
    //    else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
    //    {
    //        CreateAndSentNotification("Error", "NullMessage", 5);
    //    }
    //}

#elif UNITY_EDITOR
using System;
using Unity.Notifications.Android;
using NotificationSamples;
using UnityEngine;

public class MobileNotificationAndroidScript : MonoBehaviour
{
    public GameNotificationsManager gameNotificationManager;
    public GameManager gameManager;
    public LoadPersonInfoFromFilesScript loadPersonInfoFromFilesScript;

    public AndroidNotificationChannel notificationChannel;
    public GameNotificationChannel notificationChannelSecondVer;
    int identifier = 0;

    private void Start()
    {
        InitializationAllObjects();
        //CreateNotificationChannel();
        CreateNotificationChannelSecondVer();
    }

    private void InitializationAllObjects()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        loadPersonInfoFromFilesScript = GameObject.Find("GameManager").GetComponent<LoadPersonInfoFromFilesScript>();
    }

    private void CreateNotificationChannel()
    {
        notificationChannel = new AndroidNotificationChannel()
        {
            Id = "default_channel",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        AndroidNotificationCenter.RegisterNotificationChannel(notificationChannel);
    }

    private void CreateNotificationChannelSecondVer()   /// Рабочая версия
    {
        notificationChannelSecondVer = new GameNotificationChannel("default_channel", "Default Channel", "Generic notifications");
        gameNotificationManager.Initialize(notificationChannelSecondVer);
    }

    public void CreateAndSentNotification(string title, string textOfMessage, double time)
    {
        var notification = new AndroidNotification();
        notification.Title = title;
        notification.Text = textOfMessage;
        notification.SmallIcon = "default";     // Поменять название!
        notification.LargeIcon = "default";     // Поменять название!
        notification.FireTime = DateTime.Now.AddSeconds(time);

        AndroidNotificationCenter.SendNotification(notification, notificationChannel.Id);

        loadPersonInfoFromFilesScript.personNotification.Add(new LoadPersonInfoFromFilesScript.notificationObject(textOfMessage, DateTime.Now.AddSeconds(time).ToShortDateString() + " " + DateTime.Now.AddSeconds(time).ToShortTimeString()));
    }

    public void CreateAndSentNotificationSecondVer(string title, string textOfMessage, double time) /// Рабочая версия
    {
        loadPersonInfoFromFilesScript.personNotification.Add(new LoadPersonInfoFromFilesScript.notificationObject(textOfMessage, DateTime.Now.AddSeconds(time).ToShortDateString() + " " + DateTime.Now.AddSeconds(time).ToShortTimeString()));

        IGameNotification notification = gameNotificationManager.CreateNotification();
        if (notification != null)
        {
            notification.Title = title;
            notification.Body = textOfMessage;
            notification.DeliveryTime = DateTime.Now.AddSeconds(time);
            gameNotificationManager.ScheduleNotification(notification);
        }
    }

    //private void OnApplicationPause(bool pause)
    //{
    //    if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Scheduled)
    //    {
    //        var notification = new AndroidNotification();
    //        notification.Title = "SomeTitle";
    //        notification.Text = "SomeText";
    //        notification.SmallIcon = "default";     // Поменять название!
    //        notification.LargeIcon = "default";     // Поменять название!
    //        notification.FireTime = DateTime.Now;

    //        AndroidNotificationCenter.UpdateScheduledNotification(identifier, notification, "default_channel");
    //    }
    //    else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Delivered)
    //    {
    //        AndroidNotificationCenter.CancelNotification(identifier);
    //    }
    //    else if (AndroidNotificationCenter.CheckScheduledNotificationStatus(identifier) == NotificationStatus.Unknown)
    //    {
    //        CreateAndSentNotification("Error", "NullMessage", 5);
    //    }
    //}

#endif
}
