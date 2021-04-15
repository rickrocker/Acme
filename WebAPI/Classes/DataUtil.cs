using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Text;
using System.Xml;
using MyData;
using System.Linq;
using System.Text.RegularExpressions;

namespace WebAPI.Classes
{
    public static class DataUtil
    {
        public static object LogError { get; private set; }
        #region Public Methods

        //public static int InsertInfluencerInvite(string firstName, string lastName, string email,
        //    string phone, string country, string state, string igUsername, string youtubeUrl, string gender, DateTime? dateOfBirth, string bio)
        //{
        //    DataManager dm = new DataManager();
        //    dm.ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        //    Hashtable parameters = new Hashtable();
        //    parameters.Add("FirstName", firstName);
        //    parameters.Add("LastName", lastName);
        //    parameters.Add("Email", email);
        //    parameters.Add("Phone", phone);
        //    parameters.Add("Country", country);
        //    parameters.Add("State", state);
        //    parameters.Add("IGUserName", igUsername);
        //    parameters.Add("YoutubeUrl", youtubeUrl);
        //    parameters.Add("Gender", (object)gender ?? DBNull.Value);
        //    parameters.Add("DateOfBirth", (object)dateOfBirth ?? DBNull.Value);
        //    parameters.Add("Bio", bio);
        //    parameters.Add("InviteCode", Guid.NewGuid());
        //    parameters.Add("Invited", 0);
        //    return dm.ExecuteStoredProcedure_ExecuteNonQuery("ps_InfluencerInvites_Insert", parameters);
        //}



        public static int InsertNewTaskAndNotification(string recipientUserID, string senderUserID, int? contractID, Enum.NotificationTemplates notificationTemplate, Enum.TaskType taskType, bool sendEmail)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                try
                {
                    //create a task for influencer
                    var taskEntity = new Task();
                    taskEntity.UserID = recipientUserID;
                    taskEntity.TaskTypeID = (int)taskType;
                    taskEntity.TaskStatusID = (int)Enum.TaskStatus.Assigned;
                    taskEntity.ExpireDate = DateTime.UtcNow.AddHours(72);
                    taskEntity.DateCreated = DateTime.UtcNow;
                    entities.Tasks.Add(taskEntity);
                    entities.SaveChanges();



                     var notificationTemplateEntity = entities.NotificationTemplates.Where(e => e.ID == (int)notificationTemplate).First();


                    //replace text in template
                    var templateWithReplacedText = HelperUtil.ReplaceMessageTemplateText(notificationTemplateEntity, recipientUserID, senderUserID, taskEntity);

                    //mark past notifications with same contractID as Viewed
                    //var oldNotificationsEntity = entities.Notifications.Where(e => e.UserID == recipientUserID && )

                    //create a  notification for influencer
                    var notificationEntity = new MyData.Notification();
                    notificationEntity.UserID = recipientUserID;
                    notificationEntity.SenderUserID = senderUserID;
                    notificationEntity.ContractID = contractID;
                    notificationEntity.TaskID = taskEntity.ID;
                    //notificationEntity.IsSystemNotification = true;
                    notificationEntity.Subject = templateWithReplacedText.Subject;
                    notificationEntity.Body = templateWithReplacedText.Body;
                    notificationEntity.Abstract = templateWithReplacedText.Abstract;
                    notificationEntity.DateCreated = DateTime.UtcNow;
                    entities.Notifications.Add(notificationEntity);
                    entities.SaveChanges();

                    if (sendEmail)
                    {
                        //send Email
                        Util.SendEmailToUserID(recipientUserID, templateWithReplacedText.Subject, templateWithReplacedText.EmailBody);

                        //insert record in NotificationDeliveries table
                        var notificationDeliveriesEntity = new NotificationDelivery();
                        notificationDeliveriesEntity.NotificationID = notificationEntity.ID;
                        notificationDeliveriesEntity.NotificationMethodID = (int)Enum.NotificationMethod.Email;
                        notificationDeliveriesEntity.DateCreated = DateTime.UtcNow;
                        entities.NotificationDeliveries.Add(notificationDeliveriesEntity);
                        entities.SaveChanges();
                    }

                    return taskEntity.ID;
                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                    return 0;
                }
            }
            //return true;
        }


        public static bool InsertNewNotification(string userID, string senderUserID, int notificationTemplateID, int? contractID, int? taskID, bool sendEmail)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                try
                {
                    //create a  notification for influencer
                    var notificationEntity = new Notification();
                    notificationEntity.UserID = userID;
                    notificationEntity.SenderUserID = senderUserID;
                    notificationEntity.ContractID = contractID;
                    notificationEntity.TaskID = taskID;

                    //resolve template by TaskTypeID
                    var notificationTemplateEntity = entities.NotificationTemplates.Where(e => e.ID == notificationTemplateID).First();

                    //replace text in template
                    var templateWithReplacedText = HelperUtil.ReplaceMessageTemplateText(notificationTemplateEntity, userID, senderUserID, null);

                    notificationEntity.Subject = templateWithReplacedText.Subject;
                    notificationEntity.Body = templateWithReplacedText.Body;
                    notificationEntity.Abstract = templateWithReplacedText.Abstract;
                    notificationEntity.DateCreated = DateTime.UtcNow;
                    entities.Notifications.Add(notificationEntity);
                    entities.SaveChanges();

                    if (sendEmail)
                    {
                        //send Email
                        Util.SendEmailToUserID(userID, templateWithReplacedText.Subject, templateWithReplacedText.EmailBody);

                        //insert record in NotificationDeliveries table
                        var notificationDeliveriesEntity = new NotificationDelivery();
                        notificationDeliveriesEntity.NotificationID = notificationEntity.ID;
                        notificationDeliveriesEntity.NotificationMethodID = (int)Enum.NotificationMethod.Email;
                        notificationDeliveriesEntity.DateCreated = DateTime.UtcNow;
                        entities.NotificationDeliveries.Add(notificationDeliveriesEntity);
                        entities.SaveChanges();
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                    return false;
                }
            }
            //return true;
        }


        public static bool CompleteTask(int taskID, string userID)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                try
                {
                    var taskEntity = entities.Tasks.Where(e => e.ID == taskID && e.UserID == userID).First();
                    if (taskEntity != null)
                    {
                        taskEntity.TaskStatusID = (int)Classes.Enum.TaskStatus.Completed;
                        taskEntity.DateModified = DateTime.UtcNow;
                        entities.SaveChanges();

                        return true;
                    }
                    else
                    {
                        Util.LogUnauthorizedError("Unauthorized DataUtil.CompleteTask()", userID);
                        return false;
                    }
                    
                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                    return false;
                }
            }
        }

        public static bool CompleteAllTasks(string userID)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                try
                {
                    List<Task> tasksEntity = entities.Tasks.Where(e => e.UserID == userID).ToList();
                    foreach (Task task in tasksEntity)
                    {
                        task.TaskStatusID = (int)Classes.Enum.TaskStatus.Completed;
                        task.DateModified = DateTime.UtcNow;
                        
                    }
                    entities.SaveChanges();
                    return true;

                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                    return false;
                }
            }
        }

        public static bool MarkRelatedNotificationsAsViewed(int contractID, string userID)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                try
                {
                    var notificationsEntity = entities.Notifications.Where(e => e.ContractID == contractID && e.UserID == userID && e.Viewed != true).ToList();
                    foreach (Notification notification in notificationsEntity)
                    {
                        notification.Viewed = true;
                        notification.DateModified = DateTime.UtcNow;
                    }
                    entities.SaveChanges();
                    return true;

                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                    return false;
                }
            }
        }



        #endregion

    }
}
