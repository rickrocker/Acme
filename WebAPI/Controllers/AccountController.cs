using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using WebAPI.Models;
using WebAPI.Providers;
using WebAPI.Results;
using System.Web.Http.Cors;
using MyData;
using WebAPI.Classes;
using System.Linq;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Services;
using System.Threading;
//using System.Web.Security;

namespace WebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/Account")]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class AccountController : ApiController
    {
        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            //var userWithClaims = (ClaimsPrincipal)User;
            //var email = userWithClaims.FindFirst(ClaimTypes.Email).Value;
            string email;
            try
            {

                email = externalLogin.Email; //facebook email
                //if (email == null)
                //{
                //    email =  User.Identity.GetUserName();
                //}
            }
            catch
            {
                email = User.Identity.GetUserName();
            }

            return new UserInfoViewModel
            {
                Email = email,
                UserName = User.Identity.GetUserName(),
                UserId = User.Identity.GetUserId(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // GET api/Account/ManageInfo?returnUrl=%2F&generateState=true
        [Route("ManageInfo")]
        public async Task<ManageInfoViewModel> GetManageInfo(string returnUrl, bool generateState = false)
        {
            IdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null)
            {
                return null;
            }

            List<UserLoginInfoViewModel> logins = new List<UserLoginInfoViewModel>();

            foreach (IdentityUserLogin linkedAccount in user.Logins)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null)
            {
                logins.Add(new UserLoginInfoViewModel
                {
                    LoginProvider = LocalLoginProvider,
                    ProviderKey = user.UserName,
                });
            }

            return new ManageInfoViewModel
            {
                LocalLoginProvider = LocalLoginProvider,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            };
        }

        //POST api/Account/ChangePassword
       [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/ResetPassword
        [AllowAnonymous]
        [Route("ResetPassword")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            string userID;
            using (AcmeEntities entities = new AcmeEntities())
            {
                var passwordResetRequestsEntity = entities.PasswordResetRequests.FirstOrDefault(e => e.ID == model.PasswordRequestID);
                userID = passwordResetRequestsEntity.UserID;

                //TO DO 
                DateTime expireDate = passwordResetRequestsEntity.ResetRequestDateTime.Value.AddHours(4);
                 if (DateTime.UtcNow > expireDate)
                {
                    return BadRequest("This link has expired. Please request to reset your password again.");
                }
            }


            //IdentityResult result = await UserManager.ChangePasswordAsync(userID, model.OldPassword,
            //    model.NewPassword);
            string code = await UserManager.GeneratePasswordResetTokenAsync(userID);
            IdentityResult result = await UserManager.ResetPasswordAsync(userID, code, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }




        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {

            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            try
            {
                using (AcmeEntities entities = new AcmeEntities())
                {
                    string role = model.Role;


                    //
                    if (role == "User") 
                    {
                        var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

                        IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                        if (!result.Succeeded)
                        {
                            return GetErrorResult(result);
                        }

                        var aspNetUsersEntity = entities.AspNetUsers.FirstOrDefault(e => e.Email == model.Email);
                        aspNetUsersEntity.DateCreated = DateTime.UtcNow;

                        //add entry in UserProfile table
                        UserProfile userProfile = new UserProfile();
                        userProfile.DateCreated = DateTime.UtcNow;
                        userProfile.FirstName = model.FirstName;
                        userProfile.LastName = model.LastName;

                            userProfile.Active = true;

                            //Create new blank company
                            Company company = new Company();
                            company.Name = "";
                            company.DateCreated = DateTime.UtcNow;
                            company.Active = true;
                            entities.Companies.Add(company);
                            //add entry to CompaniesUsers table
                            CompaniesUser companiesUsers = new CompaniesUser();
                            companiesUsers.CompanyID = company.ID;
                            companiesUsers.UserID = aspNetUsersEntity.Id;
                            entities.CompaniesUsers.Add(companiesUsers);

                       
                        userProfile.UserID = aspNetUsersEntity.Id;
                        //save data to all tables
                        entities.UserProfiles.Add(userProfile);
                        UserManager.AddToRole(aspNetUsersEntity.Id, role);
                        entities.SaveChanges();

                        //create avatar
                        Util.GenerateAvatar(model.FirstName, model.LastName, aspNetUsersEntity.Id);

                        var userEntity = entities.vw_User_UserProfile.First(e => e.UserID == aspNetUsersEntity.Id);
                        return Ok(userEntity);
                    }
                    else
                    {
                        return BadRequest("Role " + model.Role + " is not permitted");
                    }

                }
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                Util.LogError(ex.ToString());
                //return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.ToString());
                return BadRequest(ex.ToString());
            }






        }


        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        //[Route("RegisterExternal")]
        [Route("RegisterExternal/{role}")]
        public async Task<IHttpActionResult> RegisterExternal(string role)
        {
            if (role == "User")

            {
                //var info = await Authentication.GetExternalLoginInfoAsync(); //default - doesn't work
                var info = await authenticationManager_GetExternalLoginInfoAsync_WithExternalBearer(); //custom method



                if (info == null)
                {
                    return InternalServerError();
                }


                
                var user = new ApplicationUser()
                {
                    UserName = info.DefaultUserName,
                    Email = info.Email,
                };

                IdentityResult result = await UserManager.CreateAsync(user);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }

                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }


                using (AcmeEntities entities = new AcmeEntities())
                {
                    var externalUserProfileData = await getExternalUserProfileDataAsync();

                    var aspNetUsersEntity = entities.AspNetUsers.FirstOrDefault(e => e.Email == externalUserProfileData.Email);
                    aspNetUsersEntity.DateCreated = DateTime.UtcNow;

                    //add entry in UserProfile table
                    UserProfile userProfile = new UserProfile();
                    userProfile.DateCreated = DateTime.UtcNow;
                    userProfile.FirstName = externalUserProfileData.FirstName;
                    if (userProfile.FirstName == null)
                    {
                        try
                        {
                            userProfile.FirstName = externalUserProfileData.Name.Trim().Substring(0, externalUserProfileData.Name.Trim().IndexOf(" "));
                        }
                        catch (Exception)
                        {
                            userProfile.FirstName = null;
                        }
                    }
                    userProfile.LastName = externalUserProfileData.LastName;
                    if (userProfile.LastName == null)
                    {
                        try
                        {
                            userProfile.LastName = externalUserProfileData.Name.Trim().Substring(externalUserProfileData.Name.Trim().IndexOf(" "));
                        }
                        catch (Exception)
                        {
                            userProfile.LastName = null;
                        }
                    }

                   
                        userProfile.Active = true;
                   
                   
                    userProfile.UserID = user.Id;

                    entities.UserProfiles.Add(userProfile);

                    //Add user to Role
                    UserManager.AddToRole(user.Id, role);

                    //save data to all tables
                    entities.SaveChanges();

                    //create avatar
                    Util.GenerateAvatar(userProfile.FirstName, userProfile.LastName, user.Id);

                   // return Ok(userProfile);

                }

                return Ok();
                
            }
            else
            {
                return BadRequest("Role " + role + " is not permitted");
            }
        }



        //custom method for external (FB and Google) auth
        private async Task<ExternalLoginInfo> authenticationManager_GetExternalLoginInfoAsync_WithExternalBearer()
        {
            ExternalLoginInfo loginInfo = null;

            var result = await Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);

            if (result != null && result.Identity != null)
            {

                var idClaimNameIdentifier = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
                var idClaimEmail = result.Identity.FindFirst(ClaimTypes.Email);
                var idName = result.Identity.FindFirst(ClaimTypes.Name);

                ExternalLoginData externalLoginData = ExternalLoginData.FromIdentity(result.Identity);
                var firstName = externalLoginData.FirstName;
                var lastName = externalLoginData.LastName;


                string username = null;
                //string email = null;
                if (idClaimEmail != null && idClaimNameIdentifier != null)
                {
                    username = idClaimEmail.Value; //try using Email as username first

                    loginInfo = new ExternalLoginInfo()
                    {
                        DefaultUserName = username,
                        Login = new UserLoginInfo(idClaimNameIdentifier.Issuer, idClaimNameIdentifier.Value),
                        Email = username,
                    };
                }
                else if (idClaimNameIdentifier != null)
                {

                    loginInfo = new ExternalLoginInfo()
                    {
                        DefaultUserName = idClaimNameIdentifier.Value,
                        Login = new UserLoginInfo(idClaimNameIdentifier.Issuer, idClaimNameIdentifier.Value),
                        Email = null, //no email supplied by external service (e.g. FB user uses mobile for login instead of email)

                    };

                }


            }
            return loginInfo;
        }



        private async Task<ExternalLoginData> getExternalUserProfileDataAsync()
        {
            //ExternalLoginInfo loginInfo = null;
            ExternalLoginData loginData = null;

            var result = await Authentication.AuthenticateAsync(DefaultAuthenticationTypes.ExternalBearer);

            if (result != null && result.Identity != null)
            {

                var idClaimNameIdentifier = result.Identity.FindFirst(ClaimTypes.NameIdentifier);
                var idClaimEmail = result.Identity.FindFirst(ClaimTypes.Email);

                ExternalLoginData externalLoginData = ExternalLoginData.FromIdentity(result.Identity);
                var firstName = externalLoginData.FirstName;
                var lastName = externalLoginData.LastName;
                var name = externalLoginData.Name;



                //facebook
                if (firstName == null && name != null & idClaimNameIdentifier.Issuer.ToLower().Trim() == "facebook")
                {
                    var namesArray = name.Trim().Split(' ');
                    if (namesArray.Length > 0)
                    {
                        firstName = name.Split(' ')[0];

                        //get last name
                        if (lastName == null & namesArray.Length > 1)
                        {
                            lastName = "";
                            int i = 0;
                            foreach (string myName in namesArray)
                            {
                                if (i != 0)
                                {
                                    lastName += myName + " ";
                                }
                                i++;
                            }
                            lastName = lastName.Trim();
                        }
                    }
                }


                string username = null;
                //string email = null;
                if (idClaimEmail != null && idClaimNameIdentifier != null)
                {
                    username = idClaimEmail.Value; //try using Email as username first


                    loginData = new ExternalLoginData()
                    {
                        UserName = username,
                        Email = username,
                        FirstName = firstName,
                        LastName = lastName,
                        Name = name
                        //LoginProvider = new UserLoginInfo(idClaimNameIdentifier.Issuer, idClaimNameIdentifier.Value).LoginProvider
                    };
                }
                else if (idClaimNameIdentifier != null)
                {

                    loginData = new ExternalLoginData()
                    {
                        UserName = idClaimNameIdentifier.Value,
                        Email = null, //no email supplied by external service (e.g. FB user uses mobile for login instead of email)
                        FirstName = firstName,
                        LastName = lastName,
                        Name = name,
                        LoginProvider = new UserLoginInfo(idClaimNameIdentifier.Issuer, idClaimNameIdentifier.Value).LoginProvider
                    };

                }


            }
            return loginData;
        }





        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }


            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));
                //claims.Add(new Claim(ClaimTypes.Email, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, UserName, null, LoginProvider));
                    //claims.Add(new Claim(ClaimTypes.Email, UserName, null, LoginProvider));
                }

                if (Email != null)
                {
                    //claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                    claims.Add(new Claim(ClaimTypes.Email, Email, null, LoginProvider));
                }

                //if (Name != null)
                //{
                //    //claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                //    claims.Add(new Claim(ClaimTypes.Name, Name, null, LoginProvider));
                //}

                if (FirstName != null)
                {
                    //claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                    claims.Add(new Claim(ClaimTypes.GivenName, FirstName, null, LoginProvider));
                }

                if (LastName != null)
                {
                    //claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                    claims.Add(new Claim(ClaimTypes.Surname, LastName, null, LoginProvider));
                }

                if (Name != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, Name, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    Email = identity.FindFirstValue(ClaimTypes.Email),
                    UserName = identity.FindFirstValue(ClaimTypes.NameIdentifier),
                    // Name = identity.FindFirstValue(ClaimTypes.Name),
                    Name = identity.Name,
                    FirstName = identity.FindFirstValue(ClaimTypes.GivenName),
                    LastName = identity.FindFirstValue(ClaimTypes.Surname),
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}