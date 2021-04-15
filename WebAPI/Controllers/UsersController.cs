using MyData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.AspNet.Identity;
using WebAPI.Classes;
using System.Web;
using System.Configuration;
using System.IO;
using ImageProcessor.Imaging.Formats;
using System.Drawing;
using ImageProcessor;
using ImageProcessor.Imaging;

namespace WebAPI.Controllers
{
    [Authorize]
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/Users")]
    public class UsersController : ApiController
    {
       
        [Route("{ID}")]
        [HttpGet]
        [Authorize(Roles = "Administrator, Super User")]
        public HttpResponseMessage Get(string id)
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                //entities.Configuration.ProxyCreationEnabled = false;
                var entity = entities.AspNetUsers.FirstOrDefault(e => e.Id == id);

                if (entity != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with ID " + id.ToString() + " not found");
                }
            }
        }

        [Route("GetUserProfile")]
        [HttpGet]
        public HttpResponseMessage GetUserProfile() //get
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                string curUserID = User.Identity.GetUserId();
                //var entity = entities.ps_Users_UserProfiles_GetByUserId(curUserID).FirstOrDefault();\
                var testEntity = entities.AspNetUsers.FirstOrDefault();
                var entity = entities.vw_User_UserProfile.Where(e => e.UserID == curUserID).FirstOrDefault();
               

                if (entity != null)
                {

                    return Request.CreateResponse(HttpStatusCode.OK, entity);
                }
                else
                {
                    var userEntity = entities.vw_User_UserProfile.Where(e => e.Id == curUserID).FirstOrDefault();
                    if (userEntity != null)
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, userEntity);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User with ID " + curUserID + " not found");
                    }
                }
            }
        }

        [Route("GetRole")]
        [AllowAnonymous]
        [HttpGet]
        public HttpResponseMessage GetRole()
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                string curUserID = User.Identity.GetUserId();
              //  var entity = entities.AspNetUsers.Where(e => e.Id == curUserID).First();

                //if (entity != null)
                //{
                    string role = string.Empty;
                    if (User.IsInRole("Influencer")) { role = "Influencer"; }
                    else if (User.IsInRole("Advertiser")) { role = "Advertiser"; }
                if (role != string.Empty)
                {
                    string json = "{\"Role\":\"" + role + "\"}";
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                } else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Role not found");
                }
                //}
                //else
                //{
                //    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Role not found");
                //}
            }
        }

        [Route("GetRoles")]
        [HttpGet]
        public HttpResponseMessage GetRoles()
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                string curUserID = User.Identity.GetUserId();
                //  var entity = entities.AspNetUsers.Where(e => e.Id == curUserID).First();

                //if (entity != null)
                //{
                string role = string.Empty;
                if (User.IsInRole("User")) { role += "User"; }
                if (User.IsInRole("Administrator")) { role += "Administrator"; }
                if (User.IsInRole("Super User")) { role += "Super User"; }
                if (role != string.Empty)
                {
                    string json = "{\"Roles\":\"" + role + "\"}";
                    return Request.CreateResponse(HttpStatusCode.OK, json);
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Role not found");
                }
            }
        }

        [HttpPut]
        [Route("UserProfile")]
        //[Route("UserProfile/{email}")]
        // public HttpResponseMessage UserProfile([FromBody] UserProfile userProfile) //edit 
        public HttpResponseMessage UserProfile([FromBody] UserProfile userProfile) //edit 
        {

            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string curUserID = HttpContext.Current.User.Identity.GetUserId();




                    //Add entry to UserProfile Table
                    var entity = entities.UserProfiles.FirstOrDefault(e => e.UserID == curUserID);
                    if (entity == null)
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
                    }
                    else
                    {
                        //entity.FirstName = userProfile.FirstName;
                        //entity.LastName = userProfile.LastName;
                        //entity.Gender = userProfile.Gender;
                        //entity.DateOfBirth = userProfile.DateOfBirth;
                        //entity.Phone = userProfile.Phone;
                        //entity.Address1 = userProfile.Address1;
                        //entity.Address2 = userProfile.Address2;
                        //entity.City = userProfile.City;
                        //entity.Country = userProfile.Country;
                        //entity.State = userProfile.State;
                        //entity.Zip = userProfile.Zip;
                        if (userProfile.FirstName != null && userProfile.FirstName.Trim().Length > 0) { entity.FirstName = userProfile.FirstName; }
                        if (userProfile.LastName != null && userProfile.LastName.Trim().Length > 0) { entity.LastName = userProfile.LastName; }
                        if (userProfile.Gender != null && userProfile.Gender.Trim().Length > 0) { entity.Gender = userProfile.Gender; }
                        if (userProfile.DateOfBirth != null && userProfile.DateOfBirth.HasValue) { entity.DateOfBirth = userProfile.DateOfBirth; }
                        if (userProfile.Phone != null && userProfile.Phone.Trim().Length > 0) { entity.Phone = userProfile.Phone; }
                        if (userProfile.Address1 != null && userProfile.Address1.Trim().Length > 0) { entity.Address1 = userProfile.Address1; }
                        if (userProfile.Address2 != null && userProfile.Address2.Trim().Length > 0) { entity.Address2 = userProfile.Address2; }
                        if (userProfile.City != null && userProfile.City.Trim().Length > 0) { entity.City = userProfile.City; }
                        if (userProfile.Country != null && userProfile.Country.Trim().Length > 0) { entity.Country = userProfile.Country; }
                        if (userProfile.State != null && userProfile.State.Trim().Length > 0) { entity.State = userProfile.State; }
                        if (userProfile.Zip != null && userProfile.Zip.Trim().Length > 0) { entity.Zip = userProfile.Zip; }


                        entity.DateModified = DateTime.UtcNow;
                        entities.SaveChanges();

                        var storedProcEntity = entities.ps_Users_UserProfiles_GetByUserId(curUserID).FirstOrDefault();
                        return Request.CreateResponse(HttpStatusCode.OK, storedProcEntity);
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
        [Route("UserEmail")]
        public HttpResponseMessage UserEmail([FromBody] AspNetUser user) //edit user email address 
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string curUserID = HttpContext.Current.User.Identity.GetUserId();
                    //TO DO: Update email address 
                    if (user.Email != null && user.Email.Trim() != "" && user.Email.Contains("@") && user.Email.Contains("."))
                    {
                        var aspnetUserEntity = entities.AspNetUsers.FirstOrDefault(e => e.Id == curUserID);
                        aspnetUserEntity.Email = user.Email.Trim();
                        aspnetUserEntity.DateModified = DateTime.UtcNow;
                        entities.SaveChanges();
                        return Request.CreateResponse(HttpStatusCode.OK);
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Invalid email address");
                    }
                }
            }
            catch (Exception ex)
            {
                Util.LogError(ex.ToString());
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
            }
        }


        [HttpPost]
        [Route("Avatar")]
        public HttpResponseMessage Avatar()
        {
            using (AcmeEntities entities = new AcmeEntities())
            {
                string curUserID = User.Identity.GetUserId();

                Dictionary<string, object> dict = new Dictionary<string, object>();
                try
                {

                    var httpRequest = HttpContext.Current.Request;

                    foreach (string file in httpRequest.Files)
                    {
                        HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                        var postedFile = httpRequest.Files[file];
                        if (postedFile != null && postedFile.ContentLength > 0)
                        {

                            int maxFileSize = 1024 * 1024 * 10; //Size = 10 MB

                            IList<string> allowedFileExtensiions = new List<string> { ".jpg", ".jpeg", ".gif", ".png" };
                            var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                            var extension = ext.ToLower();
                            if (!allowedFileExtensiions.Contains(extension))
                            {
                                var message = string.Format("Please Upload image of type .jpg, .gif, or .png");
                                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                            }
                            else if (postedFile.ContentLength > maxFileSize)
                            {
                                var message = string.Format("Please upload a file no larger than 10 MB");
                                return Request.CreateResponse(HttpStatusCode.BadRequest, message);
                            }
                            else
                            {

                                var fileName = curUserID.ToString() + "_ORIG" + extension;
                                var filePath = ConfigurationManager.AppSettings["AvatarDirectory"].ToString() + curUserID.ToString() + "\\";

                                System.IO.Directory.CreateDirectory(filePath);
                                postedFile.SaveAs(filePath + fileName);


                                //Resize file
                                byte[] photoBytes = File.ReadAllBytes(filePath + fileName);
                                ISupportedImageFormat format = new PngFormat { Quality = 80 };


                                
                               


                                using (MemoryStream inStream = new MemoryStream(photoBytes))
                                {
                                    using (MemoryStream outStream = new MemoryStream())
                                    {
                                        // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                                        using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                                        {
                                            //large image
                                            ResizeLayer resizeLargeLayer = new ResizeLayer(new Size(500, 500), ResizeMode.Max);
                                            imageFactory.Load(inStream)
                                                        .Resize(resizeLargeLayer)
                                                        .Format(format)
                                                        .Save(filePath + curUserID.ToString() + ".png");

                                            // medium image
                                            //ResizeLayer resizeMediumLayer = new ResizeLayer(new Size(300, 300), ResizeMode.Crop, AnchorPosition.Center);
                                            //imageFactory.Load(inStream)
                                            //            .Resize(resizeMediumLayer)
                                            //            .Format(format)
                                            //            .Save(filePath + "md_" + curUserID.ToString() + ".png");

                                            // small image
                                            ResizeLayer resizeSmallLayer = new ResizeLayer(new Size(100, 100), ResizeMode.Crop, AnchorPosition.Center);
                                            imageFactory.Load(inStream)
                                                        .Resize(resizeSmallLayer)
                                                        .Format(format)
                                                        .Save(filePath + "sm_" + curUserID.ToString() + ".png");

                                            // thumbnail image
                                            //ResizeLayer resizeThumbnailLayer = new ResizeLayer(new Size(50, 50), ResizeMode.Crop, AnchorPosition.Center);
                                            //imageFactory.Load(inStream)
                                            //            .Resize(resizeThumbnailLayer)
                                            //            .Format(format)
                                            //            .Save(filePath + "tn_" + curUserID.ToString() + ".png");


                                        }
  
                                    }
                                }

                            }
                        }

                        
                    }

                    var message1 = string.Format("Image updated successfully");
                    return Request.CreateResponse(HttpStatusCode.Created, message1);
                }
                catch (Exception ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
                }



            }
        }

        [Route("LogPage/{page}")]
        [AllowAnonymous]
        [HttpPost]
        public HttpResponseMessage LogPage(string page)
        {
            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string curUserID = User.Identity.GetUserId();
                    //string clientURL = ConfigurationManager.AppSettings["BaseUrl"].ToString() + "/" + page;
                    string clientURL = HttpContext.Current.Request.Url.AbsoluteUri;
                    string clientIPAddress = HttpContext.Current.Request.UserHostAddress;

                    //determine if IPv4 of IPv6
                    bool ipV4 = false;
                    bool ipV6 = false;
                    IPAddress ipAddress;
                    if (IPAddress.TryParse(clientIPAddress, out ipAddress))
                    {
                        switch (ipAddress.AddressFamily)
                        {
                            case System.Net.Sockets.AddressFamily.InterNetwork:
                                // we have IPv4
                                ipV4 = true;
                                break;
                            case System.Net.Sockets.AddressFamily.InterNetworkV6:
                                // we have IPv6
                                ipV6 = true;
                                break;
                            default:
                                // umm... yeah... I'm going to need to take your red packet and...
                                break;
                        }
                    }


                    UserLog userLogEntity = new UserLog();
                    userLogEntity.UserId = curUserID;
                    userLogEntity.Source = clientURL;
                    if (ipV4)
                    {
                        userLogEntity.IPv4 = clientIPAddress;
                    }
                    if (ipV6)
                    {
                        userLogEntity.IPv6 = clientIPAddress;
                    }
                    userLogEntity.DateCreated = DateTime.UtcNow;
                    entities.UserLogs.Add(userLogEntity);
                    entities.SaveChanges();

                    return Request.CreateResponse(HttpStatusCode.OK);
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
