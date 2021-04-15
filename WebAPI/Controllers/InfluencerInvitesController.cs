using Blurspeed.InstaMatch.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using InstaMatch.WebAPI.Classes;
using System.Configuration;
using System.Web.Http.Cors;

namespace InstaMatch.WebAPI.Controllers
{
    [Authorize(Roles = "Administrator, Super User")]
   // [EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/InfluencerInvites")]
    public class InfluencerInvitesController : ApiController
    {
        //[AllowAnonymous]
        //public IEnumerable<InfluencerInvite> Get()
        [HttpGet]
        [Route]
        public HttpResponseMessage Get()
        {
            using (InstaMatchEntities entities = new InstaMatchEntities())
            {
                //List<InfluencerInvite> list = entities.InfluencerInvites.ToList();
                List<InfluencerInvite> list = entities.InfluencerInvites.Where(e => e.Active != false).ToList();

                if (list != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, list);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NoContent, "No results returned");
                }

                //This works also
                //var query = from t in entities.InfluencerInvites
                //            orderby t.ID ascending
                //            select new { t.ID, t.FirstName, t.LastName };
                //return Request.CreateResponse(HttpStatusCode.OK, query.ToList());


            }
        }

        [HttpGet]
        [Route("{ID}")]
        public HttpResponseMessage Get(int id) //get
        {
            using (InstaMatchEntities entities = new InstaMatchEntities())
            {
                var entity = entities.InfluencerInvites.FirstOrDefault(e => e.ID == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "InfluencerInvite with ID " + id.ToString() + " not found");
                }
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetByInviteCode/{InviteCode}")]
        public HttpResponseMessage GetByInviteCode(string inviteCode)
        {
            using (InstaMatchEntities entities = new InstaMatchEntities())
            {

                var entity = entities.InfluencerInvites.FirstOrDefault(e => e.InviteCode == inviteCode && e.Active != false);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "InfluencerInvite with InviteCode " + inviteCode + " not found or has already been registered");
                }
            }
        } 

        [HttpPost]
        [AllowAnonymous]
        [Route]
        public HttpResponseMessage Post([FromBody] InfluencerInvite influencerInvite) //create
        {
            try
            {
                using (InstaMatchEntities entities = new InstaMatchEntities())
                {
                    influencerInvite.DateCreated = DateTime.Now;
                    entities.InfluencerInvites.Add(influencerInvite);
                    entities.SaveChanges();

                    var message = Request.CreateResponse(HttpStatusCode.Created, influencerInvite);
                    message.Headers.Location = new Uri(Request.RequestUri + influencerInvite.ID.ToString());
                    return message;
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
            finally
            {
                //Send Email
                try
                {
                    Classes.Notification notification = new Classes.Notification(NotificationMessageTitle.InfluencerInviteRequested, 1);
                    int notificationResult = notification.Send(notification, Classes.NotificationMethod.Email, influencerInvite.FirstName + " " + influencerInvite.LastName, null);
                    int temp = notificationResult;

                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                }
            }
        } 

        [HttpPut]
        [Route("{ID}")]
        public HttpResponseMessage Put(int id, [FromBody] InfluencerInvite influencerInvite) //edit
        {
            try
            {
                using (InstaMatchEntities entities = new InstaMatchEntities())
                {
                    var entity = entities.InfluencerInvites.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID " + id.ToString() + " not found");
                    }
                    else
                    {
                        entity.FirstName =influencerInvite.FirstName;
                        entity.LastName = influencerInvite.LastName;
                        entity.Gender = influencerInvite.Gender;
                        entity.DateOfBirth = influencerInvite.DateOfBirth;
                        entity.Email = influencerInvite.Email;
                        entity.Country = influencerInvite.Country;
                        entity.State = influencerInvite.State;
                        entity.IGUserName = influencerInvite.IGUserName;
                        entity.YoutubeUrl = influencerInvite.YoutubeUrl;
                        entity.NumberOfIGFollowers = influencerInvite.NumberOfIGFollowers;
                        entity.NumberOfYTSubscribers = influencerInvite.NumberOfYTSubscribers;
                        entity.InviteCode = influencerInvite.InviteCode;
                        entity.DateModified = DateTime.Now;

                        //entity = influencerInvite;
                        entities.SaveChanges();

                        var message = Request.CreateResponse(HttpStatusCode.Accepted, influencerInvite);
                        message.Headers.Location = new Uri(Request.RequestUri + influencerInvite.ID.ToString());
                        return message;

                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }

        }

        [HttpPut]
        [Route("SendInvite/{ID}/{InviteCode}")]
        //public HttpResponseMessage SendInvite(int id, [FromBody] InfluencerInvite influencerInvite) //edit 
        public HttpResponseMessage SendInvite(int id, string inviteCode, [FromBody] InfluencerInvite influencerInvite)
        {
            //string inviteCode = string.Empty;
            try
            {
                using (InstaMatchEntities entities = new InstaMatchEntities())
                {
                    var entity = entities.InfluencerInvites.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID " + id.ToString() + " not found");
                    }
                    else
                    {
                        if (entity.InviteCode == null || entity.InviteCode.Trim() == "")
                        {
                            inviteCode = Guid.NewGuid().ToString();
                        }
                            entity.InviteCode = inviteCode;
                            entity.DateModified = DateTime.Now;
                            entities.SaveChanges();

                            

                            return Request.CreateResponse(HttpStatusCode.OK, entity);
                        //}
                        //else
                        //{
                        //    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Employee with ID " + id.ToString() + " already has an invite code of " + entity.InviteCode);
                        //}

                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
            finally
            {
                //Send Email
                try
                {
                    Classes.Notification notification = new Classes.Notification(NotificationMessageTitle.InfluencerInviteAccepted, 1);
                    notification.SendInfluencerInvite(notification, Constants.DefaultEmailFromAddress, influencerInvite.Email, inviteCode);
                }
                catch (Exception ex)
                {
                    Util.LogError(ex.ToString());
                }
            }
        }

        [HttpDelete]
        [Route("{ID}")]
        public HttpResponseMessage Delete(int id) //delete
        {
            try
            {
                using (InstaMatchEntities entities = new InstaMatchEntities())
                {
                    var entity = entities.InfluencerInvites.FirstOrDefault(e => e.ID == id);

                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Employee with ID " + id.ToString() + " not found");
                    }
                    else
                    {
                        entity.Active = false;
                        entity.DateModified = DateTime.Now;

                        entities.SaveChanges();

                        var message = Request.CreateResponse(HttpStatusCode.OK, entity);
                        //message.Headers.Location = new Uri(Request.RequestUri + id.ToString());
                        return message;
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
