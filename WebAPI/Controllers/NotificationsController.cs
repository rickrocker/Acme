using MyData;
using WebAPI.Classes;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Notifications")]
    public class NotificationsController : ApiController
    {

        [Authorize]
        [HttpGet]
        [Route("{pageNumber}")]
        public HttpResponseMessage Get(int? pageNumber)
        {
            try
            {
                string curUserID = User.Identity.GetUserId();
                {
                    using (AcmeEntities entities = new AcmeEntities())
                    {
                        int numberOfResultsPerPage = 10;
                        if (!pageNumber.HasValue) { pageNumber = 0; }

                        //get notifications
                        //var notificaitonsEntity = entities.ps_Notifications_Get(curUserID)
                        //    .Skip(numberOfResultsPerPage * (int)pageNumber)
                        //    .Take(numberOfResultsPerPage)
                        //    .ToList();
                        var notificationsEntity = entities.vw_Notifications
                            .Where(e => (e.UserID == curUserID) && e.Deleted != true)
                            .Select(e => new
                            {
                                e.ID,
                                e.ContractID,
                                e.Subject,
                                e.Body,
                                e.DateCreated,
                                e.SenderUserID,
                                e.SenderFirstName,
                                e.SenderLastName,
                                RecipientUserID = e.UserID,
                                RecipientFirstName = "",
                                RecipientLastName = "",
                                //CompanyName = e.SenderCompanyName,
                                SortByDate = e.DateCreated,
                                e.TaskID,
                                e.TaskTypeName,
                                e.TaskStatusName,
                                e.MinutesUntilTaskExpires
                            })
                            .OrderByDescending(e => e.SortByDate)
                            .Skip(numberOfResultsPerPage * (int)pageNumber)
                            .Take(numberOfResultsPerPage)
                            .GroupBy(e => e.ContractID).Select(e => e.FirstOrDefault()) //get the first result by group ContractID
                            .ToList();

                        //get messages
                        var messagesEntity = entities.vw_Messages
                            .Where(e => (e.SenderUserID == curUserID || e.RecipientUserID == curUserID))
                            .Select(e => new
                            {
                                e.ID,
                                e.ContractID,
                                Subject = "[MESSAGE]",
                                Body = e.Message,
                                e.DateCreated,
                                e.SenderUserID,
                                e.SenderFirstName,
                                e.SenderLastName,
                                e.RecipientUserID,
                                e.RecipientFirstName,
                                e.RecipientLastName,
                                //e.CompanyName,
                                SortByDate = e.DateCreated,
                                TaskID = (int?)null,
                                TaskTypeName = "",
                                TaskStatusName = "",
                                MinutesUntilTaskExpires = (int?)null
                            })
                            .OrderByDescending(e => e.SortByDate)
                            .Skip(numberOfResultsPerPage * (int)pageNumber)
                            .Take(numberOfResultsPerPage)
                            //.GroupBy(e => e.ContractID).Select(e => e.FirstOrDefault()) //get the first result by group ContractID
                            .ToList();

                        var mergedEntity = notificationsEntity.Union(messagesEntity)
                            .OrderByDescending(e => e.SortByDate)
                            //.GroupBy(e => e.ContractID).Select(e => e.FirstOrDefault()) //get the first result by group ContractID
                            .Take(10)
                            .ToList();

                        //get most recent related task (for messages)
                        List<vw_Tasks> tasksEntity = new List<vw_Tasks>();
                        foreach (var message in mergedEntity)
                        {
                            if (message.Subject == "[MESSAGE]")
                            {
                                var taskEntity = entities.vw_Tasks.Where(e => e.ContractID == message.ContractID && e.UserID == curUserID)
                                    //.Select(e => new { e.TaskStatusName, e.TaskTypeName, e.ExpireDate, e.MinutesUntilTaskExpires })
                                    .OrderByDescending(e => e.ExpireDate)
                                    .FirstOrDefault();

                                tasksEntity.Add(taskEntity);

                                //var myMergedEntity = mergedEntity.Where(e => e.ID == message.ID).First();
                                //var adskfj = mergedEntity.Where(e => e.ID == message.ID).Select(e => { e.MinutesUntilTaskExpires = 100});
                                //myMergedEntity.TaskTypeName = taskEntity.TaskTypeName;
                                //message.TaskTypeName = taskEntity.TaskTypeName;
                                //message.MinutesUntilTaskExpires = taskEntity.MinutesUntilTaskExpires;
                            }
                        } 

                        ////merge notifications and messages
                        //var mergedEntity = notificaitonsEntity.Zip(messagesEntity, (e, f) => new vw_NotificationsMessages()
                        //{
                        //    ContractID = e.ContractID,
                        //    MessagesContractID = f.ContractID,
                        //    Subject = e.Subject,
                        //    Body = e.Body,
                        //    Abstract = e.Abstract,
                        //    Message = f.Message
                        //}
                        //).ToList();


                        if (mergedEntity != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, new { mergedEntity, tasksEntity });
                        }
                        else
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No results returned");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }



        [Authorize]
        [HttpGet]
        [Route("RelatedNotificationsAndMessages/{isMessage}/{notificationOrMessageID}/{pageNumber}")]
        public HttpResponseMessage RelatedNotificationsAndMessages(bool isMessage, int notificationOrMessageID, int? pageNumber)
        {
            try
            {
                if (!pageNumber.HasValue) { pageNumber = 0; }
                string curUserID = User.Identity.GetUserId();

                using (AcmeEntities entities = new AcmeEntities())
                {
                    int numberOfResultsPerPage = 10;

                    //int? contractID = entities.Notifications.Where(e => e.ID == notificationID && e.UserID == curUserID).Select(e => e.ContractID).First();
                    int? contractID;
                    string senderUserID;
                    if (isMessage)
                    {
                        var messageEntity = entities.Messages.Where(e => e.ID == notificationOrMessageID &&
                        (e.RecipientUserID == curUserID || e.SenderUserID == curUserID))
                        .Select(e => new { e.ContractID, e.SenderUserID, e.RecipientUserID }).First();
                        contractID = messageEntity.ContractID;
                        //sender is opposite of curUser
                        if (curUserID == messageEntity.SenderUserID)
                        {
                            senderUserID = messageEntity.RecipientUserID;
                        }
                        else
                        {
                            senderUserID = messageEntity.SenderUserID;
                        }
                    }
                    else
                    {
                        var notificationEntity = entities.Notifications.Where(e => e.ID == notificationOrMessageID && e.UserID == curUserID).Select(e => new { e.ContractID, e.SenderUserID }).First();
                        contractID = notificationEntity.ContractID;
                        senderUserID = notificationEntity.SenderUserID;
                    }
                    ;

                    if (contractID != null && contractID > 0)
                    {
                        var relatedNotificationsAndMessagesEntity = entities.vw_NotificationsMessages.
                            Where(e => (e.ContractID == contractID || e.MessagesContractID == contractID)
                            && ((e.UserID == curUserID && e.SenderUserID == senderUserID) //notification with same recipient and sender
                            || (e.MessagesRecipientUserID == curUserID && e.MessagesSenderUserID == senderUserID) //or message with same recipient and sender
                            || (e.MessagesSenderUserID == curUserID && e.MessagesRecipientUserID == senderUserID)) //or message with same sender and recipient
                            && e.Deleted != true)
                            .OrderByDescending(e => e.NotificationsDateCreated == null ? e.MessagesDateCreated : e.NotificationsDateCreated) //order by date created
                            .Take(numberOfResultsPerPage)
                            .Skip(numberOfResultsPerPage * (int)pageNumber)
                            .ToList();

                        //mark as Viewed in DB
                        foreach (var notificationOrMessage in relatedNotificationsAndMessagesEntity)
                        {
                            if (notificationOrMessage.NotificationID != null && notificationOrMessage.NotificationID > 0)
                            {
                                var myNotificationEntity = entities.Notifications.Where(e => e.ID == notificationOrMessage.NotificationID).First();
                                myNotificationEntity.Viewed = true;
                            }
                            else if (notificationOrMessage.MessageID != null && notificationOrMessage.MessageID > 0)
                            {
                                var myMessageEntity = entities.Messages.Where(e => e.ID == notificationOrMessage.MessageID).First();

                                //find out if current user is recipient before marking as read
                                if (curUserID == notificationOrMessage.MessagesRecipientUserID) //recipient
                                {
                                    myMessageEntity.RecipientViewed = true;
                                }

                            }
                        }

                        entities.SaveChanges();

                        return Request.CreateResponse(HttpStatusCode.OK, relatedNotificationsAndMessagesEntity);
                    }
                    else
                    {
                        //TO DO:  no contract ID
                        return Request.CreateResponse(HttpStatusCode.OK, "TO DO");
                    }

                }

            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }
    }
}
