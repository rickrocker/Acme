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
    [RoutePrefix("api/Messages")]
    public class MessagesController : ApiController
    {
        [Authorize]
        [HttpPost]
        public HttpResponseMessage Post([FromBody] Message message)
        {
            try
            {
                string curUserID = User.Identity.GetUserId();
                {
                    using (AcmeEntities entities = new AcmeEntities())
                    {
                        
                        //set the right recipient (opposite of curUser)
                        if (message.RecipientUserID == curUserID)
                        {
                            message.RecipientUserID = message.SenderUserID;
                        } else if (message.SenderUserID == curUserID)
                        {
                            message.RecipientUserID = message.RecipientUserID;
                        } else
                        {
                            Util.LogUnauthorizedError("Unauthorized method", curUserID);
                            return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Not Authorized");
                        }

                        message.SenderUserID = curUserID;
                        message.DateCreated = DateTime.UtcNow;

                        entities.Messages.Add(message);
                        entities.SaveChanges();

                        var returnMessage = Request.CreateResponse(HttpStatusCode.Created, message);
                        returnMessage.Headers.Location = new Uri(Request.RequestUri + message.ID.ToString());
                        return returnMessage;
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
