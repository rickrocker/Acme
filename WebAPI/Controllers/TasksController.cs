using MyData;
using WebAPI.Classes;
using Microsoft.AspNet.Identity;
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
    [Authorize]
    [RoutePrefix("api/Tasks")]
    public class TasksController : ApiController
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

                        //first get tasks not expired and order by soonest to expire
                        //var tasksNotExpiredEntity = entities.vw_Tasks.Where(e => e.UserID == curUserID &&
                        var tasksNotExpiredEntity = entities.vw_Tasks.Where(e => e.UserID == curUserID &&
                        e.TaskStatusID == (int)Classes.Enum.TaskStatus.Assigned &&
                        (e.MinutesUntilTaskExpires > 0 || e.MinutesUntilTaskExpires == null))
                            .OrderBy(e => e.MinutesUntilTaskExpires)
                            .Skip(numberOfResultsPerPage * (int)pageNumber)
                            .Take(numberOfResultsPerPage)
                            .ToList();

                        //then get tasks already expired and order by most recently expired
                        //var tasksExpiredEntity = entities.vw_Tasks.Where(e => e.UserID == curUserID &&
                        var tasksExpiredEntity = entities.vw_Tasks.Where(e => e.UserID == curUserID &&
                         e.TaskStatusID == (int)Classes.Enum.TaskStatus.Assigned &&
                        e.MinutesUntilTaskExpires <= 0)
                           //.OrderByDescending(e => e.ExpireDate)
                           .OrderByDescending(e => e.ExpireDate)
                           .Skip(numberOfResultsPerPage * (int)pageNumber)
                           .Take(numberOfResultsPerPage)
                           .ToList();

                        //combine results
                        var tasksEntity = tasksNotExpiredEntity;
                        tasksEntity.AddRange(tasksExpiredEntity);

                        if (tasksEntity != null)
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, tasksEntity);
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
        [Route("TaskDetails/{taskID}")]
        public HttpResponseMessage TaskDetails(int taskID)
        {
            try
            {


                string curUserID = User.Identity.GetUserId();

                // if (HelperUtil.IsUserTaskOwner(curUserID, taskID))
                // {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    entities.Configuration.ProxyCreationEnabled = false;

                    var taskDetailsEntity = entities.vw_Tasks.Where(e => e.TaskID == taskID && e.UserID == curUserID).First();

                    return Request.CreateResponse(HttpStatusCode.OK, new { taskDetailsEntity });

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
