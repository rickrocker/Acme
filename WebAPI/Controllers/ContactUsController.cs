using WebAPI.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace WebAPI.Controllers
{
    //[Authorize]
    [RoutePrefix("api/ContactUs")]
    public class ContactUsController : ApiController
    {
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage Put([FromBody] JObject jObject)
        {

            string body = "New entry from contact form at " + HttpContext.Current.Request.Url.AbsoluteUri + "\n\n";
            body += "IP Address: " + HttpContext.Current.Request.UserHostAddress + "\n\n";
            body += "Name: " + jObject["name"].ToString() + "\n\n";
            body += "Email: " + jObject["email"].ToString() + "\n\n";
            body += "Phone: " + jObject["phone"].ToString() + "\n\n";
            body += "Message: " + jObject["message"].ToString();

            //send email 
            try
            {
                string response = Util.SendEmail(ConfigurationManager.AppSettings["InquireEmailAddress"], "Contact Form entry", body);

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
