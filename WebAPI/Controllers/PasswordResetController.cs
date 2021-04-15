using MyData;
using WebAPI.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.Controllers
{
    //[Authorize]
    [RoutePrefix("api/PasswordReset")]
    public class PasswordResetController : ApiController
    {
        [HttpGet]
        [Route("ResetByEmail/{email}")]
        [AllowAnonymous]
        public HttpResponseMessage ResetByEmail(string email)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                //get User ID from email
                var aspNetUserEntity = entities.AspNetUsers.FirstOrDefault(e => e.Email == email);
                if (aspNetUserEntity == null)
                {
                    //no email is matched but send OK response (so user doesn't know email is not in system)
                    return Request.CreateResponse(HttpStatusCode.OK);
                }
                else
                {
                    var userID = aspNetUserEntity.Id;
                    var requestID = Guid.NewGuid().ToString();
                    var passwordRequest = new PasswordResetRequest();
                    passwordRequest.ID = requestID;
                    passwordRequest.UserID = userID;
                    passwordRequest.ResetRequestDateTime = DateTime.UtcNow;

                    entities.PasswordResetRequests.Add(passwordRequest);
                    entities.SaveChanges();

                    //send email 
                    try
                    {
                        string response = Util.SendEmail(email, "Password reset requested",
                            "You have requested a password reset. Please visit " + ConfigurationManager.AppSettings["HomeUrl"] + "/ResetPassword?resetID=" + requestID + " to reset your password.");

                        if (response.ToLower().Contains("error"))
                        {
                            return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Internal Server Error");
                        }

                    }
                    catch (Exception ex)
                    {
                        Util.LogError(ex.ToString());
                        return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
                    }


                    return Request.CreateResponse(HttpStatusCode.OK);

                }

            }
        }
    }
}