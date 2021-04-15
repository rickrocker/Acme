using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using MyData;
using System.Text.RegularExpressions;
using System.Net.Http;
using System.Net;

namespace WebAPI.Classes
{
    public static class HelperUtil
    {

        public static bool IsUserInCompany(string userID, int companyID)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    int companiesUsersEntity = entities.CompaniesUsers.Count(e => e.UserID == userID && e.CompanyID == companyID);

                    if (companiesUsersEntity > 0) //user is part of company
                    {
                        return true;
                    }
                    else
                    {
                        Util.LogUnauthorizedError("Unauthorized User is not in company: CompanyID=" + companyID.ToString() + "; UserID=" + userID, userID);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return false;
            }
        }


        public static bool IsUserNotificationOwner(string userID, int notificationID)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string myUserID = entities.Notifications.FirstOrDefault(e => e.ID == notificationID).UserID;


                    if (myUserID == userID)
                    {

                        return true;
                    }
                    else
                    {
                        Util.LogUnauthorizedError("Unauthorized Notification: NotificationID=" + notificationID.ToString() + "; UserID=" + userID, userID);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return false;
            }
        }

        public static bool IsUserTaskOwner(string userID, int taskID)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string myUserID = entities.Tasks.FirstOrDefault(e => e.ID == taskID).UserID;


                    if (myUserID == userID)
                    {

                        return true;
                    }
                    else
                    {
                        Util.LogUnauthorizedError("Unauthorized Notification: NotificationID=" + taskID.ToString() + "; UserID=" + userID, userID);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return false;
            }
        }

        public static int GetCompanyIdByUser(string userID)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    int companyID = entities.CompaniesUsers.FirstOrDefault(e => e.UserID == userID).CompanyID;
                    return companyID;
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return 0;
            }

        }

        public static Company GetCompanyByUser(string userID)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    var company = entities.CompaniesUsers.FirstOrDefault(e => e.UserID == userID).Company;
                    return company;
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return null;
            }

        }

        //public static Company CreateOrUpdateCompany (Company company, string curUserID)
        //{
        //    using (AcmeEntities entities = new AcmeEntities())
        //    {
        //        //check to see if company name exists
        //        //var myCompany = entities.Companies.FirstOrDefault(e => e.Name == company.Name);

        //        var myCompany = entities.ps_Companies_GetByUserId(curUserID).FirstOrDefault();

        //        if (myCompany == null)
        //        {
        //            //doesn't exist, create new
        //            company.DateCreated = DateTime.UtcNow;
        //            company.Active = true;
        //            entities.Companies.Add(company);
        //            entities.SaveChanges();
        //            //add entry to CompaniesUsers table
        //            CompaniesUser companiesUsers = new CompaniesUser();
        //            companiesUsers.CompanyID = company.ID;
        //            companiesUsers.UserID = curUserID;
        //            entities.CompaniesUsers.Add(companiesUsers);
        //            entities.SaveChanges();

        //            //var message = Request.CreateResponse(HttpStatusCode.Created, company);
        //            //message.Headers.Location = new Uri(Request.RequestUri + company.ID.ToString());
        //            //return message;
        //            return company;
        //        }
        //        else //company  exists, update
        //        {
        //            var myCompanyToUpdate = entities.Companies.FirstOrDefault(e => e.ID == myCompany.ID);
        //            myCompanyToUpdate.Name = company.Name;
        //            myCompanyToUpdate.Profile = company.Profile;
        //            myCompanyToUpdate.Website = company.Website;
        //            myCompanyToUpdate.NumberOfEmployees = company.NumberOfEmployees;
        //            myCompanyToUpdate.Address1 = company.Address1;
        //            myCompanyToUpdate.Address2 = company.Address2;
        //            myCompanyToUpdate.City = company.City;
        //            myCompanyToUpdate.State = company.State;
        //            myCompanyToUpdate.Zip = company.Zip;
        //            myCompanyToUpdate.Country = company.Country;
        //            myCompanyToUpdate.DateModified = DateTime.UtcNow;
        //            entities.SaveChanges();

        //            //var message = Request.CreateResponse(HttpStatusCode.Accepted, company);
        //            //message.Headers.Location = new Uri(Request.RequestUri + company.ID.ToString());
        //            //return message;
        //            return company;
        //        }
        //    }
        //}


        public static NotificationTemplate ReplaceMessageTemplateText(NotificationTemplate template, string recipientUserID, string senderUserID, Task task)
        {
            NotificationTemplate templateWithReplacedText = new NotificationTemplate();
            templateWithReplacedText.Subject = replacePlaceholdersInTemplate(template.Subject, recipientUserID, senderUserID, task);
            templateWithReplacedText.Body = replacePlaceholdersInTemplate(template.Body, recipientUserID, senderUserID, task);
            templateWithReplacedText.EmailBody = replacePlaceholdersInTemplate(template.EmailBody, recipientUserID, senderUserID, task);
            templateWithReplacedText.Abstract = replacePlaceholdersInTemplate(template.Abstract, recipientUserID, senderUserID, task);
            return templateWithReplacedText;
        }

        private static string replacePlaceholdersInTemplate(string text, string recipientUserID, string senderUserID, Task task)
        {
            string newString = text;
            try
            {
                //using (AcmeEntities entities = new AcmeEntities())
                //{
                if (text.Contains("[[SENDER_COMPANY"))
                {
                    Company company = GetCompanyByUser(senderUserID);
                    newString = newString.Replace("[[SENDER_COMPANYNAME]]", company.Name);

                }
                if (text.Contains("[[TASK") && task != null)
                {
                    newString = newString.Replace("[[TASK_LINK]]", ConfigurationManager.AppSettings["HomeUrl"].ToString() + "/Tasks?TaskID=" + task.ID);
                }
              
                //}
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
            }

            return newString;

        }

    }
}