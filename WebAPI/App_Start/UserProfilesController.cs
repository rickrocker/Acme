using MyData;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebAPI.App_Start
{
    [Authorize]
    [RoutePrefix("api/UserProfiles")]
    public class UserProfilesController : ApiController
    {
        [HttpGet]
        [Route]
        public HttpResponseMessage Get()
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                //List<InfluencerInvite> list = entities.InfluencerInvites.ToList();
                string curUserID = User.Identity.GetUserId();
                var entity = entities.UserProfiles.Where(e => e.UserID == curUserID).ToList();

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No results returned");
                }



            }
        }
    }
}
